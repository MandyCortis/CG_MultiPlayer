using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

[Serializable]

public class LobbyInstance{
    
    public string _player1;
    public string _player2;
    public string _position;
    public string _datetime;
    public string _id;



    public LobbyInstance(string player1, string player2){
        this._player1 = player1;
        this._player2 = player2;
    }
}

public class FirebaseController : MonoBehaviour
{
    private static DatabaseReference _dbRef;
    public static string _key = "";
    public static string _player1 = "";
    public static string _player2 = "";
    public static string _position = "";
    public static string _datetime = "";
    public static string _id = "";

    private void Start() {
        DontDestroyOnLoad(this.gameObject);
        _dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }


    //When a player joins the lobby, we should know
    public static void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        else
        {
            Debug.Log("Someone joined the lobby");
            foreach (var child in args.Snapshot.Children)
            {
                if (child.Key == "_player2")
                {
                    _player2 = child.Value.ToString();
                }
            }
            Debug.Log(_player2 + " has joined the lobby");
        }
    }


public static IEnumerator CreateGame(string player1){

    _player1 = player1;
    LobbyInstance lobby = new LobbyInstance(player1, "");
    _key = _dbRef.Child("Games").Push().Key;

    yield return _dbRef.Child("Games").Child(_key).SetRawJsonValueAsync(JsonUtility.ToJson(lobby));
        //Listen to any changes in this lobby
        _dbRef.Child("Games").Child(_key).ValueChanged += HandleValueChanged;
        GameManager.NextScene("Lobby");
    }

    public static void AddToLobby(string player1, string player2, string key)
    {
        LobbyInstance lobby = new LobbyInstance(player1, player2);
        _dbRef.Child("Games").Child(key).SetRawJsonValueAsync(JsonUtility.ToJson(lobby));
        //GameManager.NextScene("Lobby");
    }

    public static IEnumerator KeyExists(String key)
    {
        yield return _dbRef.Child("Games").Child(key).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Value != null)
                {
                    Debug.Log("Correct Key");

                    //Optimise
                    foreach (var child in snapshot.Children)
                    {
                        if(child.Key == "_player1")
                        {
                            _player1 = child.Value.ToString();
                        }
                    }
                    AddToLobby(_player1, _player2,key);
                }
            }
        });
    }

}
