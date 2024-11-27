using UnityEngine;

public class Wiggle : MonoBehaviour
{
    public Vector3 axis = Vector3.up;
    public float amplitude = 1;
    public float speed = 1;
    public float offset = 0;

    Vector3 initialPos;

    void Start()
    {
        initialPos = transform.position;
    }

    void Update()
    {
        float pos = Mathf.Sin(offset + Time.time * speed) * amplitude;
        transform.position = initialPos + axis.normalized * pos;
    }
}
