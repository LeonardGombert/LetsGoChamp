﻿using Kubika.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelInstantiator : MonoBehaviour
{
    [SerializeField] Text uiLevelName, uiLevelDIfficulty;
    [SerializeField] Button uiPlayButton;
    string loadToKubiCode;

    //Called when instantiated by populator
    public void FillValues(string levelName, Difficulty difficulty, string kubiCode)
    {
        uiLevelName.text = levelName;
        uiLevelDIfficulty.text = difficulty.ToString();
        loadToKubiCode = kubiCode;
    }

    //called by Unity Event Button
    public void PlayLevel()
    {
        LevelsManager.instance.loadToKubicode = loadToKubiCode;
        UIManager.instance.ButtonCallback("WORLDMAP_Play");
    }
}