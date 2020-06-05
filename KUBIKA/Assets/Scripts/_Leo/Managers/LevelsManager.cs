using Kubika.CustomLevelEditor;
using Kubika.Saving;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Kubika.Game
{
    public class LevelsManager : MonoBehaviour
    {
        private static LevelsManager _instance;
        public static LevelsManager instance { get { return _instance; } }


        #region MAIN LEVELS
        [FoldoutGroup("Biomes")] public List<LevelFile> masterList = new List<LevelFile>();
        [FoldoutGroup("Biomes")] [ShowInInspector] Queue<LevelFile> levelQueue = new Queue<LevelFile>();

        List<List<TextAsset>> listOfLists = new List<List<TextAsset>>();

        [FoldoutGroup("Full Biomes")] [SerializeField] List<TextAsset> biome1 = new List<TextAsset>();

        [FoldoutGroup("Full Biomes")] [SerializeField] List<TextAsset> biome2 = new List<TextAsset>();
        [FoldoutGroup("Full Biomes")] [SerializeField] List<TextAsset> biome3 = new List<TextAsset>();
        [FoldoutGroup("Full Biomes")] [SerializeField] List<TextAsset> biome4 = new List<TextAsset>();
        [FoldoutGroup("Full Biomes")] [SerializeField] List<TextAsset> biome5 = new List<TextAsset>();
        [FoldoutGroup("Full Biomes")] [SerializeField] List<TextAsset> biome6 = new List<TextAsset>();
        [FoldoutGroup("Full Biomes")] [SerializeField] List<TextAsset> biome7 = new List<TextAsset>();
        #endregion

        #region LEVEL EDITOR
        [FoldoutGroup("Level Editor ")] public List<string> levelNames = new List<string>();
        [FoldoutGroup("Level Editor ")] public List<LevelFile> userLevelsList = new List<LevelFile>();
        public string userSceneToTest;
        #endregion

        //which level to load to
        public string loadToKubicode;

        public TextAsset testLevel;

        public TextAsset _levelFile;
        public string _levelName;
        public Biomes _levelBiome;
        public string _Kubicode;
        public int _minimumMoves;
        public bool _lockRotate;
        void Awake()
        {
            if (_instance != null && _instance != this) Destroy(this);
            else _instance = this;

            StartCoroutine(InitializeLevelsList());
        }

        // Start is called before the first frame update
        void Start()
        {
            if (ScenesManager.isLevelEditor) RefreshUserLevels();

            //MOVE THIS SOMEWHERE ELSE, ONLY CALL IF PLAYER PRESSES CONTINUE BUTTON FROM WORLDMAP SCREEN
            loadToKubicode = SaveAndLoad.instance.LoadProgress();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) OpenTestLevel();
        }

        IEnumerator InitializeLevelsList()
        {
            listOfLists.Add(biome1);
            listOfLists.Add(biome2);
            listOfLists.Add(biome3);
            listOfLists.Add(biome4);
            listOfLists.Add(biome5);
            listOfLists.Add(biome6);
            listOfLists.Add(biome7);

            InitializeLists();

            yield return null;
        }

        // Copy all of the individual lists to the master list
        private void InitializeLists()
        {
            masterList.Clear();

            foreach (List<TextAsset> levelFileList in listOfLists)
            {
                foreach (TextAsset level in levelFileList)
                {
                    LevelFile levelInfo = UserLevelFiles.ConvertToLevelInfo(level);
                    masterList.Add(levelInfo);
                }
            }
        }

        // called when Game Scene is loaded, load specific level or set to baseState
        public void BakeLevels()
        {
            for (int i = 0; i < masterList.Count; i++)
            {
                if (masterList[i].kubicode != loadToKubicode) continue;

                else if (masterList[i].kubicode == loadToKubicode)
                {
                    // load a specific level
                    LoadSpecific(i);
                    break;
                }
            }
        }

        //Clear and recreate the levels list starting at the level you want to load
        private void LoadSpecific(int startingIndex)
        {
            levelQueue.Clear();

            for (int i = startingIndex; i < masterList.Count; i++)
            {
                levelQueue.Enqueue(masterList[i]);
            }
            _LoadNextLevel();
        }

        // called when the user selects a level on the worlldmap
        public void SelectLevel(string kubicode)
        {
            LevelFile levelFile = GetMatching(loadToKubicode);

            UIManager.instance.UpdateWMInfo(levelFile.levelName);
            loadToKubicode = kubicode;
        }

        //used to find the matching LevelFile based on KubiCode
        LevelFile GetMatching(string kubiCode)
        {
            LevelFile returnfile = new LevelFile();
            for (int i = 0; i < masterList.Count; i++)
            {
                if (masterList[i].kubicode != kubiCode) continue;
                if (masterList[i].kubicode == kubiCode) returnfile = masterList[i];
            }

            Debug.Log("I found " + returnfile.levelName);
            return returnfile;
        }


        #region //USER LEVEL EDITOR
        public void RefreshUserLevels()
        {
            levelNames = UserLevelFiles.GetUserLevelNames();

            while (UIManager.instance == null) return;
            UIManager.instance.playerLevelsDropdown.ClearOptions();

            foreach (string levelName in levelNames)
            {
                //the info used to fill out the dropdown tabs are provided by the user level files
                UIManager.instance.playerLevelsDropdown.options.Add(new Dropdown.OptionData(levelName));
            }

            UIManager.instance.playerLevelsDropdown.RefreshShownValue();
        }

        public void OpenTestLevel()
        {
            Debug.Log("Loading test level");
            SaveAndLoad.instance.UserLoadLevel(userSceneToTest);
        }
        #endregion

        #region //LOAD LEVEL PIPELINE
        public void _LoadNextLevel()
        {
            //get info
            GetNextLevelInfo();

            //load level
            StartCoroutine(LoadLevel());

            //remove from queue
            DequeueLevel();
        }

        // Get the next level's information and remove it from the queue when you load the next level
        void GetNextLevelInfo()
        {
            _levelName = levelQueue.Peek().levelName;
            _levelBiome = levelQueue.Peek().levelBiome;
            _Kubicode = levelQueue.Peek().kubicode;
            _levelFile = levelQueue.Peek().levelFile;
            _minimumMoves = levelQueue.Peek().minimumMoves;
            _lockRotate = levelQueue.Peek().lockRotate;
        }

        // Load the next level (extract the file)
        IEnumerator LoadLevel()
        {
            _KUBRotation.instance.ResetRotation();
            _FeedBackManager.instance.ResetVictoryFX();

            if (_lockRotate) UIManager.instance.TurnOffRotate();
            else UIManager.instance.TurnOnRotate();

            string json = _levelFile.ToString();

            LevelEditorData levelData = JsonUtility.FromJson<LevelEditorData>(json);

            SaveAndLoad.instance.finishedBuilding = false;

            SaveAndLoad.instance.ExtractAndRebuildLevel(levelData);

            while (!SaveAndLoad.instance.finishedBuilding) yield return null;

            // once the level is loaded 

            VictoryConditionManager.instance.CheckVictoryCubes();
            _DataManager.instance.GameSet();
            _MaterialCentral.instance.MaterialSet();
            _MaterialCentral.instance.ChangeUniverse(_levelBiome);

            UIManager.instance.GameCanvasPriority();

            yield return null;
        }

        private void DequeueLevel()
        {
            levelQueue.Dequeue();
        }
        #endregion

        public void RestartLevel()
        {
            StartCoroutine(LoadLevel());
        }
    }
}

namespace Kubika.Saving
{
    [Serializable]
    public struct LevelFile
    {
        public string levelName;
        public string kubicode;
        public Biomes levelBiome;
        public int minimumMoves;
        public bool lockRotate;
        public TextAsset levelFile;
    }
}