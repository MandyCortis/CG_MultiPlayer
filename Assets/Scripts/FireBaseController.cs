using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.SceneManagement;
using Firebase.Extensions;

[Serializable]
public class LobbyInstance
{

    public string _player1;
    public string _player2;
    public LobbyInstance(string player1, string player2)
    {
        this._player1 = player1;
        this._player2 = player2;
    }
}

public class ObjectInstanceCreate
{
    public string _InstanceNameP1;
    public string _InstanceNameP2;
    public string _Position;
    public string _DateTime;
    public string _Id;


    public ObjectInstanceCreate(string InstanceNameP1, string Position, string DateTime, string Id)
    {
        this._InstanceNameP1 = InstanceNameP1;
        this._Position = Position;
        this._DateTime = DateTime;
        this._Id = Id;
    }
}

public class FirebaseController : MonoBehaviour
{
    private static DatabaseReference _dbRef;
    public static string _key = "";
    public static string _player1 = "";
    public static string _player2 = "";

    public static string InstPos;
    public static string DateT = "";
    public static string ObjId;
    public static string InstId;
    public static string ShapeName;

    private static bool plr1 = false;

    private void Start()
    {
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

    public static IEnumerator waitForLoad()
    {
        Debug.Log("waited");
        yield return new WaitForSeconds(1f);
    }


    public static IEnumerator saveInst(string instName, string instPos, string instT, string ID)
    {
        Vector2 pos = PlayerStats.pos;
        instPos = pos.ToString();
        string dt = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");

        instT = dt;
        ID = _dbRef.Child("Objects").Push().Key;

        if (plr1 == true)
        {
            instName = "Square";
        }
        else
        {
            instName = "Circle";
        }

        ObjectInstanceCreate obj = new ObjectInstanceCreate(instName, instPos, instT, ID);
        string json = JsonUtility.ToJson(obj);

        yield return _dbRef.Child("Objects").Child(ID).SetRawJsonValueAsync(JsonUtility.ToJson(obj));
    }

    public static IEnumerator CreateGame(string player1)
    {

        _player1 = player1;
        LobbyInstance lobby = new LobbyInstance(player1, "");
        _key = _dbRef.Child("Players").Push().Key;

        yield return _dbRef.Child("Players").Child(_key).SetRawJsonValueAsync(JsonUtility.ToJson(lobby));
        //Listen to any changes in this lobby
        _dbRef.Child("Players").Child(_key).ValueChanged += HandleValueChanged;
        GameManager.NextScene("Lobby");
    }

    public static void AddToLobby(string player1, string player2, string key)
    {
        LobbyInstance lobby = new LobbyInstance(player1, player2);
        _dbRef.Child("Players").Child(key).SetRawJsonValueAsync(JsonUtility.ToJson(lobby));
        SceneManager.LoadScene("Lobby");

    }

    public static IEnumerator KeyExists(String key)
    {
        yield return _dbRef.Child("Players").Child(key).GetValueAsync().ContinueWithOnMainThread(task =>
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
                        if (child.Key == "_player1")
                        {
                            _player1 = child.Value.ToString();
                        }
                    }
                    AddToLobby(_player1, _player2, key);
                }
            }
        });
    }

    public static IEnumerator playerCheck(string key)
    {
        yield return _dbRef.Child("Matches").Child(key).GetValueAsync().ContinueWith(task =>
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
                        Debug.Log(child.Key);
                    }
                    AddToLobby(_player1, _player2, key);
                }
            }
        });
    }
}
