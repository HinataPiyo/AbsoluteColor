# 技術課題

## 概要

実装上の既知の問題と制限事項をまとめた資料。各課題の影響範囲と改善優先度を明記。

## 採点・ランク判定の問題

### 問題 1: Cランク到達不可

**影響範囲** — ValueConstants.GetScoreRank()

**詳細** — スコアランク判定の条件順序が不正

```csharp
if (scorePercentage >= 0.95f) return ScoreRank.S;
else if (scorePercentage >= 0.9f) return ScoreRank.A;
else if (scorePercentage >= 0.7f) return ScoreRank.B;      // ← B条件が先
else if (scorePercentage >= 0.8f) return ScoreRank.C;      // ← C到達不可
else return ScoreRank.D;
```

scorePercentage が 0.70～0.79 の範囲の場合、B条件で判定されてしまい、Cランクの条件に到達しない。

**改善方針** — ランク条件を降順に並べ替え（S ≥ 0.95、A ≥ 0.90、B ≥ 0.80、C ≥ 0.70、D その他）、テストで確認（改善計画 P0）。

---

### 問題 2: 得点計算基準と正規化値の関連性が不明確

**影響範囲** — ValueConstants、AllRoundTotalResult

**詳細** — 
- MaxScore = 750
- 最大係数 = 1.75
- 1ラウンド最大得点 = round(750 × 1.75) = **1313点**
- 5ラウンド最大合計 = 1313 × 5 = **6565点**

一方、ランク計算の正規化値分母：
```
denominator = 5 × 750 × 1.75 = 6562.5
```

最大合計6565点が分母6562.5を上回ること、MaxScore定数が実装上の最大値と対応していないこと。

**改善方針** — 表示上限（1ラウンドの最大表示スコア）、計算基準（MaxScore）、ランク正規化分母を明確に定義し、式を統一（改善計画 P1）。

---

## UIイベント処理の問題

### 問題 3: ScoreHistoryComposition でのクリックイベント登録失敗

**影響範囲** — ScoreHistoryComposition.cs

**詳細** — 履歴項目ボタンのクリックイベントがUIに登録されていない

```csharp
// 登録されるはずだがUI側に反映されていない可能性
button.clicked += onClick;
```

クリック時のコールバック実行が期待値と異なる。

**原因** — UI要素の参照取得または登録タイミングの問題と推測。

**改善方針** — button.clicked への登録を直接確認し、テストで動作を検証（改善計画 P1）。

---

### 問題 4: BackButton での イベント解除失敗

**影響範囲** — BackButton.cs

**詳細** — コンストラクタ内で登録したラムダをReset()で解除しようとするが、異なるラムダ参照により解除が機能しない

```csharp
// コンストラクタ
button.clicked += () => { /* 処理 */ };  // ラムダA

// Reset()
button.clicked -= () => { /* 処理 */ };  // ラムダB (新規オブジェクト)
```

ラムダA と ラムダB は別のデリゲートオブジェクトのため、-= で削除されない。結果、ハンドラが残存し、複数回初期化時に意図しない多重登録が発生。

**改善方針** — デリゲートをフィールドに保持し、同じ参照で登録・解除（改善計画 P1）。

---

## 実行時データ管理の問題

### 問題 5: CoreResource の Null チェック

**影響範囲** — CoreResource.GetColorChannelData()

**詳細** — Player実行時（ゲーム開始直後）に色データが未設定の場合、nullを返す可能性

```csharp
public static ColorChannelDataSO GetColorChannelData()
{
    return selectedChannelData;  // nullの可能性
}
```

nullをそのまま参照すると、マテリアル取得等で NullReferenceException が発生。

**改善方針** — 初期化時に必須データをチェックし、不足時に分かりやすい診断メッセージを出力（改善計画 P1）。

---

### 問題 6: 永続化がない

**影響範囲** — CoreResource、AllRoundTotalResult

**詳細** — スコア履歴、最高得点、選択モード、オプション（音量）がアプリ再起動で失われる

現在：
- CoreResource の Stack（最大100件）はメモリ上
- PlayerPrefs や ファイル保存なし

**改善方針** — 保存する項目と保存先（PlayerPrefs、local JSON等）を決定してから実装（改善計画 P2）。

---

## 初期化・参照設定の問題

### 問題 7: UI要素の名前検索における変更検出不可

**影響範囲** — UIModuleManagerBase と派生クラス

**詳細** — UI Toolkit要素を文字列で名前検索

```csharp
Label scoreLabel = root.Q<Label>("score-label");
```

UI UXML ファイルで要素名を変更した場合、コンパイル時に検出不可。実行時に null が返される。

**改善方針** — 初期化時に必須UI要素を検査し、見つからない要素名を含む診断メッセージを出力（改善計画 P3）。

---

### 問題 8: Input System との不一致

**影響範囲** — Input System 1.19.0、InputSystem_Actions.inputactions

**詳細** — Input SystemパッケージとActionsアセットが含まれているが、ゲームスクリプトからは参照されていない。全操作はUI Toolkitイベントで処理。

**改善方針** — UI Toolkit入力のみ使用する場合は Input System 関連設定を整理。Input Actions との連携が必要な場合は実装（改善計画 P2）。

---

## Unityroom外部連携

### 問題 9: ランキング送信結果の未確認

**影響範囲** — AllRoundTotalResult.ShowAllRoundResult()

**詳細** — UnityroomApiClient.Instance.SendScore() を呼び出しているが、戻り値やエラー状態を処理していない

```csharp
UnityroomApiClient.Instance.SendScore(1, totalScoreValue, ScoreboardWriteMode.Always);
// 通信結果は確認されない
```

送信失敗時の処理がない。

**改善方針** — 外部サービス側の設定確認と、送信結果の確認・エラーハンドリング追加（改善計画 P2）。

---

## 性能測定・端末確認の未実施

### 問題 10: Profiler計測なし

**影響範囲** — ゲーム全体

**詳細** — CPU、GPU、メモリ使用量の計測を実施していない

**改善方針** — 対象端末を決定後、Profiler計測と実機動作確認を実施（改善計画 P2）。

---

## 影響度・優先度サマリー

| # | 課題 | 影響範囲 | 優先度 | 理由 |
|---|---|---|---|---|
| 1 | Cランク到達不可 | ゲーム仕様 | P0 | ユーザー体験に直結 |
| 2 | 得点基準の不明確性 | 採点計算、ランク判定 | P1 | 仕様明確化、テスト追加に必須 |
| 3 | 履歴クリック登録失敗 | ホーム画面 | P1 | ユーザー操作が機能しない |
| 4 | BackButton 解除失敗 | ホーム画面 | P1 | メモリリーク、多重登録の原因 |
| 5 | CoreResource Null チェック | 全般 | P1 | 実行時エラー防止 |
| 6 | 永続化がない | データ管理 | P2 | 利便性だが仕様次第 |
| 7 | UI要素名検出不可 | UI初期化 | P3 | 開発効率、保守性 |
| 8 | Input System未使用 | 入力システム | P2 | 設定整理の効率化 |
| 9 | ランキング送信未確認 | 外部連携 | P2 | 仕様確認・エラー対応 |
| 10 | 性能測定未実施 | ゲーム全体 | P2 | 最適化判断の基礎 |
