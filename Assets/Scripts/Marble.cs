using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class Marble : MonoBehaviour
{
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform centerTransform;

    public void Update()
    {
        if (centerTransform == null)
        {
            gameObject.SetActive(false);
        }
        float length = (new Vector2(centerTransform.position.x, centerTransform.position.z) - new Vector2(transform.position.x, transform.position.z)).magnitude;
        if (length > 0.2f)
        {
            transform.position = startTransform.position;
        }
    }
}
