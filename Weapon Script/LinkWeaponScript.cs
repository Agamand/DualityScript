/**
 * 
 * WARNING : THE USE OF THIS GAMEPLAY MECHANIC HAS BEEN REVISED FOR GAME DESIGN PURPOSES, IT IS NOT USED IN THE CURRENT BUILD
 * 
 * public class LinkWeaponScript
 *  --> allows to manipulate the attraction between given objects
 * 
 * Authors: Cyril Basset
 * */
using UnityEngine;
using System.Collections;

public class LinkWeaponScript : MonoBehaviour {

	enum TWeaponState
	{
		NO_TARGET,
		ONE_TARGET,
		ON_EFFECT
	}
	
	enum TWeaponMode
	{
		NO_MODE,
		ATTRACT,
		RESET
	}
	
	public GameObject goCamera = null;
	private Camera playerCamera = null;
	private GameObject target_1 = null;
	private Vector3 target_pos;
	private bool hasPos = false;
	//private GameObject target_2 = null;
	private TWeaponMode mode = TWeaponMode.NO_MODE;
	private TWeaponState state = TWeaponState.NO_TARGET;
	
	
	void Start () 
	{
		if(goCamera != null)
			playerCamera = goCamera.GetComponent<Camera>();
	}
	
	void Reset()
	{
		if(target_1 != null)
			target_1.GetComponent<AttractObjectScript>().Stop();
		target_1 = null;
		//target_2 = null;
		hasPos = false;
		mode = TWeaponMode.NO_MODE;
	}
	
	
	void ApplyEffect()
	{
		AttractObjectScript attract =  target_1.GetComponent<AttractObjectScript>();
		attract.Start(target_pos);
	}
	
	void HandleMouse(TWeaponMode _mode)
	{
		if(_mode == TWeaponMode.RESET)
			Reset();
		
		if(_mode == TWeaponMode.ATTRACT)
		{
			Debug.Log(hasPos);
			Debug.Log(target_1);

		      RaycastHit hit;
		      Ray ray = playerCamera.ScreenPointToRay(new Vector2(Screen.width/2.0f,Screen.height/2.0f));
			  Debug.Log("OnRayCast "+Input.mousePosition.ToString());
		      if (Physics.Raycast (ray, out hit, 10000.0f))
		      {
				
		         if(hit.collider.GetComponent<AttractObjectScript>() != null)
				{
					target_1 = hit.collider.gameObject;
					Debug.Log(target_1.name);
				}
				 else
				{
					target_pos = hit.point;
					hasPos = true;
					Debug.Log(hit.collider.gameObject.name);
					Debug.Log(hit.point.ToString());
				}
		      }
			if(hasPos && target_1 != null)
			{
				Debug.Log("ApplyEffect");
				ApplyEffect();
				return;
			}
		}
	}
	
	void Update () 
	{
	
		if(Input.GetButtonDown("Fire1"))
		{
			HandleMouse(TWeaponMode.ATTRACT);
			Debug.Log("fire");
		}
		else if(Input.GetButtonDown("Fire2"))
		{
			HandleMouse(TWeaponMode.RESET);
			Debug.Log("reset");
		}
		
	}
}
