using Kubika.CustomLevelEditor;
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

        RaycastHit hit;
        public Biomes currentBiome;

        public Transform topArrowObj;
        public Transform bottomArrowObj;

        public GameObject worldMap;
        GameObject activeFace;
        public Transform[] faces;


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

            currentBiome = Biomes.Plains;
        }

        // Update is called once per frame
        void Update()
        {
            RefreshWorldArrowTargets();
            UpdateArrowPositions();
            CheckForNodeTouch();
        }

        public void RefreshWorldArrowTargets()
        {
            activeFace = worldMap.transform.GetChild(1).transform.GetChild(0).transform.GetChild((int)currentBiome).gameObject;

            topArrowObj = activeFace.transform.GetChild(3).GetChild(0).transform;
            bottomArrowObj = activeFace.transform.GetChild(3).GetChild(1).transform;
        }

        void UpdateArrowPositions()
        {
            UIManager.instance.topArrow.rectTransform.position = Camera.main.WorldToScreenPoint(topArrowObj.position);
            UIManager.instance.bottomArrow.rectTransform.position = Camera.main.WorldToScreenPoint(bottomArrowObj.position);
        }

        private void CheckForNodeTouch()
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(LevelEditor.GetUserPlatform()), out hit))
            {
                LevelCube levelCube = hit.collider.gameObject.GetComponent<LevelCube>();
                if (levelCube != null) LevelsManager.instance.SelectLevel(levelCube.kubicode);
            }
        }

        //use to focus camera on next level when worldmap loads in
        public void FocusOnNextLevel()
        {

        }
    }
}
