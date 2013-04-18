/** 
* MovingPlatformScript
*  --> makes the platform move between one point to another depending on the world the player choose
*       - changes the color of the bumpers (red for the world 2, blue for the world 1)
*  
* Members: 
*  - public float m_MinIncrement: the distance you want the platform to go backwards
*  - public float m_MaxIncrement: the distance you want the platform to go forward
*  - public enum axisEnum { x, y, z }: an enum allowing axis to be passed as parameters through Unity Editor 
*  - public axisEnum m_Axis: the axis on which you want the platform to move
*  - public float m_Speed: the speed of the platform movement
*  - public bool m_StartBackwards: defines whether or not the movement starts backwards or forwards
*  - public bool m_ChangeColor: defines wheter or not the script is responsible for the color change of the bumpers
*  - private Vector3 m_InitialPosition: the position of the platform before the automated movement
*  - private WorldControlerScript m_WorldControler: the WorldControlerScript attached to the scene "GameWorld" defining in which world the player is
*  - private int m_CurrentWorldNumber: the index of the World the player is in, 0 if World 1, 1 if World 2
*  - private Vector3 m_RelPosition: the actual position of the platform relative to its initial position
*  - private Vector3[] m_Translations: the Vector on which you want the translation to happen considering m_Axis
*  - private Renderer[] m_BumpersRenderer: the renderers of the bumpers attached to the platform
*
* Authors: Jean-Vincent Lamberti, Cyril Basset
* */

using UnityEngine;
using System.Collections;

public class MovingPlatformScript : MonoBehaviour {

    public Vector3 m_MinIncrement;
    public Vector3 m_MaxIncrement;
    public float m_Speed;
    public bool m_ChangeColor = false;
    private Vector3 m_InitialPosition;
    private WorldControllerScript m_WorldControler;
    private int m_CurrentWorldNumber;
    private Vector3 m_RelPosition;
    private Vector3[] m_Translations;
    private Renderer[] m_BumpersRenderer;
    private int m_LastUpdateWorldNumber;
    private bool m_IsMoving;
	private AnimationClip m_Forward;
	private float m_MaxTime;


    void Start () {
        m_InitialPosition = gameObject.transform.localPosition;
        m_WorldControler = GameObject.Find("GameWorld").GetComponent<WorldControllerScript>();
        m_CurrentWorldNumber = m_WorldControler.GetCurrentWorldNumber();
        m_RelPosition.Set(0, 0, 0);
        m_Translations = new Vector3[3];
        m_Translations[0].Set(m_Speed, 0, 0);
        m_Translations[1].Set(0, m_Speed, 0);
        m_Translations[2].Set(0, 0, m_Speed);
        m_BumpersRenderer = new Renderer[2];
        m_BumpersRenderer[0] = gameObject.transform.FindChild("bumperLeft").GetComponent<Renderer>();
        m_BumpersRenderer[1] = gameObject.transform.FindChild("bumperRight").GetComponent<Renderer>();
        m_LastUpdateWorldNumber = 0;
        m_IsMoving = true;
		
		gameObject.AddComponent<Animation>();
		gameObject.animation.animatePhysics = true;
        m_Forward = new AnimationClip();
		

		var curvex = new AnimationCurve();
		var curvey = new AnimationCurve();
		var curvez = new AnimationCurve();
		var minpos = m_InitialPosition+m_MinIncrement;
		var maxpos = m_InitialPosition-m_MaxIncrement;
		float dist = (minpos-maxpos).magnitude;
		m_MaxTime = dist/m_Speed;
		curvex.AddKey(0f, minpos.x);
		curvex.AddKey(dist/m_Speed, maxpos.x);
		curvey.AddKey(0f, minpos.y);
		curvey.AddKey(dist/m_Speed, maxpos.y);
		curvez.AddKey(0f, minpos.z);
		curvez.AddKey(dist/m_Speed, maxpos.z);
		m_Forward.wrapMode = WrapMode.ClampForever;

        m_Forward.SetCurve("", typeof(Transform), "localPosition.x", curvex);
		m_Forward.SetCurve("", typeof(Transform), "localPosition.y", curvey);
		m_Forward.SetCurve("", typeof(Transform), "localPosition.z", curvez);
        this.animation.AddClip(m_Forward, "Forward");
		this.animation.Play("Forward");
	}
		
	void Update () {
        m_CurrentWorldNumber = m_WorldControler.GetCurrentWorldNumber();
		
		if (m_CurrentWorldNumber != m_LastUpdateWorldNumber)
		{
			if(m_CurrentWorldNumber == 0)
			{
				animation["Forward"].speed = 1.0f;
				if(animation["Forward"].time < 0.0f)
					animation["Forward"].time = 0.0f;
			}
			else
			{
				animation["Forward"].speed = -1.0f;
				if(animation["Forward"].time > m_MaxTime)
					animation["Forward"].time = m_MaxTime;
			}
		}
		
        if (m_CurrentWorldNumber != m_LastUpdateWorldNumber || m_IsMoving)
        {
            if (m_CurrentWorldNumber == 1)
            {
                if (m_ChangeColor)
                {
                    foreach (Renderer r in m_BumpersRenderer)
                    {
                        r.material.color = Color.red;
                    }
                }
            }
            else
            {
                if (m_ChangeColor)
                {
                    foreach (Renderer r in m_BumpersRenderer)
                    {
                        r.material.color = Color.blue;
                    }
                }
            }
        }
        m_LastUpdateWorldNumber = m_CurrentWorldNumber;
	}
}
