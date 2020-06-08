using Kubika.CustomLevelEditor;
using Kubika.Saving;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        public LevelCube[] worldMapLevels;

        RaycastHit hit;
        public LayerMask cubesMask;
        public Biomes currentBiome;

        public Transform topArrowObj;
        public Transform bottomArrowObj;

        public GameObject worldMap;
        GameObject activeFace;
        public Transform[] faces;

        public string nextLevelInProgression;

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

            worldMapLevels = FindObjectsOfType<LevelCube>();
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
            if (Physics.Raycast(Camera.main.ScreenPointToRay(LevelEditor.GetUserPlatform()), out hit, Mathf.Infinity, cubesMask))
            {
                LevelCube levelCube = hit.collider.gameObject.GetComponent<LevelCube>();
                if (levelCube != null)
                {
                    LevelsManager.instance.SelectLevel(levelCube.kubicode);
                }
            }
        }

        //use to focus camera on next level when worldmap loads in
        public void FocusOnNextLevel()
        {

        }

        public void UpdateWorldMap()
        {
            Debug.Log("Updating world map");

            //the progress file holds the next player's level
            nextLevelInProgression = SaveAndLoad.instance.LoadProgress().nextLevelKubicode;

            foreach (LevelCube cube in worldMapLevels)
            {
                if (cube.kubicode == nextLevelInProgression)
                    cube.GetComponent<_ScriptMatFaceCube>().isUnlocked = true;
            }
        }
    }
}
