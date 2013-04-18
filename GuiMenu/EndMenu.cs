/**
 *  EndMenu
 *      --> The Script for end level menu 
 *          - handles the pursuit of the game
 *          - handles the upload of the score if the user has respected required conditions
 *      
 *  Authors: Cyril Basset
 **/

using UnityEngine;
using System.Collections;
using System;

public class EndMenu : MonoBehaviour {

    public GUISkin skin;

    private DataBaseHandling m_db_handler;
	private ScreenEffectScript m_ScreenEffectMenu;
    private HudScript m_Hud;



    void Start()
    {
		m_ScreenEffectMenu = GameObject.Find("MenuEffectGlobalScript").GetComponent<ScreenEffectScript>();
		m_ScreenEffectMenu.Disable();

        m_db_handler = gameObject.AddComponent<DataBaseHandling>();
        this.enabled = false;
        m_Hud = GameObject.Find("HUD").GetComponent<HudScript>();
		
    }
	
	public void Enable(bool active)
	{
		enabled = active;
        Screen.showCursor = active;
        Screen.lockCursor = !active;
        if (active)
        {
            m_Hud.Enable(false);
            m_ScreenEffectMenu.Enable();
        }
        else
        {
            m_Hud.Enable(true);
            m_ScreenEffectMenu.Disable();
        }
	}

    void OnGUI()
    {		
		GUI.Label(ResizeGUI(new Rect(320, 150, 160, 30)), "Level Finish !", skin.customStyles[1]);
		
		
		if(Application.loadedLevel == 4)
			GUI.Label(ResizeGUI(new Rect(320, 210, 160, 30)), "GG  ! trololoGames ! ", skin.customStyles[3]);

		GameSave s = SaveManager.last_save;
		if(s != null)
			GUI.Label(ResizeGUI(new Rect(320, 250, 160, 30)), "Score : " + s.score  + ", Time : " + s.time +  " seconds, Death : " + s.deathCount, skin.customStyles[2]);
		
		
		
		if(Application.loadedLevel != Application.levelCount-1)
		{
	        if (GUI.Button(ResizeGUI(new Rect(320, 300, 160, 30)), "Continue", skin.button))
	        {
				Application.LoadLevel(Application.loadedLevel+1);
	        }
		}else if(PlayerPrefs.HasKey("Playthrough") && PlayerPrefs.HasKey("IsLoggedIn"))
		{
			if (GUI.Button(ResizeGUI(new Rect(320, 300, 160, 30)), "Upload Score", skin.button))
	        {
				if(s != null)
					m_db_handler.UploadScore(PlayerPrefs.GetString("Username"),Mathf.FloorToInt(s.time).ToString(),s.deathCount.ToString(),s.score.ToString());
	        }
		}
		
        if (GUI.Button(ResizeGUI(new Rect(320, 350, 160, 30)), "Return to Main Menu", skin.button))
        {
			Application.LoadLevel(0);
        }
		

		if(!Application.isWebPlayer)
        	if (GUI.Button(ResizeGUI(new Rect(320, 400, 160, 30)), "Quit", skin.button))
				Application.Quit();
    }

    Rect ResizeGUI(Rect _rect, bool uniformScale = false)
    {
        Vector2 scale = new Vector2(Screen.width / 800.0f, Screen.height / 600.0f);
        if (uniformScale)
            scale = new Vector2(scale.x < scale.y ? scale.x : scale.y, scale.x < scale.y ? scale.x : scale.y);
        float rectX = _rect.x * scale.x;
        float rectY = _rect.y * scale.y;
        float rectWidth = _rect.width * scale.x;
        float rectHeight = _rect.height * scale.y;
        return new Rect(rectX, rectY, rectWidth, rectHeight);

    }
	
    Rect ResizeGUICenter(Rect _rect, bool uniformScale = false)
    {
        Vector2 scale = new Vector2(Screen.width / 800.0f, Screen.height / 600.0f);
        if (uniformScale)
            scale = new Vector2(scale.x < scale.y ? scale.x : scale.y, scale.x < scale.y ? scale.x : scale.y);
        float rectX = _rect.x * scale.x;
        float rectY = _rect.y * scale.y;
        float rectWidth = _rect.width * scale.x;
        float rectHeight = _rect.height * scale.y;
        return new Rect(rectX, rectY, rectWidth, rectHeight);

    }
}
