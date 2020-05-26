﻿using Kubika.CustomLevelEditor;
using Kubika.Game;
using Kubika.Saving;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
[CanEditMultipleObjects]
public class LevelEditorWindow : EditorWindow
{
    UnityEngine.Object[] levelFiles;
    string[] levels;
    int levelIndex = 0;
    int cubeTypeIndex = 0;
    private string levelName;
    private string kubiCode;
    private bool lockRotate;
    private int miminumMoves;

    string[] cubeTypes;
    CubeTypes cubeTypesLength;

    private string[] levelBiomes;
    private Biomes levelBiome;
    private int biomesIndex;

    [MenuItem("Tools/Level Editor")]
    static void Init()
    {
        var window = GetWindow<LevelEditorWindow>();
        window.position = new Rect(0, 0, 180, 80);
        window.Show();
    }

    void OnGUI()
    {
        if (Application.isPlaying)
        {
            LoadLevel();
            SelectCubeType();

            PlaceCubes();
            DeleteCubes();
            RotateCubes();
            GUILayout.Space(20);
            SaveLevel();
            SaveTestLevel();
            SaveCurrentLevel();

            GUILayout.Space(70);

            LaunchGame();

            //TestLevel();
        }
    }

    private void PlaceCubes()
    {
        if (GUILayout.Button("Place Cubes"))
            LevelEditor.instance.SwitchAction("isPlacing");
    }

    private void DeleteCubes()
    {
        if (GUILayout.Button("Delete Cubes"))
            LevelEditor.instance.SwitchAction("isDeleting");
    }

    private void RotateCubes()
    {
        if (GUILayout.Button("Rotate Cubes"))
            LevelEditor.instance.SwitchAction("isRotating");
    }

    private void LoadLevel()
    {
        levelFiles = Resources.LoadAll("MainLevels", typeof(TextAsset));

        levels = new string[levelFiles.Length];

        for (int i = 0; i < levelFiles.Length; i++)
        {
            levels[i] = levelFiles[i].name;
        }

        levelIndex = EditorGUI.Popup(new Rect(0, 20, position.width, 20), "Load Level : ", levelIndex, levels);

        GUILayout.Space(70);

        if (GUILayout.Button("Load Level !"))
        {
            SaveAndLoad.instance.DevLoadLevel(levels[levelIndex]);
            levelName = SaveAndLoad.instance.currentOpenLevelName;
            kubiCode = SaveAndLoad.instance.currentKubicode;
            lockRotate = SaveAndLoad.instance.currentLevelLockRotate;
            miminumMoves = SaveAndLoad.instance.currentMinimumMoves;
            biomesIndex = (int)SaveAndLoad.instance.currentBiome;
        }
    }

    private void SaveLevel()
    {
        levelName = EditorGUILayout.TextField("Load/Save Level Name", levelName);
        kubiCode = EditorGUILayout.TextField("KudiCode", kubiCode);

        levelBiome = (Biomes)EditorGUILayout.EnumPopup("Level Biome is ", levelBiome);

        _MaterialCentral.instance.staticIndex = levelBiome; //change the current biome

        lockRotate = EditorGUILayout.Toggle("Lock Rotation ?", lockRotate);
        miminumMoves = EditorGUILayout.IntField("Minimum Moves to Beat", miminumMoves);

        EditorGUILayout.Space(20);

        if (GUILayout.Button("Save Level")) SaveAndLoad.instance.DevSavingLevel(levelName, kubiCode, lockRotate, miminumMoves);
    }

    private void SaveTestLevel()
    {
        if (GUILayout.Button("Save Test Level")) SaveAndLoad.instance.DevSavingLevel(levelName, kubiCode, lockRotate, miminumMoves, true);
    }


    private void SaveCurrentLevel()
    {
        if (GUILayout.Button("Save Current Level"))
        {
            SaveAndLoad.instance.currentOpenLevelName = levelName;
            SaveAndLoad.instance.currentKubicode = kubiCode;
            SaveAndLoad.instance.currentLevelLockRotate = lockRotate;
            SaveAndLoad.instance.currentMinimumMoves = miminumMoves;

            SaveAndLoad.instance.DevSavingCurrentLevel();
        }
    }

    private void SelectCubeType()
    {
        cubeTypes = new string[(int)CubeTypes.ChaosBall + 1];

        for (int i = 1; i <= (int)CubeTypes.ChaosBall; i++)
        {
            cubeTypesLength = (CubeTypes)i;
            cubeTypes[i] = cubeTypesLength.ToString();
        }

        cubeTypeIndex = EditorGUI.Popup(new Rect(0, 40, position.width, 20), "Cube Type : ", cubeTypeIndex, cubeTypes);

        LevelEditor.instance.currentCube = (CubeTypes)cubeTypeIndex;
    }

    private void LaunchGame()
    {
        if (GUI.Button(new Rect(0, 375, position.width, 45), "Launch Game"))
        {
            if(SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex((int)ScenesIndex.MANAGER))
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                SceneManager.LoadSceneAsync((int)ScenesIndex.MANAGER, LoadSceneMode.Additive);
            }
        }
    }

    private void TestLevel()
    {
        if (GUI.Button(new Rect(0, 350, position.width, 45), "Test Level"))
        {
            levelName = SaveAndLoad.instance.currentOpenLevelName;
            kubiCode = SaveAndLoad.instance.currentKubicode;
            lockRotate = SaveAndLoad.instance.currentLevelLockRotate;
            miminumMoves = SaveAndLoad.instance.currentMinimumMoves;

            //SaveAndLoad.instance.DevSavingLevel(levelName, lockRotate, miminumMoves);

            string folder = Application.dataPath + "/Resources/MainLevels";

            string levelFile = "NO" + ".json";

            string path = Path.Combine(folder, levelFile);

            if (File.Exists(path))
            {
                // Load object
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
                // Select the object in the project folder
                Selection.activeObject = obj;
                // Also flash the folder yellow to highlight it
                EditorGUIUtility.PingObject(obj);
                
                Debug.Log("Assining");
                LevelsManager.instance.testLevel = (TextAsset)AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));
            }

            //AsyncOperation asynchOp = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            //while (!asynchOp.isDone) return;

            //SceneManager.LoadSceneAsync((int)ScenesIndex.MANAGER, LoadSceneMode.Additive);
        }
    }

}
