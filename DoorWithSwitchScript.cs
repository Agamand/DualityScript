/** 
* DoorWithSwitchScript
*  --> Allows a door to open and close depending on the state of the switchs it's linked with
*  
* Members: 
*  - public BallStandScript[] m_Switches : the switches the door is linked with
*  - private bool m_IsCleared = true : the state of the door, true when all switches linked to it are toggled on
*  - public float m_Speed = 1 : the speed of the door opening and closing animation
*  - private GameObject m_Door : the Door GameObject the script will handle
*  - private Vector3[] m_Translations : an array allowing generic use of the script for any axis of translation
*  - public enum axisEnum { x, y, z } : an enumeration allowing generic use of the script for any axis of translation
*  - public axisEnum m_Axis : the axis on which the door will open and close
*  - public bool m_Reverse : defines whether the door is supposed to open by going downwards (true) or upwards (false)
*  - public Material m_OpenMaterial : the Material used for the DoorPane when the door is opened
*  - public Material m_ClosedMaterial : the Material used for the DoorPane when the door is closed
*  - public float m_Height = 3.5f : the height on which the door opens (allowing independant use of the script from the scale of the door)
*  - private Vector3 m_RelPosition : a vector representing the relative position of the door, equivalent to the Transform.localPosition
 *                                      but allowing a generic use of the script independant from the axis of translations
*    
* Authors: Jean-Vincent Lamberti
* */
using UnityEngine;
using System.Collections;

public class DoorWithSwitchScript : MonoBehaviour {

    public BallStandScript[] m_Switches;
    private bool m_IsCleared = true;
    public float m_Speed = 1;
    private GameObject m_Door;
    private Vector3[] m_Translations;
    public enum axisEnum { x, y, z };
    public axisEnum m_Axis;
    public bool m_Reverse = false;

    public Material m_OpenMaterial;
    public Material m_ClosedMaterial;
    public float m_Height = 3.5f;
    private Vector3 m_RelPosition;
    private bool m_IsChangingState = true;

	void Start () {
        m_Door = gameObject.transform.FindChild("DoorPane").gameObject;
        m_Translations = new Vector3[] {
            new Vector3(m_Speed, 0, 0), 
            new Vector3(0, m_Speed, 0), 
            new Vector3(0, 0, m_Speed)
        };
        m_RelPosition.Set(0, 0, 0);
	}
	
	// Update is called once per frame
    void Update()
    {
        if (m_IsChangingState)
            CheckSwitchesState();
    }

    // Called  both when a switches changes its state and while the door is closing or opening
	public void CheckSwitchesState () {
        if (m_Switches != null)
        {
            m_IsCleared = true;

            for (int i = 0; i < m_Switches.Length; i++)
            {
                if (!m_Switches[i].GetState())
                    m_IsCleared = false;
            }
            if (m_IsCleared)
                OpenDoor();
            else
                CloseDoor();
        }
    }

    /*
     * OpenDoor()
     *  Opens the door, called when all associated switches are toggled on (i.e m_IsCleared = true)
     * */
    void OpenDoor()
    {
        if ((!m_Reverse && m_RelPosition[(int)m_Axis] < m_Height) || (m_Reverse && m_RelPosition[(int)m_Axis] >= m_Height))
        {
            m_IsChangingState = true;
            m_Door.renderer.material = m_OpenMaterial;
            if (m_Reverse)
                m_Door.transform.Translate(-1 * m_Translations[(int)m_Axis] * Time.deltaTime);
            else
                m_Door.transform.Translate(m_Translations[(int)m_Axis] * Time.deltaTime);

            m_RelPosition = m_Door.transform.localPosition;
        }
        else
            m_IsChangingState = false;
    }

    /*
     * CloseDoor()
     *  Closes the door, called when one or more switch aren't toggled on
     * */
    void CloseDoor()
    {
        if ((!m_Reverse && m_RelPosition[(int)m_Axis] > 0) || (m_Reverse && m_RelPosition[(int)m_Axis] < 0))
        {
            m_IsChangingState = true;
            m_Door.renderer.material = m_ClosedMaterial;
            if (m_Reverse)
                m_Door.transform.Translate(m_Translations[(int)m_Axis] * Time.deltaTime);
            else
                m_Door.transform.Translate(-1 * m_Translations[(int)m_Axis] * Time.deltaTime);
            m_RelPosition = m_Door.transform.localPosition;
        }
        else
            m_IsChangingState = true;
    }
}