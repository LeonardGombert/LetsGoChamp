using UnityEngine;
using UnityEditor;
using Kubika.Game;
using System.Collections.Generic;
using System.Linq;
using System;

public class LevelCubeEditorWindow : EditorWindow
{
    public Transform LevelCubeRoot;
    public Transform optLevelCubeRoot;
    public Transform anchorNodeRoot;

    bool createdOptionalLevel = false;

    GameObject selectedObject;
    GameObject activeFace;
    GameObject worldmapCube;

    Biomes currentBiome;

    GameObject worldMapFace;

    [MenuItem("Tools/Level Cube Editor")]
    public static void Open()
    {
        GetWindow<LevelCubeEditorWindow>("Level Cube Editor Window");
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        Base();
        EditorGUI.EndChangeCheck();
    }

    private void Base()
    {
        SerializedObject obj = new SerializedObject(this);

        EditorGUILayout.PropertyField(obj.FindProperty("LevelCubeRoot"));
        EditorGUILayout.PropertyField(obj.FindProperty("optLevelCubeRoot"));
        EditorGUILayout.PropertyField(obj.FindProperty("anchorNodeRoot"));

        worldmapCube = (GameObject)EditorGUILayout.ObjectField(worldmapCube, typeof(GameObject), true);

        currentBiome = (Biomes)EditorGUILayout.EnumPopup(currentBiome);
        activeFace = GameObject.Find(currentBiome.ToString());

        if (LevelCubeRoot == null || optLevelCubeRoot == null || anchorNodeRoot == null)
        {
            LevelCubeRoot = activeFace.transform.GetChild(0);
            optLevelCubeRoot = activeFace.transform.GetChild(1);
            anchorNodeRoot = activeFace.transform.GetChild(2);

            EditorGUILayout.HelpBox("Root transforms must be selected. Please assign the root transforms", MessageType.Warning);
        }

        else
        {
            GameObject worldMap = GameObject.Find("PLANETE");
            worldMapFace = worldMap.transform.GetChild(1).transform.GetChild(0).transform.GetChild((int)currentBiome).gameObject;
            EditorGUILayout.ObjectField(worldMapFace, typeof(GameObject), true);

            EditorGUILayout.BeginVertical("");
            EditorGUILayout.LabelField("Create Nodes", EditorStyles.boldLabel);
            DrawCreateButtons();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Insert Nodes", EditorStyles.boldLabel);
            DrawInsertButtons();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Clear Nodes", EditorStyles.boldLabel);
            DrawClearButtons();
            EditorGUILayout.EndVertical();
        }

        //save changes made in the inspector and set the values
        obj.ApplyModifiedProperties();
    }


    //Create button and method for adding a waypoint
    private void DrawCreateButtons()
    {
        if (GUILayout.Button("Create Level Node"))
        {
            CreateWaypoint();

            if (LevelCubeVisualizer.instance != null) LevelCubeVisualizer.instance.InitializeProperties(); //redraw connections
        }

        if (GUILayout.Button("Create Optional Level"))
        {
            CreateBranchingPath();
        }
    }

    private void DrawInsertButtons()
    {
        if (GUILayout.Button("Insert Level Node"))
        {
            InsertLevel();
        }

        if (GUILayout.Button("Insert Anchor Node"))
        {
            InsertAnchor();
        }
    }

    private void DrawClearButtons()
    {

        if (GUILayout.Button("Delete Selected Node"))
        {
            DeleteNode();
        }

        if (GUILayout.Button("Clear Current Level Nodes"))
        {
            ClearNodes();
        }
    }


    private void DeleteNode()
    {
        LevelCube LevelCubeToDestroy = Selection.activeGameObject.GetComponent<LevelCube>();

        if (LevelCubeToDestroy.nextLevel != null) LevelCubeToDestroy.nextLevel.previousLevel = LevelCubeToDestroy.previousLevel;
        if (LevelCubeToDestroy.previousLevel != null) LevelCubeToDestroy.previousLevel.nextLevel = LevelCubeToDestroy.nextLevel;

        if (LevelCubeToDestroy.nextOptionalLevel != null) LevelCubeToDestroy.previousLevel.nextOptionalLevel = LevelCubeToDestroy.nextOptionalLevel;
        if (LevelCubeToDestroy.prevOptionalLevel != null) LevelCubeToDestroy.nextLevel.prevOptionalLevel = LevelCubeToDestroy.prevOptionalLevel;

        DestroyImmediate(LevelCubeToDestroy.gameObject);

        if (LevelCubeToDestroy.isOptionalLevel) ResetNames(2);
        else if (LevelCubeToDestroy.isAnchorNode) ResetNames(3);
        else ResetNames(1);
    }

    private void ClearNodes()
    {
        var tempList = LevelCubeRoot.transform.Cast<Transform>().ToList();
        foreach (Transform child in tempList) DestroyImmediate(child.gameObject);

        tempList = optLevelCubeRoot.transform.Cast<Transform>().ToList();
        foreach (Transform child in tempList) DestroyImmediate(child.gameObject);

        tempList = anchorNodeRoot.transform.Cast<Transform>().ToList();
        foreach (Transform child in tempList) DestroyImmediate(child.gameObject);
    }

    private void CreateWaypoint()
    {
        //create a new object and name it after the child count of the transform parent
        //after the object has been instantiated, set it as a child of the parent
        //GameObject LevelCubeObject = new GameObject("Level Node " + LevelCubeRoot.childCount, typeof(LevelCube));

        GameObject levelCubeObject = (GameObject)PrefabUtility.InstantiatePrefab(worldmapCube);

        int levelNode = ((int)currentBiome + 1)  * 100 + LevelCubeRoot.childCount + 1;

        levelCubeObject.name = "Level Node " + levelNode.ToString();
        levelCubeObject.transform.SetParent(LevelCubeRoot, false);

        levelCubeObject.gameObject.layer = LayerMask.NameToLayer("LevelCubes");

        levelCubeObject.transform.position = worldMapFace.transform.GetChild(0).transform.position;

        levelCubeObject.transform.up = worldMapFace.transform.GetChild(1).transform.up;
        //levelCubeObject.transform.forward = worldMapFace.transform.GetChild(1).transform.up;

        levelCubeObject.transform.Translate(new Vector3(0, .25f, 0), Space.Self);
        //levelCubeObject.transform.position +=levelCubeObject.transform.Translate(Vector3.up, Space.Self);// levelCubeObject.transform.localPosition + new Vector3(0, .5f, 0);

        LevelCube levelCube = levelCubeObject.GetComponent<LevelCube>();

        //automatically link up the waypoints on spawn
        if (LevelCubeRoot.childCount > 1)
        {
            //as we are looking at the node placed right before the new one, we use childCount -2

            //assign the previous level as the last level in the list before this new node
            levelCube.previousLevel = LevelCubeRoot.GetChild(LevelCubeRoot.childCount - 2).gameObject.GetComponent<LevelCube>();

            //assign that node's next level as this new node
            levelCube.previousLevel.nextLevel = levelCube;

            if (createdOptionalLevel)
            {
                levelCube.prevOptionalLevel = optLevelCubeRoot.GetChild(optLevelCubeRoot.childCount - 1).gameObject.GetComponent<LevelCube>();
                LevelCube prevOptional = optLevelCubeRoot.GetChild(optLevelCubeRoot.childCount - 1).gameObject.GetComponent<LevelCube>();
                prevOptional.nextLevel = levelCube;
                createdOptionalLevel = false;
            }

            //place the new LevelCube at the last position
            levelCube.transform.position = levelCube.previousLevel.transform.position;
            levelCube.transform.rotation = levelCube.previousLevel.transform.rotation;
            //levelCubeObject.transform.up = levelCube.previousLevel.transform.up;
            //levelCube.transform.forward = levelCube.previousLevel.transform.forward;
        }

        Selection.activeGameObject = levelCube.gameObject;
    }

    private void CreateBranchingPath()
    {
        createdOptionalLevel = true;
        GameObject optLevelCubeObject = (GameObject)PrefabUtility.InstantiatePrefab(worldmapCube);
        optLevelCubeObject.name = "Optional Level Node " + optLevelCubeRoot.childCount;
        optLevelCubeObject.transform.SetParent(optLevelCubeRoot, false);

        LevelCube optLevelCube = optLevelCubeObject.GetComponent<LevelCube>();

        if (optLevelCubeRoot.childCount >= 1)
        {
            //get the last level node of the regular level Node List (so use -1 instead of -2)
            optLevelCube.previousLevel = LevelCubeRoot.GetChild(LevelCubeRoot.childCount - 1).gameObject.GetComponent<LevelCube>();
            //set the current node as the next optional level
            optLevelCube.previousLevel.nextOptionalLevel = optLevelCube;

            optLevelCube.transform.position = optLevelCube.previousLevel.transform.position;
            optLevelCube.transform.forward = optLevelCube.previousLevel.transform.forward;
            optLevelCube.transform.rotation = worldMapFace.transform.rotation;
        }
    }

    private void InsertLevel()
    {
        selectedObject = Selection.activeGameObject;
        var selectedNodeIndex = selectedObject.transform.GetSiblingIndex();

        GameObject insertedLevelObject = (GameObject)PrefabUtility.InstantiatePrefab(worldmapCube);
        insertedLevelObject.name = "Level Node " + LevelCubeRoot.childCount;
        insertedLevelObject.transform.SetParent(LevelCubeRoot, false);

        LevelCube levelCube = insertedLevelObject.GetComponent<LevelCube>();

        if (LevelCubeRoot.childCount > 1)
        {
            levelCube.previousLevel = LevelCubeRoot.GetChild(selectedNodeIndex).gameObject.GetComponent<LevelCube>();
            levelCube.nextLevel = levelCube.previousLevel.nextLevel;
            levelCube.previousLevel.nextLevel = levelCube;
            levelCube.nextLevel.previousLevel = levelCube;

            levelCube.transform.position = (levelCube.nextLevel.transform.position + levelCube.previousLevel.transform.position) / 2;
            levelCube.transform.forward = levelCube.previousLevel.transform.forward;
            levelCube.transform.rotation = worldMapFace.transform.rotation;
        }

        insertedLevelObject.transform.SetSiblingIndex(selectedNodeIndex + 1);

        ResetNames(1);
    }

    private void InsertAnchor()
    {
        selectedObject = Selection.activeGameObject;
        var selectedNodeIndex = selectedObject.transform.GetSiblingIndex();

        GameObject levelAnchorObject = new GameObject("Anchor Node " + anchorNodeRoot.childCount, typeof(LevelCube));
        levelAnchorObject.transform.SetParent(anchorNodeRoot, false);

        LevelCube levelAnchor = levelAnchorObject.GetComponent<LevelCube>();

        if (anchorNodeRoot.childCount >= 1)
        {
            levelAnchor.previousLevel = LevelCubeRoot.GetChild(selectedNodeIndex).gameObject.GetComponent<LevelCube>();
            levelAnchor.nextLevel = levelAnchor.previousLevel.nextLevel;
            levelAnchor.previousLevel.nextLevel = levelAnchor;
            levelAnchor.nextLevel.previousLevel = levelAnchor;

            levelAnchor.transform.position = (levelAnchor.nextLevel.transform.position + levelAnchor.previousLevel.transform.position) / 2;
            levelAnchor.transform.forward = levelAnchor.previousLevel.transform.forward;
        }

        levelAnchorObject.transform.SetSiblingIndex(selectedNodeIndex + 1);

        ResetNames(3);
    }

    private void ResetNames(int nodeType)
    {
        if (nodeType == 1)
        {
            var templist = LevelCubeRoot.transform.Cast<Transform>().ToList();

            for (int i = 0; i < LevelCubeRoot.childCount; i++)
            {
                int levelNode = ((int)currentBiome + 1) * 100 + i + 1;
                LevelCubeRoot.GetChild(i).gameObject.name = "Level Node " + levelNode.ToString();
            }
        }

        else if (nodeType == 2)
        {
            var templist = optLevelCubeRoot.transform.Cast<Transform>().ToList();

            for (int i = 0; i < optLevelCubeRoot.childCount; i++)
            {
                optLevelCubeRoot.GetChild(i).gameObject.name = "Optional Level Node " + i;
            }
        }

        else if (nodeType == 3)
        {
            var templist = anchorNodeRoot.transform.Cast<Transform>().ToList();

            for (int i = 0; i < anchorNodeRoot.childCount; i++)
            {
                anchorNodeRoot.GetChild(i).gameObject.name = "Anchor Node " + i;
            }
        }
    }
}