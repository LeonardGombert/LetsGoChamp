using Kubika.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelInstantiator : MonoBehaviour
{
    [SerializeField] Text uiLevelName;
    [SerializeField] Button uiPlayButton;
    string loadToKubiCode;

    public void FillValues(string levelName, string kubiCode)
    {
        uiLevelName.text = levelName;
        loadToKubiCode = kubiCode;
    }

    public void PlayLevel()
    {
        LevelsManager.instance.loadToKubicode = loadToKubiCode;
        UIManager.instance.ButtonCallback("WORLDMAP_Play");
    }
}
