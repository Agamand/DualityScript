using UnityEngine;
using System.Collections;

public class TeleporterScript : MonoBehaviour {


    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
            print("Congrats you've reached the end of the level");
        //Insert stuff
    }



}
