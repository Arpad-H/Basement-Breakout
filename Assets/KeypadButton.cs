using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeypadButton : MonoBehaviour
{
    [SerializeField] private char number;
    [SerializeField] private UnityEvent<char> pressed;
    private bool once = true;
    [SerializeField] private GameObject button;
    [SerializeField] private Material keypadRegular;
    [SerializeField] private Material keypadPressed;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 23)
        {
            button.transform.localPosition = new Vector3(-0.01f, button.transform.localPosition.y, button.transform.localPosition.z);
            if (once)
            {
                pressed.Invoke(number);
                button.GetComponent<Renderer>().material = keypadPressed;
                once = false;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    { 
        //if (other.gameObject.layer == 23)
        {
            button.transform.localPosition = new Vector3(0, button.transform.localPosition.y, button.transform.localPosition.z);
            button.GetComponent<Renderer>().material = keypadRegular;
            once = true;
        }
    }
}
