using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustPostoWaterHeight : MonoBehaviour
{
    [SerializeField] private GameObject objectToAdjust;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AdjustHeight(float height)
    {
        Debug.Log("Adjusting height to " + height);
        objectToAdjust.transform.position = new Vector3(objectToAdjust.transform.position.x, height, objectToAdjust.transform.position.z);
    }
}
