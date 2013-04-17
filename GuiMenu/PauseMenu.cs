using UnityEngine;
using System.Collections;
using System;

public class PauseMenu : MonoBehaviour {

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
    private static String[] ratio_string = { "4/3", "16/10", "16/9" };
    private static String[] resolution_4_3 = { "640x480", "800x600", "1024x768", "1280x960", "1440x1080" };
    private static String[] resolution_16_10 = { "1280x800", "1440x900", "1680x1050" };
    private static String[] resolution_16_9 = { "1280x720", "1366x768", "1600x900", "1920x1080", "2560×1440" };
    private static String[] quality_string = { "Fastest", "Fast", "Simple", "Good", "Beautiful", "Fanstatic" };
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
	private ScreenEffectScript m_ScreenEffectMenu; 
	

    enum MainMenuSelected
    {
        NO_SELECTED,
        OPTION_SELECTED,
        SCORE_SELECTED,
        CONTINUE_GAME_SELECTED
    }
    enum SubMenuSelected
    {
        NO_SELECTED,
        VIDEO_SELECTED,
        SOUND_SELECTED,
        CONTROLS_SELECTED
    }



    void Start()
    {
		m_ScreenEffectMenu = GameObject.Find("MenuEffectGlobalScript").GetComponent<ScreenEffectScript>();
		m_ScreenEffectMenu.Disable();
        m_keybindings = new String[m_keybindings_labels.Length];
        ratio_combobox = new GUIContent[3];
        _4_3_combobox = new GUIContent[resolution_4_3.Length];
        _16_10_combobox = new GUIContent[resolution_16_10.Length];
        _16_9_combobox = new GUIContent[resolution_16_9.Length];
        m_quality = new GUIContent[quality_string.Length];
        menu = MainMenuSelected.NO_SELECTED;
        submenu = SubMenuSelected.NO_SELECTED;
        for (int i = 0; i < ratio_string.Length; i++)
            ratio_combobox[i] = new GUIContent(ratio_string[i]);

        for (int i = 0; i < resolution_4_3.Length; i++)
            _4_3_combobox[i] = new GUIContent(resolution_4_3[i]);
        for (int i = 0; i < resolution_16_10.Length; i++)
            _16_10_combobox[i] = new GUIContent(resolution_16_10[i]);
        for (int i = 0; i < resolution_16_9.Length; i++)
            _16_9_combobox[i] = new GUIContent(resolution_16_9[i]);

        for (int i = 0; i < quality_string.Length; i++)
            m_quality[i] = new GUIContent(quality_string[i]);

        skin.customStyles[0].hover.background = skin.customStyles[0].onHover.background = new Texture2D(2, 2);
        LoadKeysFromPrefs();
        LoadFromPlayerPrefs("video");
        m_db_handler = gameObject.AddComponent<DataBaseHandling>();
        this.enabled = false;
    }

    void LoadResolution()
    {
        m_ratio = PlayerPrefs.GetInt("AspectRatio");
        m_resolution = PlayerPrefs.GetInt("Resolution");

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

    void LoadFromPlayerPrefs(String st = "")
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
            LoadResolution();
        }
        else if (st.Equals("video"))
        {
            m_display_crosshair = PlayerPrefs.GetInt("DisplayCrosshair") == 1 ? true : false;
            m_display_hints = PlayerPrefs.GetInt("DisplayHints") == 1 ? true : false;
            m_fov = PlayerPrefs.GetFloat("FOV");
            m_display_score = PlayerPrefs.GetInt("DisplayScore") == 1 ? true : false;
            m_fullscreen = PlayerPrefs.GetInt("Fullscreen") == 1 ? true : false;
            Screen.fullScreen = m_fullscreen;
            m_ratio = PlayerPrefs.GetInt("AspectRatio");
            quality = PlayerPrefs.GetInt("QualityLevel");
            QualitySettings.SetQualityLevel(quality);
            if (!Application.isWebPlayer)
                LoadResolution();
        }
        else if (st.Equals("sound"))
        {
            m_sound_effects_volume = PlayerPrefs.GetFloat("SoundVolume") * 10;
            m_music_volume = PlayerPrefs.GetFloat("MusicVolume") * 10;
        }
        else if (st.Equals("controls"))
        {
            m_mouse_sensitivity = PlayerPrefs.GetFloat("MouseSensitivity") / 10;
            m_inverted_mouse = PlayerPrefs.GetInt("InvertedMouse") == 1 ? false : true;
        }
    }

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
            PlayerPrefs.SetInt("DisplayCrosshair", m_display_crosshair ? 1 : 0);
            PlayerPrefs.SetInt("DisplayHints", m_display_hints ? 1 : 0);
            PlayerPrefs.SetFloat("FOV", m_fov);
            PlayerPrefs.SetInt("DisplayScore", m_display_score ? 1 : 0);
            PlayerPrefs.SetInt("QualityLevel", quality);
            PlayerPrefs.SetInt("Fullscreen", m_fullscreen ? 1 : 0);

            if (!Application.isWebPlayer)
            {
                PlayerPrefs.SetInt("AspectRatio", m_ratio);
                PlayerPrefs.SetInt("Resolution", m_resolution);
            }

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
            case KeyCode.A: return "A";
            case KeyCode.B: return "B";
            case KeyCode.C: return "C";
            case KeyCode.D: return "D";
            case KeyCode.E: return "E";
            case KeyCode.F: return "F";
            case KeyCode.G: return "G";
            case KeyCode.H: return "H";
            case KeyCode.I: return "I";
            case KeyCode.J: return "J";
            case KeyCode.K: return "K";
            case KeyCode.L: return "L";
            case KeyCode.M: return "M";
            case KeyCode.N: return "N";
            case KeyCode.O: return "O";
            case KeyCode.P: return "P";
            case KeyCode.Q: return "Q";
            case KeyCode.R: return "R";
            case KeyCode.S: return "S";
            case KeyCode.T: return "T";
            case KeyCode.U: return "U";
            case KeyCode.V: return "V";
            case KeyCode.W: return "W";
            case KeyCode.X: return "X";
            case KeyCode.Y: return "Y";
            case KeyCode.Z: return "Z";
        }
        return "nc";
    }

    void LoadKeysFromPrefs()
    {
        m_keybindings[0] = GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("ForwardKey"));
        m_keybindings[1] = GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("BackwardKey"));
        m_keybindings[2] = GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("StrafeLeftKey"));
        m_keybindings[3] = GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("StrafeRightKey"));
        m_keybindings[4] = GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("SwitchWorldKey"));
        m_keybindings[5] = GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("CarryObjectKey"));
        m_keybindings[6] = GetStringFromKeycode((KeyCode)PlayerPrefs.GetInt("RespawnKey"));
    }


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

    public void Enable(bool active)
    {
        enabled = active;
        Screen.showCursor = active;
        Screen.lockCursor = !active;
        if (active)
            m_ScreenEffectMenu.Enable();
        else
            m_ScreenEffectMenu.Disable();
    }
	

    void OnGUI()
    {		
		GUI.DrawTexture(new Rect(20f, 20f, 307*1.4f, 31*1.4f),logo);

        if (GUI.Button(ResizeGUI(new Rect(20, 150, 100, 30)), "Continue", skin.button))
        {
			Enable(false);
        }

        if (GUI.Button(ResizeGUI(new Rect(20, 200, 100, 30)), "Restart Level", skin.button))
        {
			
			
			GameSave s = SaveManager.last_save;
			
			if(s != null)
			{
				s.time += GetComponent<ControllerScript>().GetTime();
				s.deathCount++;
			}
			SaveManager.SaveToDisk();
			
            Application.LoadLevel(Application.loadedLevel);
        }

        if (GUI.Button(ResizeGUI(new Rect(20, 250, 100, 30)), "Options", skin.button))
        {

            if (menu != MainMenuSelected.OPTION_SELECTED)
                menu = MainMenuSelected.OPTION_SELECTED;
            else
            {
                menu = MainMenuSelected.NO_SELECTED;
                submenu = SubMenuSelected.NO_SELECTED;
            }
        }
        if (GUI.Button(ResizeGUI(new Rect(20, 300, 100, 30)), "High-Score", skin.button))
        {

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

        if (GUI.Button(ResizeGUI(new Rect(20, 350, 100, 30)), "Return to Main Menu", skin.button))
        {
            Application.LoadLevel(0);
        }

        if (!Application.isWebPlayer)
        {
            if (GUI.Button(ResizeGUI(new Rect(20, 400, 100, 30)), "Quit", skin.button))
            {
                Application.Quit();
            }
        }


        if (menu == MainMenuSelected.OPTION_SELECTED)
        {

            if (GUI.Button(ResizeGUI(new Rect(140, 300, 100, 30)), "Video", skin.button))
            {
                if (submenu != SubMenuSelected.VIDEO_SELECTED)
                    submenu = SubMenuSelected.VIDEO_SELECTED;
                else submenu = SubMenuSelected.NO_SELECTED;
            }
            if (GUI.Button(ResizeGUI(new Rect(140, 350, 100, 30)), "Sound", skin.button))
            {
                if (submenu != SubMenuSelected.SOUND_SELECTED)
                    submenu = SubMenuSelected.SOUND_SELECTED;
                else submenu = SubMenuSelected.NO_SELECTED;
            }
            if (GUI.Button(ResizeGUI(new Rect(140, 400, 100, 30)), "Controls", skin.button))
            {
                if (submenu != SubMenuSelected.CONTROLS_SELECTED)
                    submenu = SubMenuSelected.CONTROLS_SELECTED;
                else submenu = SubMenuSelected.NO_SELECTED;
            }
        }

        if (menu == MainMenuSelected.SCORE_SELECTED)
        {

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

        if (submenu == SubMenuSelected.VIDEO_SELECTED)
        {

            LoadFromPlayerPrefs("video");

            GUI.Box(ResizeGUI(new Rect(260, 120, 500, 400)), "Video Settings", skin.box);
            GUI.BeginGroup(ResizeGUI(new Rect(310, 180, 500, 600)));

            GUI.Label(ResizeGUI(new Rect(35, 30, 100, 40)), "Aspect Ratio", skin.label);

            int selGrid;

            GUI.Label(ResizeGUI(new Rect(40, 100, 100, 40)), "Resolution", skin.label);
			
            if ((selGrid = comboBoxQuality.List(ResizeGUI(new Rect(20, 200, 100, 30)), m_quality[quality].text, m_quality, skin.customStyles[0])) != quality)
            {
                quality = selGrid;
            }
			
            switch (m_ratio)
            {
                case 0:
                    if ((selGrid = comboBoxResolution.List(ResizeGUI(new Rect(20, 130, 100, 30)), _4_3_combobox[m_resolution].text, _4_3_combobox, skin.customStyles[0])) != m_resolution)
                    {
                        m_resolution = selGrid;
                    }
                    break;
                case 1:
                    if ((selGrid = comboBoxResolution.List(ResizeGUI(new Rect(20, 130, 100, 30)), _16_10_combobox[m_resolution].text, _16_10_combobox, skin.customStyles[0])) != m_resolution)
                    {
                        m_resolution = selGrid;
                    }
                    break;
                case 2:
                    if ((selGrid = comboBoxResolution.List(ResizeGUI(new Rect(20, 130, 100, 30)), _16_9_combobox[m_resolution].text, _16_9_combobox, skin.customStyles[0])) != m_resolution)
                    {
                        m_resolution = selGrid;
                    }
                    break;
            }

            GUI.Label(ResizeGUI(new Rect(50, 170, 100, 40)), "Quality", skin.label);
			
            if ((selGrid = comboBoxControl.List(ResizeGUI(new Rect(20, 60, 100, 30)), ratio_combobox[m_ratio].text, ratio_combobox, skin.customStyles[0])) != m_ratio)
            {
                m_ratio = selGrid;
            }

            GUI.Label(ResizeGUI(new Rect(230, 30, 100, 40)), "Field of view", skin.label);

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
}
