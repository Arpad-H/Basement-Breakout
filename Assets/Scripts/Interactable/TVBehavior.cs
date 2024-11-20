using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TVBehavior : MonoBehaviour
{
    [SerializeField] private GameObject VideoQuad;
    [SerializeField] private VideoClip[] clips;
    [SerializeField] private AudioSource switchStationSound;
    
    private VideoClip currentClip;
    private VideoPlayer videoPlayer;
    private AudioSource videoAudio;

    // Dictionary to store each clip’s last playtime and last update time
    private Dictionary<VideoClip, double> clipLastPlayTime = new Dictionary<VideoClip, double>();
    private Dictionary<VideoClip, double> clipLastUpdateTime = new Dictionary<VideoClip, double>();

    void Start()
    {
        videoPlayer = VideoQuad.GetComponent<VideoPlayer>();
        videoAudio = VideoQuad.GetComponent<AudioSource>();

        // Set the AudioSource to be used by the VideoPlayer
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

    void Update()
    {

    }

    public void changeClip()
    {
        switchStationSound.pitch = Random.Range(0.8f, 1.2f);
        switchStationSound.Play();
        Debug.Log("Changing Clip");

        // Calculate ongoing time for the current clip
        clipLastPlayTime[currentClip] += Time.time - clipLastUpdateTime[currentClip];
        clipLastPlayTime[currentClip] %= currentClip.length; // Ensure it wraps if over the length

        // Move to the next clip in sequence
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == currentClip)
            {
                if (i == clips.Length - 1)
                {
                    Debug.Log("Last Clip");
                    currentClip = clips[0];
                }
                else
                {
                    Debug.Log("Next Clip");
                    currentClip = clips[i + 1];
                }
                updateClipOnQuad();
                break;
            }
        }
    }

    public void updateClipOnQuad()
    {
        videoPlayer.clip = currentClip;

        // Calculate the virtual playback time based on the elapsed time since last update
        double elapsedTime = Time.time - clipLastUpdateTime[currentClip];
        double virtualTime = (clipLastPlayTime[currentClip] + elapsedTime) % currentClip.length;

        // Set videoPlayer time to this calculated virtual time
        videoPlayer.time = virtualTime;
        clipLastUpdateTime[currentClip] = Time.time;

        videoPlayer.Play();
    }
}
