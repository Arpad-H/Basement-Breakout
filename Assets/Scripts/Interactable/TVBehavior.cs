using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Haptics;
using UnityEngine;
using UnityEngine.Video;

public class TVBehavior : MonoBehaviour
{
    [SerializeField] private GameObject VideoQuad;
    [SerializeField] private VideoClip[] clips;
    [SerializeField] private AudioSource switchStationSound;
    [SerializeField] private HapticClip hapticClip;
    [SerializeField] private WaterBehaviour waterBehaviour;
    [SerializeField] public AudioSource HintVoiceClip;
    [SerializeField] public int timeGameStarts = 10;
    [SerializeField] private VideoClip blackScreenClip;
    [SerializeField] private AudioSource tvDamageSound;
    [SerializeField] private GameObject timeline;


    private VideoClip currentClip;
    private VideoPlayer videoPlayer;
    [SerializeField] private AudioSource videoAudio;

    private GameManager gameManager;
    private bool _tvIsDamaged = false;
    private bool _firstOnLeverAction = true;
    private bool _electricityIsOn = true;

    // Dictionaries für Wiedergabezeiten
    private Dictionary<VideoClip, double> clipLastPlayTime = new Dictionary<VideoClip, double>();
    private Dictionary<VideoClip, double> clipLastUpdateTime = new Dictionary<VideoClip, double>();
    
   

    private bool hasChangedStateAfterClip = false;

    private float lowestYVideoQuad;

    // Action zur Benachrichtigung anderer Objekte
    public static event Action<GameManager.GameState> gameStateChangedTVBehavior;

    private void Awake()
    {
        StartCoroutine(SubscribeToGameManagerEvent());
        LeverInteractable.OnLeverAction += LeverInteractableOnOnLeverAction;
    }

   

    private void LeverInteractableOnOnLeverAction(bool obj)
    {
        _electricityIsOn = obj;
        Debug.Log($"[TVBehavior] LeverInteractableOnOnLeverAction: {obj} // {(obj && _tvIsDamaged == false && _firstOnLeverAction == false)} // _tvIsDamaged = {_tvIsDamaged}] // _firstOnLeverAction = {_firstOnLeverAction}");
        if (obj && _tvIsDamaged == false && _firstOnLeverAction == false)
        {
            Debug.Log($"[TVBehavior] LeverInteractableOnOnLeverAction: Enable TV");
      
           changeClip(false);
           
        }
        else if (!obj)
        {
            Debug.Log($"[TVBehavior] LeverInteractableOnOnLeverAction: Disable TV");
            WaterDamage(videoPlayer, blackScreenClip);
        }
        else
        {
            _firstOnLeverAction = false;
        }
        
    }

    private IEnumerator SubscribeToGameManagerEvent()
    {
        while (!gameManager)
        {
            gameManager = FindObjectOfType<GameManager>();
           // Debug.Log("TVBehavior: Searching for GameManager...");
            yield return null;
        }

        GameManager.OnGameStateChanged += HandleGameStateChanged;
     //   Debug.Log("TVBehavior: Successfully subscribed to GameManager events.");
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            GameManager.OnGameStateChanged -= HandleGameStateChanged;
        }
        LeverInteractable.OnLeverAction -= LeverInteractableOnOnLeverAction;
    }

    private void Start()
    {
        videoPlayer = VideoQuad.GetComponent<VideoPlayer>();
        //videoAudio = VideoQuad.GetComponent<AudioSource>();

        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, videoAudio);
        
        lowestYVideoQuad = VideoQuad.GetComponent<Renderer>().bounds.min.y;

        currentClip = clips[0];
        videoPlayer.clip = currentClip;
        
        videoAudio.mute = true;

        // Initialize playtime and last update time for each clip
        foreach (var clip in clips)
        {
            clipLastPlayTime[clip] = 0.0;
            clipLastUpdateTime[clip] = Time.time;
        }
    }

  
    void Update()
    {
        if ((waterBehaviour.transform.position.y > lowestYVideoQuad) && !_tvIsDamaged && _electricityIsOn)
        {
            WaterDamage(videoPlayer, tvDamageSound, blackScreenClip);
            _tvIsDamaged = true;
        }
    }

    public void changeClip(bool sendGameState)
    {
        videoAudio.mute = false;
        switchStationSound.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        switchStationSound.Play();
        HapticClipPlayer hapticClipPlayer = new HapticClipPlayer(hapticClip);
        hapticClipPlayer.Play(Controller.Right);
        Debug.Log("Changing Clip");

        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == currentClip)
            {
                // Speichere den aktuellen Wiedergabestand
                double elapsedTime = Time.time - clipLastUpdateTime[currentClip];
                clipLastPlayTime[currentClip] += elapsedTime;
                clipLastPlayTime[currentClip] %= currentClip.length;

                if (i == clips.Length - 1)
                {
                    currentClip = clips[0];
                }
                else
                {
                    currentClip = clips[i + 1];
                }

                updateClipOnQuad();

                // Ändere den GameState nur nach dem ersten Clipwechsel
                if (!hasChangedStateAfterClip && sendGameState)
                {
                    StartCoroutine(StartFlooding());
                    Debug.Log("TVBehavior: Changing GameState to 'Game' after first clip switch.");
                    gameStateChangedTVBehavior?.Invoke(GameManager.GameState.Game); // Action auslösen
                    hasChangedStateAfterClip = true; // Verhindert weitere Änderungen
                }

                break;
            }
        }
    }

    IEnumerator StartFlooding()
    {
        yield return new WaitForSeconds(timeGameStarts);
        timeline.SetActive(true);
      //  waterBehaviour.HandleGameStateChanged(GameManager.GameState.Game);
        HintVoiceClip.Play();
        
        
    }

    public void updateClipOnQuad()
    {
        videoPlayer.clip = currentClip;

        // Berechne die virtuelle Zeit basierend auf der gespeicherten Zeit
        double elapsedTime = Time.time - clipLastUpdateTime[currentClip];
        double virtualTime = (clipLastPlayTime[currentClip] + elapsedTime) % currentClip.length;

        videoPlayer.time = virtualTime;
        clipLastUpdateTime[currentClip] = Time.time;

        videoPlayer.Play();
    }

    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        Debug.Log($"TVBehavior: Tutorial state detected. {newState}");
        if (newState is GameManager.GameState.Tutorial or GameManager.GameState.Win)
        {
            videoAudio.mute = false;
        }
        else
        {
            videoAudio.mute = true;
        }
    }

    private void WaterDamage( VideoPlayer videoPlayer, AudioSource audioSource, VideoClip clip)
    {
            Debug.Log("TVBehavior: water damage detected.");
            videoPlayer.clip = clip;
            videoPlayer.isLooping = true;
            videoPlayer.Play();
            tvDamageSound.loop = false;
            tvDamageSound.Play();
            Debug.Log($"TVBehavior: water damage sound {tvDamageSound.clip.name} // {tvDamageSound.isPlaying}");
        
    }
    private void WaterDamage( VideoPlayer videoPlayer, VideoClip clip )
    {
        Debug.Log("TVBehavior: water damage detected.");
        videoPlayer.clip = clip;
        videoPlayer.isLooping = true;
        videoPlayer.Play();
        
    }
    
}