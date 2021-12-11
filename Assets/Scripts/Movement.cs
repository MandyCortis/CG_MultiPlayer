using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    DatabaseReference reference;

    Text pnts1;
    Text pnts2;
    float TotalUnits = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    void Points()
    {
        if(gameObject.name == "Player1")
        {
            pnts1.text = "P1: " + Mathf.Floor(TotalUnits).ToString();
        }
        else
        {
            pnts2.text = "P2: " + Mathf.Floor(TotalUnits).ToString();
        }
    }
}
