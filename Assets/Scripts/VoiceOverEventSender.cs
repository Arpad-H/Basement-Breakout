using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class VoiceOverEventSender : MonoBehaviour
{
    [SerializeField] public VoiceOverManager.Item item;
    [SerializeField] public ColliderOrGrabbable type;
    private GameObject _targetCollider;
    // private bool _wasPlayed = false;
    
    public static event Action<VoiceOverManager.Item> OnLeverAction;


    private void Start()
    {
        _targetCollider = GameObject.Find("Player");
    }

    public enum ColliderOrGrabbable
    {
        GRABBABLE, COLLIDER
    }

    private void OnTriggerEnter(Collider other)
    {
        if (type == ColliderOrGrabbable.COLLIDER  && other.gameObject == _targetCollider)
        {
            OnLeverAction?.Invoke(item);
           
        }
    }

    public void OnGrab()
    {
        if (type == ColliderOrGrabbable.GRABBABLE)
        {
            Debug.Log("[VoiceOverEventSender] Grabbing");
           OnLeverAction?.Invoke(item);
          
        }
    }
    
    
    
}
