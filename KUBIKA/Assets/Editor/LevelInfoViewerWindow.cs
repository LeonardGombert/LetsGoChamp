using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
[CanEditMultipleObjects]
public class LevelInfoViewerWindow : EditorWindow
{
    public static LevelInfoViewerWindow instance
    {
        get { return GetWindow<LevelInfoViewerWindow>(); }
    }

    private string levelName;

    [MenuItem("Tools/Level Info Viewer")]
    static void Init()
    {
        var window = GetWindow<LevelInfoViewerWindow>();
        window.position = new Rect(0, 0, 180, 80);
        window.Show();
    }

    void OnGUI()
    {
        LevelName();
        LockRotate();
        MovesToWin();
        SaveFile();
    }

    private void LevelName()
    {
        //levelName = EditorGUILayout.TextArea("Level Name", leveName);
    }

    private void LockRotate()
    {
        throw new NotImplementedException();
    }

    private void MovesToWin()
    {
        throw new NotImplementedException();
    }

    private void SaveFile()
    {
        throw new NotImplementedException();
    }
}
