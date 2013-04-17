using UnityEngine;
using System.Collections;

public class HudScript : MonoBehaviour {
	
	private GameObject m_Score;
	private TextMesh   m_ScoreText;
    private GameObject m_ScoreOutline;
    private TextMesh   m_ScoreTextOutline;
    private GameObject m_Crosshair;
    private GameObject m_Checkpoint;
    private GameObject m_CheckpointOutline;
	// Use this for initialization
	void Start () {
		
		m_Score = transform.FindChild("Score").gameObject;
		m_ScoreText = m_Score.GetComponent<TextMesh>();
        m_ScoreOutline = transform.FindChild("ScoreOutline").gameObject;
        m_ScoreTextOutline = m_ScoreOutline.GetComponent<TextMesh>();
        m_Crosshair = transform.FindChild("Crosshair").gameObject;
        m_Checkpoint = transform.FindChild("Checkpoint").gameObject;
        m_CheckpointOutline = transform.FindChild("CheckpointOutline").gameObject;
	
	}

    void EnableCrosshair(bool enable)
    {
        m_Crosshair.SetActive(enable);
    }

    void EnableScore(bool enable)
    {
        m_Score.SetActive(enable);
        m_ScoreOutline.SetActive(enable);
    }

    public void Enable(bool enable)
    {
        gameObject.SetActive(enable);
    }
	public void SetScore(int score)
	{
		m_ScoreText.text = score.ToString();
        m_ScoreTextOutline.text = score.ToString();
	}

    public void DisplayPrompt()
    {
        m_Checkpoint.animation.Play();
        m_CheckpointOutline.animation.Play();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
