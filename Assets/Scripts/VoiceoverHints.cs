using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class VoiceoverHints : MonoBehaviour
{
    
    // public delegate void OnPickupVoicedItem(VoiceoverHints voiceoverHints);
    // public delegate void OnDropVoicedItem();
    [SerializeField] public AudioSource HintVoiceClip;
    [SerializeField] public bool isPickedUp = false;
    [SerializeField] public float TimeUntilHint = 10f;
    [SerializeField] public bool isHintPlayed = false;
    private bool blockVoiceHint = false;
    
    private  float timeSincePickup = 0;
    
    [ExecuteAlways] private void Awake()
    {
        CustomButtonMapper customButtonMapper = GetComponent<CustomButtonMapper>();
        if (customButtonMapper)
        {
            
          customButtonMapper.AddActiveListener(this.Pickup);
          customButtonMapper.AddUnactiveListener(this.Drop);
        }
        SliceObject.OnHasFuelChanged += OnHasFuelChanged;
    }
    private void Update()
    {
        if (isPickedUp && !isHintPlayed && !blockVoiceHint)
        {
            timeSincePickup += Time.deltaTime;
            if (timeSincePickup >= TimeUntilHint)
            {
                PlayHintVoice();
                timeSincePickup = 0;
                isHintPlayed = true;
            }
        }
    }
    public void Pickup()
    {
        isPickedUp = true;
        
    }
    public void Drop()
    {
        isPickedUp = false;
        isHintPlayed = false;
        timeSincePickup = 0;
    }
    public void PlayHintVoice()
    {
        HintVoiceClip.Play();
    }
    
    private void OnHasFuelChanged(bool hasFuel)
    {
        blockVoiceHint = hasFuel;
    }
}
