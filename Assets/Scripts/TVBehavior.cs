using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TVBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject VideoQuad;
    [SerializeField] private VideoClip[] clips;
    private VideoClip currentClip;
    void Start()
    {
        currentClip = clips[0];
        VideoQuad.GetComponent<VideoPlayer>().clip = currentClip;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeClip()
    {
        // add debug message
        Debug.Log("Changing Clip");
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == currentClip)
            {
                if (i == clips.Length - 1)
                {
                    Debug.Log("Last Clip");
                    currentClip = clips[0];
                    updateClipOnQuad();
                }
                else
                {
                    Debug.Log("Next Clip");
                    currentClip = clips[i + 1];
                    updateClipOnQuad();
                }
                break;
            }
        }
    }
    
    public void updateClipOnQuad()
    {
        VideoQuad.GetComponent<VideoPlayer>().clip = currentClip;
    }
}
