using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public GameObject p1;
    public GameObject p2;

    private bool isKeysEnabled;

    public int p1Moves = 0;
    public int p2Moves = 0;

    public int p1Score = 0;
    public int p2Score = 0;

    public TMPro.TMP_Text p1Text;
    public TMPro.TMP_Text p2Text;

    public GameObject panel;
    public Text pWin;

    public Vector2 pos;

    void Start()
    {
        panel = GameObject.Find("Panel");
        panel.SetActive(false);
        pWin.gameObject.SetActive(false);

        GameObject canvas = GameObject.Find("Canvas");
        p1 = canvas.transform.GetChild(0).gameObject;
        p2 = canvas.transform.GetChild(1).gameObject;

        Debug.Log(p1.transform.position);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            isKeysEnabled = true;
            p1.transform.position += new Vector3(0, 1f, 0);
            p1Moves++;
            Debug.Log("Player 1 Moved Up");
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            isKeysEnabled = true;
            p1.transform.position += new Vector3(0, -1f, 0);
            p1Moves++;
            Debug.Log("Player 1 Moved Down");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            isKeysEnabled = true;
            p2.transform.position += new Vector3(0, 1f, 0);
            p2Moves++;
            Debug.Log("Player 2 Moved Up");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            isKeysEnabled = true;
            p2.transform.position += new Vector3(0, -1f, 0);
            p2Moves++;
            Debug.Log("Player 2 Moved Down");
        }

        if (p1Moves == 1)
        {
            p1Score += 1;
            p1Moves = 0;
        }
        p1Text.GetComponent<TMPro.TextMeshProUGUI>().text = "P1: " + p1Score.ToString();
        

        if (p2Moves == 1)
        {
            p2Score += 1;
            p2Moves = 0;
        }
        p2Text.GetComponent<TMPro.TextMeshProUGUI>().text = "P2: " + p2Score.ToString();
        
        Win();
    }

    public void Win()
    {
        if (p1Score == 10)
        {
            isKeysEnabled = false;
            panel.SetActive(true);
            pWin.gameObject.SetActive(true);
            pWin.text = "CONGRATULATIONS PLAYER 1, You WIN!";
        }
        else if (p2Score == 10)
        {
            isKeysEnabled = false;
            panel.SetActive(true);
            pWin.gameObject.SetActive(true);
            pWin.text = "CONGRATULATIONS PLAYER 2, You WIN!";
        }
    }
}
