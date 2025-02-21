using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.XR.CoreUtils;

public class CustomCollisionEventHandler : MonoBehaviour
{
    [SerializeField] private LayerMask layer;
    public static event Action<GameManager.GameState> GameStateChangedCostomCollisionEventHandler;
    public static event Action<VoiceOverManager.Item> audioWonByBoatWin;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == layer.GetFirstLayerIndex())
        {
            audioWonByBoatWin?.Invoke(VoiceOverManager.Item.WonByBoatWin);
            GameStateChangedCostomCollisionEventHandler?.Invoke(GameManager.GameState.Win);
        }
    }
}
