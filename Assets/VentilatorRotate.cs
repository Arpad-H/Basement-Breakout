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

    void Start()
    {
        if (fanSound != null)
        {
            fanSound.Play();
        }
    }

    void Update()
    {
        currentRotation += rotationSpeed * Time.deltaTime;
        currentRotation %= 360f;  

        Quaternion mainRotation = Quaternion.Euler(0f, currentRotation, 0f);

        // 2. Berechne den Wackel-Effekt:
        // Oszilliert um die X- und Z-Achse mit Sinus- bzw. Kosinus-Funktion
        float wobbleX = wobbleAmplitude * Mathf.Sin(Time.time * wobbleFrequency * 2 * Mathf.PI);
        float wobbleZ = wobbleAmplitude * Mathf.Cos(Time.time * wobbleFrequency * 2 * Mathf.PI);
        Quaternion wobbleRotation = Quaternion.Euler(wobbleX, 0f, wobbleZ);

        // 3. Wende beide Rotationen an:
        // Zuerst die Hauptrotation, dann den Wackel-Effekt
        transform.localRotation = mainRotation * wobbleRotation;
    }
}
