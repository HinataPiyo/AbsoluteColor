# 企業資料版 — AbsoluteColor 技術サマリー

本資料は、本ゲームの技術構成・実装方針・開発手法を概括的に説明する企業開発資料版です。

## 作品概要

AbsoluteColorは、RGBスライダーでランダムな見本色に近づける5ラウンド制のカラーマッチングゲーム。Unity 6、Universal Render Pipeline、UI Toolkit、ScriptableObjectを用いて実装。

## 技術選択

| 要素 | 選択技術 | 理由 |
|---|---|---|
| エンジン | Unity 6000.3.13f1 | 最新安定版 |
| 描画 | Universal Render Pipeline 17.3.0 | モダン描画パイプライン、Mobile対応 |
| UI | UI Toolkit | 高速開発、パフォーマンス |
| 言語 | C# | Unity標準、型安全性 |
| ゲーム進行 | Coroutine | 手続き的ゲームループの表現性 |
| データ管理 | ScriptableObject | 編集効率、参照型管理 |
| 外部連携 | Unityroom | ゲーム配信、ランキング機能 |

## アーキテクチャの特徴

### 責務分離

進行制御（GameManager）、入力処理（AdjustUIAreaController）、採点計算（ResultData）、結果表示（RoundResult、AllRoundTotalResult）、演出（JugementUI、CameraShake）を独立したクラスに分割。

### データフロー

シーン間のデータ共有をCoreResourceで一元管理。結果情報をResultDataにまとめることで、判定UI、ラウンド結果、総合結果の各表示へ段階的に渡す。

### UI Toolkitの活用

スライダー値変更、ボタンクリックをUI Toolkitのイベントシステムで処理。UI Toolkit のvisualElement query（Q<T>）で要素を取得し、UIModuleManagerBase 派生クラスで画面ごとのモジュール初期化を統一。

## 実装上の工夫

### 1. Coroutineによるゲームループ制御

5ラウンドの進行をGameManager.GameLoop()で順序制御。各ラウンド内では、入力待機をwhile（タイマー）、結果待機をyield until で表現。コードの手続き的な流れがゲームの流れと対応し、デバッグ時の進行状態把握が容易。

### 2. 採点計算の一元化

色差計算、判定、得点算出をResultDataのメソッドにまとめ、計算ロジックと表示処理の分離を実現。ラウンド結果、総合結果の各表示が同じResultData参照から独立して情報を取得。

### 3. ScriptableObjectによるデータ管理

色チャンネル情報（説明、名前、マテリアル）と音声クリップ（BGM、SE）をScriptableObject で外部管理。新しい色チャンネルやBGM を追加する際、C#コード修正なしにInspectorで追加可能。

### 4. UI Toolkitの静的参照避難

Inspector設定に頼らず、UIModuleManagerBase.Initialize()でUI Toolkit要素を名前で動的取得。複数シーン間の参照管理を簡潔に。

## 色差と採点方式

### 色差計算

```
difference = (|Ar - Tr| + |Ag - Tg| + |Ab - Tb|) / 3
```

RGB各成分の正規化値の絶対差を平均した単純な距離度量。知覚色差（Lab、ΔE）は未実装。設計理由：計算簡潔性、ユーザーの色感覚のRGB軸への対応付けの明確性。

### 得点式

```
score = round(750 × Lerp(1.75, 0.1, difference))
```

色差0（完全一致）で1313点、色差1.0（完全相違）で75点。線形補間により段階的なスコア分布を実現。

## 既知の制限事項と改善方針

### 現在の制限

- **Cランク到達不可** — ランク判定条件の順序誤り（改善計画 P0）
- **得点基準の非標準化** — MaxScore定義と実装上の最大値の相違（改善計画 P1）
- **UIイベント登録の脆弱性** — ラムダ参照管理の不正さ（改善計画 P1）
- **永続化なし** — スコア履歴等がアプリ再起動で失われる（改善計画 P2）
- **性能測定未実施** — Profiler計測、実機テストの未実施（改善計画 P2）

### 改善の進め方

優先度体系（P0 > P1 > P2 > P3）に基づき、ユーザー体験の誤り → 実装上の重要課題 → 利便性向上 → 開発効率の順で対応。詳細は[改善計画](./ImprovementPlan.md)を参照。

## 技術課題の設計的背景

### static状態の使用

GameManager.I、AudioManager.I、CoreResourceをsingleton/staticで実装。シーン間でのデータ共有を単純化するが、テスト困難性とデータのライフサイクル管理が課題。

### UI要素の動的取得

UI Toolkit要素をQ<T>で名前検索。Inspectorの参照設定を減らし、UIXMLファイル構成の独立性を高める一方、要素名変更の検出困難さが課題（改善計画 P3）。

### Input Systemの未使用

Input System 1.19.0パッケージが含まれるが、現状はUI ToolkitのイベントのみでUI操作を処理。将来的にゲームパッド対応やアクション拡張が必要な場合のために、Input Actions設定の整理方針が必要（改善計画 P2-2）。

## 開発体制への提案

### テスト戦略

- **単体テスト** — ValueConstants、ResultData等の計算クラス
- **統合テスト** — ゲーム全流れ（ホーム→プレイ→結果）
- **手動テスト** — UI操作の応答性、端末別表示

### コードレビュー重点

- ランク判定条件、得点計算式の数値確認
- UIイベント登録・解除のペア確認
- null参照チェックの対象範囲

### ドキュメント運用

本資料セット（ゲーム概要、設計、仕様、システム構成、技術設計、クラスリファレンス、技術課題、改善計画）を定期更新。実装変更時に対応する資料を修正し、資料と実装の乖離を最小化。

## 参考資料

- [ゲーム概要](./GameOverview.md) — 基本ゲーム情報
- [ゲーム設計](./GameDesignDocument.md) — プレイ体験、進行フロー
- [ゲーム仕様](./GameSpecification.md) — 操作、ルール、数値仕様
- [システム構成](./SystemArchitecture.md) — アーキテクチャ、クラス関係
- [技術設計](./TechnicalDesign.md) — 開発環境、技術選択、設計原則
- [クラスリファレンス](./ClassReference.md) — 主要クラスの説明
- [技術課題](./CodeReview.md) — 既知の課題の詳細
- [改善計画](./ImprovementPlan.md) — 改善内容と実装方針