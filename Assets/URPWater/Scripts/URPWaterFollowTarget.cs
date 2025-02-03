using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace URPWater
{
    [RequireComponent(typeof(URPWaterDynamicEffects))]
    public class URPWaterFollowTarget : MonoBehaviour
    {

        [SerializeField]
        [Tooltip("Transform of the object that the Dynamic Effect Capture should follow.")]
        private Transform _Target = null;

        [SerializeField]
        [Tooltip("Offset Y position of the Target.")]
        private float _YOffset = 0f;

        private URPWaterDynamicEffects _DynamicEffect;


        void OnEnable()
        {
            if(_Target == null)
            {
                enabled = false;
                return;
            }

            if(_DynamicEffect == null)
            {
                _DynamicEffect = GetComponent<URPWaterDynamicEffects>();
            }
            
        }


        void Update()
        {

            if(_Target != null && _DynamicEffect != null)
            {
                var newPos = _Target.position;
                newPos.y += _DynamicEffect.CaptureDistance * 0.5f;
                newPos.y += _YOffset;

                transform.position = newPos;
            }
        }
    }
}