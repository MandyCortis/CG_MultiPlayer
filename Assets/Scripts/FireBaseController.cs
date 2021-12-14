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

    public ObjectInstanceCreate _player1;
    public ObjectInstanceCreate _player2;
    public LobbyInstance(ObjectInstanceCreate player1, ObjectInstanceCreate player2)
    {
        this._player1 = player1;
        this._player2 = player2;
    }
}

[Serializable]
public class ObjectInstanceCreate
{
    public string _InstanceName;
    public float _InstancePosX;
    public float _InstancePosY;
    public string _InstanceTime;
    public string _UniqueId;
    public string _InstanceShape;


    public ObjectInstanceCreate(string InstanceName, string InstanceTime, string UniqueId, float InstancePosX, float InstancePosY, string shape)
    {
        this._InstanceName = InstanceName;
        this._InstancePosX = InstancePosX;
        this._InstancePosY = InstancePosY;
        this._InstanceTime = InstanceTime;
        this._UniqueId = UniqueId;
        this._InstanceShape = shape;
    }
}


public class FirebaseController : MonoBehaviour
{
    private static DatabaseReference _dbRef;

    public static string _key = "";
    public static string _p1key = "";
    public static string _p2key = "";
    public static string _player1 = "";
    public static string _player2 = "";
    public static float _InstancePosX;
    public static float _InstancePosY;


    PlayerStats ps = new PlayerStats();

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        _dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        //FirebaseDatabase.DefaultInstance.GetReference("DateT").ValueChanged += saveInst();
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
            _dbRef.Child("Players").Child(_key).Child("_player2").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Value != null)
                    {
                        //Optimise
                        foreach (var child in snapshot.Children)
                        {
                            if (child.Key == "_InstanceName")
                            {
                                _player2 = child.Value.ToString();
                            }
                        }
                    }
                }
            });
            Debug.Log(_player2 + " has joined the lobby");
            //_dbRef.Child("Players").Child(_key).ValueChanged -= HandleValueChanged;
        }
    }


    public static void HandleMovement(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        else
        {
            Debug.Log("2 moving");
            _dbRef.Child("Players").Child(_key).Child("_player2").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Value != null)
                    {
                        //Optimise
                        foreach (var child in snapshot.Children)
                        {
                            if (child.Key == "_InstancePosY")
                            {
                                _player2 = child.Value.ToString();
                            }
                        }
                    }
                }
            });
            Debug.Log(_player2 + " has moved");
            //_dbRef.Child("Players").Child(_key).ValueChanged -= HandleValueChanged;
        }
    }


    public static IEnumerator CreateGame(string player1)
    {
        _key = _dbRef.Child("Players").Push().Key;
        _player1 = player1;
        _p1key = _dbRef.Child("Players").Child(_key).Push().Key;
        ObjectInstanceCreate obj = new ObjectInstanceCreate(player1, DateTime.Now.ToString(), _p1key, 0f, 0f, "square");
        LobbyInstance lobby = new LobbyInstance(obj, null);


        yield return _dbRef.Child("Players").Child(_key).SetRawJsonValueAsync(JsonUtility.ToJson(lobby));
        //Listen to any changes in this lobby
        _dbRef.Child("Players").Child(_key).ValueChanged += HandleValueChanged;
        SceneManager.LoadScene("Lobby");
    }

    public static void AddToLobby(string player1, string player2, string key)
    {
        _p2key = _dbRef.Child("Players").Child(key).Push().Key;
        ObjectInstanceCreate obj = new ObjectInstanceCreate(player2, DateTime.Now.ToString(), _p2key, 0f, 0f, "circle");

        _dbRef.Child("Players").Child(key).Child("_player2").SetRawJsonValueAsync(JsonUtility.ToJson(obj));
        SceneManager.LoadScene("Lobby");
    }

    public void UpdatePos(Vector2 p1Pos)
    {
        p1Pos = ps.p1.transform.position;

        _InstancePosX = p1Pos.x;
        _InstancePosY = p1Pos.y;
    }

    //
    public static void CheckMovement(string player2)
    {
        _dbRef.Child("Players").Child(_key).SetRawJsonValueAsync(JsonUtility.ToJson(player2));
        _dbRef.Child("Players").Child(_key).ValueChanged += HandleMovement;

    }
    //

    public static IEnumerator playerCheck(string key)
    {
        yield return _dbRef.Child("Players").Child(key).Child("_player1").GetValueAsync().ContinueWith(task =>
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
                        if (child.Key == "_InstanceName")
                        {
                            _player1 = child.Value.ToString();
                        }
                    }
                    AddToLobby(_player1, _player2, key);
                }
            }
        });
    }
}