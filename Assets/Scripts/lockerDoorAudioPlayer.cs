using System.Collections;
using System.Collections.Generic;
using Oculus.Haptics;
using UnityEngine;

public class lockerDoorAudioPlayer : MonoBehaviour
{
    private float previousRotation = 0;
    private float currentRotation = 0;
    [SerializeField] private AudioSource[] squeakSound;
    private float squeakTimer = 2.5f;
    private bool justSqueaked = false;
    [SerializeField] private bool xAxisCheck = false;
    [SerializeField] private HapticClip switchHaptic;
    private HapticClipPlayer switchHapticPlayer;

    private int frameCounter = 30;
    
    private void Start() {
        switchHapticPlayer = new HapticClipPlayer(switchHaptic);
    }
    void Update() {
        if (frameCounter > 0) frameCounter--;
        if (justSqueaked) squeakTimer -= Time.deltaTime;
        if (squeakTimer <= 0) {
            justSqueaked = false;
            squeakTimer = 5f;
        }
        
        if (!xAxisCheck) currentRotation = transform.rotation.eulerAngles.y;
        else currentRotation = transform.rotation.eulerAngles.x;
        
        if (currentRotation - previousRotation != 0) {
            bool squeaking = false;
            
            foreach (var sound in squeakSound) {
                if (sound.isPlaying) {
                    squeaking = true;
                }
            }

            if (!squeaking && !justSqueaked && frameCounter == 0) {
                switchHapticPlayer.Play(Controller.Both);
                AudioSource audio = squeakSound[Random.Range(0, squeakSound.Length - 1)];
                audio.pitch = Random.Range(0.6f, 1.0f);
                audio.Play();
                justSqueaked = true;
            }
        }
        previousRotation = currentRotation;
        
    }
}
