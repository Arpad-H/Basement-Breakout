using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceoverHints : MonoBehaviour
{
    // public delegate void OnPickupVoicedItem(VoiceoverHints voiceoverHints);
    // public delegate void OnDropVoicedItem();
    public AudioSource HintVoiceClip;
    private float timeSincePickup = 0;
    private bool isPickedUp = false;
    public float TimeUntilHint = 10f;

    private void Update()
    {
        if (isPickedUp)
        {
            timeSincePickup += Time.deltaTime;
            if (timeSincePickup >= TimeUntilHint)
            {
                PlayHintVoice();
                timeSincePickup = 0;
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
        timeSincePickup = 0;
    }
    public void PlayHintVoice()
    {
        HintVoiceClip.Play();
    }
}
