using UnityEngine;
using Fluxy;

namespace FluxySamples
{

    public class MouseInteraction : MonoBehaviour
    {
        FluxyTarget target;

        private void Awake()
        {
            target = GetComponent<FluxyTarget>();
        }

        private void Update()
        {
            if (Camera.main == null) return;

            // randomize paint color:
            if (Input.GetMouseButtonDown(0))
                target.color = Random.ColorHSV(0, 1, 0.6f, 1, 1, 1, 1, 1);

            // when the mouse is pressed, enable target and change its position. 
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                transform.position = ray.origin + ray.direction;
                target.enabled = true;
            }
            // otherwise disable target.
            else
            {
                target.enabled = false;
            }
        }
    }

}