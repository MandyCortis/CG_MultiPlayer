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

    //Welcome Scene
    public void CreateGame()
    {
        if (playerNameInput.text != "")
        {
            StartCoroutine(FirebaseController.CreateGame(playerNameInput.text));
        }
    }

    //Join Scene
    public void JoinGameLobby()
    {
        StartCoroutine(FirebaseController.KeyExists(uniqueCodeInput.text));
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
}