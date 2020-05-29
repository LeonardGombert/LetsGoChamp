using Kubika.CustomLevelEditor;
using Kubika.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Kubika.Game
{
    public class VictoryConditionManager : MonoBehaviour
    {
        private static VictoryConditionManager _instance;
        public static VictoryConditionManager instance { get { return _instance; } }

        public int currentVictoryPoints;
        public int levelVictoryPoints;

        public string progressKubiCode;

        BaseVictoryCube[] victoryCubes;
        
        PlayerProgress playerProgress;

        private void Awake()
        {
            if (_instance != null && _instance != this) Destroy(this);
            else _instance = this;
        }

        private void Start()
        {
            CreatePlayerProgressData();
        }

        private PlayerProgress CreatePlayerProgressData()
        {
            playerProgress = new PlayerProgress();
            return playerProgress;
        }

        // Call when a new level is loaded
        public void CheckVictoryCubes()
        {
            currentVictoryPoints = 0;
            levelVictoryPoints = 0;

            for (int i = 0; i < _Grid.instance.kuboGrid.Length; i++)
            {
                if (_Grid.instance.kuboGrid[i].cubeType >= CubeTypes.BaseVictoryCube &&
                    _Grid.instance.kuboGrid[i].cubeType <= CubeTypes.SwitchVictoryCube)

                {
                    Debug.Log(i);
                    levelVictoryPoints++;
                }
            }
        }

        public void IncrementVictory()
        {
            Debug.Log("I've been touched by a Victory cube");
            currentVictoryPoints++;

            VictoryConditionStatus();
        }

        public void DecrementVictory()
        {
            Debug.Log("I've lost track of a Victory cube");
            currentVictoryPoints--;

            VictoryConditionStatus();
        }

        private void VictoryConditionStatus()
        {
            if (currentVictoryPoints == levelVictoryPoints)
            {
                SaveProgress();
                StartCoroutine(WinCountdown());
            }
        }

        private void SaveProgress()
        {
            progressKubiCode = LevelsManager.instance._Kubicode;
            TextAsset progressFile = Resources.Load("PlayerSave/PlayerProgress.json") as TextAsset;

            if (progressFile != null)
            {
                Debug.Log("Overwritting existing save !");
                playerProgress.lastLevelKubicode = progressKubiCode;
                string json = JsonUtility.ToJson(playerProgress);

                JsonUtility.FromJsonOverwrite(json, progressFile);
            }

            // THIS ONLY WORKS IN EDITOR
            else
            {
                playerProgress.lastLevelKubicode = progressKubiCode;
                string json = JsonUtility.ToJson(playerProgress);

                string folder = Application.dataPath + "/Resources/PlayerSave";
                string levelFile = "PlayerProgress.json";

                string path = Path.Combine(folder, levelFile);

                File.WriteAllText(path, json);

                Debug.Log("Creating a new save at " + path);
            }
        }

        //use this to call all win animations etc.
        private IEnumerator WinCountdown()
        {
            UIManager.instance.OpenWinLevelWindow();
            yield return null;
        }
    }
}