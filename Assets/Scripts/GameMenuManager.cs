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
    [SerializeField] private GameObject environmentParent;
    //public InputActionProperty showMenuButton;
    bool menuActive = true;

    private void Awake()
    {
        GameManager.OnGameStateChanged += GameManagerOnOnGameStateChanged;
    }

    private void Start()
    {
        if (menutype == Menutype.GameOverMenu)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
        
        RotateEnvironment(head, environmentParent.transform);
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
        else if (Menutype.GameOverMenu == menutype)
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

    public void RotateEnvironment(Transform head, Transform environmentParent)
    {
        environmentParent.Rotate(0, -1*head.rotation.eulerAngles.y, 0);
    }
    
    public void CenterEnvironment(Transform head, Transform environmentParent, Transform targetPosition)
    {
        environmentParent.position += targetPosition.position - head.position;
    }
    
    
}
