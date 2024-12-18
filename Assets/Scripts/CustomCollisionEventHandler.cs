using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.XR.CoreUtils;

public class CustomCollisionEventHandler : MonoBehaviour
{
    [SerializeField] private LayerMask layer;
    public static event Action<GameManager.GameState> GameStateChangedCostomCollisionEventHandler;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == layer.GetFirstLayerIndex())
        {
            GameStateChangedCostomCollisionEventHandler?.Invoke(GameManager.GameState.Win);
        }
    }
}
