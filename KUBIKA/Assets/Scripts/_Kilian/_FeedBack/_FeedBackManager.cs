using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubika.Game
{
    public class _FeedBackManager : MonoBehaviour
    {
        public GameObject Fb_Delivry;

        private static _FeedBackManager _instance;
        public static _FeedBackManager instance { get { return _instance; } }

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
