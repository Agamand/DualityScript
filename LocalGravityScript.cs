/** 
* LocalGravityScript
*  --> change the gravity of gameObject the script is attached to
*  
* Members: 
* - static float m_gravityAcceleration: the gravity acceleration value
* - public Vector3 m_StartDir: the starting direction 
* - private Vector3 m_Gravity: the current gravity
* - private ConstantForce m_ConstForce: the constante force applied to the gameObject
* - private ControlerScript m_Player: the controller script attached to the gameObject
* - private Rigidbody m_Body: the rigidbody of the gameObject
*  
* Authors: Cyril Basset
* */

using UnityEngine;
using System.Collections;

public class LocalGravityScript : MonoBehaviour {

    static float m_gravityAcceleration = 9.8f;
    public Vector3 m_StartDir = new Vector3(0f, -1.0f, 0f);

    private Vector3 m_Gravity;
    private ConstantForce m_ConstForce = null;
    private ControllerScript m_Player = null;
    private Rigidbody m_Body = null;

    void Start()
    {
        m_Body = this.rigidbody;

        if (!m_Body)
        {
            return;
        }


        m_Body.useGravity = false;

        m_ConstForce = gameObject.AddComponent<ConstantForce>();
        SetGravityDir(m_StartDir);
        m_Player = GetComponent<ControllerScript>();
    }

    /**
     * SetGravityDir
     *  -->Sets the Gravity direction and start the animation
     * 
     * Arguments:
     *  - Vector3 newGravity: the new direction
     *  - bool playerAnim: wether or not the change should be animated
     * */
    public void SetGravityDir(Vector3 newGravity, bool playerAnim = true)
    {
        float mass = m_Body ? m_Body.mass : 1.0f;

        Vector3 old = Vector3.Normalize(m_Gravity);
        m_Gravity = newGravity;
        m_Gravity *= m_gravityAcceleration;
        m_ConstForce.force = m_Gravity * mass;
		
        if (m_Player && playerAnim)
            m_Player.OnChangeGravity(old, newGravity);

    }

    /**
     * GetStartDir()
     *  --> Returns the initial gravity direction
     * */
    public Vector3 GetStartDir()
    {
        return m_StartDir;
    }

    /*
     * GetGravity()
     *  --> Returns the current gravity
     * */
    public Vector3 GetGravity()
    {
        return m_Gravity;
    }

    public Vector3 GetGravityDir()
    {
        return m_Gravity.normalized;
    }
}
