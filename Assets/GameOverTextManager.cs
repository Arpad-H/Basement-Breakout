using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class GameOverTextManager : MonoBehaviour
{
    
    
    private void Awake()
    {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }
    
    
    private void OnGameStateChanged(GameManager.GameState obj)
    {
        switch (obj)
        {
            case GameManager.GameState.Drowned:
                showDeathMessage(composeDeathMessage("Ertrinken"));
                break;
            case GameManager.GameState.ElectricShock:
                showDeathMessage(composeDeathMessage("Elektroschock"));
                break;
            case GameManager.GameState.Win:
                showDeathMessage(composeWinMessage());
                break;
            default:
                break;
        }
    }
    
    
    
    private void showDeathMessage(string message)
    {
        TextMeshPro text = GetComponent<TextMeshPro>();
        text.SetText(message);
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
