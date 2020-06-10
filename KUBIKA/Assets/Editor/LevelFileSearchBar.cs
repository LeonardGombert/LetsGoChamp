using Kubika.Game;
using Kubika.Saving;
using NUnit.Framework;
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

    private string path;
    private string folder;
    private string levelFolder;

    List<LevelFile> levelFiles = new List<LevelFile>();
    string levelName;
    string Kubicode;
    Biomes levelBiome;
    int minimumMoves;
    bool lockRotate;
    TextAsset levelTextFile;

    bool modifiyingFile;
    bool hasFile;
    LevelEditorData levelData;
    string assetPath;

     [MenuItem("Tools/Level SearchBar")]
    static void Init()
    {
        var window = GetWindow<LevelFileSearchBar>();
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        modifiyingFile = GUILayout.Toggle(modifiyingFile, "Modify File");

        if (GUILayout.Button("Save Modifications"))
        {
            modifiyingFile = false;
            hasFile = false;

            SaveModifications();
        }

        GUILayout.EndHorizontal();

        if (!modifiyingFile)
        {
            DrawWindow();
            DrawInfo();

            if (GUI.changed)
            {
                GetInfo();
            }
        }

        else ModifyFile();
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
    {
        folder = "MainLevels";
        levelFolder = "";
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
            case Biomes.Statues:
                levelFolder = "05_Statues";
                break;
            case Biomes.Temple:
                levelFolder = "06_Temple";
                break;
            case Biomes.Chaos:
                levelFolder = "07_Chaos";
                break;
            default:
                break;
        }

        path = Path.Combine(folder, levelFolder);
        loadLevelFiles = Resources.LoadAll(path);

        //UnityEngine.Object[] loadLevelFiles = AssetDatabase.LoadAllAssetsAtPath(path);

        foreach (TextAsset file in loadLevelFiles)
        {
            levelFiles.Add(UserLevelFiles.ConvertToLevelInfo(file));
        }

        for (int i = 0; i < levelFiles.Count; i++)
        {
            if (searchIndex == levelFiles[i].levelName || searchIndex == levelFiles[i].kubicode)
            {
                levelName = levelFiles[i].levelName;
                Kubicode = levelFiles[i].kubicode;
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

    private void ModifyFile()
    {
        if (!hasFile)
        {
            assetPath = AssetDatabase.GetAssetPath(levelTextFile);
            string json = File.ReadAllText(assetPath);
            levelData = JsonUtility.FromJson<LevelEditorData>(json);
            hasFile = true;
        }

        DrawInfo();
    }

    private void SaveModifications()
    {
        levelData.levelName = levelName;
        levelData.Kubicode = Kubicode;
        levelData.biome = levelBiome;
        levelData.minimumMoves = minimumMoves;
        levelData.lockRotate = lockRotate;

        string json = JsonUtility.ToJson(levelData);
        if (File.Exists(assetPath)) File.Delete(assetPath);
        File.WriteAllText(assetPath, json);
        AssetDatabase.RenameAsset(assetPath, levelName);
    }
}
