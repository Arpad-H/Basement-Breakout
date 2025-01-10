using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioExitIsBlocked : MonoBehaviour
{

   [SerializeField] private GameObject targetObject;
   [SerializeField] private GameObject beam1;
   [SerializeField] private GameObject beam2;
   [SerializeField] private AudioSource clip;
   
   private void OnTriggerEnter(Collider other)
   {
      if ((beam1 != null) && (beam2 != null) && (other.gameObject == targetObject))
      {
         clip.Play();
      }
      
   }

   private void OnTriggerExit(Collider other)
   {
      
   }
   
   private bool CheckIfExitIsBlocked()
   {
      return (beam1 == null) && (beam2 == null);
   }
}
