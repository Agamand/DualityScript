/** 
* AutomatedMovingPlatformScript
*  --> makes the platform move between one point to another, looping
*  
* Members: 
*  - public float m_MinIncrement: the distance you want the platform to go backwards
*  - public float m_MaxIncrement: the distance you want the platform to go forward
*  - public enum axisEnum { x, y, z }: an enum allowing axis to be passed as parameters through Unity Editor 
*  - public axisEnum m_Axis: the axis on which you want the platform to move
*  - public bool m_StartBackwards: defines whether or not the movement starts backwards or forwards
*  - public float m_Speed: the speed of the platform movement
*  - public float m_CooldownStart: the amount of time the platform stays at the closer point of its course
*  - public float m_CooldownEnd: the amount of time the platform stays at the further point of its course
*  - private Vector3 m_InitialPosition: the position of the platform before the automated movement
*  - private Vector3 m_RelPosition: the actual position of the platform relative to its initial position
*  - private Vector3[] m_Translations: the Vector on which you want the translation to happen considering m_Axis
*  - private bool m_IsAtEnd: allows to know wether or not the platform is at one end of its perpetual movement
*
* Authors: Jean-Vincent Lamberti, Cyril Basset
* */


using UnityEngine;
using System.Collections;


public class AutomatedMovingPlatformScript : MonoBehaviour {

    public Vector3 m_MinIncrement;
    public Vector3 m_MaxIncrement;
    public bool m_StartBackwards = false;
    public float m_Speed;
    public float m_CooldownStart = 0;
    public float m_CooldownEnd = 0;
    private Vector3 m_InitialPosition;
    private Vector3 m_RelPosition;
    private Vector3[] m_Translations;
    private bool m_IsAtEnd = false;
	private AnimationClip m_Forward;
	private float m_MaxTime;
	private AnimationState m_ForwardState;
	private bool m_Inverse = false;

    // Use this for initialization
    void Start()
    {
        m_InitialPosition = gameObject.transform.localPosition;
        m_RelPosition.Set(0, 0, 0);
        m_Translations = new Vector3[3];
        m_Translations[0].Set(m_Speed, 0, 0);
        m_Translations[1].Set(0, m_Speed, 0);
        m_Translations[2].Set(0, 0, m_Speed);
		
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
		m_ForwardState = this.animation["Forward"];
	}
	
    // Update is called once per frame
    void Update()
    {
        if (m_Inverse)
        {
            if (this.animation["Forward"].time < 0f)
            {   
		        m_Inverse = false;
				this.animation["Forward"].speed = 1.0f;
				this.animation["Forward"].time = -m_CooldownStart;       
            }
        }
        else
        {
            if (this.animation["Forward"].time > m_MaxTime)
            {
		        m_Inverse = true;
				this.animation["Forward"].speed = -1.0f;
				this.animation["Forward"].time = m_MaxTime+m_CooldownEnd;
            }
        }
    }

}
