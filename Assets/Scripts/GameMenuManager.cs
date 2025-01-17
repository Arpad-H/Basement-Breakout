using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class GameMenuManager : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private float menuDistance = 2f;
    [SerializeField] private GameObject menu;
    //public InputActionProperty showMenuButton;
    bool menuActive = true;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnOnGameStateChanged;
    }

    private void Start()
    {
        PositionMenuInFrontOfHead();
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnOnGameStateChanged;
    }

    private void OnBecameVisible()
    {
       
            //PositionMenuInFrontOfHead();
    }

    private void GameManagerOnOnGameStateChanged(GameManager.GameState obj)
    {
        if (obj is GameManager.GameState.Tutorial or GameManager.GameState.Game)
        {
            menuActive = false;
        }
        else
        {
            menuActive = true;
        }
    }
    

    void Update() {
        PositionMenuInFrontOfHead();
    }
    
    private void PositionMenuInFrontOfHead()
    {
        
            Vector3 newPosition = head.position + head.forward * menuDistance;
            menu.transform.position = newPosition;
            menu.transform.rotation = head.transform.rotation;
        
        
    }
    
    
}
