using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kubika.Game
{
    public class WorldMapEditorWindow : EditorWindow
    {
        GameObject levelCube;
        GameObject activeFace;
        Vector3 faceRotation;

        [MenuItem("Tools/WorldMap Editor")]
        static void Init()
        {
            var window = GetWindow<WorldMapEditorWindow>();
            window.Show();
        }

        private void OnGUI()
        {
            levelCube = (GameObject)EditorGUILayout.ObjectField(levelCube, typeof(GameObject), true);

            LockPlanetFace();
            PlaceButton();
            RemoveButton();

        }

        private void LockPlanetFace()
        {
            if (GUILayout.Button("Lock on Face"))
            {
                activeFace = Selection.activeGameObject;
                faceRotation = activeFace.transform.rotation.eulerAngles;

                Debug.Log(activeFace.name);
            }
        }

        private void PlaceButton()
        {
            if (GUILayout.Button("Place New Level"))
            {
                GameObject newObj = Instantiate(levelCube);

                newObj.transform.parent = activeFace.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0);

                newObj.transform.rotation = Quaternion.Euler(faceRotation);

                newObj.transform.position = activeFace.transform.position;

                WorldmapManager.instance.levelCubes.Add(newObj);

                Selection.activeGameObject = newObj;
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
}
