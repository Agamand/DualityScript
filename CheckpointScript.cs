/**
 *  CheckpointScript
 *      --> Script used to define a checkpoint 
 *          - sets the player respawn position to its center when entering its collision box
 *         
 *  Members: 
 *      - private ControlerScript m_Cs : the ControlerScript attached to the player
 *      - public AudioClip : the soundclip to play when the player enters the checkpoint
 *      - public bool m_PlaySound : defines whether or not the sound should be played
 *
 *  Authors: Jean-Vincent Lamberti
 **/

using UnityEngine;
using System.Collections;

public class CheckpointScript : MonoBehaviour {

    private ControllerScript m_Cs = null;
    public AudioClip m_CheckpointSound;
    private HudScript m_Hud;
    public bool m_PlaySound = true;
    public bool m_DisplayPrompt = true;
    private bool m_Activated = true;

	void Start () {
        m_Cs = GameObject.FindGameObjectWithTag("Player").GetComponent<ControllerScript>();
        m_Hud = GameObject.Find("HUD").GetComponent<HudScript>();
	}
	

    /**
     *  OnTriggerEnter(Collider col)
     *      -> set the respawn postition and rotation when the player collide with the collider the script is attached to
     *      
     *  Arguments: 
     *      - Collider col: the collider of the gameObject the script is attached to
     **/
    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (m_Activated)
            {
                Debug.Log("Trigger : OnCheckPoint");
                SetRespawnIntel();
                if (m_PlaySound)
                    audio.PlayOneShot(m_CheckpointSound, PlayerPrefs.GetFloat("SoundVolume"));
                if (m_DisplayPrompt)
                    m_Hud.DisplayPrompt();
                
            }
        }
    }
	
    /**
     *  GetActive()
     *      -> returns wether or not the checkpoint is activated
     * */
	public bool GetActive()
	{
		return m_Activated;
	}
	
    /**
     * SetActive()
     *  -> defines wether or not the checkpoint is activated
     * 
     * */
	public void SetActive(bool active)
	{
		m_Activated = active;
	}
	

    /*
     * SetRespawnIntel()
     *  -> Sets the player respawn informations to this checkpoint
     * 
     * */
	public void SetRespawnIntel(){
		m_Activated = false;
        m_Cs.SetRespawn();
	}
	
}
