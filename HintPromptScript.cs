/**
 *  DeathPitScript
 *      --> Script displaying gradually one or more hint when the player walks close by
 *      
 *      Members:
 * -public GameObject[] m_Hints : the hints to handles
 * -public bool m_OnlyDisplayOnce : whether or not display the hints only once
 * -private Color c : the color used to get a gradual alpha channel
 * -private bool m_IsDisplaying : whether or not the hints are currently displayed
 * -private bool m_Enabled : whether or not the hints are enabled
 *  
 *  Authors: Jean-Vincent Lamberti
 **/

using UnityEngine;
using System.Collections;

public class HintPromptScript : MonoBehaviour {


    public GameObject[] m_Hints;
    public bool m_OnlyDisplayOnce = false;
    private Color c;
    private bool m_IsDisplaying = false;
    private bool m_Enabled = false;

    void Start () {
        c.a = 0f;
        foreach (GameObject hint in m_Hints)
        {
            hint.renderer.enabled = false;
            hint.renderer.material.color = c;
        }

	}
	
	void Update () {
        if (m_Enabled)
        {
            if (m_IsDisplaying && c.a < 1.0f)
            {
                c.a += 0.005f;
                foreach (GameObject hint in m_Hints)
                {
                    hint.renderer.material.color = c;
                }
            }
            if (!m_IsDisplaying)
            {
                if (c.a > 0f)
                {
                    c.a -= 0.005f;
                    foreach (GameObject hint in m_Hints)
                    {
                        hint.renderer.material.color = c;
                    }
                }
                else
                {
                    foreach (GameObject hint in m_Hints)
                    {
                        hint.renderer.enabled = false;
                    }
                    m_Enabled = false;
                }
            }
        }
	}

    /**
     * OnTriggerEnter(Collider col)
     *  --> Called when the player enters the area of trigger of the hint
     * 
     * */
    void OnTriggerEnter(Collider col)
    {
        if (PlayerPrefs.GetInt("DisplayHints") == 1)
        {
            m_IsDisplaying = true;
            m_Enabled = true;
            foreach (GameObject hint in m_Hints)
            {
                hint.renderer.enabled = true;
            }
        }
    }

    /**
     * OnTriggerExit(Collider col)
     *  --> Called when the player exits the area of trigger of the hint
     * 
     * */
    void OnTriggerExit(Collider col)
    {
        if (PlayerPrefs.GetInt("DisplayHints") == 1)
        {
            m_IsDisplaying = false;
            if (m_OnlyDisplayOnce)
                gameObject.SetActive(false);
        }
    }
}
