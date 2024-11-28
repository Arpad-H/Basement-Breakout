using UnityEngine;
using Fluxy;

[RequireComponent(typeof(FluxyTarget))]
public class SetRandomForce : MonoBehaviour
{
    FluxyTarget target;

    void Start()
    {
        target = GetComponent<FluxyTarget>();
    }

    void Update()
    {
        float x = Mathf.PerlinNoise(Time.time, 0)*2-1;
        float y = Mathf.PerlinNoise(Time.time, 0.5f)*2-1;
        float z = Mathf.PerlinNoise(Time.time, 1)*2-1;

        target.force = new Vector3(x, y, z);

        float r = Mathf.PerlinNoise(Time.time, 0.25f);
        float g = Mathf.PerlinNoise(Time.time, 0.75f);
        float b = Mathf.PerlinNoise(Time.time, 0.8f);

        target.color = new Color(r, g, b);
    }
}
