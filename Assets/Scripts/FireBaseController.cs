using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Storage;

[Serializable]
public class LobbyInstance
{
    public string _player1;
    public string _player2;

    public LobbyInstance()
    {

    }

    public LobbyInstance(string player1, string player2)
    {
        this._player1 = player1;
        this._player2 = player2;
    }

    //Used only for the Dictionary version
    public Dictionary<string, System.Object> ToDictionary()
    {
        Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();

        result["player1"] = _player1;
        result["player2"] = _player2; // result is the key  _player2 is the value 

        return result;
    }
}

public class FirebaseController : MonoBehaviour
{
    private static DatabaseReference dbRef;
    public static string _key;
    public static string _player1;
    public static string _player2;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
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

    // this requires serializable, using JSON
    public static IEnumerator CreateGame(string player1)
    {
        _player1 = player1;

        //Creating a unique identifier
        _key = dbRef.Child("Games").Push().Key;

        //Instantiating game lobby
        LobbyInstance lobby = new LobbyInstance(player1, "");

        //Convert lobby to Json
        string jsonLobby = JsonUtility.ToJson(lobby);

        //Wait until we insert the new JSON data in the firebase
        yield return dbRef.Child("Games").Child(_key).SetRawJsonValueAsync(jsonLobby);

        dbRef.Child("Games").Child(_key).ValueChanged += HandleValueChanged;

        GameManager.NextScene("Lobby");
    }

    private static void AddToLobby(string player1, string player2, string key)
    {
        //Create a new lobby with both players
        LobbyInstance lobby = new LobbyInstance(player1, player2);
        //Change lobby in JSON format
        dbRef.Child("Games").Child(key).SetRawJsonValueAsync(JsonUtility.ToJson(lobby));

        GameManager.NextScene("Lobby");
    }

    public static IEnumerator KeyExists(String key)
    {
        //Fetching Data from firebase
        yield return dbRef.Child("Games").Child(key).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                //if the key exists
                if (snapshot.Value != null)
                {
                    Debug.Log("Correct Key");

                    foreach (var child in snapshot.Children)
                    {
                        if (child.Key == "_player1")
                        {
                            //Get player 1 Name from the data retrieved from the database
                            _player1 = child.Value.ToString();
                        }
                    }

                    AddToLobby(_player1, _player2, key);
                }
            }
        });
    }

    // Using Dictionaries to Upload new data (not used if using JSON instead)
    public static IEnumerator CreateInstance()
    {
        //creating unique identifier
        string key = dbRef.Child("Games").Push().Key;

        //Instantiating game lobby 
        LobbyInstance lobby = new LobbyInstance("Tom", "Joan");
        Dictionary<string, System.Object> data = lobby.ToDictionary();

        //saving into firebase
        Dictionary<string, System.Object> childUpdates = new Dictionary<string, System.Object>();
        childUpdates["/Games/" + key] = data;

        yield return dbRef.UpdateChildrenAsync(childUpdates);
        GameManager.NextScene("Start");
    }

    public static IEnumerator SaveFirebase()
    {
        yield return null;
    }

    public static IEnumerator GetFirebase()
    {
        yield return null;
    }

    public static IEnumerator DownloadAssets()
    {
        yield return null;
    }
    public static IEnumerator UploadAssets()
    {
        yield return null;
    }

}
