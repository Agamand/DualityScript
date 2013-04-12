/**
 *  CheckpointScript
 *      --> Script used to define a checkpoint 
 *          - sets the player respawn position to its center when entering its collision box
 *         
 *  Members: 
 *      - private ControlerScript m_Cs : the ControlerScript attached to the player
 *      - public Quaternion playerRotation : the rotation to give to the player when respawning     
 * 
 *  Authors: Jean-Vincent Lamberti
 **/

using UnityEngine;
using System.Collections;

public class CheckpointScript : MonoBehaviour {

    public GameObject[] m_GameObjects;
    private ControllerScript m_Cs = null;
    public Quaternion m_PlayerRotationOnSpawn;
	public Vector3 m_PlayerGravityDirOnSpawn;
    public enum WorldIndexEnum { World1, World2 };
    public WorldIndexEnum m_WorldIndex = 0;
    public AudioClip m_CheckpointSound;
    public bool m_PlaySound = true;
    private bool m_Activated = true;

	// Use this for initialization
	void Start () {
        m_Cs = GameObject.Find("Player").GetComponent<ControllerScript>();
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
                if (m_PlaySound)
                    audio.PlayOneShot(m_CheckpointSound);
                setRespawnIntel();
            }
        }
    }
	
	public void setRespawnIntel(){
        m_Cs.SetRespawnPosition(gameObject.transform.position);
        m_Cs.SetRespawnRotation(m_PlayerRotationOnSpawn);
		m_Cs.SetRespawnGravityDir(m_PlayerGravityDirOnSpawn);
        /*Debug.Log("Entered in checkpoint, instantiated values are :");
        foreach (GameObject go in m_GameObjects)
        {
            Debug.Log(go.name + " x:" + go.transform.position.x + " y:" + go.transform.position.y + " z:" + go.transform.position.z);
        }
        SaveSerializedData();
        m_Cs.AddDataToLoad(m_GameObjects);*/
        //gameObject.SetActive(false);
        m_Activated = false;
	}
	
    void SaveSerializedData()
    {
        Debug.Log("Data saved");
        PlayerPrefs.SetInt("World", (int)m_WorldIndex);
        foreach (GameObject go in m_GameObjects)
        {
            PlayerPrefs.SetFloat(go.name + ".transform.position.x", (float)go.transform.position.x);
            PlayerPrefs.SetFloat(go.name + ".transform.position.y", (float)go.transform.position.y);
            PlayerPrefs.SetFloat(go.name + ".transform.position.z", (float)go.transform.position.z);
        }

    }
}
