using UnityEngine;
using System.Collections;

public class HudScript : MonoBehaviour {
	
	private GameObject m_Score;
	private TextMesh m_ScoreText;
	private GameObject m_Crosshair;	
	// Use this for initialization
	void Start () {
		
		m_Score = transform.FindChild("Score").gameObject;
		m_ScoreText = m_Score.GetComponent<TextMesh>();
		m_Crosshair = transform.FindChild("Crosshair").gameObject;
	
	}
	
	
	public void EnableScoreShow(bool enable)
	{
		m_Score.SetActive(enable);
	}
	
	public void EnableCrosshair(bool enable)
	{
		m_Crosshair.SetActive(enable);
	}
	
	public void SetScore(int score)
	{
		m_ScoreText.text = "Score : " + score;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
