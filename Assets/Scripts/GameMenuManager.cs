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
    [SerializeField] private Menutype menutype;
    //public InputActionProperty showMenuButton;
    bool menuActive = true;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnOnGameStateChanged;
    }

    private void Start()
    {
        if (menutype is Menutype.GameOverMenu)
        {
            menu.SetActive(false);
        }
        else
        {
            menu.SetActive(true);
        }
        //PositionMenuInFrontOfHead();
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnOnGameStateChanged;
    }

    private void OnBecameVisible()
    {
        StartCoroutine(WaitTobeVisible());
    }

    private IEnumerator WaitTobeVisible()
    {
        yield return new WaitForSeconds(20);
        //PositionMenuInFrontOfHead();
    }

    private void GameManagerOnOnGameStateChanged(GameManager.GameState obj)
    {
        if (obj is GameManager.GameState.Tutorial or GameManager.GameState.Game)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
    

    void Update() {
        if (gameObject.activeSelf)
        {
            PositionMenuInFrontOfHead();
        }
        
    }
    
    private void PositionMenuInFrontOfHead()
    {
        
            Vector3 newPosition = head.position + head.forward * menuDistance;
            menu.transform.position = newPosition;
            menu.transform.rotation = head.transform.rotation;
        
        
    }
    
    public enum Menutype
    {
        StartMenu,
        GameOverMenu
    }
    
    
}
