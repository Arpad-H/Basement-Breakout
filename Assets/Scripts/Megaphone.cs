using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class AudioDetection : MonoBehaviour
{
    private string micName;
    private AudioSource audioSource;
    private AudioClip recordedClip;
    private GameObject battery;
    private bool hasEnergy = false;
    //private AudioHighPassFilter filter;
    [SerializeField] private Transform winPos;
    [SerializeField] private float winDistance = 10.0f;
    [SerializeField] private float winTimer = 3.0f;
    [SerializeField] private GameObject highlighter;
    private float timer = 0;
    public static event Action<GameManager.GameState> GameStateChangedMegaPhone;
    
    void Start()
    {
        micName = Microphone.devices[0];
        battery = GameObject.Find("InsertedBattery");
        battery.SetActive(false);
        
        audioSource = GetComponent<AudioSource>();
        
        int minFreq;
        int maxFreq;
        Microphone.GetDeviceCaps(micName, out minFreq, out maxFreq);
        
        recordedClip = Microphone.Start(micName, true, 1, maxFreq);
        while (!(Microphone.GetPosition(micName) > 0)) { }

        audioSource.clip = recordedClip;
        audioSource.loop = true;
        audioSource.Play();
        audioSource.mute = true;

        /*filter = new AudioHighPassFilter();
        if (filter != null)
        {
            filter.cutoffFrequency = 1750;
        }*/
    }

    void Update()
    {
        if ((transform.position - winPos.transform.position).magnitude < winDistance && !audioSource.mute)
        {
            print(timer > winTimer);
            timer += Time.deltaTime;
            if(timer > winTimer)
                GameStateChangedMegaPhone?.Invoke(GameManager.GameState.Win);
        }
        /*if ((OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) || OVRInput.Get(OVRInput.Button.SecondaryHandTrigger)) &&
            (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))) {
            PlaySound();
        }
        else {
            StopSound();
        }*/
    }
    
    public void PlaySound()
    {
        if (hasEnergy)
        {
            audioSource.timeSamples = Microphone.GetPosition(micName);
            audioSource.mute = false;
        }
    }

    public void StopSound()
    { 
        audioSource.mute = true;
    }

    public void batteryInserted()
    {
        hasEnergy = true;
        battery.SetActive(true);
        highlighter.SetActive(false);
    }
    
}
