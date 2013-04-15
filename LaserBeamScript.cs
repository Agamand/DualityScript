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

	// Use this for initialization
	void Start () {
        m_LaserBeams = GameObject.FindGameObjectsWithTag("LaserBeam");
        m_LineRenderers = new LineRenderer[m_LaserBeams.Length];
        for (int i = 0; i < m_LaserBeams.Length; i++)
            m_LineRenderers[i] = m_LaserBeams[i].GetComponent<LineRenderer>();
	}
	
	/*
     * Called when the player switches world
     * */
	public void SetState (int currentWorldLayer) {
        for (int i = 0; i < m_LaserBeams.Length; i++)
        {
            if (currentWorldLayer == m_LaserBeams[i].layer || m_LaserBeams[i].layer == 0)
            {
                m_LineRenderers[i].renderer.enabled = true;
            }
            else if (currentWorldLayer != m_LaserBeams[i].layer)
            {
                m_LineRenderers[i].renderer.enabled = false;
            }

        }          
	}

}
