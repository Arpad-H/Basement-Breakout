using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderWaterVFX : MonoBehaviour
{
    [SerializeField] private Transform waterPos;
    [SerializeField] private Transform parent;
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    void Update()
    {
        if (parent.transform.position.y < waterPos.position.y)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
