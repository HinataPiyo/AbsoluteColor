# 技術設計

## 1. 開発環境

| 項目 | 内容 |
|---|---|
| エンジン | Unity 6000.3.13f1 |
| 言語 | C#（.NET Framework） |
| スクリプトアセンブリ | Assembly-CSharp、Assembly-CSharp-Editor |
| 描画パイプライン | Universal Render Pipeline 17.3.0 |
| UIフレームワーク | UI Toolkit（判定テキスト表示にはTextMeshPro併用） |
| 入力処理 | UI Toolkitのイベントシステム（Input System 1.19.0パッケージは含むが未使用） |
| 主要パッケージ | Cinemachine 3.1.6、Unity Test Framework 1.6.0、Unityroom client |

すべてのバージョン情報は `Packages/manifest.json` に基づく。パッケージが導入されていても、ゲームスクリプトから参照されているとは限らない。

## 2. アーキテクチャ設計

### 制御フロー

GameManagerのCoroutineが5ラウンド進行の中心。ラウンド開始、見本色生成、入力待機、採点、結果表示を順序制御。結果UIが表示されている間もCoroutineで待機。

### 責務分離

- **進行管理** — GameManager（ラウンドループ、タイマー）
- **入力処理** — AdjustUIAreaController（スライダー値管理）
- **情報表示** — TopicSphereController, TopicPreviewAreaController（見本色、タイマー表示）
- **採点計算** — ResultData（色差、判定、得点算出）
- **結果表示** — RoundResult, AllRoundTotalResult（UIレンダリング）
- **演出** — JugementUI, CameraShake（判定表示、視覚効果）
- **データ管理** — CoreResource（実行時状態）、ColorChannelDataSO、AudioDataSO（編集時データ）

### 参照パターン

**DirectInspector参照** — GameManager、AllRoundTotalResultがSerializeFieldで依存コンポーネントを参照

**動的取得** — UIModuleManagerBase派生クラスがUIDocumentからUIModuleControllerを取得

**静的参照** — GameManager.I、AudioManager.I（シングルトン）、CoreResource（staticクラス）

### 依存関係の複雑性

GameManagerが複数の具体クラス（ResultController、JugementUI、AllRoundTotalResult等）に直接依存。テスト困難性とメンテナンス性の低下につながる可能性がある。

## 3. データモデル

### 編集時データ（ScriptableObject）

**ColorChannelDataSO**
- 色チャンネルの説明、表示名、マテリアル参照を保持
- Inspectorで値を変更可能
- 複数のモードを配列管理

**AudioDataSO**
- タグ（文字列キー）とAudioClip参照をペアで保持
- Inspectorで音声クリップを追加・変更可能
- 存在しないタグでは警告出力してnullを返す

### 実行時データ

**CoreResource**（static）
- 選択中のColorChannelDataSO
- スコア履歴（Stack、最大100件）
- 色チャンネルごとの最高得点
- ファイルシステムおよびPlayerPrefsへの永続化なし

**ResultData**
- TopicColor（見本色、Color型）
- AnswerColor（回答色、Color型）
- RoundCount（ラウンド番号、int型）
- 色差、判定、得点は計算時に導出

### 拡張方針

新しいゲーム動作を追加する場合、ColorChannelDataSOの構造変更やGameManagerのCoroutine追加が必要。パラメータ化できる範囲（タイマー秒数、判定閾値など）はValueConstantsで定義。

## 4. 色処理

### 色空間

RGB値を0.0～1.0の正規化範囲で管理。Unityの標準Color型を使用。

### 色差計算

```
d = (|Ar - Tr| + |Ag - Tg| + |Ab - Tb|) / 3
```

RGB各成分の絶対差を平均した単純な距離度量。知覚色差（Lab色空間、ΔE等）は未実装。この方式の限界：
- RGB空間での距離が知覚的距離と対応しない
- 色覚の人間は色相、彩度、明度に異なる感度を持つ

### 判定アルゴリズム

色差値 d に対して段階的な判定を行い、それぞれに対応した係数でスコアを計算。スコアの最大値は知覚的に「完全一致」した場合ではなく、計算上のd=0での結果。

## 5. Coroutineによるゲーム進行

### GameLoop()メソッド

```csharp
IEnumerator GameLoop()
{
    for (int round = 1; round <= MaxRound; round++)
    {
        // ラウンド初期化
        topicPreviewArea.UpdateRoundCount(round);
        DesideTopicColor();
        IgnorePlayerInput(false);
        
        // タイマー処理
        float timeLimit = InitialTime * Mathf.Pow(0.9f, round - 1);
        float elapsed = 0f;
        
        while (elapsed < timeLimit)
        {
            topicPreviewArea.UpdateTimer(timeLimit - elapsed);
            elapsed += Time.deltaTime;
            if (isPlayerDecided) break;
            yield return null;
        }
        
        // 採点・結果表示
        isPlayerDecided = false;
        IgnorePlayerInput(true);
        ResultData resultData = result.CreateResultData(...);
        jugementUI.SetJugementText(...);
        yield return StartCoroutine(jugementUI.DisplayAndHide());
        result.ShowResult();
        
        // 結果表示待機
        yield return new WaitUntil(() => !result.IsResultDisplayed);
        jugementUI.ResetPostProcessingEffect();
    }
    
    // ゲーム終了処理
    ScoreRank rank = allRoundTotalResult.ShowAllRoundResult(...);
    gameClearParticle.SpawnParticles(rank);
}
```

各ラウンドをfor ループ内で順序制御。入力待機、採点、結果表示をyield で段階的に実行。

### タイマー実装

タイマーはCoroutineのwhileループでTime.deltaTimeを加算。実装例：

```csharp
float timeLimit = 15f * Mathf.Pow(0.9f, roundNumber - 1);
float elapsedTime = 0f;
while (elapsedTime < timeLimit)
{
    UpdateDisplay(timeLimit - elapsedTime);
    elapsedTime += Time.deltaTime;
    if (isPlayerDecided) break;
    yield return null;
}
```

フレームベースのタイマーのため、フレームレートが不安定な場合は時間計測精度に影響。

## 6. UI Toolkit統合

### UI構成

UI Toolkitでスライダー、ボタン、パネルなどの要素を構成。入力はUI ToolkitのIEventHandlerインターフェース経由で処理。

### イベント処理の課題

- **登録寿命管理** — RegisterCallback/UnregisterCallbackでの登録・解除タイミングが脆弱
- **ラムダキャプチャ** — BackButton等でコンストラクタ内のラムダと解除時ラムダが異なる場合、解除が機能しない
- **要素検索** — UI要素を文字列で名前検索するため、要素名変更時にコンパイル時検出不可

## 7. Unity技術の利用

| 技術 | 用途 | 現状 |
|---|---|---|
| MonoBehaviour | ゲーム進行、UI制御、演出管理 | 複数のMonoBehaviourをGameManagerから参照 |
| ScriptableObject | 色・音声データ編集 | ColorChannelDataSO、AudioDataSO |
| Coroutine | ラウンド進行、タイマー、カメラ揺れ | GameLoop()で5ラウンドを順序制御 |
| UI Toolkit | 画面UI構築、イベント処理 | スライダー、ボタン、パネルなど。Input Systemは未使用 |
| TextMeshPro | 判定テキスト表示 | JugementUIで判定結果を描画 |
| Cinemachine | カメラ揺れ | CameraShakeで判定時の画面振動を実装 |
| Particle System | 演出パーティクル | GameClearParticleController でランク別の演出 |
| Volume / Post Processing | ポストプロセス | Bloom、Vignette等を判定演出に使用 |
| Animator | アニメーション | UI要素やオブジェクトのアニメーション制御 |

## 8. オブジェクト管理と性能

### オブジェクト生成

- **RoundResult** — ラウンド結果UIをTemplateから毎回生成、クリック処理をセット
- **AllRoundTotalResult** — 総合結果表示時にスクロールビューへ5つのRoundResultを追加

生成量は少ないため、大規模な最適化不要と考える。Profilerでの計測は実施していない。

### メモリ使用

スコア履歴はCoreResourceのStack（最大100件）で保持。永続化がないため、アプリ再起動時にリセット。

### パフォーマンス測定

- CPU性能測定 — Profilerでの実施なし
- GPU性能測定 — URP設定はあるもMobile向け確認なし
- メモリプロファイル — 計測なし
- 実機動作確認 — 対象端末での確認なし

## 9. 外部連携

### Unityroom ランキング

AllRoundTotalResult.ShowAllRoundResult()内でUnityroomApiClient.Instance.SendScore()を呼び出し。ボードID=1で固定。通信結果の確認やエラーハンドリングは未実装。

### Input System

Input System 1.19.0とInputSystem_Actions.inputactionsアセットが含まれるが、ゲームスクリプトから参照されていない。UI Toolkitイベントで全操作を処理。

## 10. 実装上の特徴

### スコア計算式の複雑性

MaxScore（750）に係数（1.75～0.1の範囲）を掛けた結果を整数丸め。完全一致時は1313点。5ラウンド合計の最大値は6565点だが、ランク正規化の分母は6562.5。この不一致が改善対象（ImprovementPlan P1）。

### 判定ランク条件の誤り

ValueConstants.GetScoreRank()でB（≥0.70）がC（≥0.80）より先に評価されるため、Cランクに到達不可。改善対象（ImprovementPlan P0）。

### 実行時状態の集中

CoreResourceがstatic クラスで状態を一元管理。テスト時のリセット困難、複数シーン間でのデータ共有の脆弱性がある。