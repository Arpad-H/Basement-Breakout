using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extinguishcandels : MonoBehaviour
{
    private bool extinguished = false;
    public GameObject candel1;
    public GameObject candel2;
    public GameObject candel3;

    public GameObject water;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (extinguished)
        {
            return;
        }
        if (water.transform.position.y > 4.35)
        {
            candel2.SetActive(false);
            candel1.SetActive(false);
            candel3.SetActive(false);
            extinguished = true;
        }
    }
}