using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubika.Game
{
    public class _FeedBackManager : MonoBehaviour
    {
        private static _FeedBackManager _instance;
        public static _FeedBackManager instance { get { return _instance; } }

        public GameObject Fb_Delivry;

        //EXPLOSION FX
        [Space]
        [Header("EXPLOSION FX")]

        public ParticleSystem MineExplosionBase;
        public ParticleSystem MineExplosionVictory;
        public ParticleSystem PopOutParticleSystem;

        private void Awake()
        {
            if (_instance != null && _instance != this) Destroy(this);
            else _instance = this;

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
