using UnityEngine;

namespace URPWater
{
    public class DragCamera : MonoBehaviour
    {
        [SerializeField]
        float _DragSpeed = 25f;
        [SerializeField]
        float _NormalizedScale = 250f;

        float _Scale;


        void Update()
        {
            Vector3 pos = transform.position;

            _Scale = Camera.main.orthographicSize;

            if (Input.GetMouseButton(0))
            {
                pos.x -= Input.GetAxis("Mouse X") * _DragSpeed * _Scale / _NormalizedScale;
                //pos.z -= Input.GetAxis("Mouse Y") * dragSpeed * scale / normalizedScale;
            }

            transform.position = pos;
        }
    }
}