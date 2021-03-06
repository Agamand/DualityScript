/**
 * LaserBeamColliderScript
 *  --> kills the player when touching a laserBeam
 *  
 * Members: 
 * 	- private ControlerScript m_PlayerControler : the ControlerScript associated to the player
 * 	- public AudioClip m_LaserSound : the soundclip to played when the player hits a laser
 *  
 * Authors: Jean-Vincent Lamberti
 * */

using UnityEngine;
using System.Collections;

public class LaserBeamColliderScript : MonoBehaviour {

    public AudioClip m_LaserSound;
    private ControllerScript m_PlayerControler;

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
        {
            audio.PlayOneShot(m_LaserSound, PlayerPrefs.GetFloat("SoundVolume"));
            m_PlayerControler.RespawnPlayer();
        }
    }
}
