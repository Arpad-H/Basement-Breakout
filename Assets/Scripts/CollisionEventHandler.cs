using UnityEngine;
using System;

public class CollisionEventHandler : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;

    private CollisionEventHandler Instance;
    
    public static event Action<bool> OnWaterStateChangedCable;
    
    private bool hasCollided  = false;
    
    private bool playerIsInWhater = false;
    private bool cableISinWater = false;

    
    private void Awake()
    {
        //Instance = this;
    }
    


    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject == targetObject)
        {
            hasCollided = true;
            OnWaterStateChangedCable?.Invoke(hasCollided);
            Debug.Log($"Object entered: {other.gameObject.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == targetObject)
        {
            hasCollided = false;
            
            OnWaterStateChangedCable?.Invoke(hasCollided);
            Debug.Log($"Object exited: {other.gameObject.name}");
        }
    }

    

    public void SetTargetObject(GameObject newTargetObject)
    {
        targetObject = newTargetObject;
    }
    
    
}