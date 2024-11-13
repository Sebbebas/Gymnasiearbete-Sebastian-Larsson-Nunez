using UnityEngine;
using TMPro;

public class WIN : MonoBehaviour
{
    [SerializeField] Canvas winCanvas;
    [SerializeField] TextMeshProUGUI pointsWinCanvas;
    SceneLoader sceneLoader;
    PointManager pointManager;

    void Start()
    {
        sceneLoader = FindFirstObjectByType<SceneLoader>();
        pointManager = FindFirstObjectByType<PointManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            pointsWinCanvas.text = pointManager.GetPoints().ToString();
            winCanvas.gameObject.SetActive(true);

            //sceneLoader.LoadSceneNumber(0); //MAIN MENU RN
        }
    }
}
