using UnityEngine;
using System;

public class CollisionEventHandler : MonoBehaviour
{
    //wasser als targetobject
    [SerializeField] private GameObject targetObject;
    [SerializeField] private Objecttype objecttypeselection;

  
    private CollisionEventHandler _instance;

    public static event Action<bool> OnWaterStateChangedCable;
    public static event Action<bool> OnWaterStateChangedPlayer;

    //private bool hasCollided  = false;

    private bool _playerIsInWhater = false;
    private bool _cableISinWater = false;
    
    private bool _previousState = false;


    private void Awake()
    {
        //Instance = this;
    }

    private void Update()
    {
        

        bool currentState = CheckedCollisonAboveHeight();
    
        if (_previousState != currentState && objecttypeselection == Objecttype.Cable)
        {
            Debug.Log($"[CollisionEventHandler] curentstae: {currentState} previousstate: {_previousState}");;
            EventsHandler(currentState);
            _previousState = currentState;
        }
        
       
       
       
       
       
    }

    private void OnTriggerEnter(Collider other)
    {
        
        //Debug.Log("[CollisionEventHandler] Object entered: " + true + nameof(objecttypeselection));
        if (other.gameObject == targetObject)
        {
            EventsHandler(true);
            Debug.Log($"[CollisionEventHandler]Object entered : {other.gameObject.name} { (objecttypeselection)}" + true);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == targetObject)
        {
            EventsHandler(false);
            Debug.Log($"[CollisionEventHandler] Object exited: {other.gameObject.name} {objecttypeselection}" + false);
        }
    }

    private bool CheckedCollisonAboveHeight()
    {
        //Debug.Log($"[CollisionEventHandler] CheckedCollisonAboveHeight: {gameObject.transform.position.y < targetObject.transform.position.y}");
        return gameObject.transform.position.y < targetObject.transform.position.y;
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


    void EventsHandler(bool state)
    {
        Debug.Log($"[CollisionEventHandler] EventsHandler: : {state}");
        switch (objecttypeselection)
        {
            case Objecttype.Cable:
                OnWaterStateChangedCable?.Invoke(state);
                Debug.Log($"[CollisionEventHandler] EventsHandler: OnWaterStateChangedCable: {state}");
                break;
            case Objecttype.Player:
                OnWaterStateChangedPlayer?.Invoke(state);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(objecttypeselection), objecttypeselection, null);
        }
    }
}