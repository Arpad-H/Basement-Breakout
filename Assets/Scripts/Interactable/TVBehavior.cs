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


    private VideoClip currentClip;
    private VideoPlayer videoPlayer;
    private AudioSource videoAudio;

    private GameManager gameManager;

    // Dictionaries für Wiedergabezeiten
    private Dictionary<VideoClip, double> clipLastPlayTime = new Dictionary<VideoClip, double>();
    private Dictionary<VideoClip, double> clipLastUpdateTime = new Dictionary<VideoClip, double>();

    private bool hasChangedStateAfterClip = false;

    // Action zur Benachrichtigung anderer Objekte
    public static event Action<GameManager.GameState> gameStateChangedTVBehavior;

    private void Awake()
    {
        StartCoroutine(SubscribeToGameManagerEvent());
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
    }

    private void Start()
    {
        videoPlayer = VideoQuad.GetComponent<VideoPlayer>();
        videoAudio = VideoQuad.GetComponent<AudioSource>();

        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, videoAudio);

        currentClip = clips[0];
        videoPlayer.clip = currentClip;

        // Initialize playtime and last update time for each clip
        foreach (var clip in clips)
        {
            clipLastPlayTime[clip] = 0.0;
            clipLastUpdateTime[clip] = Time.time;
        }
    }

    public void changeClip()
    {
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
                if (!hasChangedStateAfterClip)
                {
                    StartCoroutine(StartFlooding());
                    Debug.LogError("TVBehavior: Changing GameState to 'Game' after first clip switch.");
                    gameStateChangedTVBehavior?.Invoke(GameManager.GameState.Game); // Action auslösen
                    hasChangedStateAfterClip = true; // Verhindert weitere Änderungen
                }

                break;
            }
        }
    }

    IEnumerator StartFlooding()
    {
        yield return new WaitForSeconds(10);
        waterBehaviour.HandleGameStateChanged(GameManager.GameState.Game);
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
        if (newState == GameManager.GameState.Tutorial)
        {
            Debug.Log("TVBehavior: Tutorial state detected.");
        }
        else if (newState == GameManager.GameState.Game)
        {
            Debug.Log("TVBehavior: Game state detected.");
        }
    }
}