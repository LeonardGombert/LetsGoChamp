﻿using Kubika.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubika.Game
{
    public class PlayerMoves : MonoBehaviour
    {
        private static PlayerMoves _instance;
        public static PlayerMoves instance { get { return _instance; } }

        public int numberOfMoves;

        private void Awake()
        {
            if (_instance != null && _instance != this) Destroy(this);
            else _instance = this;
        }

        private void Start()
        {

        }

        public void IncrementMoves()
        {
            numberOfMoves++;
        }

        //called on level restart/new level
        public void ResetMoves()
        {
            numberOfMoves = 0;
        }

        public bool CheckIfGolden()
        {
            if (numberOfMoves <= LevelsManager.instance._minimumMoves) return true;
            else return false;
        }
    }
}