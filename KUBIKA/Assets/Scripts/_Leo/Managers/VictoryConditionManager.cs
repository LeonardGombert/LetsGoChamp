using Kubika.CustomLevelEditor;
using Kubika.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Kubika.Game
{
    public class VictoryConditionManager : MonoBehaviour
    {
        private static VictoryConditionManager _instance;
        public static VictoryConditionManager instance { get { return _instance; } }

        public int currentVictoryPoints;
        public int levelVictoryPoints;

        [Header("WAIT TIME")]
        public float waitTime = 1.5f;
        public float waitTimeBeforePS = 0.0f;


        BaseVictoryCube[] victoryCubes;
        

        private void Awake()
        {
            if (_instance != null && _instance != this) Destroy(this);
            else _instance = this;
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
                Debug.Log("WIN TRANSITION");
                StartCoroutine(TransitionTime());
            }
        }

        public IEnumerator TransitionTime()
        {
            yield return new WaitForSeconds(waitTimeBeforePS);
            _FeedBackManager.instance.PlayVictoryFX();
            yield return new WaitForSeconds(waitTime);
            SaveAndLoad.instance.SaveProgress();
            StartCoroutine(WinCountdown());
        }

        //use this to call all win animations etc.
        private IEnumerator WinCountdown()
        {
            UIManager.instance.OpenWinLevelWindow();
            yield return null;
        }
    }
}