using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEditor;
using UnityEngine;

public class VoiceOverEventSender : MonoBehaviour
{
    [SerializeField] public VoiceOverManager.Item item;
    [SerializeField] public ColliderOrGrabbable type;

    [Tooltip("The viewing area for the library, fridge and dartboard")] [SerializeField]
    public float minAngleDaFrLi = 115f;
    [SerializeField] public float maxAngleDaFrLi = 250f;

    [SerializeField] public float minAngleWorkbench = 190f;
    [SerializeField] public float maxAngleWorkbench = 290f;

    private GameObject _targetCollider;
    private bool _isColliding = false;
    // private bool _wasPlayed = false;

    public static event Action<VoiceOverManager.Item> OnAction;


    private void Start()
    {
        _targetCollider = GameObject.Find("CenterEyeAnchor");
    }

    public enum ColliderOrGrabbable
    {
        GRABBABLE,
        COLLIDER
    }

    private void Update()
    {
        // Debug.Log($"[VoiceOverEventSender]  Player rotation  {_targetCollider.transform.rotation.eulerAngles}");
        if (_isColliding)
        {
            Debug.Log($"[VoiceOverEventSender] update Player rotatio y {_targetCollider.transform.rotation.eulerAngles.y} minAngle {minAngleDaFrLi} maxAngle {maxAngleDaFrLi}");
            switch (item)
            {
                case VoiceOverManager.Item.Dartboad:
                case VoiceOverManager.Item.Fridge:
                case VoiceOverManager.Item.Library:
                    SendEvent(minAngleDaFrLi, maxAngleDaFrLi, _targetCollider.transform.rotation.y);
                    break;
                case VoiceOverManager.Item.Workbench:
                    SendEvent(minAngleWorkbench, maxAngleWorkbench, _targetCollider.transform.rotation.y);
                    break;
                default:
                    Debug.Log($"[VoiceOverEventSender] Unknown Item: {item}] not found");
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (type == ColliderOrGrabbable.COLLIDER && other.gameObject == _targetCollider)
        {
            _isColliding = true;
            Debug.Log(
                $"[VoiceOverEventSender] OnTriggerEnter:  {other.gameObject.name} item: {item.ToString()} GameObject: {gameObject.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (type == ColliderOrGrabbable.COLLIDER && other.gameObject == _targetCollider)
        {
            _isColliding = false;
            Debug.Log(
                $"[VoiceOverEventSender] Event send OnTriggerExit:  {other.gameObject.name} item: {item.ToString()} GameObject: {gameObject.name}");
        }
    }


    private void SendEvent(float minAngle, float maxAngle, float angle)
    {
        Debug.Log($"[VoiceOverEventSender] Sending event: {item.ToString()} Im Blickfeld: {minAngle > angle && maxAngle > angle}");
        if (minAngle > angle && maxAngle > angle)
        {
            OnAction?.Invoke(item);
        }
    }

    public void OnGrab()
    {
        if (type == ColliderOrGrabbable.GRABBABLE)
        {
            Debug.Log($"[VoiceOverEventSender] Grabbing item: {item.ToString()}  GameObject: {gameObject.name}");
            OnAction?.Invoke(item);
        }
    }
}