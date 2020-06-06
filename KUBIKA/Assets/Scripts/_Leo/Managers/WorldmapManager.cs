using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kubika.Game
{
    //[InitializeOnLoad]
    //[CanEditMultipleObjects]
    public class WorldmapManager : MonoBehaviour
    {
        private static WorldmapManager _instance;
        public static WorldmapManager instance { get { return _instance; } }

        public List<GameObject> levelCubes = new List<GameObject>();

        private Biomes currentBiome;
        private GameObject activeFace;

        public Transform[] faces;

        private GameObject worldMapFace;

        //[RuntimeInitializeOnLoadMethod]
        private void Awake()
        {
            if (_instance != null && _instance != this) Destroy(this);
            else _instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            faces = new Transform[(int)Biomes.Count];

            for (int i = 0; i < faces.Length; i++)
            {
                currentBiome = (Biomes)i;
                faces[i] = GameObject.Find(currentBiome.ToString()).transform;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        //use to focus camera on next level when worldmap loads in
        public void FocusOnNextLevel()
        {

        }
    }
}
