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
		Debug.Log("(" + x + ", " + y + ", " + z + ", " + w + ")"); 
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
		position = new Vector3Serializable(t.localPosition);
		rotation = new QuaternionSerializable(t.localRotation);
		scale = new Vector3Serializable(t.localScale);
	}
	
	public void CopyTo(Transform t)
	{
		t.localPosition = position.ToVector3();
		t.localRotation = rotation.ToQuaternion();
		Debug.Log(t.rotation.ToString());
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
	static GameSave last_save = null;
	public static bool m_MustLoad = false;
	public static void LoadFromDisk(string path)
	{
		Stream s = System.IO.File.Open(path,System.IO.FileMode.Open);
		if(s == null)
			return;
		BinaryFormatter f = new BinaryFormatter();	
		f.Binder = new VersionDeserializationBinder();
		GameSave gamesave;
		gamesave = (GameSave)f.Deserialize(s);
		s.Close();
		last_save = gamesave;	
	}
	
	public static void LoadLastSave()
	{
		if(last_save == null)
			return;
		Debug.Log("LOAD");
		Debug.Log("loadedlevel : " + Application.loadedLevel + ", load level : " + last_save.level);
		if( Application.loadedLevel != last_save.level)
		{
			m_MustLoad = true;
			Application.LoadLevel(last_save.level);		
			return;
		}
		GameObject[] gameObject = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
		int id = 1;
		foreach(GameObject go in gameObject)
		{
			Save save = last_save.GetSave(id);
			if(save == null)
				continue;
			
			Saveable savecomponent = go.GetComponent<Saveable>();
			if(savecomponent == null)
				continue;
			
			Debug.Log("Load object : " + save.m_Id);
			savecomponent.Load(save);
			id++;
		}
		WorldControllerScript worldController = null;
		GameObject worldControllerGo = GameObject.Find("GameWorld");
		if(worldControllerGo != null)
		{
			if((worldController = worldControllerGo.GetComponent<WorldControllerScript>()) != null)
				worldController.SetWorld(last_save.world);
		}
		m_MustLoad = false;
	}
	
	
	
	public static void SaveLastSave()
	{
		if(m_MustLoad)
			return;
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
		int id = 1;
		foreach(GameObject go in gameObject)
		{
			Saveable savecomponent = null;
			if((savecomponent = go.GetComponent<Saveable>()) != null)
			{
				Save save = savecomponent.SaveTo();
				save.m_Id = id;
				Debug.Log ("Save object : " + save.m_Id);
				gamesave.AddSave(save);
				id++;
			}
		}
		
		gamesave.level = Application.loadedLevel;
		last_save = gamesave;	
	}
	
	public static void SaveToDisk(string path)
	{
		if(last_save == null)
			return;
		Stream s = System.IO.File.Open(path,System.IO.FileMode.Create);
		BinaryFormatter f = new BinaryFormatter();	
		f.Binder = new VersionDeserializationBinder(); 
		f.Serialize(s,last_save);
		s.Close();
	}
	
	public static bool CheckSaveFile(string path)
	{
		return System.IO.File.Exists(path);
	}
}
