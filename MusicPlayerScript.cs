using UnityEngine;
using System.Collections;

public class MusicPlayerScript : MonoBehaviour {

    public AudioClip MusicTrack;

	void Update () {
	    audio.volume = PlayerPrefs.GetFloat("MusicVolume")/2.5f;
	}
}
