using Amazon.ElasticMapReduce.Model;
using Kubika.CustomLevelEditor;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Kubika.Saving
{
    [Serializable]
    public class DevEditorData
    {
        public string levelName;
        public string Kubicode;
        public Biomes biome;
        public Difficulty difficulty;
        public int minimumMoves;
        public bool lockRotate;
        public List<Node> nodesToSave;
        public List<Decor> decorToSave;
    }

    [Serializable]
    public class UserEditorData
    {
        public string creatorId;
        public string levelName;
        public Difficulty creatorDifficulty;

        public int minimumMoves; // necessary ????
        public bool lockRotate;
        public Biomes biome;

        public List<Node> nodesToSave;
        public List<Decor> decorToSave;
    }

    [Serializable]
    public struct LevelFile
    {
        public string levelName;
        public string kubicode;
        public Biomes levelBiome;
        public Difficulty difficulty;
        public int minimumMoves;
        public bool lockRotate;
        [HideInInspector] public bool levelIsBeaten; //not saved in the file, but in player progress
        public TextAsset levelFile;
    }

    [Serializable]
    public struct CommunityLevel
    {
        public string creatorId;
        public string levelName;
        public Difficulty creatorDifficulty;

        public int minimumMoves;
        public bool lockRotate;
        public Biomes biome;

        public TextAsset levelFile;
    }

    //user info file is used to store all of the user's level names, which you can then use to find the files
    [Serializable]
    public class UserLevels
    {
        public int numberOfUserLevels;
        public List<string> levelNames  = new List<string>();
        public List<string> uploadedLevels = new List<string>(); // used to check which levels have previously been uploaded
    }


    [Serializable]
    public class DynamoDBInfo
    {
        public List<int> listOfIndexes = new List<int>();
        public List<int> backupListOfIndexes = new List<int>();
        public int lastIdUsed;
    }

    [Serializable]
    public class DynamoReceivedInfo
    {
        public string kubicode = "";
        public string levelName = "";

        public void ClearNames()
        {
            kubicode = "";
            levelName = "";
        }
    }
}
