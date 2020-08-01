using Amazon.ElasticMapReduce.Model;
using Kubika.CustomLevelEditor;
using System.Collections.Generic;

namespace Kubika.Saving
{
    [System.Serializable]
    public class DevEditorData
    {
        public string levelName;
        public string Kubicode;
        public Biomes biome;
        public int minimumMoves;
        public bool lockRotate;
        public List<Node> nodesToSave;
        public List<Decor> decorToSave;
    }

    [System.Serializable]
    public class UserEditorData
    {
        public string creatorId;
        public string levelName;
        public string creatorDifficulty;

        public int minimumMoves; // necessary ????
        public bool lockRotate;
        public Biomes biome;

        public List<Node> nodesToSave;
        public List<Decor> decorToSave;
    }

    //user info file is used to store all of the user's level names, which you can then use to find the files
    [System.Serializable]
    public class UserLevels
    {
        public int numberOfUserLevels;
        public List<string> levelNames  = new List<string>();
        public List<string> uploadedLevels = new List<string>(); // used to check which levels have previously been uploaded
    }


    [System.Serializable]
    public class DynamoDBInfo
    {
        public List<int> listOfIndexes = new List<int>();
        public List<int> backupListOfIndexes = new List<int>();
        public int lastIdUsed;
    }

    [System.Serializable]
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
