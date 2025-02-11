using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class Switch : OneGrabRotateTransformer
{
    [SerializeField] public OneGrabRotateTransformer rotateTransform;
    [SerializeField] public GameObject switchObject;
    private bool _switchState = false;
    
    private bool _previousMaxAngleState = false;
    private bool _previousMinAngleState = false;
    Vector3 _vector3;
    private void Start()
    {
        _vector3 = transform.rotation.eulerAngles;
    }
    
    void Update()
    {
        float relativAngle = Mathf.DeltaAngle(0, (transform.rotation.eulerAngles.x - _vector3.x) -360);
        bool isAtMaxAngle = relativAngle >= rotateTransform.Constraints.MaxAngle.Value;
        bool isAtMinAngle = relativAngle <= rotateTransform.Constraints.MinAngle.Value;

        if (isAtMaxAngle && !_previousMaxAngleState) 
        {
            Debug.Log($"Switch State: {_switchState} Light disable");
            switchObject.SetActive(false);
            _switchState = !_switchState;
        }
        else if (isAtMinAngle && !_previousMinAngleState)
        {
            Debug.Log($"Switch State: {_switchState} Light enable");
            switchObject.SetActive(true);
            _switchState = !_switchState;
        }

        _previousMaxAngleState = isAtMaxAngle;
        _previousMinAngleState = isAtMinAngle;
    }
    
    
    
}

