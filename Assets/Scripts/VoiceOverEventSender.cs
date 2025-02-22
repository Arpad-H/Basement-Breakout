using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class VoiceOverEventSender : MonoBehaviour
{
    [SerializeField] public VoiceOverManager.Item item;
    [SerializeField] public ColliderOrGrabbable type;
    private Collider _collider;
    private  float viewAngleThreshold = 70f;
    private float distanceThreshold = 0f;

    private GameObject _targetCollider;
    private bool _isColliding = false;
    // private bool _wasPlayed = false;

    public static event Action<VoiceOverManager.Item> OnAction;


    private void Start()
    {
        _targetCollider = GameObject.Find("CenterEyeAnchor");
        _collider = GetComponent<BoxCollider>();
    }

    public enum ColliderOrGrabbable
    {
        Grabbable,
        Collider,
    }

    private void Update()
    {
        
        
        if (type == ColliderOrGrabbable.Collider)
        {
            Debug.Log($"[VoiceOverEventSender] update  im Blickfeld und Distanze: {CheckObjectInFieldOfView(transform, _targetCollider.transform, viewAngleThreshold, distanceThreshold)} // Angle: {viewAngleThreshold} // Distance:{distanceThreshold} // Item {item.ToString()}");
            if (CheckObjectInFieldOfView(transform, _targetCollider.transform, viewAngleThreshold, distanceThreshold))
            {
                OnAction?.Invoke(item);
            }
           
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (type == ColliderOrGrabbable.Collider && other.gameObject == _targetCollider)
        {
            _isColliding = true;
            // Debug.Log(
            //     $"[VoiceOverEventSender] OnTriggerEnter:  {other.gameObject.name} item: {item.ToString()} GameObject: {gameObject.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (type == ColliderOrGrabbable.Collider && other.gameObject == _targetCollider)
        {
            _isColliding = false;
            // Debug.Log(
            //     $"[VoiceOverEventSender] Event send OnTriggerExit:  {other.gameObject.name} item: {item.ToString()} GameObject: {gameObject.name}");
        }
    }

    public void OnGrab()
    {
        if (type == ColliderOrGrabbable.Grabbable)
        {
            Debug.Log($"[VoiceOverEventSender] Grabbing item: {item.ToString()}  GameObject: {gameObject.name}");
            OnAction?.Invoke(item);
        }
    }


    private bool CheckObjectInFieldOfView(Transform objecTransform, Transform player, float viewAngleThreshold, float distanceThreshold)
    {
        Vector3 directionToObject = (objecTransform.position - player.position).normalized;
       float distanceToObject = (player.position- _collider.ClosestPoint(player.position)).magnitude;
       
        float angle = Vector3.Angle(player.forward, directionToObject);
        Debug.Log($"[VoiceOverEventSender] Checking object: {directionToObject} // angle: {angle} // distance: {distanceToObject}");
        return angle <= viewAngleThreshold && distanceToObject <= distanceThreshold;
    }
}