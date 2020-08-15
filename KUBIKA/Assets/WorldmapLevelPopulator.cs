using Kubika.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldmapLevelPopulator : MonoBehaviour
{
    [SerializeField] GameObject levelPrefab;

    LevelsManager levelsManager;

    // Start is called before the first frame update
    void Start()
    {
        levelsManager = LevelsManager.instance;
        PopulateScreen(1);
    }

    void PopulateScreen(int currentWorld)
    {
        foreach (var item in levelsManager.gameMasterList)
        {
            if (item.kubicode.Contains("Worl" + currentWorld))
            {
                GameObject newObj = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity, transform);
                LevelInstantiator level = newObj.GetComponent<LevelInstantiator>();
                level.FillValues(item.levelName, item.difficulty, item.kubicode);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
