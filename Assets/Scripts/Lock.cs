using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Oculus.Haptics;
using Oculus.Interaction;
using TMPro;
using UnityEngine;

public class Lock : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject tankDummy;
    [SerializeField] private GameObject tank;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private TMPro.TextMeshProUGUI text;
    [SerializeField] private Material green;
    [SerializeField] private GameObject LCD;
    
    [SerializeField] private AudioSource beep;
    [SerializeField] private AudioSource unlock;
    [SerializeField] private AudioSource wrong;
    
    
    //private GameObject key;
    private readonly string code = "0451";
    
    void Start() {
        
        
        tank.SetActive(false);
        //key = GameObject.Find("KeyInLock");
        //key.SetActive(false);
        //door.GetComponent<Grabbable>().enabled = false;
        door.SetActive(false);
        leftHand.SetActive(false);
        rightHand.SetActive(false);
    }

    public void keyPressed(char number) {
        int z = Convert.ToInt32(new string(number, 1));
        float x = z / 20f;
        beep.pitch = 0.8f + x;
        beep.Play();
        for (int i = 0; i < text.text.Length; i++) {
            if (text.text[i] == '-') {
                StringBuilder sb = new StringBuilder(text.text);
                sb[i] = number;
                text.text = sb.ToString();
                break;
            }
        }
    }
    
    public void keyDeleted() {
        text.text = "----";
        wrong.Play();
    }

    public void keyCheck() {
        if (text.text == code) {
            unlocked();
        }
        else {
            keyDeleted();
        }
    }
    
    private void unlocked() {
        unlock.Play();
        text.text = "OPEN";
        LCD.GetComponent<MeshRenderer>().material = green;
        door.SetActive(true);
        tank.SetActive(true);
        //key.SetActive(true);
        //door.GetComponent<Grabbable>().enabled = true;
        Destroy(tankDummy);
        //GetComponent<Rigidbody>().isKinematic = false;
        //Destroy(other);
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.layer == 21) {
            leftHand.SetActive(true);
        }

        if (other.gameObject.layer == 22) {
            rightHand.SetActive(true);
        }
    }
    
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == 21) {
            leftHand.SetActive(false);
        }

        if (other.gameObject.layer == 22) {
            rightHand.SetActive(false);
        }
    }
}
