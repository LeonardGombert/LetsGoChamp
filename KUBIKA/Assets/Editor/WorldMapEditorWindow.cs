using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WorldMapEditorWindow : EditorWindow
{
    GameObject levelCube;
    GameObject activeObject;
    RaycastHit hit;
    Vector3 rotation;

    [MenuItem("Tools/WorldMap Editor")]
    static void Init()
    {
        var window = GetWindow<WorldMapEditorWindow>();
        window.Show();
    }

    private void OnGUI()
    {
        LockPlanetFace();
        PlaceButton();
        RemoveButton();

    }

    private void LockPlanetFace()
    {
        if (GUILayout.Button("Lock on Face"))
        {
            activeObject = Selection.activeGameObject;
            rotation = activeObject.transform.rotation.eulerAngles;
            Debug.Log(rotation);
        }
    }

    private void PlaceButton()
    {
        levelCube = (GameObject)EditorGUILayout.ObjectField(levelCube, typeof(GameObject), true);

        if (GUILayout.Button("Place New Level"))
        {
            GameObject newObj = Instantiate(levelCube);
            newObj.transform.position = activeObject.transform.position;
            newObj.transform.rotation = Quaternion.Euler(rotation);
            WorldmapManager.instance.levelCubes.Add(newObj);
        }
    }

    private void RemoveButton()
    {
        if (GUILayout.Button("Remove Selected Level"))
        {
            //remove selected level
        }
    }
}
