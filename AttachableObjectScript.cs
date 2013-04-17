/**
 *  AttachableObjectScript
 *      --> Script used to define if an object is attachable to another : 
 *          - keep the initial parent of the object in order to restore it while unattached
 *          
 *   Public Members: 
 *      - private Transform m_OriginalTransform : the parent of the gameObject before any dynamic attachment
 *      - private Vector3 m_OriginalGravity: the inital Gravity before the object is being grabbed of attached;
 *      - private GameObject m_Grabber: the GameObject carrying the gameObject if the gameObject is grabbable
 *      - public bool m_IsGrabbable: whether or not the gameObject can be carried by another
 *          
 *  Authors: Jean-Vincent Lamberti
 **/

using UnityEngine;
using System.Collections;

public class AttachableObjectScript : MonoBehaviour {

    private Transform m_OriginalTransform;
    private Vector3 m_OriginalGravity;
    private GameObject m_Grabber;
    public bool m_IsGrabbable = false;
	

    void Start () {
		m_OriginalTransform = gameObject.transform.parent;
	}

    /**
     * GetOriginalTransform()
     *  --> returns the initial parent of the gameObject
     **/
    public Transform GetOriginalTransform()
    {
        return m_OriginalTransform;
    }

    /**
     * SetGrabber(GameObject grabber)
     *  --> Defines the grabber currently parenting the object
     *  
     * Arguments: 
     *  - GameObject grabber: the GameObject the grabbable object is attached to
     * */
    public void SetGrabber(GameObject grabber)
    {
        m_Grabber = grabber;
    }


    /**
     * GetGrabber()
     *  --> Return the grabber currently parenting the object
     *  
     * Arguments: 
     *  - GameObject grabber: the GameObject the grabbable object is attached to
     * */
    public GameObject GetGrabber()
	{
		return m_Grabber;
	}
}
