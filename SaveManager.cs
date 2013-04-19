using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

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

/**
 * public class Vector3Serializable
 *  --> allows to serialize Vector3
 * */
[System.Serializable]
[XmlRoot("vec3")]
public class Vector3Serializable
{
	[XmlAttribute("x")]
	public float x = 0f;
	[XmlAttribute("y")]
	public float y = 0f;
	[XmlAttribute("z")]
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
	}
}


/**
 * public class QuaternionSerializable
 *  --> allows to serialize Quaternions 
 * */
[System.Serializable]
[XmlRoot("quat")]
public class QuaternionSerializable
{
	[XmlAttribute("x")]
	public float x = 0f;
	[XmlAttribute("y")]
	public float y = 0f;
	[XmlAttribute("z")]
	public float z = 0f;
	[XmlAttribute("w")]
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

/*
 * public class TransformSerializable
 *  --> allows to serialize Transform
 * */
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
		t.localScale = scale.ToVector3();
	}
}

/*
 * public class TransformSerializable
 *  --> allows to serialize RigidBody
 * */

[System.Serializable]
[XmlRoot("rigidbody")]
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


/*
 * public class Save
 *  --> allows to serialize any object, using previously defined serialization classes
 * */
[System.Serializable]
public class Save
{
	[XmlAttribute("id")]
	public int m_Id;

    public string m_Name = "";
	
	public TransformSerializable m_Transform = new TransformSerializable();
	
	public RigidBodySerializable m_RigidBody = new RigidBodySerializable();
	
	public Vector3Serializable m_Gravity = new Vector3Serializable();
	
	[XmlAttribute("enable")]
    public bool m_Enable = false;
	
	public Save()
	{
		m_Id = 0;
	}
	public Save(int id)
	{
		m_Id = id;
	}
}

/*
 * public class GameSave
 *  --> allows to serialize every serialized objects and global info
 * */
[System.Serializable]
[XmlRoot("gamesave")]
public class GameSave
{
	[XmlAttribute("level")]
	public int level;
	[XmlAttribute("world")]
	public int world;
	
	[XmlAttribute("score")]
	public int score = 0;
	[XmlAttribute("time")]
	public float time = 0;
	[XmlAttribute("death")]
	public int deathCount = 0;
	
 	[XmlArray("Saves")]
 	[XmlArrayItem("Save")]
	public List<Save> m_Saves;
	public GameSave()
	{
		m_Saves = new List<Save>();
	}
	
	public void AddSave(Save save)
	{
		m_Saves.Add(save);
	}
	
	public Save GetSave(int id)
	{
		if(--id < m_Saves.Count)
			return m_Saves[id];
		return null;
	}
}

/*
 * public static class SaveManager
 *  --> static class allowing to save to disk every serialized objects at any given time
 *  
 * Members:
 *  - public static GameSave last_save: the last GameSave that has been saved
 *	- public static bool m_MustLoad: force the SaveManager to load
 *	- private static string m_FilePath: the file path used to save data on hard disk if the user in the stand alone mode
 *	
 * Authors : Cyril Basset
 * */
public static class SaveManager
{
	public static GameSave last_save = null;
	public static bool m_MustLoad = false;
    private static string m_DirPath = (!Application.isWebPlayer ? System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : "") + "/Imaginarium";
    private static string m_FilePath = m_DirPath + "/save.dat";

    /**
     * LoadFromDisk()
     *  --> loads a GameSave from the Hard Disk if stand alone, from the playerPrefs if in WebPlayer
     * */
	public static void LoadFromDisk()
	{
		if(Application.isWebPlayer)
		{
			var serializer = new XmlSerializer(typeof(GameSave));
		 	if(!PlayerPrefs.HasKey("save"))
				return;
			
			TextReader textReader = new StringReader(PlayerPrefs.GetString("save"));			
			GameSave gamesave;
		 	gamesave = (GameSave)serializer.Deserialize(textReader);
			if(gamesave == null)
			{
				Debug.Log ("Warning : deserialization fail");
				return;
			}
			last_save = gamesave;
		}
		else
		{
            GameSave gamesave = null;
            try
            {
			    Stream s = System.IO.File.Open(m_FilePath,System.IO.FileMode.Open);
			    if(s == null)
				    return;
			    BinaryFormatter f = new BinaryFormatter();	
			    f.Binder = new VersionDeserializationBinder();
			
			    gamesave = (GameSave)f.Deserialize(s);
			    s.Close();                 
            }
            catch (IOException e)
            { 
            }
            if (gamesave == null)
            {
                Debug.Log("Warning : deserialization fail");
                return;
            }
			last_save = gamesave;
		}
	}
	
    /**
     * LoadLastSave()
     *  --> loads the last allocated GameSave
     * */
	public static void LoadLastSave()
	{
		if(last_save == null)
			return;
		Debug.Log("LOAD");
		if( Application.loadedLevel != last_save.level)
		{
			m_MustLoad = true;
            if (last_save.level >= Application.levelCount)
                return;
			Application.LoadLevel(last_save.level);		
			return;
		}
		GameObject[] gameObject = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
		int id = 1;
		Save save = last_save.GetSave(id);
		foreach(GameObject go in gameObject)
		{
			if(save == null)
				break;
			
			Saveable savecomponent = go.GetComponent<Saveable>();
			if(savecomponent == null)
				continue;
						
			savecomponent.Load(save);
			id++;
			save = last_save.GetSave(id);
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
	
	
	/**
     * SaveLastSave()
     *  --> overwrite the last GameSave allocated
     * */
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
				gamesave.AddSave(save);
				id++;
			}
		}
		
		gamesave.level = Application.loadedLevel;
		if(last_save != null)
		{
			gamesave.time = last_save.time;
			gamesave.deathCount = last_save.deathCount;
			gamesave.score = last_save.score;
		}
		if(gamesave == null)
		{
			Debug.Log ("Warning : save fail");
			return;
		}
		last_save = gamesave;	
	}
	
    /**
     * SaveToDisk()
     *  --> Writes the last save to disk if in stand alone, in PlayerPrefs if in WebPlayer
     * */
	public static void SaveToDisk()
	{
		if(last_save == null && !PlayerPrefs.HasKey("Playthrough"))
			return;

        if (Application.isWebPlayer)
		{
			var serializer = new XmlSerializer(typeof(GameSave));
			String str = "";
			TextWriter textWriter = new StringWriter();
		 	serializer.Serialize(textWriter,last_save);
			PlayerPrefs.SetString("save",textWriter.ToString());
		}
		else
		{
            Debug.Log(m_FilePath);
            if (!System.IO.Directory.Exists(m_DirPath))
                System.IO.Directory.CreateDirectory(m_DirPath);
            try
            {

                
                Stream s = System.IO.File.Open(m_FilePath, System.IO.FileMode.Create);
                BinaryFormatter f = new BinaryFormatter();
                f.Binder = new VersionDeserializationBinder();
                f.Serialize(s, last_save);
                s.Close();
            }
            catch (IOException e)
            { 
            }
		}
	}

    /**
     * CheckSaveFile()
     *  --> Checks whether or not the player has a save file
     * */
    public static bool CheckSaveFile()
	{
        if (Application.isWebPlayer)
			return PlayerPrefs.HasKey("save");
		return System.IO.File.Exists(m_FilePath);
	}
	
    /**
     * DeleteSaveFile()
     *  --> Delete the save file
     * */
	public static void DeleteSaveFile()
	{
        if (Application.isWebPlayer)
            PlayerPrefs.DeleteKey("save");
        else
        {
            try
            {
                System.IO.File.Delete(m_FilePath);
            }
            catch (IOException e)
            {
            }
        }
        //
	}

}
