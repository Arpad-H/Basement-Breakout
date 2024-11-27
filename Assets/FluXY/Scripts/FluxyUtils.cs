using UnityEngine;

namespace Fluxy
{
    public static class FluxyUtils
    {
        public const float epsilon = 0.00001f;

        public static float RelativeScreenHeight(Camera camera, float distance, float size)
        {
            if (camera.orthographic)
                return size * 0.5F / camera.orthographicSize;

            var halfAngle = Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView * 0.5F);
            var relativeHeight = size * 0.5F / (distance * halfAngle);
            return relativeHeight;
        }
    }
}
