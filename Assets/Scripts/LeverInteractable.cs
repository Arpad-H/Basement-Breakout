using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class LeverInteractable : MonoBehaviour
{
    [SerializeField] private float angleThreshold = 45f;

    public static event Action<bool> onLeverAction;


    private HingeJoint hingeJoint;
    private bool isActivated = false;
    private bool wasOverThreshold = false;

    private void Awake()
    {
        
    }

    private void Update()
    {
        float leverAngle = hingeJoint.angle;
        bool isOverThreshold = Mathf.Abs(leverAngle) > angleThreshold;


        if (isOverThreshold != wasOverThreshold)
        {
            isActivated = isOverThreshold;
            onLeverAction?.Invoke(isActivated);
        }

        wasOverThreshold = isOverThreshold;
    }
}