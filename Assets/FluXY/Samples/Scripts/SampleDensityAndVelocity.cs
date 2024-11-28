using UnityEngine;
using Fluxy;

namespace FluxySamples
{
    public class SampleDensityAndVelocity : MonoBehaviour
    {
        public FluxyContainer container;

        Renderer rend;

        void Awake()
        {
            rend = GetComponentInChildren<Renderer>();
        }

        void FixedUpdate()
        {
            var velocity = container.GetVelocityAt(transform.position);
            var density = container.GetDensityAt(transform.position);

            transform.rotation = Quaternion.LookRotation(velocity);
            rend.material.color = density;
        }
    }
}
