using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterAudioReverbZone : MonoBehaviour
{
    [SerializeField] private Transform waterPos;
    [SerializeField] private Transform playerPos;
    AudioReverbZone audioReverbZone;
    void Start() {
        audioReverbZone = GetComponent<AudioReverbZone>();
    }
    void Update() {
        audioReverbZone.enabled = playerPos.position.y < waterPos.position.y;
    }
}
