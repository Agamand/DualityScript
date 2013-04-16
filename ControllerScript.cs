/**
*    FIXEDUPDATE
 *  ControllerScript
 *      --> The script used as a Character Controller which handles : 
 *          - player movement (partial jump handling)
 *          - input management
 *          - partial world change
 *          - camera
 *  
 *  Members: 
 *      public float m_Speed : the speed of the player
 *	    public float m_BackSpeed : the speed of the player while moving backwards
 *      public float m_Jump : the amount of force to give to the jump
 *	    public float m_MouseSpeed : the sensibility of the mouse
 *      public float m_MaxSpeed : the max speed of the player
 *      public float m_Baseforce : the base force factor to each speed and force
 *      private float m_Incl : mouse gradient
 *      private float m_Rot_Y : mouse rotation
 *      private Vector3 m_RespawnPosition : the respawn position of the player
 *      private Quaternion m_RespawnRotation : the respawn rotation of the player
 *      private Vector3 m_InitialVelocity : the velocity of the player when respawning
 *	    private JumperScript m_JumpHandler : the JumperScript attached to the "Jumper" GameObject composing the player prefab
 *	    private WorldControlerScript m_WorldHandler : the WorldControlerScript attached to the scene "GameWorld" on which the player and the world he's in depends
 *      private Quaternion m_Oldquaternion : 
 *      private Quaternion m_Newquaternion : 
 *      private float m_AnimationTimer :
 *      private const float m_AnimationTime :
 *      private bool m_IsInAnimation : 
 *      
 *  Authors: Cyril Basset
 **/

using UnityEngine;
using System.Collections;

public class ControllerScript : MonoBehaviour
{


    public float m_Speed = 1.0f;
    public float m_BackSpeed = 0.5f;
    public float m_Jump = 40000.0f;
    public float m_MouseSpeed = 100.0f;
    public float m_MaxSpeed = 30.0f;
    public float m_Baseforce = 40000;
    private float m_Incl;
    private float m_Rot_Y;
    private Vector3 m_RespawnPosition;
    private Vector3 m_RespawnGravityDir;
    private Quaternion m_RespawnRotation;
    private Vector3 m_InitialVelocity;
    private ArrayList m_goToLoad;
    private Collider m_PlayerCollider;
    private bool m_GoForward;
    private bool m_GoBackward;
    private bool m_GoLeft;
    private bool m_GoRight;
    private bool m_GoJump;

	private float m_Time = 0f;
	private bool m_InPause = false;


    JumperScript m_JumpHandler = null;
    WorldControllerScript m_WorldHandler = null;
    AttachToPlayerScript m_AttachToPlayer = null;
    LocalGravityScript m_LocalGravityScript = null;
    public AudioClip m_JumpingSound;
    public AudioClip m_SwitchWorldSound;



    /*Animation for changeGravity*/

    private Quaternion m_Oldquaternion;
    private Quaternion m_Newquaternion;
    private float m_AnimationTimer = 0.0f;
    private const float m_AnimationTime = 1.0f;
    private bool m_IsInAnimation = false;
    private GameObject m_Camera;
    private PauseMenu m_PauseMenu;

    void Start()
    {
        m_Incl = 0.0f;
        m_Rot_Y = 0.0f;
        m_JumpHandler = GetComponentInChildren<JumperScript>();
        GameObject world = GameObject.Find("GameWorld");
        m_WorldHandler = world.GetComponent<WorldControllerScript>();
        Screen.showCursor = false;
        m_RespawnPosition = transform.position;
        m_RespawnRotation = transform.rotation;
        m_InitialVelocity.Set(0, 0, 0);
        m_LocalGravityScript = gameObject.GetComponent<LocalGravityScript>();
        m_RespawnGravityDir = m_LocalGravityScript.GetStartDir();
        m_AttachToPlayer = GameObject.Find("Grabber").GetComponent<AttachToPlayerScript>();
        m_Camera = transform.FindChild("Camera").gameObject;
        Screen.lockCursor = true;
        m_goToLoad = new ArrayList();
        m_PlayerCollider = gameObject.collider;
        m_PauseMenu = gameObject.GetComponent<PauseMenu>();
        m_PauseMenu.enabled = false;
    }

    /**
     * ???????
     **/
    private float Modulof(float a, float b)
    {
        return (a - b * Mathf.Floor(a / b));
    }

    /**
     * ??????
     * */
    void OnMouseDown()
    {
        // Lock the cursor
        Screen.lockCursor = true;
    }

    /**
     * ????????
     * */
    private void UpdateMouse()
    {
        m_MouseSpeed = PlayerPrefs.GetFloat("MouseSensitivity");
        float inclInc = Input.GetAxis("Vertical") * m_MouseSpeed * Time.deltaTime;
        m_Rot_Y = Input.GetAxis("Horizontal") * m_MouseSpeed * Time.deltaTime;
        m_Rot_Y = Modulof(m_Rot_Y, 360.0f);

        if (m_Incl + inclInc > 89.0f)
        {
            inclInc = 89.0f - m_Incl;
        }
        else if (m_Incl + inclInc < -89.0f)
        {
            inclInc = -89.0f - m_Incl;
        }

        m_Incl += inclInc;

        m_Camera.transform.Rotate(Vector3.right, (float)PlayerPrefs.GetInt("InvertedMouse")*inclInc);
    }

    public void AddDataToLoad(GameObject[] gameObjects)
    {
        foreach (GameObject go in gameObjects)
            m_goToLoad.Add(go);
    }


    /**
     * IsOnGround
     *  --> returns whether or not the player is touching the ground
     *  
     * -> true : the player is touching the ground
     * -> false : the player is not touching the ground
     * 
     * */
    private bool IsOnGround()
    {
        if (!m_JumpHandler)
            return true;
        else return m_JumpHandler.IsOnGround();
    }


    /**
     * ???????????
     * */
    public void OnChangeGravity(Vector3 from, Vector3 to)
    {
        from = -from;
        to = -to;
        if (Vector3.Dot(from, to) >= 1.0f)
            return;
        m_AnimationTimer = m_AnimationTime;
        m_Oldquaternion = transform.rotation;
        m_Newquaternion = Quaternion.FromToRotation(from, to);

        m_IsInAnimation = true;
    }

    void Update()
    {
        if (SaveManager.m_MustLoad)
            SaveManager.LoadLastSave();
		
        if (Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("MenuKey")))
			m_PauseMenu.Enable(!m_PauseMenu.enabled);

        m_Camera.camera.fieldOfView = PlayerPrefs.GetFloat("FOV");


        if (!m_PauseMenu.enabled)
        {
            UpdateMouse();
            transform.Rotate(Vector3.up, m_Rot_Y);

            if (Input.GetKey((KeyCode)PlayerPrefs.GetInt("ForwardKey")))
                m_GoForward = true;
            else
                m_GoForward = false;

            if (Input.GetKey((KeyCode)PlayerPrefs.GetInt("BackwardKey")))
                m_GoBackward = true;
            else
                m_GoBackward = false;

            if (Input.GetKey((KeyCode)PlayerPrefs.GetInt("StrafeRightKey")))
                m_GoRight = true;
            else
                m_GoRight = false;


            if (Input.GetKey((KeyCode)PlayerPrefs.GetInt("StrafeLeftKey")))
                m_GoLeft = true;
            else
                m_GoLeft = false;

            if (Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("RespawnKey")))
            {
                RespawnPlayer();
            }

            if (Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("CarryObjectKey")))
            {
                if (m_AttachToPlayer.IsGrabbing())
                    m_AttachToPlayer.Release();
                else
                    m_AttachToPlayer.Grab();
            }

            if (Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("SwitchWorldKey")))
            {
                audio.PlayOneShot(m_SwitchWorldSound, PlayerPrefs.GetFloat("SoundVolume"));
                m_WorldHandler.SwitchWorld();
                //m_JumpHandler.SetMaxCharge(m_WorldHandler.GetCurrentWorldNumber());
            }

            if (Input.GetKeyDown(KeyCode.F5))
                SaveManager.SaveToDisk();

            if (Input.GetKeyDown(KeyCode.F9))
            {
                SaveManager.LoadFromDisk();
                SaveManager.LoadLastSave();
            }

            Vector3 vforce = new Vector3(0.0f, 0.0f, 0.0f);

            if (Input.GetKeyDown((KeyCode)PlayerPrefs.GetInt("JumpKey")) && m_JumpHandler.CanJump())
            {
                audio.PlayOneShot(m_JumpingSound, PlayerPrefs.GetFloat("SoundVolume"));
                vforce += Vector3.up * m_Jump;
                m_JumpHandler.OnJump();
            }

            vforce = transform.rotation * vforce;
            rigidbody.AddForce(vforce);

        
			m_Time += Time.deltaTime;
        }
        UpdateAnimation();
            
    }

    void FixedUpdate()
    {
		if(m_PauseMenu.enabled)
			return;
        float dTime = Time.deltaTime;
        float force = m_Speed;
        Vector3 vforce = new Vector3(0.0f, 0.0f, 0.0f);
		
		
        if (m_GoForward)
        {
            vforce += Vector3.forward;
        }
        if (m_GoBackward)
        {
            vforce += Vector3.back;
            force = m_BackSpeed;

        }
        if (m_GoRight)
        {
            vforce += Vector3.right;

        }
        if (m_GoLeft)
        {
            vforce += Vector3.left;
        }

        vforce = Vector3.Normalize(vforce) * force * m_Baseforce * dTime;

        if (!IsOnGround())
            vforce = vforce * 0.5f;

        vforce = transform.rotation * vforce;
        rigidbody.AddForce(vforce);

    }

    /**
     *  RespawnPlayer(): 
     *      --> respawns the player at the position, rotation and velocity defined in the data members
     * */
    public void RespawnPlayer()
    {
		if (m_AttachToPlayer.IsGrabbing())
			m_AttachToPlayer.Release();
		SaveManager.LoadLastSave();
		GameSave s = SaveManager.last_save;
		s.deathCount++;
		s.time += m_Time;
		m_Time = 0;
		SaveManager.SaveToDisk();
    }

    void OnTriggerEnter(Collider col)
    {
        /*if (m_AttachToPlayer.IsGrabbing() && (col.name.Equals("Grabber") || col == m_AttachToPlayer.GetGrabbed()))
        {
            m_AttachToPlayer.Release();
        }*/
    }


    /**
     *  SetRespawnPosition(Vector3 new_Position)
     *      --> sets the respawn position of the player
     *      
     *  Arguments:
     *      - Vector3 new_position : the position to set as the respawn position
     * */
    public void SetRespawnPosition(Vector3 newPosition)
    {
        SaveManager.SaveLastSave();
        SaveManager.SaveToDisk();    
    }

    /**
     *  SetRespawnRotation(Vector3 new_Position)
     *      --> sets the respawn rotation of the player
     *      
     *  Arguments:
     *      - Vector3 new_position : the position to set as the respawn rotation
     * */
    public void SetRespawnRotation(Quaternion newRotation)
    {
        m_RespawnRotation = newRotation;
    }

    public void SetRespawnGravityDir(Vector3 newGravityDir)
    {
        m_RespawnGravityDir = newGravityDir;
    }



    /**
     * ???????????
     * */
    private void UpdateAnimation()
    {
        if (!m_IsInAnimation)
            return;

        float dTime = Time.deltaTime;

        if (m_AnimationTimer >= dTime)
            m_AnimationTimer -= dTime;
        else
        {
            m_AnimationTimer = 0.0f;
        }

        transform.rotation = Quaternion.Lerp(m_Oldquaternion, m_Newquaternion * m_Oldquaternion, (m_AnimationTime - m_AnimationTimer) / m_AnimationTime);
        if (m_AnimationTimer == 0.0f)
            m_IsInAnimation = false;
    }

    public bool CheckIfInsideObject(int layer)
    {
        Vector3 p1 = transform.position + Vector3.up * -0.5f;
        Vector3 p2 = p1 + Vector3.up * 1;
        Collider[] hitColliders = Physics.OverlapSphere(p1, 0.49f);
        foreach (Collider col in hitColliders)
            if (col.gameObject.layer == layer)
            {
                print("Top end is inside " + col.name);
                RespawnPlayer();
                return false;
            }

        Collider[] hitColliders2 = Physics.OverlapSphere(p2, 0.49f);
        foreach (Collider col in hitColliders2)
            if (col.gameObject.layer == layer)
            {
                print("Low end is inside " + col.name);
                RespawnPlayer();
                return false;
            }

        return true;
    }
}
