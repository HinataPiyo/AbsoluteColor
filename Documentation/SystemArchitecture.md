# システム構成・アーキテクチャ

## 1. 全体像

このプロジェクトは、ホーム／ゲームプレイの2シーンを、`MonoBehaviour`の管理クラスとUI ToolkitベースのUIモジュールで進行させる構成である。ゲーム進行の中心は `GameManager` のコルーチン。モード・オーディオの編集データはScriptableObject、シーン間や結果履歴の実行時データは静的クラス等で扱う。

```mermaid
flowchart LR
    SO[ColorChannelDataSO / AudioDataSO]
    HM[HomeManager / Home UI]
    CR[CoreResource]
    GM[GameManager]
    UI[Gameplay UI modules]
    RS[ResultController / RoundResult]
    JU[JugementUI]
    AR[AllRoundTotalResult]
    AM[AudioManager]
    FX[CameraShake / Particle / Post Processing]

    SO --> HM
    HM --> CR
    CR --> GM
    GM --> UI
    UI --> GM
    GM --> RS
    RS --> JU
    GM --> AR
    GM --> AM
    JU --> AM
    JU --> FX
```

図はC#参照と処理フローを要約したもの。具体的な参照の一部はInspector上のシーン設定に依存する。

## 2. システム一覧

| システム | 主な構成 | 責務・連携 |
|---|---|---|
| ホーム／モード | `HomeManager`、`HomeUIModuleManager`、`SelectModePanel`、`CoreResource` | 初期モード情報、ホームUI、ゲーム開始／遷移。 |
| RGB入力／プレビュー | `AdjustSlider`、`AdjustUIAreaController`、`PreviewSlider`、ホーム色プレビュー | チャンネル値を保持し、調整色をプレビューへ反映。ホームとゲーム中に近い操作を提供する。 |
| ゲーム進行 | `GameManager`、`ValueConstants` | ラウンド数・時間・見本生成・入力待ち・結果待ち・最終処理を順に実行。 |
| 採点／結果 | `ResultController`、`ResultData`、`RoundResult`、`JugementUI`、`AllRoundTotalResult` | 色差、判定、得点、ラウンド／総合表示を担当。 |
| UIモジュール共通化 | `UIModuleManagerBase`、`IPanelChangeElement`、`IUIModuleController` | UI要素の初期化やパネル切り替えを共通化する。要素の取得には文字列パスを使用。 |
| オーディオ | `AudioDataSO`、`AudioManager` | タグからBGM／SEクリップを解決して再生する。 |
| 演出 | `CameraShake`、`GameClearParticleController`、`JugementUI`、Animator／Volume | カメラ、パーティクル、アニメーション、ポストプロセス等の結果演出。 |
| 外部ランキング | `AllRoundTotalResult` とUnityroomクライアント依存 | 終了時にランキング送信を呼び出す実装がある。実運用上の成功は未検証。 |

敵、戦闘、Wave、ネットワーク対戦、セーブデータの独立システムは確認できない。

## 3. 主要なデータフロー

```mermaid
flowchart TD
    C[ColorChannelDataSO] --> HM[HomeManager]
    HM --> CR[CoreResource.CurrentColorChannelData]
    CR --> UI[モード表示・色チャンネル素材]
    GM[GameManager] --> T[ランダム見本RGB]
    T --> SP[TopicSphereController]
    UI --> A[AdjustUIAreaControllerの回答RGB]
    T --> RC[ResultController]
    A --> RC
    RC --> RD[ResultData: 見本・回答・ラウンド]
    RD --> J[判定用の値]
    J --> GM[GameManager]
    GM --> JU[JugementUI]
    RD --> RR[ラウンド結果]
    RD --> AR[総合結果用の全ラウンドデータ]
    AD[AudioDataSO] --> AM[AudioManager]
    JU --> AM
```

`ResultData` は構築後に値を変更できない形（private setter）で扱われる。色差／判定／得点は結果処理の中で算出される。モード情報の一部は `CoreResource` の静的状態を介してシーンをまたぐ。

## 4. ラウンド処理フロー

```mermaid
sequenceDiagram
    participant GM as GameManager
    participant Topic as TopicSphereController
    participant Adjust as AdjustUIAreaController
    participant Result as ResultController
    participant Judgement as JugementUI
    participant Round as RoundResult

    GM->>Topic: ランダムな見本色を設定
    GM->>Adjust: ラウンド開始・入力可能化
    loop 決定まで、または制限時間終了まで
        Adjust-->>GM: 決定状態を更新
    end
    GM->>Adjust: 現在の回答RGBを取得
    GM->>Result: CreateResultData(topicColor, answerColor)
    Result-->>GM: ResultDataを保存
    GM->>Judgement: 判定・割合を表示
    GM->>Result: ShowResult()
    Result->>Round: 最新ResultDataを表示
    Round-->>GM: 次へ操作で待機解除
```

この図はコード上の依存を概略化したもの。直接呼び出しとInspector参照の詳細は、対象シーンのInspector設定をUnity Editor上で確認する必要がある。

## 5. クラス関係（概略）

```mermaid
classDiagram
    class GameManager {
      +Start()
      +GameLoop()
    }
    class HomeManager
    class UIModuleManagerBase
    class HomeUIModuleManager
    class GamePlayUIModuleManager
    class CoreResource {
      +GetColorChannelData()
    }
    class ColorChannelDataSO
    class AudioDataSO
    class AudioManager
    class ResultController {
      +CreateResultData()
      +ShowResult()
    }
    class ResultData
    class RoundResult
    class JugementUI
    class AllRoundTotalResult
    UIModuleManagerBase <|-- HomeUIModuleManager
    UIModuleManagerBase <|-- GamePlayUIModuleManager
    HomeManager --> ColorChannelDataSO
    HomeManager --> CoreResource
    GameManager --> GamePlayUIModuleManager
    GameManager --> ResultController
    GameManager --> JugementUI
    GameManager --> AllRoundTotalResult
    ResultController --> ResultData
    ResultController --> RoundResult
    AudioManager --> AudioDataSO
```

継承関係と依存はスクリプト構造に基づく。`GameManager` から複数の具体クラスへ参照が集まっており、汎用的なイベントバスやDIコンテナを中心とする構成ではない。

## 6. シーン・アセット構成

- **HomeScene**：ホーム管理、UI管理、モード選択、履歴、オプション、色プレビュー、オーディオ等のスクリプトがシーンに結び付いている。
- **GamePlayScene**：`GameManager`、見本球、RGB調整、結果／判定、総合結果、演出、オーディオ等が存在する。
- Build Settings上はホームが先、ゲームプレイが次。
- Git管理対象のPrefabは確認できない。少なくとも確認したゲーム固有のオブジェクト構成はScene直置きであり、Prefab再利用を前提とした構成とは確認できない。
- ScriptableObjectアセットは色チャンネルデータと音声タグデータを確認。`Resources` フォルダーは空。
- UI Toolkit用の素材一式は存在するが、コード上の具体的な画面構造・実機表示はシーンとUnity Editorでの確認が必要。

## 7. 依存関係上の特徴とリスク

- `GameManager.I`、`AudioManager.I`、`CameraShake.I` 等の静的インスタンス参照と `CoreResource` の静的状態を使用する。シーン間連携は簡潔になる一方、初期化順序・テスト分離・シーン重複時の振る舞いに注意が必要。
- UI Toolkit要素の名前／階層を文字列で取得するため、UXML／シーン側の命名変更は実行時の欠落につながりやすい。
- シーン参照・演出コンポーネントの設定に依存する箇所があり、Inspector未設定を検出する一貫した検証機構は確認できない。
- `ScoreHistoryComposition` のボタン購読処理と `BackButton` の解除処理にはイベント配線上の不具合候補がある。詳細は[コードレビュー](./CodeReview.md)を参照。
