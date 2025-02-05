using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URPWater
{
    [System.Serializable]
    public class URPWaterWaveDefinition //: ISerializationCallbackReceiver
    {
        public float WaveLength = 5.0f;
        [Range(0.0f, 2.0f)]
        public float Steepness = 0.5f;
        public float Speed = 0.5f;
        [Range(0.0f, 1.0f)]
        public float Direction = 0.0f;
        public float Offset = 1.5f;

        /*
        private bool _Deserialized = false;

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            if (_Deserialized)
            {
                return;
            }

            
            this.WaveLength = 5.0f;
            this.Steepness = 0.5f;
            this._Deserialized = true;
            
        }
        */

    }
}