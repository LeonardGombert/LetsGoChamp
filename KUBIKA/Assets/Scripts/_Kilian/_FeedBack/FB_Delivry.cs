using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubika.Game
{
    public class FB_Delivry : MonoBehaviour
    {
        public ParticleSystem PS;
        public Transform pivotPS;

        public void ActivatePSFB()
        {
            PS.Play();
        }
    }
}
