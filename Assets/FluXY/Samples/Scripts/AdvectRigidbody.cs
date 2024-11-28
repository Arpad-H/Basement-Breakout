using UnityEngine;
using Fluxy;

namespace FluxySamples
{
    [RequireComponent(typeof(Rigidbody))]
    public class AdvectRigidbody : MonoBehaviour
    {
        public FluxyContainer container;
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            rb.velocity = container.GetVelocityAt(rb.position);
        }
    }
}
