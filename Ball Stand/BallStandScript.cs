/**
 * BallStandScript
 *  --> Switch that can be turned On and Off when a ball trigger it
 *
 * Members: 
 *  - private bool m_IsOn: whether or not the switch is on
 *  - private Light[] m_Lights: the lights of the switch
 *  - public DoorWithSwitchScript m_AssociatedDoor: the door to which the switches are assigned
 *	- public AudioClip m_SwitchOnSound : the soundclip to play when the switches toggles on
 * 	- public AudioClip m_SwitchOffSound : the soudclip to play when the switches toggles off
 * 	
 *  Authors: Jean-Vincent Lamberti
 * */

using UnityEngine;
using System.Collections;

public class BallStandScript : MonoBehaviour {

    private bool m_IsOn = false;
    private Light[] m_Lights;
    public DoorWithSwitchScript m_AssociatedDoor;
	public AudioClip m_SwitchOnSound; 
	public AudioClip m_SwitchOffSound;

	void Start () {
        m_Lights = new Light[2];
        m_Lights[0] = gameObject.transform.FindChild("leftLight").light;
        m_Lights[1] = gameObject.transform.FindChild("rightLight").light;
	}

    /**
     * OnStateSwitch()
     *  Called when the switch changes state, signals the associated door that one of its associated switched has changes its state
     * */
    void OnStateSwitch()
    {
        m_AssociatedDoor.CheckSwitchesState();
    }

    /**
     * FlickOn()
     *  Called by a script which want to turn the switch on, passes the lights to green and the state of the switch to on
     * */
    public void FlickOn()
    {
        audio.PlayOneShot(m_SwitchOnSound, PlayerPrefs.GetFloat("SoundVolume"));
        m_IsOn = true;
        foreach (Light l in m_Lights)
                l.color = Color.green;
        OnStateSwitch();
    }

    /**
     * FlickOff()
     *  Called by a script which want to turn the switch off, passes the lights to red and the state of the switch to off
     * */
    public void  FlickOff()
    {
        audio.PlayOneShot(m_SwitchOffSound, PlayerPrefs.GetFloat("SoundVolume"));
        m_IsOn = false;
        foreach (Light l in m_Lights)
            l.color = Color.red;
        OnStateSwitch();
    }

    /**
     *  GetState()
     *      Returns whether or not the switch is on
     * */
    public bool GetState()
    {
        return m_IsOn;
    }
}
