using Kubika.CustomLevelEditor;
using System.Collections.Generic;

namespace Kubika.Saving
{
    [System.Serializable]
    public class LevelEditorData
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
    public class UserLevels
    {
        public int numberOfUserLevels;
        public List<string> levelNames  = new List<string>();
    }
}
