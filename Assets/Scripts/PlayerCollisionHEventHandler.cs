using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHEventHandler : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    private CollisionEventHandler Instance;
    
    public static event Action<bool> OnWaterStateChangedPlayer;
    
    private bool hasCollided  = false;
    
    
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject == targetObject)
        {
            hasCollided = true;
            OnWaterStateChangedPlayer?.Invoke(hasCollided);
            Debug.Log($"Object entered: {other.gameObject.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == targetObject)
        {
            hasCollided = false;
            
            OnWaterStateChangedPlayer?.Invoke(hasCollided);
            Debug.Log($"Object exited: {other.gameObject.name}");
        }
    }

    

    public void SetTargetObject(GameObject newTargetObject)
    {
        targetObject = newTargetObject;
    }
    
    
    
}
