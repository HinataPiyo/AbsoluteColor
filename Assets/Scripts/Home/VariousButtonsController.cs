using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class VariousButtonsController : MonoBehaviour, IUIModuleController
{
    Button selectModeButton;
    Button playButton;
    Button collectionButton;
    Button eventButton;
    Button rankingButton;
    Button scoreHistoryButton;

    public void Initialize(VisualElement root, Material[] Mats = null)
    {
        selectModeButton = root.Q<Button>("button-select-mode");
        playButton = root.Q<Button>("button-play");
        collectionButton = root.Q<Button>("button-collection");
        eventButton = root.Q<Button>("button-event");
        rankingButton = root.Q<Button>("button-ranking");
        scoreHistoryButton = root.Q<Button>("button-score-history");

        playButton.clicked += PlayButtonOnClick;
    }

    /// <summary>
    /// PlayButtonを押したらゲームプレイシーンに遷移する
    /// </summary>
    void PlayButtonOnClick()
    {
        SceneManager.LoadScene(NameConstant.GamePlayScene);
    }

    /// <summary>
    /// SelectModeButtonを押したらモード選択パネルを表示する
    /// </summary>
    public void SelectModeButtonOnClick(System.Action action)
    {
        selectModeButton.clicked += () => action();
    }
}