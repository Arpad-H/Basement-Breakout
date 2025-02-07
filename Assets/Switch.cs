using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] public OneGrabRotateTransformer rotateTransform;
    [SerializeField] public GameObject switchObject;
    private bool _switchState = false;
    
    private bool _previousMaxAngleState = false;
    private bool _previousMinAngleState = false;

    void Update()
    {
        bool isAtMaxAngle = rotateTransform.GetConstrainedRelativeAngle() >= rotateTransform.Constraints.MaxAngle.Value;
        bool isAtMinAngle = rotateTransform.GetConstrainedRelativeAngle() <= rotateTransform.Constraints.MinAngle.Value;

        if (isAtMaxAngle && !_previousMaxAngleState) 
        {
            Debug.Log($"[Switch] Switch is off {rotateTransform.GetConstrainedRelativeAngle()}");
            switchObject.SetActive(false);
            _switchState = !_switchState;
        }
        else if (isAtMinAngle && !_previousMinAngleState)
        {
            Debug.Log($"[Switch] Switch is on {rotateTransform.GetConstrainedRelativeAngle()}");
            switchObject.SetActive(true);
            _switchState = !_switchState;
        }

        _previousMaxAngleState = isAtMaxAngle;
        _previousMinAngleState = isAtMinAngle;
    }
}

