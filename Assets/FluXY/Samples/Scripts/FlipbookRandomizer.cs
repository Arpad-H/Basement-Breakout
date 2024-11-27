using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipbookRandomizer : MonoBehaviour
{

	public GameObject flipbook;
	public float radius = 5;
	public int amount = 10;

	private MaterialPropertyBlock mpb;

    void Start()
    {
        mpb = new MaterialPropertyBlock();
        for (int i = 0; i < amount; ++i)
        {
            var circle = Random.insideUnitCircle * radius;
            var instance = GameObject.Instantiate(flipbook, new Vector3(circle.x,transform.position.y,circle.y), Quaternion.identity);

            var rend = instance.GetComponent<MeshRenderer>();
            if (rend != null)
            {
                rend.GetPropertyBlock(mpb);
                mpb.SetFloat("_PlaybackOffset", Random.value * 2);
                mpb.SetFloat("_PlaybackSpeed", 0.5f + Random.value);
                rend.SetPropertyBlock(mpb);
            }
        }
    }
}
