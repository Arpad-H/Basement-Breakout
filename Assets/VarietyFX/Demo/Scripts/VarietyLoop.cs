using System;
using UnityEngine;
using System.Collections;

namespace VarietyFX
{
    public class VarietyLoop : MonoBehaviour
    {

        public GameObject chosenEffect;
        public float loopTimeLimit = 2.0f;

        // void Start()
        // {
        //     PlayEffect();
        // }
        

        public void PlayEffect()
        {
            StartCoroutine("EffectLoop");
        }


        IEnumerator EffectLoop()
        {
            while (true) // Infinite loop to keep the effect running
            {
                GameObject effectPlayer = Instantiate(chosenEffect);
                effectPlayer.transform.position = transform.position;

                Debug.Log(
                    "VarietyLoop: PlayEffect: effectPlayer.transform.position: " + effectPlayer.transform.position);

                yield return new WaitForSeconds(loopTimeLimit);

                Debug.Log("Destroying effectPlayer");
                Destroy(effectPlayer);
            }
        }
    }
}