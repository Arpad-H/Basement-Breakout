using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceOverManager : MonoBehaviour
{
    // Start is called before the first frame update
    
    private bool _chainsawCanisterWasPlayed = false;
    private bool _boatRemoteControllerBoatWasPlayed = false;
    [Header("Audio References")]
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
    
    private Dictionary<item, AudioSource> _audioMap;


    private void Awake()
    {
        VoiceOverEventSender.OnLeverAction += VoiceOverEventSenderOnOnLeverAction;
        _audioMap = new()
        {
            { item.LOCKER, audioLocker },
            { item.CHAINSAW, audioChainsaw },
            { item.CANISTER, audioCanister },
            { item.STAIRS, audioStairs },
            { item.MEGAPHONE, audioMegaphone },
            { item.BOAT, audioBoat },
            { item.REMOTECONTROLLERBOAT, audioRemoteControllerBoat },
            { item.FUSEBOX, audioFusebox },
            { item.DARTBOAD, audioDartboad },
            { item.FRIDGE, audioFridge },
            { item.WORKBENCH, audioWorkbench },
            { item.LIBRARY, audioLibrary }
        };
        
    }

    private void OnDestroy()
    {
        VoiceOverEventSender.OnLeverAction -= VoiceOverEventSenderOnOnLeverAction;
    }

    private void VoiceOverEventSenderOnOnLeverAction(item obj)
    {
     
        
        if (_audioMap.TryGetValue(obj, out AudioSource audio))
        {
            // Spezialbehandlung für Trigger-Sounds
            Debug.Log($"[VoiceOverManager] BOAT REMOTECONTRALLERBOAT {obj} //_boatRemoteControllerBoatWasPlayed {_boatRemoteControllerBoatWasPlayed} //_chainsawCanisterWasPlayed {_chainsawCanisterWasPlayed}");
            switch (obj)
            {
                case item.CHAINSAW when !_chainsawCanisterWasPlayed:
                case item.CANISTER when !_chainsawCanisterWasPlayed:
                    audio.Play();
                    _chainsawCanisterWasPlayed = true;
                    break;
            
                case item.BOAT when !_boatRemoteControllerBoatWasPlayed:
                case item.REMOTECONTROLLERBOAT when !_boatRemoteControllerBoatWasPlayed:
                    Debug.Log($"[VoiceOverManager] BOAT REMOTECONTRALLERBOAT {obj} // {_boatRemoteControllerBoatWasPlayed}");
                    audio.Play();
                    _boatRemoteControllerBoatWasPlayed = true;
                    break;
            
                default:
                    audio.Play();
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"[VoiceOverManager] Keine AudioSource für {obj} vorhanden");
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    public enum item
    {
        LOCKER, CHAINSAW, CANISTER, STAIRS, MEGAPHONE, BOAT, REMOTECONTROLLERBOAT, FUSEBOX, DARTBOAD, FRIDGE, WORKBENCH, LIBRARY
    }
    
    
}
