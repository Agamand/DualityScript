/**
 * LaserBeamColliderScript
 *  --> kills the player when touching a laserBeam
 *  
 * Members: 
 * 	- private ControlerScript m_PlayerControler : the ControlerScript associated to the player
 *  
 * Authors: Jean-Vincent Lamberti
 * */

using UnityEngine;
using System.Collections;

public class LaserBeamColliderScript : MonoBehaviour {


    private ControllerScript m_PlayerControler;

        // Use this for initialization
	void Start () {
        m_PlayerControler = GameObject.Find("Player").GetComponent<ControllerScript>();
	}

    /**
     * OnTriggerEnter(Collider col)
     *  --> called when the player collides with the beam
     *      - respawns the player
     * */
    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
            m_PlayerControler.RespawnPlayer();
    }
}
