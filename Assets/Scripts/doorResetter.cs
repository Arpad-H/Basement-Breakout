using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorResetter : MonoBehaviour
{

    void Awake()
    {
        GameManager.OnGameStateChanged += doorShut;
    }
    void OnDestroy()
    {
        GameManager.OnGameStateChanged -= doorShut;
    }
    
    private void doorShut(GameManager.GameState state)
    {
        if (state != GameManager.GameState.Game || state != GameManager.GameState.Tutorial)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }
}
