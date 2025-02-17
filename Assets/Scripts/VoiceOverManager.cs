using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Meta.XR.ImmersiveDebugger.UserInterface;
using UnityEngine;
using Random = System.Random;

public class VoiceOverManager : MonoBehaviour
{
    // Start is called before the first frame update
    
    private bool _chainsawCanisterWasPlayed = false;
    private bool _boatRemoteControllerBoatWasPlayed = false;
    [Header("Audio references for items state Game")]
    [SerializeField] private AudioSource audioLockerGame;
    [SerializeField] private AudioSource audioChainsawGame;
    [SerializeField] private AudioSource audioBoatGame;
    [SerializeField] private AudioSource audioMegaphoneGame;
    [SerializeField] private AudioSource audioRemoteControllerBoatGame;
    [SerializeField] private AudioSource audioStairsGame;
    [SerializeField] private AudioSource audioFuseboxGame;
    [SerializeField] private AudioSource audioCanisterGame;

    [Header("Audio references for items state Tutorial")]
    [SerializeField] private AudioSource audioLockerTutorial;
    [SerializeField] private AudioSource audioChainsawTutorial;
    [SerializeField] private AudioSource audioBoatTutorial;
    [SerializeField] private AudioSource audioMegaphoneTutorial;
    [SerializeField] private AudioSource audioRemoteControllerBoatTutorial;
    [SerializeField] private AudioSource audioStairsTutorial;
    [SerializeField] private AudioSource audioFuseboxTutorial;
    [SerializeField] private AudioSource audioCanisterTutorial;

    [Header("Audio references for items state Stateless")]
    [SerializeField] private AudioSource audioFridge;
    [SerializeField] private AudioSource audioWorkbench;
    [SerializeField] private AudioSource audioLibrary;
    [SerializeField] private AudioSource audioDartboard;
    
    
    
    [Header("Audio references for reminders")]
    [SerializeField] private AudioSource[] reminders;
    [SerializeField] private float reminderTime = 60f;
    
    
    private GameManager.GameState gameState;
    private float reminderTimer = 0f;
    private Dictionary<Item, AudioSource> _audioGameMap;
    private Dictionary<Item, AudioSource> _audioTutorialMap;
    private Dictionary<Item, AudioSource> _audioStatelessMap;
    private Dictionary<AudioSource, bool> _audioPlayedStatus;
    
    
    


    private void Awake()
    {
        VoiceOverEventSender.OnLeverAction += VoiceOverEventSenderOnOnLeverAction;
        GameManager.OnGameStateChanged += SetgameState;
        _audioGameMap = new()
        {
            { Item.Locker, audioLockerGame },
            { Item.Chainsaw, audioChainsawGame },
            { Item.Boat, audioBoatGame },
            { Item.Megaphone, audioMegaphoneGame },
            { Item.Remotecontrollerboat, audioRemoteControllerBoatGame },
            { Item.Stairs, audioStairsGame },
            { Item.Fusebox, audioFuseboxGame },
            {Item.Canister, audioCanisterGame}
        };
        _audioTutorialMap = new()
        {
            { Item.Locker, audioLockerTutorial },
            { Item.Chainsaw, audioChainsawTutorial },
            { Item.Boat, audioBoatTutorial },
            { Item.Megaphone, audioMegaphoneTutorial },
            { Item.Remotecontrollerboat, audioRemoteControllerBoatTutorial },
            { Item.Stairs, audioStairsTutorial },
            { Item.Fusebox, audioFuseboxTutorial },
            { Item.Canister, audioCanisterTutorial }
        };
         _audioStatelessMap = new()
         {
             { Item.Fridge, audioFridge },
             { Item.Workbench, audioWorkbench },
             { Item.Library, audioLibrary },
             { Item.Dartboad, audioDartboard }
         };
         InitializeAudioPlayedStatus();
    }

    private void OnDestroy()
    {
        VoiceOverEventSender.OnLeverAction -= VoiceOverEventSenderOnOnLeverAction;
        GameManager.OnGameStateChanged -= SetgameState;
    }

    private void VoiceOverEventSenderOnOnLeverAction(Item obj)
    {

    Debug.Log($"[VoiceOverManager]On Lever Action {obj.ToString()} // Gamestate {gameState.ToString()}");
        if (gameState == GameManager.GameState.Game)
        {
            PlayVoice(obj, _audioGameMap, _audioStatelessMap);
        }
        else if (gameState == GameManager.GameState.Tutorial)
        {
            PlayVoice(obj, _audioTutorialMap, _audioStatelessMap);
        }
    }
    private void InitializeAudioPlayedStatus()
    {
        _audioPlayedStatus = new Dictionary<AudioSource, bool>();
        
        foreach (var audio in _audioGameMap.Values
                     .Concat(_audioTutorialMap.Values)
                     .Concat(_audioStatelessMap.Values))
        {
            _audioPlayedStatus[audio] = false;
        }
    }

    private void PlayVoice(Item obj, Dictionary<Item, AudioSource> audioMapState, Dictionary<Item, AudioSource> audioMapStateless)
    {
        Debug.Log($"[VoiceOverManager] audioMapState.TryGetValue(obj, out AudioSource audio) {audioMapState.TryGetValue(obj, out AudioSource audio2)} //audio != null {audio2 != null}");
        if (audioMapState.TryGetValue(obj, out AudioSource audio) && audio != null)
        {
            Debug.Log($"[VoiceOverManager] // _audioPlayedStatus.TryGetValue(audio, out bool wasPlayed) { _audioPlayedStatus.TryGetValue(audio, out bool wasPlayed2)} //!wasPlayed {wasPlayed2}; ");
            if (_audioPlayedStatus.TryGetValue(audio, out bool wasPlayed) && !wasPlayed)
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
                _audioPlayedStatus[audio] = true;
                Debug.Log($"[VoiceOverManager] {obj.ToString()} // {audio}");
            }
            
                
        }
        else if (audioMapStateless.TryGetValue(obj, out AudioSource audio1) && audio1 != null )
        {
            if ( _audioPlayedStatus.TryGetValue(audio1, out bool wasPlayed1) && !wasPlayed1)
            {
                audio1.Play();
                _audioPlayedStatus[audio1] = true;
                Debug.Log($"[VoiceOverManager] {obj.ToString()} // {audio1}");
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
                PlayARandomRiminder(reminders);
                reminderTimer = 0f;
            }
        }
    }

    private static void PlayARandomRiminder(AudioSource[] audioSources)
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

        if (gameState == GameManager.GameState.Game)
        {
            _boatRemoteControllerBoatWasPlayed = false;
            _chainsawCanisterWasPlayed = false;
        }
    }
    
    
    public enum Item
    {
        Locker, Chainsaw, Canister, Stairs, Megaphone, Boat, Remotecontrollerboat, Fusebox, Dartboad, Fridge, Workbench, Library
    }
    
    
}
