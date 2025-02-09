using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lockerDoorAudioPlayer : MonoBehaviour
{
    private float previousYRotation = 0;
    private float currentYRotation = 0;
    [SerializeField] private AudioSource[] squeakSound;
    private float squeakTimer = 2.5f;
    private bool justSqueaked = false;

    private int frameCounter = 30;
    void Update() {
        if (frameCounter > 0) frameCounter--;
        if (justSqueaked) squeakTimer -= Time.deltaTime;
        if (squeakTimer <= 0) {
            justSqueaked = false;
            squeakTimer = 5f;
        }
        
        currentYRotation = transform.rotation.eulerAngles.y;
        if (currentYRotation - previousYRotation != 0) {
            bool squeaking = false;
            
            foreach (var sound in squeakSound) {
                if (sound.isPlaying) {
                    squeaking = true;
                }
            }

            if (!squeaking && !justSqueaked && frameCounter == 0) {
                AudioSource audio = squeakSound[Random.Range(0, squeakSound.Length - 1)];
                audio.pitch = Random.Range(0.6f, 1.0f);
                audio.Play();
                justSqueaked = true;
            }
        }
        previousYRotation = currentYRotation;
        
    }
}
