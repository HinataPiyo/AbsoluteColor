# AbsoluteColor — ドキュメント案内

## ゲーム紹介

[AbsoluteColorをUnityroomでプレイする](https://unityroom.com/games/absolutecolor01)

AbsoluteColorは、提示されたお題の色をRGBスライダーで再現するカラーマッチングゲームです。毎回ランダムに提示される色に対して、赤・緑・青の各成分を調整し、制限時間内に決定します。全5ラウンドをプレイし、ラウンドごとの色合わせ結果や総合結果を確認します。ホーム画面には、ゲーム開始前に色調整を試せるRGBプレビューもあります。

操作は、RGBスライダーで各色成分を調整し、決定ボタンを押して回答する形式です。詳細は[Unityroomのゲーム紹介・操作説明](https://unityroom.com/games/absolutecolor01)をご覧ください。

## この資料について

このフォルダーには、ゲームの企画・仕様・システム・実装技術をリポジトリと公開ページの情報に基づいて整理した資料があります。コード・設定で確認できた実装と、意図の推定、未確認事項、改善提案を区別しています。

公開ページはプレイヤー向けの紹介・操作説明、各設計資料はリポジトリ内のコード・設定に基づく技術分析を主な根拠としています。両者で説明の粒度や表現が異なる場合は、その旨を各資料で確認してください。公開ページに記載された内容を、すべてコードで確認済みの仕様として扱ってはいません。

## 資料一覧

| 資料 | 内容 |
|---|---|
| [GameOverview.md](Documentation/GameOverview.md) | ゲーム概要、企画上の位置づけ、基本ループ |
| [GameDesignDocument.md](Documentation/GameDesignDocument.md) | プレイヤー体験、ゲームデザイン、進行の考え方 |
| [GameSpecification.md](Documentation/GameSpecification.md) | 操作、ラウンド、採点、UI、遷移など実装確認済みの仕様 |
| [SystemArchitecture.md](Documentation/SystemArchitecture.md) | システム構成、クラス関係、処理・データフロー |
| [TechnicalDesign.md](Documentation/TechnicalDesign.md) | Unity環境、パッケージ、データ設計、技術選定と制約 |
| [TechnicalHighlights.md](Documentation/TechnicalHighlights.md) | 根拠付きの技術的アピールポイントと客観評価 |
| [ClassReference.md](Documentation/ClassReference.md) | 主要クラスの役割、依存、改善余地 |
| [CodeReview.md](Documentation/CodeReview.md) | 実装品質、確認できた問題、未確認事項 |
| [ImprovementPlan.md](Documentation/ImprovementPlan.md) | 優先度・難易度を付けた改善案 |
| [PortfolioTechnicalSummary.md](Documentation/PortfolioTechnicalSummary.md) | 採用面接・ポートフォリオ向けの説明整理 |

## 読み進め方

- **ゲームを手早く把握する**：[GameOverview.md](Documentation/GameOverview.md) → [GameSpecification.md](Documentation/GameSpecification.md)
- **コード構造を把握する**：[SystemArchitecture.md](Documentation/SystemArchitecture.md) → [ClassReference.md](Documentation/ClassReference.md)
- **技術評価と改善点を確認する**：[TechnicalHighlights.md](Documentation/TechnicalHighlights.md) → [CodeReview.md](Documentation/CodeReview.md) → [ImprovementPlan.md](Documentation/ImprovementPlan.md)
- **ポートフォリオ説明を準備する**：[PortfolioTechnicalSummary.md](Documentation/PortfolioTechnicalSummary.md)

## 技術概要

- Unity `6000.3.13f1`、C#、Universal Render Pipeline
- ホーム画面とゲームプレイ画面の2シーン
- UI Toolkitを中心としたRGBスライダー操作・プレビュー・結果表示
- Coroutineによる5ラウンドの進行
- ScriptableObjectによる色チャンネル関連データとBGM／SEデータの管理
- Cinemachine、ポストプロセス、アニメーション／パーティクル等による演出

詳細は[TechnicalDesign.md](Documentation/TechnicalDesign.md)を参照してください。パッケージの導入状況は実際の利用状況と同義ではなく、Input SystemのActionsアセットは存在しますが、ゲームスクリプトから利用するコードは確認できていません。

## 分析上の注意

- 設計資料はコード・Unity設定・アセットを中心にした現状分析です。Profilerによる性能測定、端末別の動作、Unityroom外部ランキングの通信結果などは検証していません。
- スコア計算では`MaxScore`の基準値としての意味に確認事項があり、ランク条件の順序によりCランクが到達不能です。詳細と改善案は[GameSpecification.md](Documentation/GameSpecification.md)および[ImprovementPlan.md](Documentation/ImprovementPlan.md)に記載しています。
- スコア履歴のクリックイベント登録など、コード上で確認できる不具合もあります。完成済み機能と改善提案を混同しないでください。
