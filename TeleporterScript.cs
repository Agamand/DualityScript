/**
 * public class TeleporterScript
 *  --> Trigger allowing to know when the player reaches the end of a level
 * 
 * Authors: Cyril Basset
 * */
using UnityEngine;
using System.Collections;

public class TeleporterScript : MonoBehaviour {

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
			col.GetComponent<ControllerScript>().OnLevelEndReach();
        }
    }



}
