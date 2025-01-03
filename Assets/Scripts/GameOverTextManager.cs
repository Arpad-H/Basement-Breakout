using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class GameOverTextManager : MonoBehaviour
{
    private TextMeshProUGUI text;
    
    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void Start()
    {
        text  = GetComponent<TextMeshProUGUI>();
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }
    
    
    private void OnGameStateChanged(GameManager.GameState obj)
    {
        Debug.Log($"[GameOverTextManager]: {obj}");
        switch (obj)
        {
            case GameManager.GameState.Drowned:
                showDeathMessage(composeDeathMessage(" Ertrinken"));
                break;
            case GameManager.GameState.ElectricShock:
                showDeathMessage(composeDeathMessage(" Elektroschock"));
                break;
            case GameManager.GameState.Win:
                showDeathMessage(composeWinMessage());
                text.color = Color.green;
                break;
            default:
                break;
        }
    }
    
    
    
    private void showDeathMessage(string message)
    {
        Debug.Log($"[GameOverTextManager]: {message}");
        text.text = message;
    }
    
    private string composeDeathMessage(string message)
    {
        return "Du bist gestorben an" + message + "! :(";
    }
    
    private string composeWinMessage()
    {
        return "Du hast alle Rätsel gelösst! :)";
    }
}
