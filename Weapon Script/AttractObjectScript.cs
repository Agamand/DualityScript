using UnityEngine;
using System.Collections;

public class AttractObjectScript : MonoBehaviour {

	private Vector3 target_pos;
	private LocalGravityScript force = null;
	private bool handleEffect = false;
	static float m_gravityAcceleration = 9.8f;
	
	void Start () {
		force = this.gameObject.GetComponent<LocalGravityScript>();
	}
	
	public void Start(Vector3 _target_pos)
	{
		target_pos = _target_pos;
		handleEffect = true;
	}
	
	public void Stop()
	{
		handleEffect = false;
		force.setGravityDir(new Vector3(0,-1,0));
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(handleEffect)
		{
			Debug.Log ("handleEffect");
			Vector3 dir = target_pos-transform.position;
			float dist = dir.magnitude;
			dir = Vector3.Normalize(dir);
			force.setGravityDir(dir);
		}
	
	}
}
