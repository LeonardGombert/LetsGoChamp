using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Kubika.Game;
using Sirenix.OdinInspector.Editor;

[CustomEditor(typeof(LevelsManager))]
public class LevelsQueueEditor : OdinEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LevelsManager levelsQueue = (LevelsManager)target;

        if (GUILayout.Button("Load Next Level"))
        {
            levelsQueue._LoadNextLevel();
        }
    }
}
