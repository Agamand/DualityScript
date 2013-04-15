using UnityEngine;
using System.Collections;

public class Saveable : MonoBehaviour {
	

	public bool m_SaveTransform;
	public bool m_SavePhysics;
	public bool m_SaveGravity;
	public bool m_SaveEnable;
	
	public Save SaveTo()
	{
		
		Save _save = new Save(this.gameObject.GetInstanceID());
		if(m_SaveTransform)
			_save.m_Transform = new TransformSerializable(transform);
		if(m_SavePhysics)
			_save.m_RigidBody = new RigidBodySerializable(rigidbody);
		if(m_SaveGravity)
		{
			LocalGravityScript script = null;
			if((script = GetComponent<LocalGravityScript>()) != null)
				_save.m_Gravity = new Vector3Serializable(script.GetStartDir());
		}
		if(m_SaveEnable && gameObject.tag.Equals("Checkpoint"))
			_save.m_Enable = GetComponent<CheckpointScript>().GetActive();
		
		_save.m_Name = (string)gameObject.name.Clone();
		return _save;
	}
	
	public void Load(Save save)
	{
		if(m_SavePhysics)
			save.m_RigidBody.CopyTo(rigidbody);
		
		if(m_SaveTransform)
		{
			AttachableObjectScript attach;
			if((attach = GetComponent<AttachableObjectScript>()) != null)
				transform.parent = attach.GetOriginalTransform();
			save.m_Transform.CopyTo(transform);
		}
		if(m_SaveGravity)
		{
			LocalGravityScript script = null;
			if((script = GetComponent<LocalGravityScript>()) != null)
				script.setGravityDir(save.m_Gravity.ToVector3(), false);
		}
		
		if(m_SaveEnable && gameObject.tag.Equals("Checkpoint"))
			GetComponent<CheckpointScript>().SetActive(save.m_Enable);
	}
}
