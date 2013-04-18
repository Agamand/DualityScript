using UnityEngine;
using System.Collections;

public class HudScript : MonoBehaviour {
	
	private GameObject m_Score;
	private TextMesh   m_ScoreText;
    private GameObject m_Crosshair;
    private GameObject m_Checkpoint;
	// Use this for initialization
	void Start () {
		
		m_Score = transform.FindChild("Score").gameObject;
		m_ScoreText = m_Score.GetComponent<TextMesh>();
        m_Crosshair = transform.FindChild("Crosshair").gameObject;
        m_Checkpoint = transform.FindChild("CheckpointPrompt").gameObject;

        MeshRenderer mesh = m_Checkpoint.GetComponent<MeshRenderer>();
        if (mesh)
        {
            Color c = mesh.material.color;
            c.a = 0.0f;
            mesh.material.color = c;
        }

        if (PlayerPrefs.GetInt("DisplayCrosshair") == 0)
            EnableCrosshair(false);

        if (PlayerPrefs.GetInt("DisplayScore") == 0)
            EnableScore(false);

	}

    public void EnableCrosshair(bool enable)
    {
        m_Crosshair.SetActive(enable);
    }

    public void EnableScore(bool enable)
    {
        m_Score.SetActive(enable);
    }

    public void Enable(bool enable)
    {
        gameObject.SetActive(enable);
    }
	public void SetScore(int score)
	{
		m_ScoreText.text = score.ToString();
	}

    public void DisplayPrompt()
    {
        m_Checkpoint.animation.Play();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
