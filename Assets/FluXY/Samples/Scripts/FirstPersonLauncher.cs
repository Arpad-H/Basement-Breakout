using UnityEngine;
using Fluxy;

namespace FluxySamples
{
    public class FirstPersonLauncher : MonoBehaviour
    {

        public GameObject prefab;
        public float power = 2;

        void Update()
        {

            if (Input.GetMouseButtonDown(0))
            {

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                GameObject projectile = Instantiate(prefab, ray.origin, Quaternion.identity);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.velocity = ray.direction * power;
                }

            }
        }
    }
}
