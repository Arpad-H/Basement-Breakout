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
    [SerializeField] private GameObject winPos;
    [SerializeField] private float winDistance = 10.0f;
    [SerializeField] private float winTimer = 3.0f;
    [SerializeField] private GameObject highlighter;
    private float timer = 0;

    private bool playOnce = true;
    private bool winOnce = true;
    public static event Action<GameManager.GameState> GameStateChangedMegaPhone;
    public static event Action<VoiceOverManager.Item> audioWonByMegaphoneWin;
    private bool yelled = false;
    
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
            yelled = true;
        }
        if (yelled)
        {
            timer += Time.deltaTime;
            if (timer > winTimer && playOnce)
            {
                playOnce = false;
                winPos.GetComponent<AudioSource>().Play();
            }
        }

        if (winOnce && !playOnce && !winPos.GetComponent<AudioSource>().isPlaying)
        {
            audioWonByMegaphoneWin?.Invoke(VoiceOverManager.Item.WonByMegaphoneWin);
            GameStateChangedMegaPhone?.Invoke(GameManager.GameState.Win);
            
            winOnce = false;
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
