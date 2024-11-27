using UnityEngine;
using Fluxy;

[RequireComponent(typeof(FluxyTarget))]
public class RotateTarget : MonoBehaviour
{
    
    public float speed = 1;

    FluxyTarget target;

    void Start()
    {
        target = GetComponent<FluxyTarget>();
    }

    void Update()
    {
        target.rotation += speed * Time.deltaTime * Mathf.Rad2Deg;
    }
}
