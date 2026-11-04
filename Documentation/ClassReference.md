# クラスリファレンス

## ゲーム進行・管理

### GameManager

**ファイルパス** — `Assets/Scripts/GamePlay/GameManager.cs`

**責務** — 5ラウンドのゲーム進行フローを制御。見本色生成、タイマー管理、採点タイミングの判定。

**管理データ**
- `topicColor` — 現在のラウンドの見本RGB
- `isPlayerDecided` — プレイヤーの決定フラグ

**主な処理**
- `Awake()` — UIコンポーネント参照を取得
- `Start()` — BGM再生、GameLoop()開始
- `GameLoop()` — ラウンド開始から総合結果まで5ラウンドのフローをCoroutineで制御
- `DesideTopicColor()` — 0～1の範囲でランダムなRGB値を生成
- `IgnorePlayerInput()` — 結果表示中のプレイヤー入力を無視

**依存関係** — TopicSphereController、ResultController、JugementUI、AllRoundTotalResult、AdjustUIAreaController、TopicPreviewAreaController

**パターン** — 静的シングルトン `GameManager.I`

---

## 採点・結果

### ResultController

**ファイルパス** — `Assets/Scripts/GamePlay/Result/ResultController.cs`

**責務** — 見本色と回答色からResultDataを生成し、採点結果をUIに表示。

**管理データ**
- `resultColors` — List<ResultData>。ラウンドごとの結果を保持

**主な処理**
- `CreateResultData()` — TopicColorとPlayerColorからResultDataを新規作成
- `ShowResult()` — ラウンド結果UIパネルを開き、RoundResultを構築
- `GetJugement()` — ResultDataから判定タイプとパーセンテージを取得

**関連クラス** — ResultData、RoundResult

---

### ResultData

**ファイルパス** — `Assets/Scripts/GamePlay/Result/ResultController.cs`内の部分クラス

**責務** — 見本色・回答色・ラウンド番号を保持し、色差と採点に関する計算を行う。

**管理データ**
- `TopicColor` — 見本色（Color型）
- `AnswerColor` — 回答色（Color型）
- `RoundCount` — ラウンド番号（1～5）

**主な処理**
- `GetDifferenceByValue()` — RGB各成分の絶対差を平均して返す（0.0～1.0）
- `GetScore()` — 色差からスコアを計算（最大1313点）
- `GetJugementType()` — 色差からJugementType (Perfect/Excellent/Great/Good/Bad) を判定
- `GetParcentage()` — スコア割合を計算（0～100）

---

### JugementUI

**ファイルパス** — `Assets/Scripts/GamePlay/Result/JugementUI.cs`

**責務** — ラウンド採点時の判定表示、効果音、画面演出を制御。

**管理データ** — TextMeshPro、Volume、Animator、CameraShakeへの参照（Inspectorで設定）

**主な処理**
- `SetJugementText()` — 判定タイプとパーセンテージをUI表示
- `DisplayAndHide()` — 判定表示アニメーション実行
- `ResetPostProcessingEffect()` — ポストプロセス効果（Bloom、Vignette）をリセット

**依存関係** — AudioManager（効果音）、CameraShake、Post Processing Volume

---

### AllRoundTotalResult

**ファイルパス** — `Assets/Scripts/GamePlay/Result/AllRoundTotalResult.cs`

**責務** — 5ラウンド全体の結果集計、総合スコア計算、ランク判定、履歴記録、UI表示。

**管理データ**
- `resultScrollView` — ラウンド結果をスクロール表示するUI
- `totalScore` — 合計スコア表示ラベル
- `scoreRank` — ランク表示ラベル

**主な処理**
- `ShowAllRoundResult()` — 5ラウンドの結果をスクロールビューに追加し、合計スコアとランクを計算・表示
- `CalculateTotalScore()` — 各ラウンドのスコアを合算
- CoreResourceへ履歴追加、Unityroomへスコア送信

**出力** — ScoreRank（S/A/B/C/D）

---

### RoundResult

**ファイルパス** — `Assets/Scripts/GamePlay/Result/RoundResult.cs`

**責務** — 1ラウンドの結果をUIに表示。見本色、回答色、スコア、判定を描画。

**入力** — VisualElement、Material配列、ResultData、コールバック

**主な処理**
- コンストラクタで受け取ったResultDataからスコア、判定、色を抽出してUI更新
- 「次へ」ボタンのクリックコールバック設定

---

## 入力・RGB調整

### AdjustUIAreaController

**ファイルパス** — `Assets/Scripts/GamePlay/AdjustColor/AdjustUIAreaController.cs`

**責務** — ゲーム画面のRGBスライダーコンポーネント管理、入力値保持、プレビュー更新。

**管理データ** — R/G/Bスライダー（0.0～1.0）、現在のRGB値

**主な処理**
- スライダーのchangeイベント登録
- 決定ボタンのclickedイベント登録で `GameManager.I.isPlayerDecided = true`
- `GetDecidedColor()` — 現在のスライダー値をColor型で返す

**関連クラス** — AdjustSlider、PreviewSlider

---

### AdjustSlider

**ファイルパス** — `Assets/Scripts/GamePlay/AdjustColor/AdjustSlider.cs`

**責務** — 個別のRGBスライダーの値変更イベント処理、プレビュー更新。

**主な処理**
- スライダーのvalueプロパティ変更をイベント取得
- 色プレビューに反映

---

## データ・状態管理

### CoreResource

**ファイルパス** — `Assets/Scripts/Constants/CoreResource.cs`

**責務** — 実行時の共有状態を静的に管理。選択中の色データ、スコア履歴、最高得点。

**管理データ**（全てstatic）
- `GetColorChannelData()` — 選択中のColorChannelDataSO（nullの可能性あり）
- `ScoreHistoryDatas` — Stack<ScoreHistoryData>（最大100件）
- 色チャンネルごとの最高得点

**注意点**
- Player実行時に色データ未設定ならnullを返すリスク
- アプリ再起動後の永続化なし
- テスト時のリセット困難

---

### ValueConstants

**ファイルパス** — `Assets/Scripts/Constants/ValueConstants.cs`

**責務** — ゲーム全体の定数定義。ラウンド数、タイマー、得点係数、判定条件。

**定数**
- `MaxRound = 5`
- `MaxScore = 750`
- `InitialOneRoundTimeLimit = 15f`
- `MaxScoreCoefficient = 1.75f`（完全一致時の倍率）
- `MinScoreCoefficient = 0.1f`（完全不一致時の倍率）

**主な処理**
- `GetScoreRank()` — 合計スコアの正規化値からランクを判定
  - **既知の誤り** — B条件（≥0.70）がC条件（≥0.80）より先に評価されるため、Cランク到達不可
- `GetScoreCoefficient()` — 色差から得点係数をLerpで計算
- `GetScore()` — 色差からスコアを計算

**ランク判定テーブル**
```
ScoreRankColors — ランク別のカラー（UI表示用）
```

---

## UI・ホーム画面

### HomeManager

**ファイルパス** — `Assets/Scripts/Home/HomeManager.cs`

**責務** — ホーム画面の初期化、色チャンネル選択データの設定、BGM再生。

**主な処理**
- ColorChannelDataSO配列から色チャンネルデータを読み込み
- CoreResourceへ初期データ設定（配列の先頭を使用）
- ホーム画面BGM再生

---

### SelectModePanel

**ファイルパス** — `Assets/Scripts/Home/SelectMode/SelectModePanel.cs`

**責務** — 色チャンネル選択UI。ユーザー操作を受けてColorChannelDataSOをCoreResourceに設定。

**主な処理**
- 各モードボタンのクリックイベント登録
- クリック時にCoreResourceへ選択データを設定

---

### ScoreHistoryPanel、ScoreHistoryComposition

**ファイルパス** — `Assets/Scripts/Home/ScoreHistory/ScoreHistoryPanel.cs`、`ScoreHistoryComposition.cs`

**責務** — スコア履歴UIの表示と管理。

**管理データ** — CoreResourceのScoreHistoryDatas Stack（最大100件）

**既知の課題** — ScoreHistoryComposition でクリックイベントをボタンに登録できていない

---

### AudioManager

**ファイルパス** — `Assets/Scripts/Audio/AudioManager.cs`

**責務** — BGM/SE再生。AudioDataSOからタグでクリップを検索して再生。

**主な処理**
- `PlayBGM()` — タグからBGM用AudioClipを取得して再生
- `PlaySE()` — タグからSE用AudioClipを取得して再生
- 存在しないタグの場合は警告出力してnull返却

**パターン** — 静的シングルトン `AudioManager.I`

**依存関係** — AudioDataSO

---

## データアセット

### ColorChannelDataSO

**ファイルパス** — `Assets/Scripts/ScriptableObjects/ColorChannelDataSO.cs`

**責務** — 色チャンネル関連情報をScriptableObjectで管理。

**保持データ**
- 説明テキスト（UI表示用）
- 表示名
- マテリアル配列（プレビュー用）

**参照元** — HomeManager、CoreResource

---

### AudioDataSO

**ファイルパス** — `Assets/Scripts/ScriptableObjects/AudioDataSO.cs`

**責務** — BGM/SEクリップをタグで管理。

**構造**
- タグ（"GamePlayScene", "SpawnTopic", "NormalButton"等）
- AudioClip参照

**アクセス** — AudioManager がタグで検索

---

## 情報表示

### TopicSphereController

**ファイルパス** — `Assets/Scripts/GamePlay/TopicSphere/TopicSphereController.cs`

**責務** — 見本色を球体で表示。

**主な処理**
- `SetColor()` — 見本RGB値を受け取り、マテリアル色を更新

---

### TopicPreviewAreaController

**ファイルパス** — `Assets/Scripts/GamePlay/OtherInfo/TopicPreviewAreaController.cs`

**責務** — ラウンド番号、残り時間などの情報表示更新。

**主な処理**
- `UpdateRoundCount()` — ラウンド番号表示
- `UpdateTimer()` — 残り秒数表示

---

## UI基盤

### UIModuleManagerBase

**ファイルパス** — `Assets/Scripts/Constants/UIModuleManagerBase.cs`

**責務** — UI Toolkit要素取得とUIModuleController初期化。

**主な処理**
- `Initialize()` — UIDocumentからUI要素をQ<T>で名前検索
- 派生クラスで画面別モジュールを初期化

**派生クラス**
- HomeUIModuleManager
- GamePlayUIModuleManager

**注意点** — 要素名の変更をコンパイル時に検出できない

---

## 演出・オブジェクト管理

### CameraShake

**ファイルパス** — `Assets/Scripts/Constants/CameraShake.cs`

**責務** — カメラ揺れ演出をCoroutineで制御。

---

### GameClearParticleController

**ファイルパス** — `Assets/Scripts/GamePlay/GameClearParticleController.cs`

**責務** — 最終ランクに応じたパーティクル生成。

**主な処理**
- `SpawnParticles()` — ランク別の演出パーティクルを生成

---

## スコア・履歴

### ScoreHistoryData

**責務** — スコア履歴の1件分（色チャンネル、ランク、スコア、時刻）を保持。

---

## クラス間の主要フロー

1. **HomeManager** — ColorChannelDataSOを選択してCoreResourceに設定
2. **GameManager** — CoreResourceから色データを参照、ゲーム開始
3. **ゲーム進行中** — AdjustUIAreaController からRGB値を取得
4. **採点** — ResultController が ResultDataを生成
5. **結果表示** — RoundResult、AllRoundTotalResult でUI描画
6. **音声・演出** — JugementUI、CameraShake、GameClearParticleController
