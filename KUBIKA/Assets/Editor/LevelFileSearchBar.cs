using Kubika.Game;
using Kubika.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LevelFileSearchBar : EditorWindow
{
    private string searchIndex;
    Biomes loadLevelBiome;
    UnityEngine.Object[] loadLevelFiles;

    List<LevelFile> levelFiles = new List<LevelFile>();
    string levelName;
    string Kubicode;
    Biomes levelBiome;
    int minimumMoves;
    bool lockRotate;
    TextAsset levelTextFile;

    [MenuItem("Tools/Level SearchBar")]
    static void Init()
    {
        var window = GetWindow<LevelFileSearchBar>();
        window.position = new Rect(0, 0, 180, 80);
        window.Show();
    }

    private void OnGUI()
    {
        DrawWindow();
        GetInfo();
        DrawInfo();
    }

    private void DrawWindow()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.FlexibleSpace();

        searchIndex = EditorGUILayout.TextField(searchIndex, EditorStyles.toolbarTextField);
        loadLevelBiome = (Biomes)EditorGUILayout.EnumPopup("Load From Biome ", loadLevelBiome, EditorStyles.toolbarDropDown);

        GUILayout.EndHorizontal();
    }

    void GetInfo()
    {string folder = "MainLevels";
        string levelFolder = "";
        switch (loadLevelBiome)
        {
            case Biomes.Plains:
                levelFolder = "01_Plains";
                break;
            case Biomes.Mountains:
                levelFolder = "02_Mountains";
                break;
            case Biomes.Underwater:
                levelFolder = "03_Underwater";
                break;
            case Biomes.Ruins:
                levelFolder = "04_Ruins";
                break;
            case Biomes.Temple:
                levelFolder = "05_Temple";
                break;
            case Biomes.Statues:
                levelFolder = "06_Statues";
                break;
            case Biomes.Chaos:
                levelFolder = "07_Chaos";
                break;
            default:
                break;
        }
        string path = Path.Combine(folder, levelFolder);
        loadLevelFiles = Resources.LoadAll(path);

        //UnityEngine.Object[] loadLevelFiles = AssetDatabase.LoadAllAssetsAtPath(path);

        foreach (TextAsset file in loadLevelFiles)
        {
            Debug.Log(file.name);
            levelFiles.Add(UserLevelFiles.ConvertToLevelInfo(file));
        }

        for (int i = 0; i < levelFiles.Count; i++)
        {
            if (searchIndex == levelFiles[i].levelName || searchIndex == levelFiles[i].Kubicode)
            {
                levelName = levelFiles[i].levelName;
                Kubicode = levelFiles[i].Kubicode;
                levelBiome = levelFiles[i].levelBiome;
                minimumMoves = levelFiles[i].minimumMoves;
                lockRotate = levelFiles[i].lockRotate;
                levelTextFile = levelFiles[i].levelFile;
            }
        }
    }

    private void DrawInfo()
    {
        levelName = EditorGUILayout.TextField("Level Name : ", levelName);
        Kubicode = EditorGUILayout.TextField("Kubicode : ", Kubicode);
        levelBiome = (Biomes)EditorGUILayout.EnumPopup("Biome : ", levelBiome);
        minimumMoves = EditorGUILayout.IntField("Minimum to Beat : ", minimumMoves);
        lockRotate = EditorGUILayout.Toggle("Rotate is Locked : ", lockRotate);
        levelTextFile = (TextAsset)EditorGUILayout.ObjectField(levelTextFile, typeof(UnityEngine.Object), true);
    }
}
