using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    private GameObject[] gameObjects;
    private Outline[] outlines;
    
    void Start()
    {
        gameObjects = GameObject.FindGameObjectsWithTag("Outline");
        outlines = new Outline[gameObjects.Length];
        int i = 0;
        foreach(GameObject gameObject in gameObjects)
        {
            if (gameObject.GetComponent<Outline>() != null)
            {
                outlines[i] = gameObject.GetComponent<Outline>();
                outlines[i].enabled = false;
                ++i;
            }
        }
        GameManager.OnGameStateChanged += showOutlines;
    }

    private void enable()
    {
        for (int i = 0; i < outlines.Length; ++i)
        {
            outlines[i].enabled = true;
        }
    }

    private void disable()
    {
        for (int i = 0; i < outlines.Length; ++i)
        {
            outlines[i].enabled = false;
        }
    }
    
    private void showOutlines(GameManager.GameState state)
    {
        if (state == GameManager.GameState.Tutorial)
        {
            enable();
        }
        else if(state != GameManager.GameState.Tutorial || state == GameManager.GameState.Game)
        {
            disable();
        }
    }
}
