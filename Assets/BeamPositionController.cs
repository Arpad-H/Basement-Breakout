using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BeamPositionController : MonoBehaviour
{
    public GameObject beam1;
    public GameObject beam2;
    
    public AudioSource fallingsound;
    private void OnEnable()
    {
        
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }
    
    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }
    
    private void OnGameStateChanged(GameManager.GameState state)
    {
        
        switch (state)
        {
            case GameManager.GameState.Menu:
                beam1.transform.rotation = Quaternion.Euler(0, 0, 0);
                beam2.transform.rotation = Quaternion.Euler(0, 0, 0);
                

                
                beam1.transform.position = new Vector3(-2.67F, 3.45F, 4.3F);
                beam2.transform.position = new Vector3(-2.9F, 3.45F, 4.3F);
                
                break;
            case GameManager.GameState.Game:
                beam1.transform.rotation = Quaternion.Euler(30, 0, 0);
                beam2.transform.rotation = Quaternion.Euler(-30, 0, 0);
                
                beam1.transform.position = new Vector3(-2.67F, 3.45F, 3.9F);
                beam2.transform.position = new Vector3(-2.9F, 3.47F, 3.9F);
                
                fallingsound.Play();
                

                
                break;

        }
    }
    

}
