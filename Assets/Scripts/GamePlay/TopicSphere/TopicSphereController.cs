using UnityEngine;

public class TopicSphereController : MonoBehaviour
{
    [SerializeField] Renderer topicRenderer;
    Material mat;

    void Awake()
    {
        mat = topicRenderer.GetComponent<Renderer>().material;
        if (mat == null) { Debug.LogError("Materialが見つかりませんでした"); return; }
    }

    /// <summary>
    /// 色を設定するメソッド
    /// </summary>
    public void SetColor(Color color)
    {
        mat.color = color;
    }
}