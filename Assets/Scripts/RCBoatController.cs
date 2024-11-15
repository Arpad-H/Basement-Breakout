using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RCBoatController : MonoBehaviour
{
    
    void Start()
    {
        
        
    }
    
    void Update()
    {
        OVRInput.Update();
        OVRInput.FixedUpdate();
        Vector2 thumbstick = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
        print(thumbstick);
    }
}
