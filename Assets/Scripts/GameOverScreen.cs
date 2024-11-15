using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class GameOverScreen : MonoBehaviour
{
    
    [SerializeField] private Transform head;
    [SerializeField] private float menuDistance;
    [SerializeField] private GameObject menu;
    [FormerlySerializedAs("reasonOfDeathText")] [SerializeField] private GameObject reasonOfDeath;
    public InputActionProperty showMenuButton;
    private bool gameOverScreen = false;
    private TextMeshProUGUI reasonOfDeathText;

    void Start()
    {
        if (reasonOfDeath != null)
        {
            reasonOfDeathText = reasonOfDeath.GetComponent<TextMeshProUGUI>();
            if (reasonOfDeathText == null)
            {
                Debug.LogError("TextMeshProUGUI component not found on GameObjekt reasonOfDeath.");
            }
        }
        else
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
        setText(reasonOfDeath);
    }


    private void setText(string reasonOfDeath)
    {
        reasonOfDeathText.SetText(reasonOfDeath);
    }
   
   
}
