using UnityEngine;

[RequireComponent(typeof(Light))]
public class RotateLight : MonoBehaviour
{
    Light l;

    void Awake()
    {
        l = GetComponent<Light>();
    }

    public void SetRotation(float angle)
    {
        var rot = l.transform.localRotation;
        var euler = rot.eulerAngles;
        euler.y = angle;
        rot.eulerAngles = euler;
        l.transform.localRotation = rot;
    }
}
