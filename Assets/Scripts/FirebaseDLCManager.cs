using System.Collections.Generic;
using UnityEngine;
using Firebase.Storage;
using Firebase.Extensions;

public class FirebaseDLCManager : MonoBehaviour
{
    FirebaseStorage storage;
    StorageReference storageRef;

    List<string> photoToDownload;

    void Start()
    {
        storage = FirebaseStorage.DefaultInstance;
        storageRef = storage.GetReferenceFromUrl("gs://mandycortisconnectedgaming.appspot.com/");

        photoToDownload = new List<string>();
        photoToDownload.Add("circle.png");
        photoToDownload.Add("square.png");

        for (int i = 0; i < 2; i++)
        {
            StorageReference image = storageRef.Child("DLC").Child(photoToDownload[i]);
            DownloadDLC(image);
        }
    }

    //Download File from Firebase Storage
    private void DownloadDLC(StorageReference reference)
    {
        const long maxAllowedSize = 1 * 1024 * 1024;

        reference.GetBytesAsync(maxAllowedSize).ContinueWithOnMainThread(task => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogException(task.Exception);
                // Uh-oh, an error occurred!
            }
            else
            {
                Debug.Log("Finished downloading!");
                byte[] fileContents = task.Result;

                // Load the image into Unity

                //Create Texture
                Texture2D tex = new Texture2D(1024, 1024);
                tex.LoadImage(fileContents);

                //Create Sprite
                Sprite mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                Sprite mySprite2 = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                GetComponent<SpriteRenderer>().sprite = mySprite;

            }
        });
    }
}