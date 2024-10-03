using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    // Configurable Parameters \\
    [SerializeField] Canvas gameoverCanvas;

    // Private Variables \\
    public bool dead;

    // Cached References \\
    SceneLoader sceneLoader;

    void Start()
    {
        sceneLoader = FindFirstObjectByType<SceneLoader>();
        gameoverCanvas.gameObject.SetActive(false);
        dead = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            gameoverCanvas.gameObject.SetActive(true);
            dead = true;
        }
    }

    public void Retry()
    {
        sceneLoader.ReloadScene();
    }
}
