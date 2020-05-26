using Kubika.CustomLevelEditor;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NodeInfoSearchBar : EditorWindow
{
    private int searchIndex;

    CubeTypes cubeType;
    CubeLayers cubeLayers;
    FacingDirection facingDirection;
    GameObject cubeOnPosition;
    Vector3 worldPosition;

    [MenuItem("Tools/Node SearchBar")]
    static void Init()
    {
        var window = GetWindow<NodeInfoSearchBar>();
        window.position = new Rect(0, 0, 180, 80);
        window.Show();
    }

    private void OnGUI()
    {
        /*
        EditorGUIUtility.LookLikeInspector();

        GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));

        GUILayout.FlexibleSpace();

        searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"));

        if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
        {
            // Remove focus if cleared
            searchString = "";
            GUI.FocusControl(null);
        }

        GUILayout.EndHorizontal();
        */

        DrawWindow();
        DrawInfo();
    }

    private void DrawWindow()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.FlexibleSpace();

        searchIndex = EditorGUILayout.IntField(searchIndex, EditorStyles.toolbarTextField);
        GUILayout.EndHorizontal();

        cubeType = _Grid.instance.kuboGrid[searchIndex - 1].cubeType;
        cubeLayers = _Grid.instance.kuboGrid[searchIndex - 1].cubeLayers;
        facingDirection = _Grid.instance.kuboGrid[searchIndex - 1].facingDirection;
        cubeOnPosition = _Grid.instance.kuboGrid[searchIndex - 1].cubeOnPosition;
        worldPosition = _Grid.instance.kuboGrid[searchIndex - 1].worldPosition;
    }

    private void DrawInfo()
    {
        cubeType = (CubeTypes)EditorGUILayout.EnumPopup("My CubeType is ", cubeType);
        cubeLayers = (CubeLayers)EditorGUILayout.EnumPopup("My CubeLayer is ", cubeLayers);
        facingDirection = (FacingDirection)EditorGUILayout.EnumPopup("My FacingDirection is ", facingDirection);
        cubeOnPosition = (GameObject)EditorGUILayout.ObjectField("The Object is ", cubeOnPosition, typeof(GameObject), true);
        worldPosition = (Vector3)EditorGUILayout.Vector3Field("The Object's Position is ", worldPosition);
    }

}
