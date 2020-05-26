using Kubika.CustomLevelEditor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(_Grid))]
public class GridEditor : OdinEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _Grid grid = (_Grid)target;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Redraw Grid")) grid.RefreshGrid();
        if (GUILayout.Button("Clear Level")) grid.ResetIndexGrid();
        EditorGUILayout.EndHorizontal();
    }
}
