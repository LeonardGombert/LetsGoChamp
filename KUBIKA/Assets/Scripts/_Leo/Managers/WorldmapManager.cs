﻿using Kubika.CustomLevelEditor;
using Kubika.Saving;
using System.Collections.Generic;
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

        public List<string> playerBeatenLevels = new List<string>();
        public List<string> playerGoldenLevels = new List<string>();

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
                    foreach (LevelCube cube in worldMapLevels) cube.GetComponent<_ScriptMatFaceCube>().isSelected = false;

                    LevelsManager.instance.SelectLevel(levelCube.kubicode);
                    levelCube.gameObject.GetComponent<_ScriptMatFaceCube>().isSelected = true;
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

            playerBeatenLevels = SaveAndLoad.instance.LoadProgress().beatenLevels;
            playerGoldenLevels = SaveAndLoad.instance.LoadProgress().goldenLevels;

            foreach (LevelCube cube in worldMapLevels)
            {
                if(playerBeatenLevels.Contains(cube.kubicode))
                    cube.GetComponent<_ScriptMatFaceCube>().isPlayed = true;

                else if (playerGoldenLevels.Contains(cube.kubicode))
                    cube.GetComponent<_ScriptMatFaceCube>().isGold = true;

                else cube.GetComponent<_ScriptMatFaceCube>().isLocked = true;

                if (cube.kubicode == nextLevelInProgression)
                    cube.GetComponent<_ScriptMatFaceCube>().isUnlocked = true;

            }
        }
    }
}
