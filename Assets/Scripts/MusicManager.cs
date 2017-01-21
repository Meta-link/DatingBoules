using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    private AudioSource debut;
    private AudioSource music;

	void Start () {

		if(GameObject.FindGameObjectsWithTag("").Length > 1)
        {
            GameObject.Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        debut = GetComponents<AudioSource>()[0];
        music = GetComponents<AudioSource>()[1];
    }
	
	void Update () {
		if(!debut.isPlaying && !music.isPlaying)
        {
            music.Play();
        }
	}
}
