using Kubika.CustomLevelEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubika.Game
{
    public class VictoryConditionManager : MonoBehaviour
    {
        private static VictoryConditionManager _instance;
        public static VictoryConditionManager instance { get { return _instance; } }

        public int currentVictoryPoints;
        public int levelVictoryPoints;

        BaseVictoryCube[] victoryCubes;

        private void Awake()
        {
            if (_instance != null && _instance != this) Destroy(this);
            else _instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            _DataManager.instance.EndFalling.AddListener(VictoryConditionStatus);
        }

        // Update is called once per frame
        void Update()
        {
            VictoryConditionStatus();
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
        }

        public void DecrementVictory()
        {
            Debug.Log("I've lost track of a Victory cube");
            currentVictoryPoints--;
        }

        private void VictoryConditionStatus()
        {
            if (currentVictoryPoints == levelVictoryPoints)
            {
                StartCoroutine(WinCountdown());
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