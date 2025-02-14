using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class VoiceOverManager : MonoBehaviour
{
    // Start is called before the first frame update
    
    private bool _chainsawCanisterWasPlayed = false;
    private bool _boatRemoteControllerBoatWasPlayed = false;
    [Header("Audio references for items")]
    [SerializeField] private AudioSource audioLocker;
    [SerializeField] private AudioSource audioChainsaw;
    [SerializeField] private AudioSource audioCanister;
    [SerializeField] private AudioSource audioStairs;
    [SerializeField] private AudioSource audioMegaphone;
    [SerializeField] private AudioSource audioBoat;
    [SerializeField] private AudioSource audioRemoteControllerBoat;
    [SerializeField] private AudioSource audioFusebox;
    [SerializeField] private AudioSource audioDartboad;
    [SerializeField] private AudioSource audioFridge;
    [SerializeField] private AudioSource audioWorkbench;
    [SerializeField] private AudioSource audioLibrary;
    
    
    
    [Header("Audio references for reminders")]
    [SerializeField] private AudioSource[] reminders;
    [SerializeField] private float reminderTime = 60f;
    
    
    private GameManager.GameState gameState;
    private float reminderTimer = 0f;
    private Dictionary<Item, AudioSource> _audioMap;
    


    private void Awake()
    {
        VoiceOverEventSender.OnLeverAction += VoiceOverEventSenderOnOnLeverAction;
        GameManager.OnGameStateChanged += SetgameState;
        _audioMap = new()
        {
            { Item.Locker, audioLocker },
            { Item.Chainsaw, audioChainsaw },
            { Item.Canister, audioCanister },
            { Item.Stairs, audioStairs },
            { Item.Megaphone, audioMegaphone },
            { Item.Boat, audioBoat },
            { Item.Remotecontrollerboat, audioRemoteControllerBoat },
            { Item.Fusebox, audioFusebox },
            { Item.Dartboad, audioDartboad },
            { Item.Fridge, audioFridge },
            { Item.Workbench, audioWorkbench },
            { Item.Library, audioLibrary }
        };
        
    }

    private void OnDestroy()
    {
        VoiceOverEventSender.OnLeverAction -= VoiceOverEventSenderOnOnLeverAction;
        GameManager.OnGameStateChanged -= SetgameState;
    }

    private void VoiceOverEventSenderOnOnLeverAction(Item obj)
    {
     
        
        if (_audioMap.TryGetValue(obj, out AudioSource audio))
        {
            switch (obj)
            {
                case Item.Chainsaw :
                case Item.Canister:
                    if (!_chainsawCanisterWasPlayed)
                    {
                        audio.Play();
                        _chainsawCanisterWasPlayed = true;
                    }
                    break;
                case Item.Boat:
                case Item.Remotecontrollerboat:
                    if (!_boatRemoteControllerBoatWasPlayed)
                    {
                        audio.Play();
                        _boatRemoteControllerBoatWasPlayed = true;
                    }
                    break;
                default:
                    audio.Play();
                    break;
            }
        }
    }
    void Update()
    {
        reminderTimer += Time.deltaTime;
        if (gameState == GameManager.GameState.Tutorial)
        {
            if (reminderTimer >= reminderTime)
            {
                playARandomRiminder(reminders);
                reminderTimer = 0f;
            }
        }
    }

    private void playARandomRiminder(AudioSource[] audioSources)
    {
        if(audioSources == null || audioSources.Length == 0)
        {
            Debug.LogWarning("[VoiceOverManager] Audio reminder array is empty!");
            return;
        }
        Random r = new Random();
        int rInt = r.Next(0, audioSources.Length-1);
        AudioSource audio = audioSources[rInt];
        if (audio != null)
        {
            Debug.Log($"[VoiceOverManager] audioSources[rInt].Play(); {audio.clip.name}");
            audioSources[rInt].Play();
        }
        
    }

    private void SetgameState(GameManager.GameState gameState)
    {
        this.gameState = gameState;
    }
    
    
    public enum Item
    {
        Locker, Chainsaw, Canister, Stairs, Megaphone, Boat, Remotecontrollerboat, Fusebox, Dartboad, Fridge, Workbench, Library
    }
    
    
}
