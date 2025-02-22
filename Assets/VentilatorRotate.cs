using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentilatorRotate : MonoBehaviour
{
    [Header("Hauptrotation")]
    [Tooltip("Drehgeschwindigkeit in Grad pro Sekunde")]
    public float rotationSpeed = 360f;

    [Header("Wackel-Effekt")]
    [Tooltip("Amplitude des Wackelns in Grad")]
    public float wobbleAmplitude = 5f;
    [Tooltip("Frequenz des Wackelns (Zyklen pro Sekunde)")]
    public float wobbleFrequency = 2f;

    [Header("Audio")]
    public AudioSource fanSound;

    // Hilfsvariable zur Speicherung des aktuellen Drehwinkels (um die Y-Achse)
    private float currentRotation = 0f;
    
    private bool _isElectricityOn = true;
    private bool  _previousState = false;

    void Start()
    {
        LeverInteractable.OnLeverAction += LeverInteractableOnOnLeverAction;
        if (fanSound != null)
        {
            fanSound.Play();
        }
    }

    private void OnDestroy()
    {
        LeverInteractable.OnLeverAction -= LeverInteractableOnOnLeverAction;
    }

    private void LeverInteractableOnOnLeverAction(bool obj)
    {
        Debug.Log($"[Ventilator] LeverInteractableOnOnLeverAction {obj}");
        _isElectricityOn = obj;
    }

    void Update()
    {
        
        
        
        if (_isElectricityOn)
        {
            if (fanSound != null)
            {
                fanSound.mute = false;
            }
            currentRotation += rotationSpeed * Time.deltaTime;
            currentRotation %= 360f;  

            Quaternion mainRotation = Quaternion.Euler(0f,  0f, currentRotation);

            float wobbleX = wobbleAmplitude * Mathf.Sin(Time.time * wobbleFrequency * 2 * Mathf.PI);
            float wobbleZ = wobbleAmplitude * Mathf.Cos(Time.time * wobbleFrequency * 2 * Mathf.PI);
            Quaternion wobbleRotation = Quaternion.Euler(wobbleX, 0f, wobbleZ);

            transform.localRotation = mainRotation * wobbleRotation;
          
            
        }
        else if (!_isElectricityOn)
        {
            
            if (fanSound != null)
            {
                fanSound.mute = true;
            }
            
        }
    }
}
