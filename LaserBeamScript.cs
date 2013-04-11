/**
 * LaserBeamScript
 *  --> Global Script : activates and deactivates all the lasers tagged with "laserBeam" in the level depdending on the world the player is in
 *  
 * Members: 
 * 	- private GameObject []m_LaserBeams : an array containing all the GameObjects tagged with "laserBeam"
 *  - private GameObject m_CurrentWorld : the world the player is in;
 *  - private WorldControlerScript m_worldController : the WorldControlerScript attached to the scene "GameWorld" defining in which world the player is
 *  
 * Authors: Jean-Vincent Lamberti
 * */


using UnityEngine;
using System.Collections;

public class LaserBeamScript : MonoBehaviour {

    private GameObject []m_LaserBeams;
    private GameObject m_CurrentWorld;
    private WorldControllerScript m_worldController;

	// Use this for initialization
	void Start () {
        m_LaserBeams = GameObject.FindGameObjectsWithTag("laserBeam");
        m_worldController = GameObject.Find("GameWorld").GetComponent<WorldControllerScript>();
        m_CurrentWorld = m_worldController.GetCurrentWorld();
        InitializeLasers();
	}
	
	/*
     * Called when the player switches world
     * */
	public void SwitchState () {
            m_CurrentWorld = m_worldController.GetCurrentWorld();
            foreach (GameObject LaserBeam in m_LaserBeams)
            {
                if (m_CurrentWorld.layer == LaserBeam.layer || LaserBeam.layer == 0)
                {
                    LaserBeam.SetActive(true);

                }
                else if (m_CurrentWorld.layer != LaserBeam.layer)
                {
                    LaserBeam.SetActive(false);
                }

            }          
	}

    public void InitializeLasers()
    {
        foreach (GameObject LaserBeam in m_LaserBeams)
        {
            if (8 == LaserBeam.layer || LaserBeam.layer == 0)
            {
                LaserBeam.SetActive(true);

            }
            else if (8 != LaserBeam.layer)
            {
                LaserBeam.SetActive(false);
            }

        }          
    }
}
