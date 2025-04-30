using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MLand
{
    class ParticleManager : MonoBehaviour
    {
        List<GameObject> mParticles;

        public void Init()
        {
            mParticles = new List<GameObject>();
        }

        public GameObject Aquire(string name, Transform parent=null)
        {            
            var particle = MLand.ObjectPool.AcquireObject($"FX/{name}", parent);

            mParticles.Add(particle);
            
            return particle;
        }

        public GameObject Aquire(string name, Transform parent = null, Vector3? pos = null)
        {
            var particle = Aquire(name, parent);

            if ( pos != null )
            {
                particle.transform.localPosition = pos.Value;
            }

            return particle;
        }


        public void Release(GameObject go, Transform parent= null)
        {
            if (IsValid(go))
            {
                mParticles.Remove(go);

                MLand.ObjectPool.ReleaseObject(go);
            }
        }

        bool IsValid(GameObject go)
        {
            return go != null && 
                   go.GetComponent<ParticleSystem>() != null && 
                   mParticles.Contains(go);
        }


        void Update()
        {
            for(int i = 0; i < mParticles.Count; ++i)
            {
                var ps = mParticles[i].GetComponent<ParticleSystem>();
                if (IsValid(mParticles[i]) && ps.isPlaying == false)
                {
                    Release(mParticles[i], mParticles[i].GetComponentInParent<Transform>());

                    i--;
                }
            }
        }
    }
}
