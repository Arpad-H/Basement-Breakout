using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioDetection : MonoBehaviour
{
    
    private AudioSource audioSource;
    private AudioClip recordedClip;
    private GameObject battery;
    private bool hasEnergy = false;
    
    void Start()
    {
        battery = GameObject.Find("InsertedBattery");
        battery.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        recordedClip = Microphone.Start(Microphone.devices[0], true, 3599, AudioSettings.outputSampleRate); //"Headset Microphone (Oculus Virtual Audio Device)"
    }

    public void PlaySound()
    {
        if (hasEnergy)
        {
            print("RAAAAHHHHHH");
            audioSource.clip = recordedClip;
            audioSource.time = (Microphone.GetPosition(Microphone.devices[0]) - 1280) / (float)AudioSettings.outputSampleRate;
            audioSource.Play();
        }
    }

    public void StopSound()
    { 
        audioSource.Stop();
    }

    public void batteryInserted()
    {
        hasEnergy = true;
        battery.SetActive(true);
    }
}
