using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class GameMenuManager : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private float menuDistance;
    [SerializeField] private GameObject menu;
    public InputActionProperty showMenuButton;
    bool menuActive = true;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnOnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnOnGameStateChanged;
    }

    private void GameManagerOnOnGameStateChanged(GameManager.GameState obj)
    {
        if (obj== GameManager.GameState.Menu)
        {
            menuActive = true;
        }
    }
    //TODO: richtigen Zeitpunkt finden, wann der Spieler auch wirklich das menü sieht
    private void OnEnable()
    {
        menuActive = true;
    }


    void Update() {
        //TODO: Funktiniert nicht
        if (showMenuButton.action.WasPressedThisFrame() || menuActive) {
            //menu.SetActive(!menu.activeSelf);
            Debug.Log($"GameMenuManager : {menuActive}");
           // menu.transform.position = head.position + new Vector3(head.forward.x, head.forward.y, head.forward.z).normalized * menuDistance;
             menu.transform.position = head.position + head.forward * menuDistance;
        }
        
      
        menu.transform.LookAt(new Vector3(head.position.x, menu.transform.position.y, head.position.z));
        menu.transform.forward *= -1;
        menuActive = false;
    }
}
