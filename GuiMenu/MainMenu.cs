/**
 *  MainMenu
 *      --> The Script for the main menu : 
 *          - handles all player prefs for options preferences, account settings and score
 *          - allows to start or continue the game
 *          - allows to play an unlocked level
 *          - allows to see the top ten high scores
 *          - allows to change every option of the game
 *  
 *      
 *  Authors: Cyril Basset, Jean-Vincent Lamberti
 **/

using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System;

public class MainMenu : MonoBehaviour {

    public GUISkin skin;
	public Texture logo;

    private MainMenuSelected menu;
    private SubMenuSelected submenu;
    private int m_ratio = 0;
    private int m_resolution = 0;
    private int quality = 0;
    private float m_fov = 90.0f;
    private float m_music_volume = 10.0f;
    private float m_sound_effects_volume = 10.0f;
    private bool m_fullscreen = false;
    private bool m_display_score = true;
    private bool m_display_crosshair = true;
    private bool m_display_hints = true;
    private bool display_connection_message = false;
    private bool connection_in_progress = false;
    private int display_warning_login = 0;


    private float m_mouse_sensitivity = 2.0f;
    private bool m_inverted_mouse = false;
    private Vector2 m_keybindings_scrollPosition = Vector2.zero;

    private static String[] m_keybindings_labels = {
                                               "Go Forward", 
                                               "Go Backward", 
                                               "Strafe Left", 
                                               "Strafe Right", 
                                               "Switch World", 
                                               "Carry Object", 
                                               "Respawn"
                                          };

    private static String[] m_keybindings_default = {
                                               "Z",
                                               "S",
                                               "Q",
                                               "D",
                                               "A",
                                               "E",
                                               "R"
                                          };

    private static String[] m_keybindings;
    private String username = "";
    private String password = "";
    private static String[] ratio_string = {"4/3","16/10","16/9"};
    private static String[] resolution_4_3 = { "640x480", "800x600","1024x768", "1280x960", "1440x1080"};
    private static String[] resolution_16_10 = { "1280x800", "1440x900", "1680x1050", "1920x1200", "2560x1600"};
    private static String[] resolution_16_9 = { "1280x720", "1366x768", "1600x900", "1920x1080", "2560x1440"};
    private static String[] quality_string = {"Fast","Good","Beautiful","Fanstatic"};
    private GUIContent[] ratio_combobox;

    private GUIContent[] _4_3_combobox;
    private GUIContent[] _16_10_combobox;
    private GUIContent[] _16_9_combobox;

    private GUIContent[] m_quality;

    private ComboBox comboBoxControl = new ComboBox();
    private ComboBox comboBoxResolution = new ComboBox();
    private ComboBox comboBoxQuality = new ComboBox();

    public Texture[] level_images;

    private String highScoresSt;
    private DataBaseHandling m_db_handler;


    enum MainMenuSelected
    {
        NO_SELECTED,
        LEVEL_SELECTED,
        OPTION_SELECTED,
        SCORE_SELECTED,
        NEW_GAME_SELECTED,
        CONTINUE_GAME_SELECTED
    }
    enum SubMenuSelected
    {
        NO_SELECTED,
        VIDEO_SELECTED,
        SOUND_SELECTED,
        CONTROLS_SELECTED,
        ACCOUNT_SELECTED
    }



    void Start()
    {
        Screen.showCursor = true;
        Screen.lockCursor = false;
        m_keybindings = new String[m_keybindings_labels.Length];
        ratio_combobox = new GUIContent[3];
        _4_3_combobox = new GUIContent[resolution_4_3.Length];
        _16_10_combobox = new GUIContent[resolution_16_10.Length];
        _16_9_combobox = new GUIContent[resolution_16_9.Length];
        m_quality = new GUIContent[quality_string.Length];
        menu = MainMenuSelected.NO_SELECTED;
        submenu = SubMenuSelected.NO_SELECTED;
        for (int i = 0; i < ratio_string.Length; i++ )
            ratio_combobox[i] = new GUIContent(ratio_string[i]);

        for (int i = 0; i < resolution_4_3.Length; i++ )
            _4_3_combobox[i] = new GUIContent(resolution_4_3[i]);
        for (int i = 0; i < resolution_16_10.Length; i++ )
            _16_10_combobox[i] = new GUIContent(resolution_16_10[i]);
        for (int i = 0; i < resolution_16_9.Length; i++ )
            _16_9_combobox[i] = new GUIContent(resolution_16_9[i]);

        for(int i  = 0; i < quality_string.Length; i++)
            m_quality[i] = new GUIContent(quality_string[i]);

        skin.customStyles[0].hover.background = skin.customStyles[0].onHover.background = new Texture2D(2, 2);
        InitializePlayerPrefs();
        LoadKeysFromPrefs();
        LoadFromPlayerPrefs("video");
        m_db_handler = gameObject.AddComponent<DataBaseHandling>();
        comboBoxControl.SetSelectedItemIndex(m_ratio);
        comboBoxQuality.SetSelectedItemIndex(quality);
        comboBoxResolution.SetSelectedItemIndex(m_resolution);
    }

    /*
     * InitializePlayerPrefs()
     *  Creates the player prefs concerning options and preferences
     * */
    void InitializePlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("MaxLevelReached"))
            PlayerPrefs.SetInt("MaxLevelReached", 1);

        if (!PlayerPrefs.HasKey("MusicVolume"))
            PlayerPrefs.SetFloat("MusicVolume", 0.5f);

        if (!PlayerPrefs.HasKey("SoundVolume"))
            PlayerPrefs.SetFloat("SoundVolume", 0.8f);

        if (!PlayerPrefs.HasKey("AspectRatio"))
            PlayerPrefs.SetInt("AspectRatio", 2);

        if (!PlayerPrefs.HasKey("Resolution"))
            PlayerPrefs.SetInt("Resolution", 0);

        if (!PlayerPrefs.HasKey("QualityLevel"))
            PlayerPrefs.SetInt("QualityLevel", 2);

        if (Application.isWebPlayer)
            PlayerPrefs.SetInt("Fullscreen", 0);
        else if (!PlayerPrefs.HasKey("Fullscreen"))
            PlayerPrefs.SetInt("Fullscreen", Screen.fullScreen ? 1 : 0);
        
        if (!PlayerPrefs.HasKey("DisplayScore"))
            PlayerPrefs.SetInt("DisplayScore", 1);

        if (!PlayerPrefs.HasKey("DisplayCrosshair"))
            PlayerPrefs.SetInt("DisplayCrosshair", 1);

        if (!PlayerPrefs.HasKey("DisplayHints"))
            PlayerPrefs.SetInt("DisplayHints", 1);

        if (!PlayerPrefs.HasKey("FOV"))
            PlayerPrefs.SetFloat("FOV", 90);

        if (!PlayerPrefs.HasKey("MenuKey"))
        {
            if (Application.isWebPlayer)
                PlayerPrefs.SetInt("MenuKey", (int)KeyCode.F1);
            else
                PlayerPrefs.SetInt("MenuKey", (int)KeyCode.Escape);
        }

        if (!PlayerPrefs.HasKey("JumpKey"))
            PlayerPrefs.SetInt("JumpKey", (int)KeyCode.Space);

       if (!PlayerPrefs.HasKey("ForwardKey"))
            PlayerPrefs.SetInt("ForwardKey", (int)KeyCode.Z);

       if (!PlayerPrefs.HasKey("BackwardKey"))
            PlayerPrefs.SetInt("BackwardKey", (int)KeyCode.S);

       if (!PlayerPrefs.HasKey("StrafeLeftKey"))
            PlayerPrefs.SetInt("StrafeLeftKey", (int)KeyCode.Q);

       if (!PlayerPrefs.HasKey("StrafeRightKey"))
            PlayerPrefs.SetInt("StrafeRightKey", (int)KeyCode.D);

       if (!PlayerPrefs.HasKey("SwitchWorldKey"))
            PlayerPrefs.SetInt("SwitchWorldKey", (int)KeyCode.A);

      if (!PlayerPrefs.HasKey("CarryObjectKey"))
            PlayerPrefs.SetInt("CarryObjectKey", (int)KeyCode.E);

      if (!PlayerPrefs.HasKey("RespawnKey"))
            PlayerPrefs.SetInt("RespawnKey", (int)KeyCode.R);

      if (!PlayerPrefs.HasKey("MouseSensitivity"))
          PlayerPrefs.SetFloat("MouseSensitivity", 80);

      if (!PlayerPrefs.HasKey("InvertedMouse"))
          PlayerPrefs.SetInt("InvertedMouse", 1);

          PlayerPrefs.Save();
    }

    /**
     * LoadResolution()
     *  --> loads and set the resolutions to the parameters given by the user in the options menu
     * */
    void LoadResolution()
    {
        String resSt;
        if (m_ratio == 0)
            resSt = resolution_4_3[m_resolution];
        else if (m_ratio == 1)
            resSt = resolution_16_10[m_resolution];
        else
            resSt = resolution_16_9[m_resolution];


        String[] resTab = resSt.Split('x');

        Screen.SetResolution(int.Parse(resTab[0]), int.Parse(resTab[1]), m_fullscreen);

    }

    /**
     * LoadFromPlayerPrefs()
     *  -> assign corresponding values to UI elements from PlayerPrefs
     * */
    void LoadFromPlayerPrefs(String st="")
    {
        if (st.Equals(""))
        {
            m_sound_effects_volume = PlayerPrefs.GetFloat("SoundVolume") * 10;
            m_music_volume = PlayerPrefs.GetFloat("MusicVolume") * 10;
            m_mouse_sensitivity = PlayerPrefs.GetFloat("MouseSensitivity") / 10;
            m_inverted_mouse = PlayerPrefs.GetInt("InvertedMouse") == 1 ? false : true;
            m_display_crosshair = PlayerPrefs.GetInt("DisplayCrosshair") == 1 ? true : false;
            m_display_hints = PlayerPrefs.GetInt("DisplayHints") == 1 ? true : false;
            m_fov = PlayerPrefs.GetFloat("FOV");
            m_display_score = PlayerPrefs.GetInt("DisplayScore") == 1 ? true : false;
            m_fullscreen = PlayerPrefs.GetInt("Fullscreen") == 1 ? true : false;
            m_ratio = PlayerPrefs.GetInt("AspectRatio");
        }
        else if (st.Equals("video"))
        {
            m_display_crosshair = PlayerPrefs.GetInt("DisplayCrosshair") == 1 ? true : false;
            m_display_hints = PlayerPrefs.GetInt("DisplayHints") == 1 ? true : false;
            m_fov = PlayerPrefs.GetFloat("FOV");
            m_display_score = PlayerPrefs.GetInt("DisplayScore") == 1 ? true : false;
            m_fullscreen = PlayerPrefs.GetInt("Fullscreen") == 1 ? true : false;
            m_ratio = PlayerPrefs.GetInt("AspectRatio");
            quality = PlayerPrefs.GetInt("QualityLevel");
            m_resolution = PlayerPrefs.GetInt("Resolution");
            QualitySettings.SetQualityLevel(quality);
            if (!Application.isWebPlayer)
                LoadResolution();

        }
        else if (st.Equals("sound"))
        {
            m_sound_effects_volume = PlayerPrefs.GetFloat("SoundVolume")*10;
            m_music_volume = PlayerPrefs.GetFloat("MusicVolume")*10;
        }
        else if (st.Equals("controls"))
        {
            m_mouse_sensitivity = PlayerPrefs.GetFloat("MouseSensitivity") / 10;
            m_inverted_mouse = PlayerPrefs.GetInt("InvertedMouse") == 1 ? false : true;
        }
    }

    /**
     * SetPlayerPrefs()
     *  --> sets the player prefs to match ui component values
     * */
    void SetPlayerPrefs(String st = "")
    {
        if (st.Equals(""))
        {
            PlayerPrefs.SetInt("DisplayCrosshair", m_display_crosshair ? 1 : 0);
            PlayerPrefs.SetInt("DisplayHints", m_display_hints ? 1 : 0);
            PlayerPrefs.SetFloat("FOV", m_fov);
            PlayerPrefs.SetInt("DisplayScore", m_display_score ? 1 : 0);
            PlayerPrefs.SetInt("Fullscreen", m_fullscreen ? 1 : 0);
            PlayerPrefs.SetInt("AspectRatio", m_ratio);
            PlayerPrefs.SetInt("Resolution", m_resolution);
            PlayerPrefs.SetFloat("SoundVolume", m_sound_effects_volume / 10);
            PlayerPrefs.SetFloat("MusicVolume", m_music_volume / 10);
            PlayerPrefs.SetFloat("MouseSensitivity", m_mouse_sensitivity * 10);
            PlayerPrefs.SetInt("InvertedMouse", m_inverted_mouse ? -1 : 1);

        }
        else if (st.Equals("video"))
        {
            bool _ReloadNeeded = false;

            int _ratio = PlayerPrefs.GetInt("AspectRatio");
            int _resolution = PlayerPrefs.GetInt("Resolution");
            bool _fullscreen = PlayerPrefs.GetInt("Fullscreen")  == 1;

            if (_ratio != m_ratio || _resolution != m_resolution || _fullscreen != m_fullscreen)
                _ReloadNeeded = true;

            PlayerPrefs.SetInt("DisplayCrosshair", m_display_crosshair ? 1 : 0);
            PlayerPrefs.SetInt("DisplayHints", m_display_hints ? 1 : 0);
            PlayerPrefs.SetFloat("FOV", m_fov);
            PlayerPrefs.SetInt("DisplayScore", m_display_score ? 1 : 0);
            PlayerPrefs.SetInt("Fullscreen", m_fullscreen ? 1 : 0);
            PlayerPrefs.SetInt("AspectRatio", m_ratio);
            PlayerPrefs.SetInt("Resolution", m_resolution);
            PlayerPrefs.SetInt("QualityLevel", quality);

            if (!Application.isWebPlayer && _ReloadNeeded)
                LoadResolution();
            
        }
        else if (st.Equals("sound"))
        {
            PlayerPrefs.SetFloat("SoundVolume", m_sound_effects_volume / 10);
            PlayerPrefs.SetFloat("MusicVolume", m_music_volume / 10);
        }

        else if (st.Equals("controls"))
        {
            PlayerPrefs.SetFloat("MouseSensitivity", m_mouse_sensitivity * 10);
            PlayerPrefs.SetInt("InvertedMouse", m_inverted_mouse ? -1 : 1);
        }

        PlayerPrefs.Save();
    }

    KeyCode GetKeyCode(String st)
    {
        String st2 = st.ToUpper();

        switch (st2)
        {
            case "A": return KeyCode.A;
            case "B": return KeyCode.B;
            case "C": return KeyCode.C;
            case "D": return KeyCode.D;
            case "E": return KeyCode.E;
            case "F": return KeyCode.F;
            case "G": return KeyCode.G;
            case "H": return KeyCode.H;
            case "I": return KeyCode.I;
            case "J": return KeyCode.J;
            case "K": return KeyCode.K;
            case "L": return KeyCode.L;
            case "M": return KeyCode.M;
            case "N": return KeyCode.N;
            case "O": return KeyCode.O;
            case "P": return KeyCode.P;
            case "Q": return KeyCode.Q;
            case "R": return KeyCode.R;
            case "S": return KeyCode.S;
            case "T": return KeyCode.T;
            case "U": return KeyCode.U;
            case "V": return KeyCode.V;
            case "W": return KeyCode.W;
            case "X": return KeyCode.X;
            case "Y": return KeyCode.Y;
            case "Z": return KeyCode.Z;
        }
        return KeyCode.Dollar;
    }

    String GetStringFromKeycode(KeyCode kc)
    {
        switch (kc)
        {
            case KeyCode.A : return "A";
            case KeyCode.B : return "B";
            case KeyCode.C : return "C";
            case KeyCode.D : return "D";
            case KeyCode.E : return "E";
            case KeyCode.F : return "F";
            case KeyCode.G : return "G";
            case KeyCode.H : return "H";
            case KeyCode.I : return "I";
            case KeyCode.J : return "J";
            case KeyCode.K : return "K";
            case KeyCode.L : return "L";
            case KeyCode.M : return "M";
            case KeyCode.N : return "N";
            case KeyCode.O : return "O";
            case KeyCode.P : return "P";
            case KeyCode.Q : return "Q";
            case KeyCode.R : return "R";
            case KeyCode.S : return "S";
            case KeyCode.T : return "T";
            case KeyCode.U : return "U";
            case KeyCode.V : return "V";
            case KeyCode.W : return "W";
            case KeyCode.X : return "X";
            case KeyCode.Y : return "Y";
            case KeyCode.Z : return "Z";
        }
        return "nc";
    }

    /**
     * LoadKeysFromPrefs()
     *  --> assign gui component values concerning key bindings to match player prefs values
     * */
    void LoadKeysFromPrefs()
    {
        m_keybindings[0] =  GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("ForwardKey"));
        m_keybindings[1] =  GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("BackwardKey"));
        m_keybindings[2] =  GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("StrafeLeftKey"));
        m_keybindings[3] =  GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("StrafeRightKey"));
        m_keybindings[4] =  GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("SwitchWorldKey"));
        m_keybindings[5] =  GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("CarryObjectKey"));
        m_keybindings[6] =  GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("RespawnKey"));
    }

    /**
     * SetKeysPlayerPrefs()
     *  --> sets players prefs concerning key bindings to match gui component values
     * */
    void SetKeysPlayerPrefs()
    {
        KeyCode kb;
        kb = GetKeyCode(m_keybindings[0]);
        if (kb != KeyCode.Dollar)
            PlayerPrefs.SetInt("ForwardKey", (int)kb);
        kb = GetKeyCode(m_keybindings[1]);
        if (kb != KeyCode.Dollar)
            PlayerPrefs.SetInt("BackwardKey", (int)kb);
        kb = GetKeyCode(m_keybindings[2]);
        if (kb != KeyCode.Dollar)
            PlayerPrefs.SetInt("StrafeLeftKey", (int)kb);
        kb = GetKeyCode(m_keybindings[3]);
        if (kb != KeyCode.Dollar)
            PlayerPrefs.SetInt("StrafeRightKey", (int)kb);
        kb = GetKeyCode(m_keybindings[4]);
        if (kb != KeyCode.Dollar)
            PlayerPrefs.SetInt("SwitchWorldKey", (int)kb);
        kb = GetKeyCode(m_keybindings[5]);
        if (kb != KeyCode.Dollar)
            PlayerPrefs.SetInt("CarryObjectKey", (int)kb);
        kb = GetKeyCode(m_keybindings[6]);
        if (kb != KeyCode.Dollar)
            PlayerPrefs.SetInt("RespawnKey", (int)kb);
    }

    /**
     * LoadPlayerPrefsConnection()
     *  --> assign corresponding values to gui component to match player prefs values concerning the account options 
     * */
    void LoadPlayerPrefsConnection()
    {
        if (PlayerPrefs.HasKey("IsLoggedIn"))
        {
            username = PlayerPrefs.GetString("Username");
            password = PlayerPrefs.GetString("Password");
        }
    }
    
    /**
     * SetPlayerPrefsConnection()
     *  --> sets the player prefs values to match gui component values concerning account options
     * */
    void SetPlayerPrefsConnection()
    {
        PlayerPrefs.SetString("Username", username);
        PlayerPrefs.SetString("Password", password);
        PlayerPrefs.SetInt("IsLoggedIn", 1);

        print("Player Prefs DEFINED TO " + username + " " + password + " " + PlayerPrefs.HasKey("IsLoggedIn"));
    }

    public void StartNewGame()
    {
        PlayerPrefs.SetFloat("Score", 0);
        PlayerPrefs.SetFloat("ElapsedTime", 0);
        PlayerPrefs.SetInt("DeathCount", 0);
		PlayerPrefs.SetInt("Playthrough",1);
        print("Start new Game");
		SaveManager.last_save = null;
        Application.LoadLevel("level_one");

    }

    public void ContinueGame()
    {
        print("Continue");
		PlayerPrefs.SetInt("Playthrough",1);
		SaveManager.LoadFromDisk();
        SaveManager.LoadLastSave();
    }

    void OnGUI()
    {
		
		GUI.DrawTexture(new Rect(20, 50, 307*1.4f, 31*1.4f),logo);
        GUI.Box(ResizeGUI(new Rect(10, 530, 780, 40)), "", skin.box);

        GUI.Label(ResizeGUI(new Rect(20, 530, 500, 40)), "Student project made by Cyril Basset and Jean-Vincent Lamberti", skin.label);
        GUI.Label(ResizeGUI(new Rect(20, 550, 800, 40)), "For the music tracks all credits goes to Parametric, go check his work at http://http://sgustokmusic.org/", skin.label);

        if (SaveManager.CheckSaveFile())
		{
        	if (GUI.Button(ResizeGUI(new Rect(20, 150, 100, 30)), "Continue", skin.button))
        	{
	            if (menu != MainMenuSelected.CONTINUE_GAME_SELECTED)
	            {
	                menu = MainMenuSelected.CONTINUE_GAME_SELECTED;
	                submenu = SubMenuSelected.NO_SELECTED;
	            }
	            else
	            {
	                menu = MainMenuSelected.NO_SELECTED;
	                submenu = SubMenuSelected.NO_SELECTED;
	            }
	
	            if (!PlayerPrefs.HasKey("IsLoggedIn"))
	            {
	                display_warning_login = 2;
	            }
	            else
	                ContinueGame();
       	 	}
		}
		
        if (GUI.Button(ResizeGUI(new Rect(20, 200, 100, 30)), "New game", skin.button))
        {
            if (menu != MainMenuSelected.NEW_GAME_SELECTED)
            {
                menu = MainMenuSelected.NEW_GAME_SELECTED;
                submenu = SubMenuSelected.NO_SELECTED;
            }
            else
            {
                menu = MainMenuSelected.NO_SELECTED;
                submenu = SubMenuSelected.NO_SELECTED;
            }

            if (!PlayerPrefs.HasKey("IsLoggedIn"))
            {
                display_warning_login = 1;
            }
            else
                StartNewGame();

        }

        if (GUI.Button(ResizeGUI(new Rect(20, 250, 100, 30)), "Levels", skin.button))
        {
            display_warning_login = 0;

            if (menu != MainMenuSelected.LEVEL_SELECTED)
            {
                menu = MainMenuSelected.LEVEL_SELECTED;
                submenu = SubMenuSelected.NO_SELECTED;
            }
            else
            {
                menu = MainMenuSelected.NO_SELECTED;
                submenu = SubMenuSelected.NO_SELECTED;
            }
        }
        if (GUI.Button(ResizeGUI(new Rect(20, 300, 100, 30)), "Options", skin.button))
        {
            display_warning_login = 0;

            if (menu != MainMenuSelected.OPTION_SELECTED)
                menu = MainMenuSelected.OPTION_SELECTED;
            else
            {
                menu = MainMenuSelected.NO_SELECTED;
                submenu = SubMenuSelected.NO_SELECTED;
            }
        }
        if (GUI.Button(ResizeGUI(new Rect(20, 350, 100, 30)), "High-Score", skin.button))
        {
            display_warning_login = 0;

            m_db_handler.FetchScores();

            if (menu != MainMenuSelected.SCORE_SELECTED)
            {
                menu = MainMenuSelected.SCORE_SELECTED;
                submenu = SubMenuSelected.NO_SELECTED;
            }
            else
            {
                menu = MainMenuSelected.NO_SELECTED;
                submenu = SubMenuSelected.NO_SELECTED;
            }
        }
        if (!Application.isWebPlayer)
        {
            if (GUI.Button(ResizeGUI(new Rect(20, 400, 100, 30)), "Quit", skin.button))
            {
                Application.Quit();
            }
        }

        
        if(menu == MainMenuSelected.OPTION_SELECTED)
        {
            display_warning_login = 0;

            if (GUI.Button(ResizeGUI(new Rect(140, 300, 100, 30)), "Video", skin.button))
            {
	            if(submenu != SubMenuSelected.VIDEO_SELECTED)
	                submenu = SubMenuSelected.VIDEO_SELECTED;
	            else submenu = SubMenuSelected.NO_SELECTED;
            }
            if (GUI.Button(ResizeGUI(new Rect(140, 350, 100, 30)), "Sound", skin.button))
            {
	            if(submenu != SubMenuSelected.SOUND_SELECTED)
	                submenu = SubMenuSelected.SOUND_SELECTED;
	            else submenu = SubMenuSelected.NO_SELECTED;
            }
            if (GUI.Button(ResizeGUI(new Rect(140, 400, 100, 30)), "Controls", skin.button))
            {
	            if(submenu != SubMenuSelected.CONTROLS_SELECTED)
	                submenu = SubMenuSelected.CONTROLS_SELECTED;
	            else submenu = SubMenuSelected.NO_SELECTED;
            }
            if (GUI.Button(ResizeGUI(new Rect(140, 450, 100, 30)), "Account", skin.button))
            {
                display_connection_message = false;
                connection_in_progress = false;
                LoadPlayerPrefsConnection();

	            if(submenu != SubMenuSelected.ACCOUNT_SELECTED)
	                submenu = SubMenuSelected.ACCOUNT_SELECTED;
	            else submenu = SubMenuSelected.NO_SELECTED;
            }
        }

        if (menu == MainMenuSelected.LEVEL_SELECTED)
        {
            display_warning_login = 0;

            GUI.Box(ResizeGUI(new Rect(260, 120, 500, 400)), "Level selection", skin.box);
            GUI.BeginGroup(ResizeGUI(new Rect(260, 120, 500, 400)));

            int max_reached = PlayerPrefs.GetInt("MaxLevelReached");
            int j=0;
            for (int i = 0; i < max_reached; i++)
            {
                if (i % 3 == 0)
                    j++;
                if (GUI.Button(ResizeGUI(new Rect((i % 3 * 140) + (i % 3 + 1) * 20, (j + 1) * 30 + j * 140 - 170, 70 * 2, 70 * 2), true), "Level " + (i + 1), skin.button))
				{
					PlayerPrefs.DeleteKey("Playthrough");
					SaveManager.last_save = null;
                    Application.LoadLevel(i + 1);
				}
            }

            GUI.EndGroup();
        }
       
        if (menu == MainMenuSelected.SCORE_SELECTED)
        {
            display_warning_login = 0;

            GUI.Box(ResizeGUI(new Rect(260, 120, 500, 400)), "High Scores", skin.box);
            String[] tab = m_db_handler.GetScoresTab();

            if (tab.Length == 0)
                GUI.Box(ResizeGUI(new Rect(385, 200, 250, 200)), "Loading Scores", skin.box);
            else
            {
                int i = 0;
                GUI.Label(ResizeGUI(new Rect(270 + (i++ * 100), 150, 100, 30)), "Rank", skin.label);
                GUI.Label(ResizeGUI(new Rect(270 + (i++ * 100), 150, 100, 30)), "Username", skin.label);
                GUI.Label(ResizeGUI(new Rect(270 + (i++ * 100), 150, 100, 30)), "Time", skin.label);
                GUI.Label(ResizeGUI(new Rect(270 + (i++ * 100), 150, 100, 30)), "Deathcount", skin.label);
                GUI.Label(ResizeGUI(new Rect(270 + (i++ * 100), 150, 100, 30)), "Score", skin.label);

                int c = 0;
                for (int j = 0; j < tab.Length / 5; j++)
                {
                    for (i = 0; i < 5; i++)
                    {
                        GUI.Label(ResizeGUI(new Rect(270 + (i * 100), 180 + (j * 30), 100, 30)), tab[c++], skin.label);
                    }
                }
            }
        }

        if (display_warning_login != 0)
        {
            GUI.Box(ResizeGUI(new Rect(260, 120, 500, 150)), "Warning, you are not logged in,", skin.box);
            GUI.Label(ResizeGUI(new Rect(310, 140, 400, 400)), "you won't be able to upload your score unless you are logged, in Options / Account, during your whole playthrough. Do you wish to continue ?", skin.label);
            if (GUI.Button(ResizeGUI(new Rect(380, 200, 100, 40)), "Continue"))
            {
                if (display_warning_login == 1)
                    StartNewGame();
                else if (display_warning_login == 2)
                    ContinueGame();
            }
            if (GUI.Button(ResizeGUI(new Rect(520, 200, 100, 40)), "Cancel"))
            {
                display_warning_login = 0;
            }

        }

		if(submenu == SubMenuSelected.VIDEO_SELECTED)
		{

            GUI.Box(ResizeGUI(new Rect(260, 120, 500, 400)), "Video Settings", skin.box);
            GUI.BeginGroup(ResizeGUI(new Rect(310, 120, 500, 600)));

            if (!Application.isWebPlayer)
            {

                GUI.Label(ResizeGUI(new Rect(35, 30, 100, 40)), "Aspect Ratio", skin.label);

                int selGrid;
                if ((selGrid = comboBoxQuality.List(ResizeGUI(new Rect(20, 330, 100, 20)), m_quality[quality].text, m_quality, skin.customStyles[0])) != quality)
                {
                    quality = selGrid;
                }

                GUI.Label(ResizeGUI(new Rect(40, 140, 100, 40)), "Resolution", skin.label);

                switch (m_ratio)
                {
                    case 0:
                        if ((selGrid = comboBoxResolution.List(ResizeGUI(new Rect(20, 170, 100, 20)), _4_3_combobox[m_resolution].text, _4_3_combobox, skin.customStyles[0])) != m_resolution)
                        {
                            m_resolution = selGrid;
                        }
                        break;
                    case 1:
                        if ((selGrid = comboBoxResolution.List(ResizeGUI(new Rect(20, 170, 100, 20)), _16_10_combobox[m_resolution].text, _16_10_combobox, skin.customStyles[0])) != m_resolution)
                        {
                            m_resolution = selGrid;
                        }
                        break;
                    case 2:
                        if ((selGrid = comboBoxResolution.List(ResizeGUI(new Rect(20, 170, 100, 20)), _16_9_combobox[m_resolution].text, _16_9_combobox, skin.customStyles[0])) != m_resolution)
                        {
                            m_resolution = selGrid;
                        }
                        break;
                }

                GUI.Label(ResizeGUI(new Rect(50, 300, 100, 40)), "Quality", skin.label);

                if ((selGrid = comboBoxControl.List(ResizeGUI(new Rect(20, 60, 100, 20)), ratio_combobox[m_ratio].text, ratio_combobox, skin.customStyles[0])) != m_ratio)
                {
                    m_ratio = selGrid;
                }
            }
            else
            {
                int selGrid;
                GUI.Label(ResizeGUI(new Rect(50, 100, 100, 40)), "Quality", skin.label);
                if ((selGrid = comboBoxQuality.List(ResizeGUI(new Rect(20, 130, 100, 20)), m_quality[quality].text, m_quality, skin.customStyles[0])) != quality)
                {
                    quality = selGrid;
                }

            }

            GUI.Label(ResizeGUI(new Rect(230, 30, 100, 40)),"Field of view",skin.label);

            m_fov = GUI.HorizontalSlider(ResizeGUI(new Rect(170, 65, 200, 30)), m_fov, 50.0f, 130.0f, skin.horizontalSlider, skin.horizontalSliderThumb);

            GUI.Label(ResizeGUI(new Rect(390, 60, 220, 40)), Mathf.RoundToInt(m_fov).ToString());

            GUI.Label(ResizeGUI(new Rect(200, 110, 100, 40)), "Score :");

            m_display_score = GUI.Toggle(ResizeGUI(new Rect(290, 110, 100, 40)), m_display_score, m_display_score ? "Show" : "Hide");


            GUI.Label(ResizeGUI(new Rect(200, 160, 100, 40)), "Fullscreen :");

            m_fullscreen = GUI.Toggle(ResizeGUI(new Rect(290, 160, 100, 40)), m_fullscreen, m_fullscreen ? "True" : "False");

            GUI.Label(ResizeGUI(new Rect(200, 210, 100, 40)), "Crosshair :", skin.label);
            m_display_crosshair = GUI.Toggle(ResizeGUI(new Rect(290, 210, 100, 40)), m_display_crosshair, m_display_crosshair ? "  Show" : "  Hide", skin.toggle);

            GUI.Label(ResizeGUI(new Rect(200, 260, 100, 40)), "Hints and tutorials :", skin.label);
            m_display_hints = GUI.Toggle(ResizeGUI(new Rect(290, 260, 100, 40)), m_display_hints, m_display_hints == true ? "  Show" : "  Hide", skin.toggle);

            GUI.EndGroup();

            SetPlayerPrefs("video");

		}
        
        if (submenu == SubMenuSelected.SOUND_SELECTED)
        {

            LoadFromPlayerPrefs("sound");

            GUI.Box(ResizeGUI(new Rect(260, 120, 500, 400)), "Sound Settings", skin.box);
            GUI.BeginGroup(ResizeGUI(new Rect(260, 120, 500, 400)));
            GUI.Label(ResizeGUI(new Rect(50, 50, 200, 40)), "Music Volume", skin.label);
            m_music_volume = GUI.HorizontalSlider(ResizeGUI(new Rect(50, 90, 200, 20)), m_music_volume, 0, 10.0f, skin.horizontalSlider, skin.horizontalSliderThumb);
            GUI.Label(ResizeGUI(new Rect(260, 80, 60, 40)), Math.Round(m_music_volume * 10, 0).ToString() + "%", skin.label);
            GUI.Label(ResizeGUI(new Rect(50, 130, 300, 40)), "Sound Effects Volume", skin.label);
            m_sound_effects_volume = GUI.HorizontalSlider(ResizeGUI(new Rect(50, 170, 200, 20)), m_sound_effects_volume, 0, 10.0f, skin.horizontalSlider, skin.horizontalSliderThumb);
            GUI.Label(ResizeGUI(new Rect(260, 160, 60, 40)), Math.Round(m_sound_effects_volume * 10, 0).ToString() + "%", skin.label);


            GUI.EndGroup();

            SetPlayerPrefs("sound");
        }
        if (submenu == SubMenuSelected.CONTROLS_SELECTED)
        {

            LoadFromPlayerPrefs("controls");

            GUI.Box(ResizeGUI(new Rect(260, 120, 500, 400)), "Controls Settings", skin.box);
            GUI.BeginGroup(ResizeGUI(new Rect(260, 120, 500, 400)));
            GUI.Label(ResizeGUI(new Rect(10, 30, 70, 40)), "Keybindings :", skin.label);
            //Key Bindings
            GUI.Box(ResizeGUI(new Rect(80, 30, 400, 250)), "", skin.box);
            m_keybindings_scrollPosition = GUI.BeginScrollView(ResizeGUI(new Rect(80, 30, 400, 250)), m_keybindings_scrollPosition, ResizeGUI(new Rect(0, 0, 200, 25 * (m_keybindings_labels.Length + 1))));
            GUI.Label(ResizeGUI(new Rect(10, 0, 400, 40)), "You can click and type any letter from A to Z to assign it", skin.label);

            int i;
            for (i = 0; i < m_keybindings_labels.Length; i++)
            {
                GUI.Label(ResizeGUI(new Rect(10, 25 * (i + 1), 80, 40)), m_keybindings_labels[i] + " :", skin.label);
                m_keybindings[i] = GUI.TextField(ResizeGUI(new Rect(130, 25 * (i + 1), 20, 20)), m_keybindings[i], 1, skin.textField);
            }
            GUI.Label(ResizeGUI(new Rect(10, 25 * (i + 1), 80, 40)), "Jump :", skin.label);
            GUI.Label(ResizeGUI(new Rect(130, 25 * (i + 1), 80, 40)), "Space", skin.label);
            i++;
            GUI.Label(ResizeGUI(new Rect(10, 25 * (i + 1), 80, 40)), "Pause :", skin.label);
            GUI.Label(ResizeGUI(new Rect(130, 25 * (i + 1), 150, 40)), "Escape (F1 on Webplayer)", skin.label);



            GUI.EndScrollView();
            //End Key Bindings

            if (GUI.Button(ResizeGUI(new Rect(420, 290, 60, 20)), "Apply", skin.button))
            {
                SetKeysPlayerPrefs();
                LoadKeysFromPrefs();
            }
            if (GUI.Button(ResizeGUI(new Rect(350, 290, 60, 20)), "Default", skin.button))
            {
                m_keybindings = m_keybindings_default;
                SetKeysPlayerPrefs();
                LoadKeysFromPrefs();
            }

            GUI.Label(ResizeGUI(new Rect(20, 290, 150, 40)), "Mouse Sensitivity", skin.label);
            m_mouse_sensitivity = GUI.HorizontalSlider(ResizeGUI(new Rect(20, 315, 150, 10)), m_mouse_sensitivity, 0.1f, 10.0f, skin.horizontalSlider, skin.horizontalSliderThumb);
            GUI.Label(ResizeGUI(new Rect(175, 310, 60, 40)), Math.Round(m_mouse_sensitivity, 2).ToString(), skin.label);

            GUI.Label(ResizeGUI(new Rect(20, 330, 90, 40)), "Vertical Mouse :", skin.label);
            m_inverted_mouse = GUI.Toggle(ResizeGUI(new Rect(110, 330, 70, 20)), m_inverted_mouse, m_inverted_mouse == true ? "  Inverted" : "  Classic", skin.toggle);

            GUI.EndGroup();

            SetPlayerPrefs("controls");
        }
        if (submenu == SubMenuSelected.ACCOUNT_SELECTED)
        {

            GUI.Box(ResizeGUI(new Rect(260, 120, 500, 400)), "Account Settings", skin.box);
            GUI.BeginGroup(ResizeGUI(new Rect(260, 150, 500, 600)));
            GUI.Label(ResizeGUI(new Rect(10, 5, 340, 40)), "Don't have an account yet? Create one on our website by clicking on the register button", skin.label);
            
            if (GUI.Button(ResizeGUI(new Rect(360, 5, 100, 40)), "Register"))
            {
                if (Application.isWebPlayer)
                    Application.ExternalEval("window.open('http://www.flyingminutegames.com/registration/','Registration')");
                else
                    Application.OpenURL("http://www.flyingminutegames.com/registration/");
            }

            GUI.Label(ResizeGUI(new Rect(220, 50, 100, 40)), "Username", skin.label);

            username = GUI.TextField(ResizeGUI(new Rect(150, 80, 200, 40)), username, skin.textField);

            GUI.Label(ResizeGUI(new Rect(220, 130, 100, 40)), "Password", skin.label);

            password = GUI.PasswordField(ResizeGUI(new Rect(150, 160, 200, 40)), password, '*', skin.textField);

            if (GUI.Button(ResizeGUI(new Rect(80, 260, 100, 40)), "Try"))
            {
                m_db_handler.TryConnection(username, password);
                display_connection_message = true;
            }
            if (display_connection_message)
                GUI.Label(ResizeGUI(new Rect(80, 220, 500, 40)), m_db_handler.GetConnectionMessage(), skin.label);

            if (GUI.Button(ResizeGUI(new Rect(220, 260, 100, 40)), "Apply"))
            {
                m_db_handler.TryConnection(username, password);
                display_connection_message = true;
                connection_in_progress = true;
            }

            if (connection_in_progress)
            {
                if (m_db_handler.GetValidPassword() != null && m_db_handler.GetValidUsername() != null)
                {
                    if (m_db_handler.GetValidUsername().Equals(username) && m_db_handler.GetValidPassword().Equals(password))
                    {
                        SetPlayerPrefsConnection();
                        LoadPlayerPrefsConnection();
                        connection_in_progress = false;
                    }
                }
            }

            if (GUI.Button(ResizeGUI(new Rect(360, 260, 100, 40)), "Logout"))
            {
                connection_in_progress = false;
                display_connection_message = false;
                
                if (PlayerPrefs.HasKey("IsLoggedIn"))
                {
                    PlayerPrefs.DeleteKey("IsLoggedIn");
                    PlayerPrefs.DeleteKey("Username");
                    PlayerPrefs.DeleteKey("Password");
                    username = "";
                    password = "";
                }
            }

            GUI.EndGroup();
        }


    }

    Rect ResizeGUI(Rect _rect, bool uniformScale = false)
    {
		Vector2 scale = new Vector2(Screen.width/800.0f,Screen.height/600.0f);
		if(uniformScale)
			  scale = new Vector2(scale.x < scale.y ? scale.x : scale.y, scale.x < scale.y ? scale.x : scale.y);
		float rectX = _rect.x * scale.x;
        float rectY = _rect.y * scale.y;
        float rectWidth = _rect.width * scale.x;
        float rectHeight = _rect.height * scale.y;
		return new Rect(rectX, rectY, rectWidth, rectHeight);
		
    }
}
