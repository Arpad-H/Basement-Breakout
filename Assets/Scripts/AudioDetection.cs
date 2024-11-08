using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDetection : MonoBehaviour
{
    
    private AudioSource audioSource;
    private AudioClip recordedClip;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        recordedClip = Microphone.Start(Microphone.devices[0], true, 3599, AudioSettings.outputSampleRate); //"Headset Microphone (Oculus Virtual Audio Device)"
    }

    void Update()
    {
        audioSource.time = (Microphone.GetPosition(Microphone.devices[0])) / (float)AudioSettings.outputSampleRate;
    }

    public void PlaySound()
    {
        audioSource.clip = recordedClip;
        audioSource.Play();
    }

    public void StopSound()
    { 
        audioSource.Stop();
    }
}
