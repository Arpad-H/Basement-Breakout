using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    bool isOn = false;
    public GameObject spotlight;
    // Start is called before the first frame update
    void Start()
    {
        spotlight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleFlashlight()
    {
        Debug.Log("Toggle flashlight");
        isOn = !isOn;
        if (isOn)
        {
            spotlight.SetActive(true);
        }
        else
        {
            spotlight.SetActive(false);
        }
    }
    
        
}
