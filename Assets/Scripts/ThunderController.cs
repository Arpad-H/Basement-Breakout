using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderController : MonoBehaviour
{
    public GameObject lightning1;
    public GameObject lightning2;
    public GameObject lightning3;

    //public GameObject audio;
    void Start()
    {
        lightning1.SetActive(false);
        lightning2.SetActive(false);
        lightning3.SetActive(false);
        //audio.SetActive(false);

        Invoke("CallLightning", 1.75f);
    }
    void Update()
    {
        
    }

    void CallLightning(){
        int r = Random.Range(0,3);
        switch(r){
            case 0: 
            lightning1.SetActive(true);
            Invoke("EndLightning", .125f);
            break;
            case 1: 
            lightning2.SetActive(true);
            Invoke("EndLightning", .105f);
            break;
            case 2: 
            lightning3.SetActive(true);
            Invoke("EndLightning", .75f);
            break;
        }
        float rand = Random.Range(0.5f,10f);
        Invoke("CallLightning", rand);
    }

    void EndLightning(){
        lightning1.SetActive(false);
        lightning2.SetActive(false);
        lightning3.SetActive(false);
    }

    void CallThunder(){}

    void EndThunder(){}
}
