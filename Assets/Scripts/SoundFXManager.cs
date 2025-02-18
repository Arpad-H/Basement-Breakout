using System;
using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.Attributes;
using UnityEngine;
using UnityEngine.Audio;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;
    [SerializeField] private AudioSource soundFXObject;
    
    [Header("Window Burst Sound FX")]
    [SerializeField] private Transform windowBurstPosition;
    [SerializeField] private float windowBurstVolume;
    [SerializeField] private AudioSource windowBurstSound;
    [SerializeField] private SoundMixerManager soundMixerManager;

    [Header("Water Sound FX")] [SerializeField]
    private AudioSource waterSound;
    [SerializeField] private float waterSoundVolume = 0.6f;
    
    

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    private void OnDestroy()
    {
       
    }

    

    public void PlaySoundFX(AudioClip audioClip, Transform spawnTransform, float volume) {
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        
        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
    
    public void PlayWindowBurstFX() {
        PlaySoundFX(windowBurstSound.clip, windowBurstPosition, windowBurstVolume);
    }

    public void PlayWaterSound()
    {
       
        waterSound.volume = waterSoundVolume;
        waterSound.Play();
        Debug.Log($"[SoundFXManager] Playing water sound] {waterSound.clip.name}");
    }

}
