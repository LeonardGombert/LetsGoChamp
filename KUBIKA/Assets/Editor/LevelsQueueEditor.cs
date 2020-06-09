using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Kubika.Game;
using Sirenix.OdinInspector.Editor;
using System.IO;
using Kubika.Saving;

[CustomEditor(typeof(LevelsManager))]
public class LevelsQueueEditor : OdinEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LevelsManager levelsQueue = (LevelsManager)target;

        /*if (GUILayout.Button("Load Next Level"))
        {
            levelsQueue._LoadNextLevel();
        }*/

        if(GUILayout.Button("Reset Player Progress"))
        {
            string folder = Application.persistentDataPath + "/UserSaves";
            string levelFile = "PlayerProgress";
            string path = Path.Combine(folder, levelFile) + ".json";

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                PlayerProgress loadedPlayerProgress = JsonUtility.FromJson<PlayerProgress>(json);

                loadedPlayerProgress.nextLevelKubicode = "Worl101";
                loadedPlayerProgress.beatenLevels.Clear();
                loadedPlayerProgress.goldenLevels.Clear();

                string newJson = JsonUtility.ToJson(loadedPlayerProgress);
                File.WriteAllText(path, newJson);
            }
        }
    }
}
