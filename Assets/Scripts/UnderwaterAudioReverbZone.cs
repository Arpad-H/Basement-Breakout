using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterAudioReverbZone : MonoBehaviour
{
    [SerializeField] private Transform waterPos;
    [SerializeField] private Transform playerPos;
    [SerializeField] private AudioSource ambientRain;
    [SerializeField] private AudioSource ambientWaterSlow;
    [SerializeField] private AudioSource underwaterAudio;
    private AudioReverbZone audioReverbZone;
    private bool isUnderwater = false;
    private bool wasUnderwater = false;
    private bool fadeToUnderwater = false;
    private bool fadeToAboveWater = false;
    void Start() {
        audioReverbZone = GetComponent<AudioReverbZone>();
    }
    void Update() {
        isUnderwater = playerPos.position.y < waterPos.position.y;
        if (isUnderwater) {
            audioReverbZone.enabled = underwaterAudio.enabled = true;
            ambientRain.volume = Mathf.Lerp(ambientRain.volume, 0, Time.deltaTime);
            underwaterAudio.volume = Mathf.Lerp(underwaterAudio.volume, 1, Time.deltaTime);
        } else {
            audioReverbZone.enabled = underwaterAudio.enabled  = false;
            ambientRain.volume = Mathf.Lerp(ambientRain.volume, 1, Time.deltaTime);
            underwaterAudio.volume = Mathf.Lerp(underwaterAudio.volume, 0, Time.deltaTime);
        }
        wasUnderwater = isUnderwater;
    }
}