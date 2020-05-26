using Kubika.Game;
using Sirenix.OdinInspector.Editor;
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(_CubeMove))]
public class CubeMoveEditor : OdinEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _CubeMove cubeScript = (_CubeMove)target;

        BaseVictoryCube timerCube = cubeScript.gameObject.GetComponent<BaseVictoryCube>();

        if(timerCube != null)
        {
            HideAllValues();
        }
    }

    private void HideAllValues()
    {
        throw new NotImplementedException();
    }
}
