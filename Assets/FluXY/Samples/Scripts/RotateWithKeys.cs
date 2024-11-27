using UnityEngine;

namespace FluxySamples
{
    public class RotateWithKeys : MonoBehaviour
    {

        public float speed = 20;

        [Range(0, 1)]
        public float angularDrag = 0.8f;


        private float angularAccel = 0;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.A))
            {
                angularAccel = Time.deltaTime * -speed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                angularAccel = Time.deltaTime * speed;
            }
            angularAccel *= Mathf.Pow(1 - angularDrag, Time.deltaTime);
            transform.Rotate(Vector3.forward, angularAccel);
        }
    }
}
