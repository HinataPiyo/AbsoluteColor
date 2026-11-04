# AbsoluteColor — ドキュメント案内

## ゲーム紹介

[AbsoluteColorをUnityroomでプレイする](https://unityroom.com/games/absolutecolor01)

AbsoluteColorは、ランダムに提示される見本色にRGBスライダーで近づけ、制限時間内に回答する5ラウンド制のカラーマッチングゲームです。ラウンドごとの色合わせを採点し、最後に総合結果を表示します。ホーム画面ではプレイ前にRGBの色調整を試せます。

操作方法は[Unityroomのゲーム紹介・操作説明](https://unityroom.com/games/absolutecolor01)をご覧ください。

## この資料について

このフォルダーでは、ゲームの仕様、画面構成、C#実装、現在の課題を紹介しています。実装している機能と、今後対応したい内容を分けて記載しています。

## 資料一覧

| 資料 | 内容 |
|---|---|
| [GameOverview.md](Documentation/GameOverview.md) | ゲーム概要と基本ループ |
| [GameDesignDocument.md](Documentation/GameDesignDocument.md) | プレイヤー操作とゲーム進行 |
| [GameSpecification.md](Documentation/GameSpecification.md) | 操作、ラウンド、採点、UI、遷移 |
| [SystemArchitecture.md](Documentation/SystemArchitecture.md) | システム構成、クラス関係、データフロー |
| [TechnicalDesign.md](Documentation/TechnicalDesign.md) | Unity環境、パッケージ、データ設計 |
| [TechnicalHighlights.md](Documentation/TechnicalHighlights.md) | 技術的な見どころと実装上の課題 |
| [ClassReference.md](Documentation/ClassReference.md) | 主要クラスと担当処理 |
| [CodeReview.md](Documentation/CodeReview.md) | 実装内容、現在の課題、改善案 |
| [ImprovementPlan.md](Documentation/ImprovementPlan.md) | 優先して取り組みたい改善 |
| [PortfolioTechnicalSummary.md](Documentation/PortfolioTechnicalSummary.md) | ポートフォリオ向け技術紹介 |

## 読み進め方

- **ゲームを知る**：[GameOverview.md](Documentation/GameOverview.md) → [GameSpecification.md](Documentation/GameSpecification.md)
- **コード構成を知る**：[SystemArchitecture.md](Documentation/SystemArchitecture.md) → [ClassReference.md](Documentation/ClassReference.md)
- **技術と課題を知る**：[TechnicalHighlights.md](Documentation/TechnicalHighlights.md) → [CodeReview.md](Documentation/CodeReview.md) → [ImprovementPlan.md](Documentation/ImprovementPlan.md)
- **作品を紹介する**：[PortfolioTechnicalSummary.md](Documentation/PortfolioTechnicalSummary.md)

## 技術概要

- Unity `6000.3.13f1`、C#、Universal Render Pipeline
- ホーム画面とゲームプレイ画面の2シーン
- UI ToolkitでRGBスライダー、プレビュー、結果画面を実装
- Coroutineで5ラウンドを進行
- ScriptableObjectで色チャンネル関連データとBGM／SEデータを管理
- Cinemachine、ポストプロセス、アニメーション、パーティクルによる演出

Input SystemパッケージとActionsアセットはプロジェクトに含まれていますが、`Assets/Scripts`にはInput Systemを使うコードがありません。現在の操作はUI Toolkitのイベントで処理しています。詳細は[技術設計](Documentation/TechnicalDesign.md)をご覧ください。

## 現在の課題

- 得点式では`MaxScore`に1.75倍の係数を掛け、完全一致時は1ラウンドで丸め後1313点になります。総合ランクの正規化式との関係を整理したいと考えています。
- ランク判定でBの条件がCより先に評価されるため、現在の実装ではCランクに到達できません。
- スコア履歴の項目にクリック処理を登録できておらず、バックボタンのイベント解除にも問題があります。
- Profilerでの性能測定、端末別の動作確認は行っていません。Unityroomランキングは送信を呼び出すコードを実装していますが、通信結果は確認していません。

詳しくは[ゲーム仕様](Documentation/GameSpecification.md)と[改善計画](Documentation/ImprovementPlan.md)をご覧ください。