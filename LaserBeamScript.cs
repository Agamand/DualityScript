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
    private LineRenderer []m_LineRenderers;
    private GameObject m_CurrentWorld;
    private WorldControllerScript m_worldController;

	// Use this for initialization
	void Start () {
        m_LaserBeams = GameObject.FindGameObjectsWithTag("laserBeam");
        m_LineRenderers = new LineRenderer[m_LaserBeams.Length];
        for (int i = 0; i < m_LaserBeams.Length; i++)
            m_LineRenderers[i] = m_LaserBeams[i].GetComponent<LineRenderer>();
        m_worldController = GameObject.Find("GameWorld").GetComponent<WorldControllerScript>();
        m_CurrentWorld = m_worldController.GetCurrentWorld();
        InitializeLasers();
	}
	
	/*
     * Called when the player switches world
     * */
	public void SwitchState () {
            m_CurrentWorld = m_worldController.GetCurrentWorld();
            for (int i = 0; i < m_LaserBeams.Length; i++)
            {
                if (m_CurrentWorld.layer == m_LaserBeams[i].layer || m_LaserBeams[i].layer == 0)
                {
                    m_LineRenderers[i].renderer.enabled = true;
                }
                else if (m_CurrentWorld.layer != m_LaserBeams[i].layer)
                {
                    m_LineRenderers[i].renderer.enabled = false;
                }

            }          
	}

    public void InitializeLasers()
    {
        for (int i = 0; i < m_LaserBeams.Length; i++)
        {
            if (8 == m_LaserBeams[i].layer || m_LaserBeams[i].layer == 0)
            {
                m_LineRenderers[i].renderer.enabled = true;

            }
            else if (8 != m_LaserBeams[i].layer)
            {
                m_LineRenderers[i].renderer.enabled = false;
            }

        }          
    }
}
