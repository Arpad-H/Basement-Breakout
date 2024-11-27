using UnityEngine;
using System;
using System.Collections.Generic;

namespace Fluxy
{
    [AddComponentMenu("Physics/FluXY/TargetProviders/Target Detector", 800)]
    public class FluxyTargetDetector : FluxyTargetProvider
    {
        public Vector3 size = new Vector3(0.5f, 0.5f, 0.5f);
        public int maxColliders = 32;
        public LayerMask layers = ~0;

        private Collider[] colliders = new Collider[0];
        private List<FluxyTarget> targets = new List<FluxyTarget>();

        public void OnValidate()
        {
            Array.Resize(ref colliders, maxColliders);
        }

        public void Awake()
        {
            Array.Resize(ref colliders, maxColliders);
        }

        public override List<FluxyTarget> GetTargets()
        {
            targets.Clear();
            int targetCount = Physics.OverlapBoxNonAlloc(transform.position, size * 0.5f, colliders, Quaternion.identity, layers);

            for (int i = 0; i < targetCount; ++i)
            {
                if (colliders[i].TryGetComponent(out FluxyTarget target))
                    targets.Add(target);
            }

            return targets;
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.5f,0.8f,1,0.5f);
            Gizmos.DrawWireCube(transform.position, size);
        }
    }
}
