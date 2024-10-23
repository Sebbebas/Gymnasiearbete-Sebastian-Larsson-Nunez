using UnityEngine;
using TMPro;

public class PointManager : MonoBehaviour
{
    //Configurable Parameters
    [SerializeField] TextMeshProUGUI pointHUD;
    [SerializeField] int totalPoints;

    //Private Variables

    //Cached References

    void Start()
    {
        totalPoints = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            totalPoints++;
            pointHUD.text = totalPoints.ToString() +"p";
            Destroy(collision.gameObject);
        }
    }
}
