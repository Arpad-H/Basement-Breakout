using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    
    [SerializeField] private OVRScreenFade fader;
    private bool _isInPlayArea = true;


    // private void OnCollisionStay(Collision other)
    // {
    //     Debug.Log($"[Fade] On Coolision stay {other.gameObject.tag}");
    //     
    // }
    //
    // private void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log($"[Fade] On Trigger stay {other.gameObject.tag}");
    //     if (other.gameObject.CompareTag("GameBorder"))
    //     {
    //         fader.FadeOut();
    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"[Fade] On Trigger stay {other.gameObject.tag}");
        if (other.gameObject.CompareTag("PlayArea"))
        {
            if (_isInPlayArea)
            {
                Debug.Log($"[Fade] On Trigger stay Fade in");
                _isInPlayArea = false;
                fader.FadeOut();
                
            }
        }
    }


    private void Update()
    {
        // if (!_isInPlayArea)
        // {
        //     //Debug.Log($"[Fade] update Fade out");
        //     _isInPlayArea = false;
        //     // fader.FadeOut();
        // }
        // else
        // {
        //     
        // }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"[Fade] On Trigger exit {other.gameObject.tag}");
        if (other.gameObject.CompareTag("PlayArea"))
        {
            if (!_isInPlayArea)
            {
                _isInPlayArea = true;

                fader.FadeIn();
            }
        }
    }

    // private void OnTriggerExit(Collider other)
    // {
    //     Debug.Log($"[Fade] On Trigger exit {other.gameObject.tag}");
    //     if (other.gameObject.CompareTag("GameBorder"))
    //     {
    //         fader.FadeIn();
    //     }
    // }
    //
    // private void OnCollisionExit(Collision other)
    // {
    //     Debug.Log($"[Fade] On Coolision exist {other.gameObject.tag}");
    //     
    // }
}
