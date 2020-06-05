using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Kubika.Saving
{
    public class UserLevelFiles : MonoBehaviour
    {
        // use this function to extract information from text file asset and into a LevelFile
        public static LevelFile ConvertToLevelInfo(TextAsset levelFile)
        {
            LevelFile levelInfo = new LevelFile();
            LevelEditorData levelData = JsonUtility.FromJson<LevelEditorData>(levelFile.ToString());

            levelInfo.levelFile = levelFile;
            levelInfo.levelName = levelData.levelName;
            levelInfo.levelBiome = levelData.biome;
            levelInfo.kubicode = levelData.Kubicode;
            levelInfo.minimumMoves = levelData.minimumMoves;
            levelInfo.lockRotate = levelData.lockRotate;

            return levelInfo;
        }

        //use to initialize if null, or refresh an existing user info file
        public static void InitializeUserLevelInfo(UserLevels newUserLevels = default)
        {
            if (newUserLevels == default) newUserLevels = new UserLevels();

            string json = JsonUtility.ToJson(newUserLevels);
            string folder = Application.persistentDataPath + "/UserLevels";
            string levelFile = "_UserLevelInfo.json";

            string path = Path.Combine(folder, levelFile);

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            if (File.Exists(path)) File.Delete(path);
            File.WriteAllText(path, json);
        }

        //create a new level file
        public static void AddNewUserLevel(string levelName)
        {
            string folder = Application.persistentDataPath + "/UserLevels";
            string file = "_UserLevelInfo.json";
            string path = Path.Combine(folder, file);

            if (!File.Exists(path)) InitializeUserLevelInfo();

            string json = File.ReadAllText(path);
            UserLevels userLevels = JsonUtility.FromJson<UserLevels>(json);

            userLevels.numberOfUserLevels++;
            userLevels.levelNames.Add(levelName);

            InitializeUserLevelInfo(userLevels);
        }

        //delete an existing user level file
        public static void DeleteUserLevel(string levelName)
        {
            string folder = Application.persistentDataPath + "/UserLevels";
            string levelsInfo = Application.persistentDataPath + "/UserLevels/UserLevelInfo";
            UserLevels userLevels = JsonUtility.FromJson<UserLevels>(levelsInfo);

            userLevels.levelNames.Remove(levelName);
            userLevels.numberOfUserLevels--;

            InitializeUserLevelInfo(userLevels);
        }

        //use to get all the level names from the user level info file
        public static List<string> GetUserLevelNames()
        {
            UserLevels userLevels;
            List<string> levelsToLoad = new List<string>();

            string folder = Application.persistentDataPath + "/UserLevels";
            string file = "_UserLevelInfo.json";
            string path = Path.Combine(folder, file);

            if (!File.Exists(path)) InitializeUserLevelInfo();
            string defaultMessage = "You haven't built any levels yet !";
            
            string json = File.ReadAllText(path);

            userLevels = JsonUtility.FromJson<UserLevels>(json);

            if (userLevels.numberOfUserLevels <= 0) levelsToLoad.Add(defaultMessage);
            else levelsToLoad = userLevels.levelNames;

            return levelsToLoad;
        }
    }
}