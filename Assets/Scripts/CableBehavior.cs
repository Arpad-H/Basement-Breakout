using UnityEngine;
using System;

public class CableBehavior : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;

    private CableBehavior Instance;
    public event Action<Collider> OnObjectEntered;
    public event Action<Collider> OnObjectExited;
    
    public static event Action<bool> OnWaterStateChanged;
    
    private bool isInWhater  = false;


    private void Awake()
    {
        Instance = this;
    }
    


    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject == targetObject)
        {
            isInWhater = true;
            OnWaterStateChanged?.Invoke(isInWhater);
            Debug.Log($"Object entered: {other.gameObject.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == targetObject)
        {
            isInWhater = false;
            
            OnWaterStateChanged?.Invoke(isInWhater);
            Debug.Log($"Object exited: {other.gameObject.name}");
        }
    }

    

    public void SetTargetObject(GameObject newTargetObject)
    {
        targetObject = newTargetObject;
    }
    
    
}