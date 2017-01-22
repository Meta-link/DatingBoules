using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    private AudioSource debut;
    private AudioSource music;
    private bool _check;

	void Start () {

        DontDestroyOnLoad(gameObject);
        if (GameObject.FindGameObjectsWithTag("Music").Length > 1)
        {
            GameObject.Destroy(gameObject);
        }


        debut = GetComponents<AudioSource>()[0];
        music = GetComponents<AudioSource>()[1];
    }
	
	void Update () {

        if(!_check && debut.isPlaying)
        {
            _check = true;
        }

		if(_check && !debut.isPlaying && !music.isPlaying)
        {
            music.Play();
        }
	}
}
