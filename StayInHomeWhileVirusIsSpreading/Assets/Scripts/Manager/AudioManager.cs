using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : Singleton<AudioManager>
{
    private AudioSource _audioSource;
    
    public AudioClip ClickSound;
    public AudioClip BackGroundMusic;
    public AudioClip HitSound1;
    public AudioClip HitSound2;
    public AudioClip HitSound3;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayClickSound()
    {
        _audioSource.clip = ClickSound;
        _audioSource.Play();
    }

    public void PlayHitSound()
    {
        var value = Random.Range(0f, 1f);

        if (value < 0.33f)
        {
            _audioSource.clip = HitSound1;
        }
        else if (value < 0.66f)
        {
            _audioSource.clip = HitSound2;
        }
        else
        {
            _audioSource.clip = HitSound3;
        }
        
        _audioSource.Play();

    }
}
