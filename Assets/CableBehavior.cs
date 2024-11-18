using UnityEngine;
using System;

public class CollisionEventHandler : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;


    public event Action<Collider> OnObjectEntered;
    public event Action<Collider> OnObjectExited;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == targetObject)
        {
            OnObjectEntered?.Invoke(other);
            Debug.Log($"Object entered: {other.gameObject.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == targetObject)
        {
            OnObjectExited?.Invoke(other);

            Debug.Log($"Object exited: {other.gameObject.name}");
        }
    }

    public void SetTargetObject(GameObject newTargetObject)
    {
        targetObject = newTargetObject;
    }
}