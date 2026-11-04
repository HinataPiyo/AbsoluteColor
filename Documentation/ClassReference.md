# 主要クラスリファレンス

本書ではゲーム進行・入力・採点・データ連携を優先して掲載する。メソッド名はコードから確認できたものだけを明記し、Inspector参照の具体値はUnity Editorでの確認が必要である。

## 重要度A：ゲーム進行・結果

### 1. `GameManager`

- **ファイル**：`Assets/Scripts/GamePlay/GameManager.cs`
- **役割**：ゲームプレイのラウンド進行管理。
- **主な処理**：`Awake()`でUIコントローラー参照を解決し、`Start()`でゲームBGMとゲームループを開始。`GameLoop()`で規定ラウンドを進め、`DesideTopicColor()`／`GetRandomZeroToOne()`で見本色を生成し、入力待ち、制限時間、結果表示・次ラウンド待ち、最終結果へつなぐ。`IgnorePlayerInput(bool)`は結果オーバーレイを切り替える。
- **管理データ／依存**：現在のラウンド、決定状態等。`TopicSphereController`、`AdjustUIAreaController`、`ResultController`、`JugementUI`、`AllRoundTotalResult`、UI管理クラス等の具体参照を持つ。
- **設計上の特徴**：コルーチン中心に順番を制御する。シーン上の各クラスを束ねる一方、依存先が多い。
- **改善余地**：フェーズと状態遷移の明示、参照検証、キャンセル／シーン破棄時の処理を整える。
- **根拠**：`GameManager.cs:17-34,37-119`

### 2. `ResultController`／`ResultData`

- **ファイル**：`Assets/Scripts/GamePlay/Result/ResultController.cs`
- **役割**：1ラウンド分の色差・判定・得点の算出と、結果表示系への受け渡し。
- **主なメソッド**：`CreateResultData(topicColor, playerColor)`は`ResultData`を作成・保存し、結果表示状態を設定する。`ShowResult()`は保存した最新の結果から`RoundResult`を構築して表示し、閉じる操作で結果表示状態を解除する。色差・得点・判定は`ResultData`側で計算する。
- **管理データ**：`ResultData`にラウンド番号、見本色、回答色を保持。プロパティはprivate setterで生成後の変更を抑える。
- **依存関係**：判定UI、ラウンド結果UI、総合結果等。
- **設計上の特徴**：UIから独立した値オブジェクトを用意し、複数の結果表示へ渡す。
- **改善余地**：色差／スコア／ランク仕様の一元化と境界値テスト。`ShowResult()`で結果表示クラスを都度生成する構造も整理する。
- **根拠**：`ResultController.cs:7-19,22-69,78-127`

`ShowResult()`は各ラウンドで`RoundResult`を構築する。`RoundResult`のコンストラクターは結果UI、見本色／回答色プレビュー、次へボタンを結び付ける。`CreatePreviewSliders(...)`と`AssignResultDataToUI(...)`がプレビュー生成・表示値設定を担当する（`RoundResult.cs:14-111`）。

### 3. `AllRoundTotalResult`

- **ファイル**：`Assets/Scripts/GamePlay/Result/AllRoundTotalResult.cs`
- **役割**：全ラウンド終了後の合計結果、ランク、履歴・最高点、最終演出、再試行／ホーム遷移を扱う。
- **主なメソッド**：`Initialize(VisualElement)`はUI要素・再試行／ホームボタンを設定する。`ShowAllRoundResult(List<ResultData>)`は全ラウンド表示を作り、合計・ランク・履歴・最高点を更新してランキング送信を呼び出す。`CalculateTotalScore(...)`は得点を合計する。
- **依存関係**：`CoreResource`、総合結果UI、スコアランク、シーン管理、Unityroomクライアント等。
- **設計上の特徴**：最終結果のユーザー操作と外部ランキング連携が同じ機能領域にある。
- **改善余地**：外部送信の成功／失敗・オフライン応答を明示し、外部サービス呼び出しと結果表示を分離する。
- **根拠**：`Assets/Scripts/GamePlay/Result/AllRoundTotalResult.cs:22-110`

### 4. `ValueConstants`

- **ファイル**：`Assets/Scripts/Constants/ValueConstants.cs`
- **役割**：ラウンド数・得点・ランク等の定数と計算規則を提供。
- **主なメソッド**：`GetScoreRank(int)`は総得点を正規化しS〜Dランクを返す。`GetScoreCoefficient(float)`と`GetScore(float)`は色差から得点を算出する。`ScoreRankColors`はランク別表示色を持つ。
- **ランク条件**：正規化値に対しS `>=0.95`、A `>=0.90`、B `>=0.70`、C `>=0.80`、それ以外Dの順で判定するため、Cは到達不能。
- **設計上の特徴**：共通値をクラスに集めることで呼び出し側の数値重複を抑える。
- **問題点**：Cランク条件はBランク条件より後にあるため到達不能。`MaxScore=750`の意味（基準値か1ラウンド上限か）は名前だけでは明確でない。
- **根拠**：`ValueConstants.cs:5-58`

## 重要度B：操作・データ・共通UI

### 5. `AdjustUIAreaController`／`AdjustSlider`

- **ファイル**：`Assets/Scripts/GamePlay/AdjustColor/AdjustUIAreaController.cs`、`AdjustSlider.cs`
- **役割**：ゲーム中のRGB調整、値の保持、決定操作、プレビュー更新。
- **主なメソッド**：`Initialize(VisualElement)`は各スライダー／決定ボタンを解決してイベントを登録する。`OnSliderValueChanged(...)`はRGB値とプレビューを同期し、`GetDecidedColor()`は回答色を返す。ボタン操作は `GameManager.I.isPlayerDecided` を更新する。
- **連携**：3本のスライダー、プレビュー、ゲーム進行、操作効果音。
- **設計上の特徴**：1成分のスライダーと領域全体の色／決定管理を分けている。
- **改善余地**：ホーム側RGB調整と共通ロジックを検討し、初期値とスライダー表示値の一致をテストする。
- **根拠**：`AdjustSlider.cs:13-35`、`AdjustUIAreaController.cs:13-94`

### 6. `CoreResource`

- **ファイル**：`Assets/Scripts/Constants/CoreResource.cs`
- **役割**：選択中の色データ、履歴、最高得点等の静的な実行時共有状態とUI補助処理。
- **主なメソッド**：`GetColorChannelData()`／`SetColorChannelData(...)`、`UpdateHighScore(...)`、`ChangePanelMotion(...)`。共有モード、セッション中の最高点、CSSクラスによるパネル切替を扱う。
- **設計上の特徴**：シーンを越えて参照できるため、ホームからゲームへモード情報を渡しやすい。
- **問題点**：永続化ではなくプロセス内状態。Playerビルドで色データ未設定時にnullとなる可能性がある。
- **改善余地**：初期値保証、null診断、責務分割。将来の保存仕様に合わせて保存層を導入する。
- **根拠**：`CoreResource.cs:10-88`

### 7. `ColorChannelDataSO`／`AudioDataSO`

- **ファイル**：`Assets/Scripts/ScriptableObjects/ColorChannelDataSO.cs`、`AudioDataSO.cs`
- **役割**：色チャンネルの表示データと、音声タグ／クリップの対応をUnityアセットとして保持する。
- **主なメソッド**：`ColorChannelDataSO.GetColorChannelData(...)`がジャンルから定義を検索する。`AudioDataSO.GetSEClip(string)`／`GetBGMClip(string)`はシリアライズ済み配列からタグを検索する。
- **連携**：ホーム初期化／モード選択、`AudioManager`の再生要求。
- **設計上の特徴**：設定をInspectorで差し替えられる。ただしデータモデル外の挙動追加はコード変更が必要。
- **改善余地**：重複タグ、未設定Clip、マテリアル数等の検証を追加。
- **根拠**：`ColorChannelDataSO.cs:9-41`、`AudioDataSO.cs:8-30`

### 8. `UIModuleManagerBase`と派生クラス

- **ファイル**：`Assets/Scripts/Constants/UIModuleManagerBase.cs`、`Assets/Scripts/Home/HomeUIModuleManager.cs`、`Assets/Scripts/GamePlay/GamePlayUIModuleManager.cs`
- **役割**：UI Toolkitのルート要素から名前付き要素を取得し、画面固有モジュールへ初期化・参照を渡す。
- **設計上の特徴**：ホームとゲームに共通するUI取得の雛形がある。
- **弱点**：文字列で指定した要素名に依存し、UXML名変更や欠落をコンパイル時に検出しにくい。
- **改善余地**：取得失敗の明示的なエラー、UI要素の参照整合性検証、モジュール初期化のテスト。
- **根拠**：`UIModuleManagerBase.cs:5-16`、`HomeUIModuleManager.cs:38-48`、`GamePlayUIModuleManager.cs:34-47`

### 9. `AudioManager`

- **ファイル**：`Assets/Scripts/Audio/AudioManager.cs`
- **主なメソッド**：`Awake()`でSingletonを設定し、`PlaySE(string)`／`PlayBGM(string)`がSOからタグ検索したClipを再生する。
- **依存関係**：`AudioDataSO`、AudioSource／AudioMixer等のシーン設定。
- **設計上の特徴**：呼び出し側が音源アセットの直接参照を持たず、キーで再生要求を行える。
- **改善余地**：未登録タグ・Clip未設定時の診断、音量設定の保存、オーディオ設定値の初期化検証。
- **根拠**：`AudioManager.cs:10-30`、`AudioDataSO.cs:8-30`

### 10. `JugementUI`／演出クラス

- **ファイル**：`Assets/Scripts/GamePlay/Result/JugementUI.cs`、`Assets/Scripts/Constants/CameraShake.cs`、`Assets/Scripts/GamePlay/GameClearParticleController.cs`
- **役割**：判定表示、効果音、ポストプロセス／カメラ揺れ、ランクに応じた最終演出。
- **依存関係**：TextMeshPro、Volume ProfileのBloom/Vignette、Cinemachine、Particle System等のUnityコンポーネント。
- **設計上の特徴**：採点規則と演出処理を別クラスに分けている。
- **問題点**：ポストプロセス参照が未取得でも後続処理で参照する経路があり、シーン設定不備時に例外となる可能性がある。
- **改善余地**：必須参照の初期化時検証と、任意演出が欠けても採点結果を表示できる分離。
- **主なメソッド**：`JugementUI.SetJugementText(...)`、`DisplayAndHide()`、`AssignPostProcessingEffect(...)`、`CameraShake.Shake(...)`、`GameClearParticleController.SpawnParticles(ScoreRank)`。
- **根拠**：`JugementUI.cs:70-132`、`CameraShake.cs:17-38`、`GameClearParticleController.cs:20-30`

## 重要度C：ホーム・設定・履歴

### 11. `HomeManager`／`HomeUIModuleManager`

- **ファイル**：`Assets/Scripts/Home/HomeManager.cs`、`Assets/Scripts/Home/HomeUIModuleManager.cs`
- **役割**：ホーム画面の初期モード設定、UIモジュール初期化、色プレビュー更新、BGM開始。
- **主な処理**：`HomeManager.Awake()`で最初の色チャンネルデータを共有状態へ設定し、`UpdatePreviewColor(Color)`で球／UI表示を更新、`Start()`でホームBGMを再生する。`HomeUIModuleManager.Initialize()`はモード・履歴・RGB編集・オプション等を初期化する。
- **改善余地**：初期モードを「リストの先頭」とする前提の明示と、空データ時の診断。
- **根拠**：`HomeManager.cs:17-40`、`HomeUIModuleManager.cs:26-72`

### 12. `SelectModePanel`／`SelectModeComposition`

- **ファイル**：`Assets/Scripts/Home/SelectMode/SelectModePanel.cs`、`SelectModeComposition.cs`
- **役割**：色モード選択肢を構築し、選択中モードを共有状態へ反映する。
- **主な処理**：`Initialize(...)`で色データごとに選択UIを生成し、選択時にモード／ハイスコア表示を更新する。パネル表示・戻る操作も扱う。
- **改善余地**：SOデータが空／重複時の扱いと、選択状態表示の検証。
- **根拠**：`SelectModePanel.cs:16-76`、`SelectModeComposition.cs:12-42`

### 13. `ScoreHistoryPanel`／`ScoreHistoryComposition`

- **ファイル**：`Assets/Scripts/Home/ScoreHistory/ScoreHistoryPanel.cs`、`ScoreHistoryComposition.cs`
- **役割**：メモリ上の最大100件の履歴を表示し、項目データにモード・ランク・得点・日時を保持する。
- **問題点**：`ScoreHistoryComposition`の45行目でローカルな`onClick`デリゲートを変更しており、ボタンの`clicked`イベントに項目コールバックが登録されない。
- **根拠**：`ScoreHistoryPanel.cs:16-59`、`ScoreHistoryComposition.cs:5-45`

### 14. `Option`

- **ファイル**：`Assets/Scripts/Home/Option/Option.cs`
- **役割**：BGM／SE音量スライダーをAudioMixerパラメーターへ接続し、オプションパネルを表示する。
- **改善余地**：スライダー値の永続化やMixerパラメーター名の検証は確認できない。
- **根拠**：`Option.cs:15-62`

### 15. `TopicSphereController`／`TopicPreviewAreaController`

- **ファイル**：`Assets/Scripts/GamePlay/TopicSphere/TopicSphereController.cs`、`Assets/Scripts/GamePlay/OtherInfo/TopicPreviewAreaController.cs`
- **役割**：見本色を球のマテリアルへ適用し、ゲームHUDのRGBモード最高点・タイマー・ラウンド数等を表示する。
- **主なメソッド**：`TopicSphereController.SetColor(Color)`、`TopicPreviewAreaController.Initialize(...)`、`UpdateTimer(...)`、`UpdateRoundCount(...)`、`UpdateMaxScore(...)`。
- **根拠**：`TopicSphereController.cs:8-23`、`TopicPreviewAreaController.cs:9-38`

## 関係のまとめ

`HomeManager`が色モードデータを `CoreResource` に設定し、`GameManager`がゲーム進行を統括する。ゲーム中の入力は `AdjustUIAreaController` が回答色として保持し、`ResultController`が結果データを作る。`JugementUI`と`RoundResult`がラウンド表示を行い、`AllRoundTotalResult`がゲーム全体の集計・終了処理を担う。オーディオと演出は各表示イベントから別クラスへ委譲される。
