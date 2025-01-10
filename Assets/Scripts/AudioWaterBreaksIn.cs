using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioWaterBreaksIn : MonoBehaviour
{
    private AudioSource clip;
    [SerializeField] private int DelayAfterGameState = 10;
    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }
    
    private void Start()
    {
        clip = GetComponent<AudioSource>();
    }
    
    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }
    
    
    private void OnGameStateChanged(GameManager.GameState obj)
    {
        if (obj == GameManager.GameState.Game)
        {
            StartCoroutine(PlayClipAfterDelay());
        }
    }

    private IEnumerator PlayClipAfterDelay()
    {
        yield return new WaitForSeconds(DelayAfterGameState); 
        clip.Play();
    }
}
