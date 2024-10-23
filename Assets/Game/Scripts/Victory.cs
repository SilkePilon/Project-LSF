using UnityEngine;

public class Victory : MonoBehaviour
{
    public GameObject victoryCanvas;

    public GameObject player1;

    public GameObject player2;
    // Start is called before the first frame update
    void Start()
    {
        if (victoryCanvas != null)
        {
            victoryCanvas.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player1 == null)
        {
            victoryCanvas.SetActive(true);
        }

        if (player2 == null)
        {
            victoryCanvas.SetActive(true);
        }
    }
    
}
