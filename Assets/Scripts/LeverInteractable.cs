using System;
using Oculus.Interaction;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class LeverInteractable : MonoBehaviour
{
    [SerializeField] private float angleThreshold = 45f;

    public static event Action<bool> onLeverAction;


    private Quaternion initialRotation;
    private bool isActivated = false;
    private bool wasOverThreshold = false;
    
    private OneGrabRotateTransformer rotateTransform;

    private void Awake()
    {
        
        if (rotateTransform == null)
        {
            rotateTransform = GetComponent<OneGrabRotateTransformer>();
        }

        if (rotateTransform == null)
        {
            Debug.LogError("OneGrabRotateTransform component is required on the same GameObject!");
        }
        else
        {
            initialRotation = transform.localRotation;
        }
        
    }

    private void Update()
    {
        if (rotateTransform == null) return;

        
        Quaternion currentRotation = transform.localRotation;
        float angle = Quaternion.Angle(initialRotation, currentRotation);
        
       
        
        bool overThreshold = angle > angleThreshold;
        
        if (isActivated != overThreshold)
        {
            Debug.Log($"[Lever interactable] changed to: {isActivated} Angle: {angle} Threshold: {angleThreshold}");
            onLeverAction?.Invoke(isActivated);
        }
        
        isActivated = overThreshold;
        
        
    }
    
    
}

