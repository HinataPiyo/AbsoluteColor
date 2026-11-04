# 改善計画

ゲーム実装上の既知の課題に対する改善方針をまとめた資料。各改善項目の内容、実装手順、テスト方針を記載。

詳細は[技術課題](./CodeReview.md)を参照。

## 優先度体系

- **P0** — ユーザー体験に直結する機能不全、実装する
- **P1** — 仕様明確化やテスト追加に必須の重要課題、実装優先
- **P2** — 利便性・保守性向上、設定整理、仕様決定後に実装
- **P3** — 開発効率向上、拡張性改善、任意実装

## P0: ゲーム仕様の誤り

### P0-1: ランク判定条件の修正

**課題** — Cランク到達不可

**現在の状態**
```csharp
if (scorePercentage >= 0.95f) return ScoreRank.S;
else if (scorePercentage >= 0.9f) return ScoreRank.A;
else if (scorePercentage >= 0.7f) return ScoreRank.B;   // B先
else if (scorePercentage >= 0.8f) return ScoreRank.C;   // C到達不可
else return ScoreRank.D;
```

**改善内容** — 条件を降順に整列
```csharp
if (scorePercentage >= 0.95f) return ScoreRank.S;       // 95%以上
else if (scorePercentage >= 0.90f) return ScoreRank.A;  // 90以上95未満
else if (scorePercentage >= 0.80f) return ScoreRank.B;  // 80以上90未満
else if (scorePercentage >= 0.70f) return ScoreRank.C;  // 70以上80未満
else return ScoreRank.D;                                 // 70未満
```

**テスト方針**
- 各ランク境界の直前・同値・直後（例: 0.699, 0.70, 0.701）でテスト
- 全ランク（S/A/B/C/D）に到達できることを確認
- 既存のテストがあれば更新

**影響範囲** — ValueConstants.GetScoreRank()

**見積り** — 低（修正3行程度）

---

## P1: 実装上の重要課題

### P1-1: 得点とランク仕様の明確化

**課題** — MaxScore定義と実装上の最大得点が対応していない

**現在の状態**
- MaxScore = 750
- 最大係数 = 1.75
- 1ラウンド最大得点 = round(750 × 1.75) = 1313点
- 5ラウンド最大合計 = 6565点
- ランク正規化分母 = 5 × 750 × 1.75 = 6562.5

**改善内容**

1. **定数の役割を明確化**
   - MaxScore: UI表示値の基準か、計算基準か
   - 係数の意味（最大時の倍率、最小時の倍率）

2. **正規化値の定義を統一**
   ```csharp
   // 案: 実際の最大合計を分母に
   normalized = totalScore / (5 × 1313f);
   
   // 案: MaxScore基準に統一
   MaxScorePerRound = 1313;
   ```

3. **UI表示と計算基準を明確化**
   - 1ラウンドの表示上限は1313点か、750点か
   - AllRoundTotalResultに表示される最大合計スコア

**実装手順**
1. ゲーム仕様書で得点体系を明確化
2. ValueConstants の定数を整理
3. AllRoundTotalResult の計算式を更新
4. UIで表示する値の丸め処理を統一

**テスト方針**
- 完全一致（d=0）時の得点が期待値と一致
- ランク計算の分母が正規化値と対応
- 各ランク条件が正しく評価される

**影響範囲** — ValueConstants、AllRoundTotalResult、UI表示

**見積り** — 中（定義確認、計算式見直し、テスト追加）

---

### P1-2: 履歴ボタンのクリックイベント登録修正

**課題** — ScoreHistoryComposition でクリックハンドラが機能していない

**現在の状態**
```csharp
// 登録がUI側に反映されていない可能性
button.clicked += onClick;
```

**改善内容**

1. button.clicked への登録を直接実装
2. クリック時のコールバック（例：詳細表示、選択状態変更）を確認

**実装例**
```csharp
public class ScoreHistoryComposition
{
    private Button _historyButton;
    
    public ScoreHistoryComposition(VisualElement element, ScoreHistoryData data)
    {
        _historyButton = element.Q<Button>();
        if (_historyButton != null)
        {
            _historyButton.clicked += () => OnHistoryClicked(data);
        }
    }
    
    private void OnHistoryClicked(ScoreHistoryData data)
    {
        // 履歴アイテムがクリックされた時の処理
    }
}
```

**テスト方針**
- UI上のボタンが視認的に反応
- クリック時にコールバックが実行される
- 複数アイテムの独立したコールバック動作

**影響範囲** — ScoreHistoryComposition.cs

**見積り** — 低（登録パターン確認、テスト）

---

### P1-3: BackButton のイベント解除修正

**課題** — ラムダ参照の異なりにより、clicked イベント解除が機能しない

**現在の状態**
```csharp
public class BackButton
{
    public BackButton(Button button)
    {
        button.clicked += () => { /* 処理 */ };  // ラムダA
    }
    
    public void Reset(Button button)
    {
        button.clicked -= () => { /* 処理 */ };  // ラムダB (別オブジェクト)
        // ラムダB は登録されていないので何も起こらない
    }
}
```

**改善内容** — デリゲートをフィールドに保持
```csharp
public class BackButton
{
    private UnityAction _onClickHandler;
    
    public BackButton(Button button)
    {
        _onClickHandler = () => { /* 処理 */ };
        button.clicked += _onClickHandler;
    }
    
    public void Reset(Button button)
    {
        if (_onClickHandler != null)
        {
            button.clicked -= _onClickHandler;
        }
    }
}
```

**テスト方針**
- 初期化 → Reset → 再初期化 を複数回繰り返し、多重登録がないこと確認
- Reset() 後にボタンクリックがハンドラを実行しない

**影響範囲** — BackButton.cs

**見積り** — 低（フィールド追加、テスト）

---

### P1-4: CoreResource の Null チェック強化

**課題** — Player実行時に色データ未設定時の null リスク

**改善内容**

1. **初期化時のチェック**
```csharp
public static ColorChannelDataSO GetColorChannelData()
{
    if (selectedChannelData == null)
    {
        Debug.LogError(
            "ColorChannelData is not initialized. " +
            "Ensure HomeManager.Initialize() is called before GamePlayScene."
        );
    }
    return selectedChannelData;
}
```

2. **nullコアレッシング対応**
```csharp
Material[] materials = CoreResource.GetColorChannelData()?.Materials 
    ?? new Material[0];
```

3. **初期化診断ログ**
```
// Game初期化時に出力
[CoreResource] Initialization check:
  - ColorChannelData: OK
  - ScoreHistory: OK
  - MaxRound: 5
```

**テスト方針**
- Player実行時に初期化前アクセスをテスト
- nullセーフなコード経路を確認

**影響範囲** — CoreResource、GameManager、AllRoundTotalResult

**見積り** — 低（ガード追加）

---

## P2: 利便性・保守性向上

### P2-1: データ永続化の設計・実装

**課題** — スコア履歴等がアプリ再起動で失われる

**改善内容**

1. **保存対象の決定**
   - スコア履歴：保存する
   - 最高得点：保存する
   - 選択モード：保存する
   - オプション（音量）：保存する

2. **保存先の決定**
   - PlayerPrefs: 少量のデータ（最高得点、選択モード）
   - JSON ファイル: スコア履歴（大量データ向け）
   - StreamingAssets: 初期設定

3. **実装例（PlayerPrefs）**
```csharp
public static void SaveHighScore(string channelKey, int score)
{
    string key = $"HighScore_{channelKey}";
    PlayerPrefs.SetInt(key, score);
    PlayerPrefs.Save();
}

public static int LoadHighScore(string channelKey)
{
    string key = $"HighScore_{channelKey}";
    return PlayerPrefs.GetInt(key, 0);
}
```

4. **実装例（JSON）**
```csharp
[System.Serializable]
public class SaveData
{
    public List<ScoreHistoryData> scoreHistory;
}

public static void SaveToJson(string path, SaveData data)
{
    string json = JsonUtility.ToJson(data, true);
    System.IO.File.WriteAllText(path, json);
}
```

**テスト方針**
- 保存→再起動→読み込みの確認
- 破損ファイルの対応
- PlayerPrefs キー重複の確認

**影響範囲** — CoreResource、AllRoundTotalResult

**見積り** — 中（設計、実装、テスト）

---

### P2-2: Input System 設定の整理

**課題** — Input System が含まれているが未使用

**改善内容**

1. **UI Toolkit のみ使用する場合**
   - InputSystem_Actions.inputactions 削除またはマーク
   - Input System パッケージのバージョン管理明確化

2. **Input Actions を使用する場合**
   - GamePlayScene でアクション参照（例：スライダー入力の割り当て）
   - ゲームパッド対応等の拡張

**テスト方針** — 選択した方針に基づいて入力動作確認

**影響範囲** — Packages/manifest.json、入力処理全般

**見積り** — 低（設定整理）

---

### P2-3: Unityroom ランキング送信の確認・エラー処理

**課題** — 送信結果の未確認、エラー処理なし

**改善内容**

1. **外部サービス側の設定確認**
   - Unityroomボード ID=1 の設定確認
   - ScoreboardWriteMode の意味確認

2. **エラーハンドリング実装**
```csharp
private void SendScoreToUnityroom(int boardId, int score)
{
    try
    {
        UnityroomApiClient.Instance.SendScore(
            boardId, score, ScoreboardWriteMode.Always
        );
        Debug.Log($"[Unityroom] Score sent: {score}");
    }
    catch (System.Exception ex)
    {
        Debug.LogWarning($"[Unityroom] Send failed: {ex.Message}");
        // ユーザーに通知するか判断
    }
}
```

**テスト方針** — Unityroom管理画面でボード確認

**影響範囲** — AllRoundTotalResult.ShowAllRoundResult()

**見積り** — 中（外部仕様確認、エラー処理）

---

### P2-4: 性能測定・端末確認の実施

**課題** — Profiler計測、実機テスト未実施

**改善内容**

1. **計測対象端末の決定** — Web GL、PC、iOS、Android など

2. **Profiler 計測項目**
   - CPU 時間（主要処理ごと）
   - GPU メモリ使用量
   - フレームレート安定性

3. **計測手順**
   - Profiler Window で実行中に記録
   - ボトルネック箇所を特定
   - 必要な最適化を判断

**テスト方針** — 対象端末で一定時間プレイ

**影響範囲** — ゲーム全体

**見積り** — 中（計測、分析、最適化判断）

---

## P3: 開発効率・保守性

### P3-1: UI要素検出エラーの診断強化

**課題** — UI要素名変更時にコンパイル時検出できない

**改善内容** — 初期化時の要素検出失敗ログ

```csharp
public override void Initialize(VisualElement root)
{
    var missingElements = new List<string>();
    
    var scoreLabel = root.Q<Label>("score-label");
    if (scoreLabel == null) missingElements.Add("score-label");
    
    var nameField = root.Q<TextField>("name-field");
    if (nameField == null) missingElements.Add("name-field");
    
    if (missingElements.Count > 0)
    {
        Debug.LogError(
            $"Missing UI elements: {string.Join(", ", missingElements)}"
        );
    }
}
```

**テスト方針** — 要素名を意図的に変更してエラー検出

**影響範囲** — UIModuleManagerBase 派生クラス

**見積り** — 低（チェックコード追加）

---

## 実装フロー（推奨順序）

1. **P0-1** — ランク判定修正（即座）
2. **P1-1** → **P1-2** → **P1-3** → **P1-4** — 重要課題（次回反復）
3. **P2-1** — データ永続化（ユーザー利便性）
4. **P2-2** → **P2-3** → **P2-4** — その他P2項目
5. **P3-1** — 開発効率（参考実装）

---

## テスト方針の統一

### 単体テスト対象

- ValueConstants.GetScoreRank() — 全ランク到達確認
- ResultData.GetScore() — 境界値テスト
- CoreResource.GetColorChannelData() — null 安全性

### 統合テスト対象

- ゲーム全流れ（ホーム→プレイ→結果）
- 全ラウンド完了後のランク表示
- 履歴保存と再読み込み

### 手動テスト対象

- UI操作の応答性
- 音声・演出の同期
- 端末別の表示・パフォーマンス