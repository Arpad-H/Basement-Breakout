using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.XR.CoreUtils;

public class CustomCollisionEventHandler : MonoBehaviour
{
    [SerializeField] private LayerMask layer;
    public static event Action<GameManager.GameState> gameStateChanged;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == layer.GetFirstLayerIndex())
        {
            gameStateChanged?.Invoke(GameManager.GameState.Win);
        }
    }
}
