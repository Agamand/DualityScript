using UnityEngine;
using System.Collections;

public class HintPromptScript : MonoBehaviour {


    public GameObject[] m_Hints;
    public bool m_OnlyDisplayOnce = false;
    private Color c;
    private bool m_IsDisplaying = false;
    private bool m_Enabled = false;
	// Use this for initialization
	void Start () {
        c.a = 0f;
        foreach (GameObject hint in m_Hints)
        {
            hint.renderer.enabled = false;
            hint.renderer.material.color = c;
        }

	}
	
	// Update is called once per frame
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

    void OnTriggerEnter(Collider col)
    {
        m_IsDisplaying = true;
        m_Enabled = true;
        foreach (GameObject hint in m_Hints)
        {
            hint.renderer.enabled = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        m_IsDisplaying = false;
        if (m_OnlyDisplayOnce)
            gameObject.SetActive(false);
    }
}
