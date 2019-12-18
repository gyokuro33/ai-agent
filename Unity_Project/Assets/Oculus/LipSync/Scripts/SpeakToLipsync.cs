using System;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Threading;

public class SpeakToLipsync : MonoBehaviour

{
    public AudioSource currentAudioSource = null;
    public AudioSource speakAudioSource = null;
    public GameObject speakOb;

    private AudioClip beforeAudioClip;

    void Awake()
    {
        speakAudioSource = speakOb.GetComponent<AudioSource>();
        if (!currentAudioSource) currentAudioSource = GetComponent<AudioSource>();
        if (!currentAudioSource) return; 
        
    }
    // Start is called before the first frame update
    void Start()
    {
        currentAudioSource.mute = false;
        beforeAudioClip = speakAudioSource.clip;

    }

    // Update is called once per frame
    void Update()
    {

        if (beforeAudioClip != speakAudioSource.clip)
        {
            currentAudioSource.clip = speakAudioSource.clip;
            currentAudioSource.Play();
        }
        beforeAudioClip = speakAudioSource.clip;


    }

}
