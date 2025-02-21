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

    [Header("Audio references for items state Game")] [SerializeField]
    private AudioSource audioBoatGame;

    [SerializeField] private AudioSource audioCanisterGame;
    [SerializeField] private AudioSource audioChainsawGame;
    [SerializeField] private AudioSource audioFuseboxGame;
    [SerializeField] private AudioSource audioMegaphoneGame;
    [SerializeField] private AudioSource audioRemoteControllerBoatGame;
    [SerializeField] private AudioSource audioStairsGame;

    [Header("Audio references for items state Tutorial")] [SerializeField]
    private AudioSource audioBoatTutorial;

    [SerializeField] private AudioSource audioCanisterTutorial;
    [SerializeField] private AudioSource audioChainsawTutorial;
    [SerializeField] private AudioSource audioFuseboxTutorial;
    [SerializeField] private AudioSource audioMegaphoneTutorial;
    [SerializeField] private AudioSource audioRemoteControllerBoatTutorial;
    [SerializeField] private AudioSource audioStairsTutorial;

    [Header("Audio references for items state Stateless")] [SerializeField]
    private AudioSource audioDartboard;

    [SerializeField] private AudioSource audioFridge;
    [SerializeField] private AudioSource audioLibrary;
    [SerializeField] private AudioSource audioLocker;
    [SerializeField] private AudioSource audioWorkbench;

    [Header("Audio references for items state event")] [SerializeField]
    private AudioSource audioBreatJoke;

    [SerializeField] private AudioSource audioDrowning;
    [SerializeField] private AudioSource audioTVAfterElectroShock;
    [SerializeField] private AudioSource audioWonByBoatWin;
    [SerializeField] private AudioSource audioWonByDoorWin;
    [SerializeField] private AudioSource audioWonByMegaphoneWin;
    [SerializeField] private AudioSource audioWoodchipWallpaperJoke;

    private AudioSource currentlyPlayingSource;


    [Header("Audio references for reminders")] [SerializeField]
    private AudioSource[] reminders;

    [SerializeField] private float reminderTime = 60f;


    private GameManager.GameState gameState;
    private float reminderTimer = 0f;
    private Dictionary<Item, AudioSource> _audioGameMap;
    private Dictionary<Item, AudioSource> _audioTutorialMap;
    private Dictionary<Item, AudioSource> _audioStatelessMap;
    private Dictionary<AudioSource, bool> _audioPlayedStatus;


    private void Awake()
    {
        VoiceOverEventSender.OnAction += VoiceOverEventSenderOnOnAction;
        GameManager.OnGameStateChanged += SetgameState;
        TVBehavior.TVElectricShock +=  VoiceOverEventSenderOnOnAction;
        WinCollision.audioWonByDoorWin += VoiceOverEventSenderOnOnAction;
        AudioDetection.audioWonByMegaphoneWin += VoiceOverEventSenderOnOnAction;
        CustomCollisionEventHandler.audioWonByBoatWin += VoiceOverEventSenderOnOnAction;
        _audioGameMap = new()
        {
            { Item.Chainsaw, audioChainsawGame },
            { Item.Boat, audioBoatGame },
            { Item.Megaphone, audioMegaphoneGame },
            { Item.Remotecontrollerboat, audioRemoteControllerBoatGame },
            { Item.Stairs, audioStairsGame },
            { Item.Fusebox, audioFuseboxGame },
            { Item.Canister, audioCanisterGame }
        };
        _audioTutorialMap = new()
        {
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
            { Item.Dartboad, audioDartboard },
            { Item.Locker, audioLocker },
            { Item.TVAfterElectroShock, audioTVAfterElectroShock },
            { Item.Drowning, audioDrowning },
            { Item.BreatJoke, audioBreatJoke },
            { Item.WoodchipWallpaperJoke, audioWoodchipWallpaperJoke },
            { Item.WonByBoatWin, audioWonByBoatWin },
            { Item.WonByDoorWin, audioWonByDoorWin },
            { Item.WonByMegaphoneWin, audioWonByMegaphoneWin }
        };
        InitializeAudioPlayedStatus();
    }

    private void OnDestroy()
    {
        VoiceOverEventSender.OnAction -= VoiceOverEventSenderOnOnAction;
        GameManager.OnGameStateChanged -= SetgameState;
        TVBehavior.TVElectricShock -= VoiceOverEventSenderOnOnAction;
        WinCollision.audioWonByDoorWin -= VoiceOverEventSenderOnOnAction;
        AudioDetection.audioWonByMegaphoneWin -= VoiceOverEventSenderOnOnAction;
        CustomCollisionEventHandler.audioWonByBoatWin -= VoiceOverEventSenderOnOnAction;
    }

    private void VoiceOverEventSenderOnOnAction(Item obj)
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


    private void PlayVoice(Item obj, Dictionary<Item, AudioSource> audioMapState,
        Dictionary<Item, AudioSource> audioMapStateless)
    {
        Debug.Log(
            $"[VoiceOverManager] audioMapState.TryGetValue(obj, out AudioSource audio) {audioMapState.TryGetValue(obj, out AudioSource audio2)} //audio != null {audio2 != null} //Item obj {obj.ToString()}");
        if (audioMapState.TryGetValue(obj, out AudioSource audio) && audio != null)
        {
            Debug.Log(
                $"[VoiceOverManager] // _audioPlayedStatus.TryGetValue(audio, out bool wasPlayed) {_audioPlayedStatus.TryGetValue(audio, out bool wasPlayed2)} //!wasPlayed {wasPlayed2}; ");
            if (_audioPlayedStatus.TryGetValue(audio, out bool wasPlayed) && !wasPlayed)
            {
                switch (obj)
                {
                    case Item.Chainsaw:
                    case Item.Canister:
                        if (!_chainsawCanisterWasPlayed)
                        {
                            // audio.Play();
                            _chainsawCanisterWasPlayed = PlaySound(audio);
                        }

                        break;
                    case Item.Boat:
                    case Item.Remotecontrollerboat:
                        if (!_boatRemoteControllerBoatWasPlayed)
                        {
                            // audio.Play();
                            _boatRemoteControllerBoatWasPlayed = PlaySound(audio);
                        }

                        break;
                    default:
                        // audio.Play();
                        break;
                }

                _audioPlayedStatus[audio] = PlaySound(audio);
                Debug.Log($"[VoiceOverManager]  audio.Play(); {obj.ToString()} // {audio}");
            }
        }
        else if (audioMapStateless.TryGetValue(obj, out AudioSource audio1) && audio1 != null)
        {
            Debug.Log($"[VoiceOverManager] else if audio1 =! {audio1 != null} // audio1: {audio1.clip.name} //");
            if (_audioPlayedStatus.TryGetValue(audio1, out bool wasPlayed1) && !wasPlayed1)
            {
                // audio1.Play();
                _audioPlayedStatus[audio1] = PlaySound(audio1);
                Debug.Log($"[VoiceOverManager] audio.Play(); {obj.ToString()} // {audio1}");
            }
            else
            {
                Debug.Log(
                    $"[VoiceOverManager]  _audioPlayedStatus.TryGetValue(audio1, out bool wasPlayed1) && !wasPlayed1 FAILED: //{_audioPlayedStatus.TryGetValue(audio1, out bool wasPlayed2)} //!wasPlayed1 {wasPlayed1}; ) ");
            }
        }
    }

    private bool PlaySound(AudioSource audioSource)
    {
        if (currentlyPlayingSource != null && currentlyPlayingSource.isPlaying)
        {
            Debug.Log($"[VoiceOverManager] PlaySound NOT: currentlyPlayingSource{currentlyPlayingSource.clip.name} // audioSource: {audioSource.clip.name} ");
            return false;
        }
        else
        {
            
            currentlyPlayingSource = audioSource;
            currentlyPlayingSource.Play();
            Debug.Log($"[VoiceOverManager] PlaySound TUE: currentlyPlayingSource{currentlyPlayingSource.clip.name} // audioSource: {audioSource.clip.name} ");
            return true;
        }
       
    }


    void Update()
    {
        if (gameState == GameManager.GameState.Tutorial)
        {
            reminderTimer += Time.deltaTime;
            if (reminderTimer >= reminderTime)
            {
                PlayARandomRiminder(reminders);
                reminderTimer = 0f;
            }
        }
    }

    private void PlayARandomRiminder(AudioSource[] audioSources)
    {
        if (audioSources == null || audioSources.Length == 0)
        {
            Debug.LogWarning("[VoiceOverManager] Audio reminder array is empty!");
            return;
        }

        Random r = new Random();
        int rInt = r.Next(0, audioSources.Length);
        AudioSource audio = audioSources[rInt];
        if (audio != null)
        {
            Debug.Log($"[VoiceOverManager] audioSources[rInt].Play(); {audio.clip.name}");
            // audioSources[rInt].Play();
            if (!PlaySound(audioSources[rInt]))
            {
                reminderTimer = reminderTime - 10;
            }
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
        Locker,
        Chainsaw,
        Canister,
        Stairs,
        Megaphone,
        Boat,
        Remotecontrollerboat,
        Fusebox,
        Dartboad,
        Fridge,
        Workbench,
        Library,
        TVAfterElectroShock,
        Drowning,
        BreatJoke,
        WoodchipWallpaperJoke,
        WonByBoatWin,
        WonByDoorWin,
        WonByMegaphoneWin
    }
}