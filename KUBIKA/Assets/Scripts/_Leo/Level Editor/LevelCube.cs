using Kubika.Game;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCube : MonoBehaviour
{
    public string kubicode;

    [FoldoutGroup("Node Info")] public LevelCube previousLevel;
    [FoldoutGroup("Node Info")] public LevelCube nextLevel;
    [FoldoutGroup("Node Info")] public LevelCube nextOptionalLevel;
    [FoldoutGroup("Node Info")] public LevelCube prevOptionalLevel;
    [FoldoutGroup("Node Info")] public float width = .5f;
    [FoldoutGroup("Node Info")] public bool isOptionalLevel;
    [FoldoutGroup("Node Info")] public bool isAnchorNode;

    [FoldoutGroup("Kubika Level Info")] public string levelIndex;
    [FoldoutGroup("Kubika Level Info")] public string levelName;
    [FoldoutGroup("Kubika Level Info")] public int minimalMoves;
    [FoldoutGroup("Kubika Level Info")] public int previousPlayerScore;
    [FoldoutGroup("Kubika Level Info")] public bool isBeaten;
    [FoldoutGroup("Kubika Level Info")] public bool isSelected; //set by tapping on the object

    void Start()
    {
        gameObject.name = gameObject.name.Replace("Level Node ", "");

        if(!isAnchorNode)
        {
            for (int i = 0; i < LevelsManager.instance.masterList.Count; i++)
            {
                if (gameObject.name == i.ToString())
                {
                    //isBeaten = LevelsManager.instance.masterList[i].levelBeaten;
                    levelIndex = LevelsManager.instance.masterList[i].Kubicode;
                    levelName = LevelsManager.instance.masterList[i].levelName;
                    minimalMoves = LevelsManager.instance.masterList[i].minimumMoves;
                    //previousPlayerScore = LevelsManager.instance.masterList[i].prevPlayerScore;
                }
            }
        }
    }

    private void Update()
    {
        if (isSelected)
        {
            /*UIManager.instance.selectedLevelIndex = levelIndex;
            UIManager.instance.selectedLevelName.text = levelName;
            UIManager.instance.minimalMoves.text = minimalMoves.ToString();
            UIManager.instance.prevPlayerScore.text = previousPlayerScore.ToString();*/
        }
    }
}
