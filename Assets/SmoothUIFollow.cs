using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothUIFollow : MonoBehaviour
{
    public Transform playerCamera;  // Die VR-Kamera (Hauptkamera des Players)
    public float maxAngle = 45f;    // Maximaler Winkel, bevor UI zurückgesetzt wird
    public float returnSpeed = 2f;  // Geschwindigkeit, mit der UI zurückkommt
    public float distance = 1.5f;   // Distanz zum Spieler

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    
    void Start()
    {
        UpdateUIPosition();
    }

    void Update()
    {
        // Berechne den Winkel zwischen Kamera-Vorwärtsrichtung und UI
        Vector3 toUI = (transform.position - playerCamera.position).normalized;
        float angle = Vector3.Angle(playerCamera.forward, toUI);

        if (angle > maxAngle)
        {
            // Wenn der Winkel überschritten ist, berechne eine neue Position direkt vor dem Spieler
            UpdateUIPosition();
        }

        // Sanfte Bewegung des UI an die neue Position und Rotation
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * returnSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * returnSpeed);
        
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0);
    }

    void UpdateUIPosition()
    {
        // Berechne eine neue Position direkt vor dem Spieler
        targetPosition = playerCamera.position + playerCamera.forward * distance;

        // UI soll den Spieler anschauen
        targetRotation = Quaternion.LookRotation(transform.position - playerCamera.position);
        
        
    }
}
