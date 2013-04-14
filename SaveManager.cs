using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System;
using System.Reflection;


public sealed class VersionDeserializationBinder : SerializationBinder 
{ 
    public override Type BindToType( string assemblyName, string typeName )
    { 
        if ( !string.IsNullOrEmpty( assemblyName ) && !string.IsNullOrEmpty( typeName ) ) 
        { 
            Type typeToDeserialize = null; 
 
            assemblyName = Assembly.GetExecutingAssembly().FullName; 
 
            typeToDeserialize = Type.GetType( String.Format( "{0}, {1}", typeName, assemblyName ) ); 
 
            return typeToDeserialize; 
        } 
 
        return null; 
    } 
}


[System.Serializable]
public class Vector3Serializable
{
	public float x = 0f;
	public float y = 0f;
	public float z = 0f;
	
	public Vector3Serializable()
	{}
	
	public Vector3Serializable(Vector3 v)
	{
		x = v.x;
		y = v.y;
		z = v.z;
	}
	
	public Vector3 ToVector3()
	{
		return new Vector3(x,y,z);
	}
	
	public void CopyTo(Vector3 v)
	{
		v.x = x;
		v.y = y;
		v.z = z;
		Debug.Log("COPY : " + v.ToString());
	}
}

[System.Serializable]
public class QuaternionSerializable
{
	public float x = 0f;
	public float y = 0f;
	public float z = 0f;
	public float w = 0f;
	
	public QuaternionSerializable()
	{}
	
	public QuaternionSerializable(Quaternion q)
	{
		x = q.x;
		y = q.y;
		z = q.z;
		w = q.w;
	}
	
	public Quaternion ToQuaternion()
	{
		return new Quaternion(x,y,z,w);
	}
	
	public void CopyTo(Quaternion q)
	{
		q.x = x;
		q.y = y;
		q.z = z;
		q.w = w;
	}
}

[System.Serializable]
public class TransformSerializable
{
	public Vector3Serializable position = new Vector3Serializable();
	public QuaternionSerializable rotation = new QuaternionSerializable();
	public Vector3Serializable scale = new Vector3Serializable();
	
	public TransformSerializable()
	{}
	
	public TransformSerializable(Transform t)
	{
		position = new Vector3Serializable(t.position);
		rotation = new QuaternionSerializable(t.rotation);
		scale = new Vector3Serializable(t.localScale);
	}
	
	public void CopyTo(Transform t)
	{
		t.position = position.ToVector3();
		t.rotation = rotation.ToQuaternion();
		t.localScale = scale.ToVector3();
		Debug.Log("COPY POS : " + t.position.ToString()); 
	}
}

[System.Serializable]
public class RigidBodySerializable
{
	public Vector3Serializable velocity = new Vector3Serializable();
	public Vector3Serializable angularVelocity = new Vector3Serializable();
	
	public RigidBodySerializable()
	{}
	
	public RigidBodySerializable(Rigidbody r)
	{
		velocity = new Vector3Serializable(r.velocity);
		angularVelocity = new Vector3Serializable(r.angularVelocity);
	}
	
	public void CopyTo(Rigidbody r)
	{
		r.velocity = velocity.ToVector3();
		r.angularVelocity = angularVelocity.ToVector3();
	}
}



[System.Serializable]
public class Save
{
	public int m_Id;
	public TransformSerializable m_Transform = new TransformSerializable();
	public RigidBodySerializable m_RigidBody = new RigidBodySerializable();
	public Vector3Serializable m_Gravity = new Vector3Serializable();
	public Save(int id)
	{
		m_Id = id;
	}
}


[System.Serializable]
public class GameSave
{
	public int level;
	public int world;
	private Dictionary<int,Save> m_SaveMaps;
	public GameSave()
	{
		m_SaveMaps = new Dictionary<int,Save>();
	}
	
	public void AddSave(Save save)
	{
		m_SaveMaps.Add(save.m_Id,save);
	}
	
	public Save GetSave(int id)
	{
		try
		{
			return m_SaveMaps[id];
		}catch(KeyNotFoundException e)
		{
			return null;
		}
	}
}

public static class SaveManager
{
	static MemoryStream last_save = null;
	public static void Load(string path)
	{
		Stream s = System.IO.File.Open(path,System.IO.FileMode.Open);
		if(s == null)
			return;
		BinaryFormatter f = new BinaryFormatter();	
		f.Binder = new VersionDeserializationBinder();
		GameSave gamesave;
		gamesave = (GameSave)f.Deserialize(s);
		s.Close();
		Application.LoadLevel(gamesave.level);
		GameObject[] gameObject = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
		foreach(GameObject go in gameObject)
		{
			Save save = gamesave.GetSave(go.GetInstanceID());
			if(save == null)
				continue;
			
			Saveable savecomponent = go.GetComponent<Saveable>();
			if(savecomponent == null)
				continue;
			
			Debug.Log("Load object : " + save.m_Id);
			savecomponent.Load(save);
		}
		
	}
	
	public static void LoadLastSave()
	{
		if(last_save == null)
			return;
		Debug.Log("LOAD");
		last_save.Seek(0, SeekOrigin.Begin);
		BinaryFormatter f = new BinaryFormatter();	
		f.Binder = new VersionDeserializationBinder();
		GameSave gamesave;
		gamesave = (GameSave)f.Deserialize(last_save);
		if( Application.loadedLevel != gamesave.level)
			Application.LoadLevel(gamesave.level);
		GameObject[] gameObject = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
		foreach(GameObject go in gameObject)
		{
			Save save = gamesave.GetSave(go.GetInstanceID());
			if(save == null)
				continue;
			
			Saveable savecomponent = go.GetComponent<Saveable>();
			if(savecomponent == null)
				continue;
			
			Debug.Log("Load object : " + save.m_Id);
			savecomponent.Load(save);
		}
		WorldControllerScript worldController = null;
		GameObject worldControllerGo = GameObject.Find("GameWorld");
		if(worldControllerGo != null)
		{
			if((worldController = worldControllerGo.GetComponent<WorldControllerScript>()) != null)
				worldController.SetWorld(gamesave.world);
		}
		
	}
	
	public static void SaveLastSave()
	{
		Debug.Log("SAVE");
		GameSave gamesave = new GameSave();
		
		WorldControllerScript worldController = null;
		GameObject worldControllerGo = GameObject.Find("GameWorld");
		if(worldControllerGo != null)
		{
			if((worldController = worldControllerGo.GetComponent<WorldControllerScript>()) != null)
				gamesave.world = worldController.GetCurrentWorldNumber();
		}
		
		
		GameObject[] gameObject = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
		foreach(GameObject go in gameObject)
		{
			Saveable savecomponent = null;
			if((savecomponent = go.GetComponent<Saveable>()) != null)
			{
				Save save = savecomponent.SaveTo();
				Debug.Log ("Save object : " + save.m_Id);
				gamesave.AddSave(save);
			}
		}
		
		gamesave.level = Application.loadedLevel;
			
		last_save = new MemoryStream();
		BinaryFormatter f = new BinaryFormatter();	
		f.Binder = new VersionDeserializationBinder(); 
		f.Serialize(last_save,gamesave);
	}
	
	public static void Save(string path)
	{
		GameSave gamesave = new GameSave();
		GameObject[] gameObject = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
		foreach(GameObject go in gameObject)
		{
			Saveable savecomponent = null;
			if((savecomponent = go.GetComponent<Saveable>()) != null)
			{
				Save save = savecomponent.SaveTo();
				gamesave.AddSave(save);
			}
		}
		
		gamesave.level = Application.loadedLevel;
			
		Stream s = System.IO.File.Open(path,System.IO.FileMode.CreateNew);
		BinaryFormatter f = new BinaryFormatter();	
		f.Binder = new VersionDeserializationBinder(); 
		f.Serialize(s,gamesave);
		s.Close();
	}
}
