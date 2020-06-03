using Kubika.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(_CalculTripleMatrix))]
public class MatrixCalculEditor : Editor
{
    public override void OnInspectorGUI()
    {
        _CalculTripleMatrix calculTripleMatrix = (_CalculTripleMatrix)target;
        
        base.OnInspectorGUI();

        if (GUILayout.Button("Calculate Data Bank")) calculTripleMatrix.AssignDataBank();

        EditorUtility.SetDirty(calculTripleMatrix.indexBankScriptable);
    }
}