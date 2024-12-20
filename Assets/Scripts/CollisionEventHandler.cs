using UnityEngine;
using System;

public class CollisionEventHandler : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private Objecttype objecttypeselection;

    private CollisionEventHandler Instance;

    public static event Action<bool> OnWaterStateChangedCable;
    public static event Action<bool> OnWaterStateChangedPlayer;

    //private bool hasCollided  = false;

    private bool playerIsInWhater = false;
    private bool cableISinWater = false;


    private void Awake()
    {
        //Instance = this;
    }


    private void OnTriggerEnter(Collider other)
    {
        
        //Debug.Log("[CollisionEventHandler] Object entered: " + true + nameof(objecttypeselection));
        if (other.gameObject == targetObject)
        {
            eventsHandler(true);
            Debug.LogError($"[CollisionEventHandler]Object entered : {other.gameObject.name} { (objecttypeselection)}" + true);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == targetObject)
        {
            eventsHandler(false);
            Debug.LogError($"[CollisionEventHandler] Object exited: {other.gameObject.name} {objecttypeselection}" + false);
        }
    }


    public void SetTargetObject(GameObject newTargetObject)
    {
        targetObject = newTargetObject;
    }


    public enum Objecttype
    {
        Cable,
        Player
    }


    void eventsHandler(bool state)
    {
        switch (objecttypeselection)
        {
            case Objecttype.Cable:
                OnWaterStateChangedCable?.Invoke(state);
                break;
            case Objecttype.Player:
                OnWaterStateChangedPlayer?.Invoke(state);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(objecttypeselection), objecttypeselection, null);
        }
    }
}