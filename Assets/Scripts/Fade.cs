using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDistanceThreshold = 0.5f;
    [SerializeField] private float fadeSpeed = 2.0f;
    [SerializeField] public OVRCameraRig cameraRig;
    
    
    private float targetAlpha = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Color currentColor = fadeImage.color;
        currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
        fadeImage.color = currentColor;
    }


    private void OnCollisionStay(Collision other)
    {
        Debug.Log($"[Fade] On Coolision stay {other.gameObject.tag}");
        if (other.gameObject.CompareTag("GameBorder"))
        {
            Vector3 playerPos = cameraRig.centerEyeAnchor.position;
            Vector3 closetPoint = other.collider.ClosestPoint(playerPos);
            float distance = Vector3.Distance(closetPoint, playerPos);
            float fadeFacotor = 1f- Mathf.Clamp01(distance / fadeDistanceThreshold);
            targetAlpha = fadeFacotor;
            
        }
    }

    private void OnCollisionExit(Collision other)
    {
        Debug.Log($"[Fade] On Coolision exist {other.gameObject.tag}");
        if (other.gameObject.CompareTag("GameBorder"))
        {
            targetAlpha = 0f;
        }
    }
}
