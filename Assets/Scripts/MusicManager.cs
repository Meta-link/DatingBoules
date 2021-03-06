﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public AudioClip music;

    private AudioSource _source;
    private bool _check = false;

	void Start () {

        DontDestroyOnLoad(gameObject);
        if (GameObject.FindGameObjectsWithTag("Music").Length > 1)
        {
            GameObject.Destroy(gameObject);
        }

        _source = GetComponent<AudioSource>();

        //EVENTS
        GameManager.OnState += _OnState;
    }

    void OnDestroy()
    {
        GameManager.OnState -= _OnState;
    }
	
	void Update () {
        if(_source.isPlaying && !_check)
        {
            _check = true;
        }
		else if(_check && !_source.isPlaying)
        {
            _source.clip = music;
            _source.loop = true;
            _source.Play();
        }
	}

    void _OnState(EState state)
    {
        if(state == EState.LOOSE)
        {
            _source.Pause();
        }
        /*else if(state == EState.START)
        {
            _source.Play();
        }*/
    }
}
