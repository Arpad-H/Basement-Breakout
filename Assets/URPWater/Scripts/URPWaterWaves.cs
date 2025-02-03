using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace URPWater {
    [CreateAssetMenu(fileName = "WavesDefinition", menuName = "URPWater/WavesDefinition", order = 1)]
    public class URPWaterWaves : ScriptableObject
    {

        public List<URPWaterWaveDefinition> Waves = new List<URPWaterWaveDefinition>();

        private void OnValidate()
        {
           if(Waves.Count > 4)
            {
                Waves.RemoveAt(3);
            }
        }

    }
}