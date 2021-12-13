using Firebase.Extensions;
using Firebase.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseDLC : MonoBehaviour
{
    List<string> downloadImg;

    void Start()
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference storageRef = storage.GetReferenceFromUrl("gs://mandycortisconnectedgaming.appspot.com/");

        downloadImg = new List<string>();
        downloadImg.Add("circle.png");
        downloadImg.Add("square.png");

        for (int i = 0; i < 2; i++)
        {
            StorageReference img = storageRef.Child("DLC").Child(downloadImg[i]);
            DownloadDLC(img);
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
                Sprite circle = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                Sprite square = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                GetComponent<SpriteRenderer>().sprite = circle;
                GetComponent<SpriteRenderer>().sprite = square;
            }
        });

    }
}
