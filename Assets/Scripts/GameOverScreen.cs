using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    
    [SerializeField] private Transform head;
    [SerializeField] private float menuDistance;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject reasonOfDeathText;
    public InputActionProperty showMenuButton;
    private bool gameOverScreen = false;

    void Start()
    {
        if (reasonOfDeathText == null)
        {
            Debug.LogError("GameObject with TextMeshProUGUI component not found");
        }
        
    }
    
    void Update()
    {
        if (gameOverScreen) {
            menu.SetActive(true);
            menu.transform.position = head.position + new Vector3(head.forward.x, head.forward.y, head.forward.z).normalized * menuDistance;
            gameOverScreen = false;
        }
        menu.transform.LookAt(new Vector3(head.position.x, menu.transform.position.y, head.position.z));
        menu.transform.forward *= -1;
    }


   public void showGameOverScreen(string reasonOfDeath)
    {
        Debug.Log("Game Over Screen: " + reasonOfDeath);
        gameOverScreen = true;
    }


    private void setText(string reasonOfDeathText)
    {
        
    }
   
   
}
