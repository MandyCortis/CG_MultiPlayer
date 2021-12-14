using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField playerNameInput;
    [SerializeField] private TMPro.TMP_InputField uniqueCodeOutput;
    [SerializeField] private TMPro.TMP_InputField uniqueCodeInput;
    [SerializeField] private TMPro.TMP_Text player1Name;
    [SerializeField] private TMPro.TMP_Text player2Name;

    private void Awake()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Welcome":
                break;
            case "Lobby":
                uniqueCodeOutput.text = FirebaseController._key;
                player1Name.text = "Player 1: " + FirebaseController._player1;
                player2Name.text = "Player 2: " + FirebaseController._player2;
                break;
            case "Join":
                break;
            default:
                break;
        }
    }
    public static void NextScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public static void StartGame()
    {
        if (FirebaseController._player2 == "")
        {
            Debug.Log("No other players in the lobby");
        }
        else
        {
            NextScene("Main");
        }
    }

    //Welcome Scene
    public void CreateGame()
    {
        if (playerNameInput.text != "")
        {
            StartCoroutine(FirebaseController.CreateGame(playerNameInput.text));
            player2Name.text = "Player 2: " + FirebaseController._player2;
        }
    }

    //Join Scene
    public void JoinGameLobby()
    {
        if (uniqueCodeInput.text != "")
        {
            StartCoroutine(FirebaseController.playerCheck(uniqueCodeInput.text));
            NextScene("Lobby");
        }
    }

    //Welcome Scene
    public void JoinGame()
    {
        if (playerNameInput.text != "")
        {
            FirebaseController._player2 = playerNameInput.text;
            NextScene("Join");
        }
    }

    void Update()
    {
        if(SceneManager.GetActiveScene().name == "Lobby")
        {
            player1Name.text = "Player 1: " + FirebaseController._player1;
            player2Name.text = "Player 2: " + FirebaseController._player2;
        }
    }
}
