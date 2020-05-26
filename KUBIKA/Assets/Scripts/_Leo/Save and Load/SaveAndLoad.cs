﻿using Kubika.Game;
using Kubika.CustomLevelEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using Kubika.Gam;

namespace Kubika.Saving
{
    public class SaveAndLoad : MonoBehaviour
    {
        private static SaveAndLoad _instance;
        public static SaveAndLoad instance { get { return _instance; } }

        //a list of the nodes in grid node that have cubes on them
        List<Node> activeNodes = new List<Node>();

        Node currentNode;

        LevelEditorData levelData;

        public GameObject cubePrefab;

        public string currentOpenLevelName;
        public string currentKubicode;
        public bool currentLevelLockRotate;
        public int currentMinimumMoves;
        public Biomes currentBiome; //sets itself on save

        public bool finishedBuilding = false;

        _Grid grid;

        private void Awake()
        {
            if (_instance != null && _instance != this) Destroy(this);
            else _instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            CreateEditorData();
        }

        private LevelEditorData CreateEditorData()
        {
            levelData = new LevelEditorData();
            levelData.nodesToSave = new List<Node>();
            return levelData;
        }

        public void DevSavingLevel(string levelName, string kubiCode, bool rotateLock, int minimumMoves = 0, bool testLevel = false)
        {
            for (int i = 0; i < _Grid.instance.kuboGrid.Length; i++)
            {
                if (_Grid.instance.kuboGrid[i].cubeOnPosition != null) activeNodes.Add(_Grid.instance.kuboGrid[i]);
            }

            //storing data in levelDataFile
            levelData.levelName = levelName;
            levelData.biome = _MaterialCentral.instance.staticIndex;
            levelData.lockRotate = rotateLock;
            levelData.minimumMoves = minimumMoves;
            levelData.Kubicode = kubiCode;

            currentOpenLevelName = levelName;
            currentKubicode = kubiCode;
            currentBiome = _MaterialCentral.instance.staticIndex;
            currentLevelLockRotate = rotateLock;
            currentMinimumMoves = minimumMoves;

            foreach (Node node in activeNodes)
            {
                node.savedCubeType = Node.ConvertTypeToString(node.cubeType);
                levelData.nodesToSave.Add(node);
            }

            string json = JsonUtility.ToJson(levelData);
            string folder;

            if (!testLevel) folder = Application.dataPath + "/Resources/MainLevels";
            else folder = Application.dataPath + "/Resources/TestLevels";

            string levelFile = "";

            if (levelName != "") levelFile = levelName + ".json";
            else levelFile = "New_Level.json";

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, levelFile);

            if (File.Exists(path)) File.Delete(path);

            File.WriteAllText(path, json);

            levelData.nodesToSave.Clear();
            activeNodes.Clear();

            Debug.Log("Level Saved by Dev at " + path);
        }

        public void DevSavingCurrentLevel()
        {
            levelData.levelName = currentOpenLevelName;
            levelData.Kubicode = currentKubicode;
            levelData.biome = currentBiome;
            levelData.lockRotate = currentLevelLockRotate;
            levelData.minimumMoves = currentMinimumMoves;

            DevSavingLevel(currentOpenLevelName, currentKubicode, currentLevelLockRotate, currentMinimumMoves);
        }

        public void DevLoadLevel(string levelName)
        {
            Debug.Log("Dev is Loading a Level !");
            string folder = Application.dataPath + "/Resources/MainLevels";

            string levelFile = levelName + ".json";

            string path = Path.Combine(folder, levelFile);

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                levelData = JsonUtility.FromJson<LevelEditorData>(json);

                ExtractAndRebuildLevel(levelData);
            }

            currentOpenLevelName = levelData.levelName;
            currentKubicode = levelData.Kubicode;
            currentBiome = levelData.biome;
            currentLevelLockRotate = levelData.lockRotate;
            currentMinimumMoves = levelData.minimumMoves;

            levelData.nodesToSave.Clear();
            activeNodes.Clear();
        }

        public void UserSavingLevel(string levelName)
        {
            for (int i = 0; i < _Grid.instance.kuboGrid.Length; i++)
            {
                if (_Grid.instance.kuboGrid[i].cubeOnPosition != null) activeNodes.Add(_Grid.instance.kuboGrid[i]);
            }

            //storing data in levelDataFile
            levelData.levelName = levelName;
            levelData.biome = _MaterialCentral.instance.staticIndex;

            currentOpenLevelName = levelName;

            foreach (Node node in activeNodes)
            {
                node.savedCubeType = Node.ConvertTypeToString(node.cubeType);
                levelData.nodesToSave.Add(node);
            }

            string json = JsonUtility.ToJson(levelData);
            string levelFile = levelName + ".json";

            string folder = Application.persistentDataPath + "/UserLevels";

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, levelFile);

            if (File.Exists(path)) File.Delete(path);

            File.WriteAllText(path, json);

            UserLevelFiles.AddNewUserLevel(levelName);
            LevelsManager.instance.RefreshUserLevels();

            levelData.nodesToSave.Clear();
            activeNodes.Clear();

            Debug.Log("Level Saved by User at " + path);
        }

        public void UserSavingCurrentLevel()
        {
            levelData.levelName = currentOpenLevelName;
            levelData.Kubicode = currentKubicode;
            levelData.biome = currentBiome;

            UserSavingLevel(currentOpenLevelName);
        }

        public void UserLoadLevel(string levelName)
        {
            Debug.Log("User is Loading a Level !");

            string folder = Application.persistentDataPath + "/UserLevels";
            string levelFile = levelName + ".json";
            string path = Path.Combine(folder, levelFile);


            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                levelData = JsonUtility.FromJson<LevelEditorData>(json);

                ExtractAndRebuildLevel(levelData);
            }

            currentOpenLevelName = levelData.levelName;
            currentKubicode = levelData.Kubicode;
            currentBiome = levelData.biome;

            levelData.nodesToSave.Clear();
            activeNodes.Clear();
        }

        public void UserDeleteLevel(string levelName)
        {
            UserLevelFiles.DeleteUserLevel(levelName);
            LevelsManager.instance.RefreshUserLevels();
        }

        public void ExtractAndRebuildLevel(LevelEditorData recoveredData)
        {
            finishedBuilding = false;

            grid = _Grid.instance;

            // start by resetting the grid's nodes to their base states
            grid.ResetIndexGrid();

            foreach (Node recoveredNode in recoveredData.nodesToSave)
            {
                // EXTREMELY IMPORTANT -> CONVERTS THE CUBE'S TYPE FROM STRING TO ENUM
                recoveredNode.cubeType = Node.ConvertStringToCubeType(recoveredNode.savedCubeType);

                //Set the universe textures
                _MaterialCentral.instance.ChangeUniverse(recoveredData.biome);

                currentNode = recoveredNode;

                GameObject newCube = Instantiate(cubePrefab);

                _Grid.instance.placedObjects.Add(newCube);

                // get the kuboGrid and set the information on each of the nodes
                SetNodeInfo(newCube, recoveredNode.nodeIndex, recoveredNode.worldPosition, recoveredNode.worldRotation, recoveredNode.facingDirection, recoveredNode.cubeLayers, recoveredNode.cubeType);

                // check the node's cube type and setup the relevant cube and its transform + individual information
                switch (recoveredNode.cubeType)
                {
                    case CubeTypes.FullStaticCube:
                        newCube.AddComponent(typeof(StaticCube));
                        StaticCube fullStaticCube = newCube.GetComponent<StaticCube>();
                        SetCubeInfo(fullStaticCube as _CubeBase, CubeLayers.cubeFull, CubeTypes.FullStaticCube, true);
                        break;

                    case CubeTypes.EmptyStaticCube:
                        newCube.AddComponent(typeof(StaticCube));
                        StaticCube emptyCube = newCube.GetComponent<StaticCube>();
                        SetCubeInfo(emptyCube as _CubeBase, CubeLayers.cubeFull, CubeTypes.EmptyStaticCube, true);
                        break;

                    case CubeTypes.TopStaticCube:
                        newCube.AddComponent(typeof(StaticCube));
                        StaticCube topStaticCube = newCube.GetComponent<StaticCube>();
                        SetCubeInfo(topStaticCube as _CubeBase, CubeLayers.cubeFull, CubeTypes.TopStaticCube, true);
                        break;

                    case CubeTypes.CornerStaticCube:
                        newCube.AddComponent(typeof(StaticCube));
                        StaticCube cornerStaticCube = newCube.GetComponent<StaticCube>();
                        SetCubeInfo(cornerStaticCube as _CubeBase, CubeLayers.cubeFull, CubeTypes.CornerStaticCube, true);
                        break;

                    case CubeTypes.TripleStaticCube:
                        newCube.AddComponent(typeof(StaticCube));
                        StaticCube tripleStaticCube = newCube.GetComponent<StaticCube>();
                        SetCubeInfo(tripleStaticCube as _CubeBase, CubeLayers.cubeFull, CubeTypes.TripleStaticCube, true);
                        break;

                    case CubeTypes.QuadStaticCube:
                        newCube.AddComponent(typeof(StaticCube));
                        StaticCube quadStaticCube = newCube.GetComponent<StaticCube>();
                        SetCubeInfo(quadStaticCube as _CubeBase, CubeLayers.cubeFull, CubeTypes.QuadStaticCube, true);
                        break;

                    case CubeTypes.MoveableCube:
                        newCube.AddComponent(typeof(MoveableCube));
                        MoveableCube moveableCube = newCube.GetComponent<MoveableCube>();
                        SetCubeInfo(moveableCube as _CubeBase, CubeLayers.cubeMoveable, CubeTypes.MoveableCube, false);
                        break;

                    case CubeTypes.BaseVictoryCube:
                        newCube.AddComponent(typeof(BaseVictoryCube));
                        BaseVictoryCube baseVictoryCube = newCube.GetComponent<BaseVictoryCube>();
                        SetCubeInfo(baseVictoryCube as _CubeBase, CubeLayers.cubeMoveable, CubeTypes.BaseVictoryCube, false);
                        break;

                    case CubeTypes.ConcreteVictoryCube:
                        newCube.AddComponent(typeof(ConcreteVictoryCube));
                        ConcreteVictoryCube concreteVictoryCube = newCube.GetComponent<ConcreteVictoryCube>();
                        SetCubeInfo(concreteVictoryCube as _CubeBase, CubeLayers.cubeMoveable, CubeTypes.ConcreteVictoryCube, false);
                        break;

                    case CubeTypes.BombVictoryCube:
                        newCube.AddComponent(typeof(BombVictoryCube));
                        BombVictoryCube bombVictoryCube = newCube.GetComponent<BombVictoryCube>();
                        SetCubeInfo(bombVictoryCube as _CubeBase, CubeLayers.cubeMoveable, CubeTypes.BombVictoryCube, false);
                        break;

                    case CubeTypes.SwitchVictoryCube:
                        newCube.AddComponent(typeof(SwitchVictoryCube));
                        SwitchVictoryCube switchVictoryCube = newCube.GetComponent<SwitchVictoryCube>();
                        SetCubeInfo(switchVictoryCube as _CubeBase, CubeLayers.cubeFull, CubeTypes.SwitchVictoryCube, true);
                        break;

                    case CubeTypes.DeliveryCube:
                        newCube.AddComponent(typeof(DeliveryCube));
                        DeliveryCube deliveryCube = newCube.GetComponent<DeliveryCube>();
                        SetCubeInfo(deliveryCube as _CubeBase, CubeLayers.cubeFull, CubeTypes.DeliveryCube, true);
                        break;

                    case CubeTypes.BlueElevatorCube:
                        newCube.AddComponent(typeof(ElevatorCube));
                        ElevatorCube blueElevatorCube = newCube.GetComponent<ElevatorCube>();
                        blueElevatorCube.isGreen = false;
                        SetCubeInfo(blueElevatorCube as _CubeBase, CubeLayers.cubeFull, CubeTypes.BlueElevatorCube, false);
                        break;

                    case CubeTypes.GreenElevatorCube:
                        newCube.AddComponent(typeof(ElevatorCube));
                        ElevatorCube greenElevatorCube = newCube.GetComponent<ElevatorCube>();
                        greenElevatorCube.isGreen = true;
                        SetCubeInfo(greenElevatorCube as _CubeBase, CubeLayers.cubeFull, CubeTypes.GreenElevatorCube, false);
                        break;

                    case CubeTypes.ConcreteCube:
                        newCube.AddComponent(typeof(ConcreteCube));
                        ConcreteCube concreteCube = newCube.GetComponent<ConcreteCube>();
                        SetCubeInfo(concreteCube as _CubeBase, CubeLayers.cubeMoveable, CubeTypes.ConcreteCube, false);
                        break;

                    case CubeTypes.BombCube:
                        newCube.AddComponent(typeof(BombCube));
                        BombCube bombCube = newCube.GetComponent<BombCube>();
                        SetCubeInfo(bombCube as _CubeBase, CubeLayers.cubeMoveable, CubeTypes.BombCube, false);
                        break;

                    case CubeTypes.TimerCube:
                        newCube.AddComponent(typeof(TimerCube));
                        TimerCube timerCube = newCube.GetComponent<TimerCube>();
                        SetCubeInfo(timerCube as _CubeBase, CubeLayers.cubeFull, CubeTypes.TimerCube, true);
                        break;

                    case CubeTypes.SwitchButton:
                        newCube.AddComponent(typeof(SwitchButton));
                        SwitchButton switchButton = newCube.GetComponent<SwitchButton>();
                        SetCubeInfo(switchButton as _CubeBase, CubeLayers.cubeFull, CubeTypes.SwitchButton, true);
                        break;

                    case CubeTypes.SwitchCube:
                        newCube.AddComponent(typeof(SwitchCube));
                        SwitchCube switchCube = newCube.GetComponent<SwitchCube>();
                        SetCubeInfo(switchCube as _CubeBase, CubeLayers.cubeFull, CubeTypes.SwitchCube, true);
                        break;

                    case CubeTypes.RotatorRightTurner:
                        newCube.AddComponent(typeof(RotateRightCube));
                        RotateRightCube rotatorRightTurn = newCube.GetComponent<RotateRightCube>();
                        SetCubeInfo(rotatorRightTurn as _CubeBase, CubeLayers.cubeFull, CubeTypes.RotatorRightTurner, true);
                        break;

                    case CubeTypes.RotatorLeftTurner:
                        newCube.AddComponent(typeof(RotateLeftCube));
                        RotateLeftCube rotatorLeftTurn = newCube.GetComponent<RotateLeftCube>();
                        SetCubeInfo(rotatorLeftTurn as _CubeBase, CubeLayers.cubeFull, CubeTypes.RotatorLeftTurner, true);
                        break;

                    case CubeTypes.RotatorLocker:
                        newCube.AddComponent(typeof(RotatorLocker));
                        RotatorLocker rotatorLocker = newCube.GetComponent<RotatorLocker>();
                        SetCubeInfo(rotatorLocker as _CubeBase, CubeLayers.cubeFull, CubeTypes.RotatorLocker, true);
                        break;

                    case CubeTypes.ChaosBall:
                        newCube.AddComponent(typeof(ChaosBall));
                        ChaosBall chaosBall = newCube.GetComponent<ChaosBall>();
                        SetCubeInfo(chaosBall as _CubeBase, CubeLayers.cubeMoveable, CubeTypes.ChaosBall, false);
                        break;

                    default:
                        //set epmty cubes as cubeEmpty
                        grid.kuboGrid[recoveredNode.nodeIndex - 1].cubeLayers = CubeLayers.cubeEmpty;
                        grid.kuboGrid[recoveredNode.nodeIndex - 1].cubeType = CubeTypes.None;
                        break;
                }
            }

            finishedBuilding = true;
        }

        void SetNodeInfo(GameObject newCube, int nodeIndex, Vector3 worldPosition, Vector3 worldRotation, FacingDirection facingDirection, CubeLayers cubeLayers, CubeTypes cubeTypes)
        {
            grid.kuboGrid[currentNode.nodeIndex - 1].cubeOnPosition = newCube;
            grid.kuboGrid[currentNode.nodeIndex - 1].nodeIndex = nodeIndex;
            grid.kuboGrid[currentNode.nodeIndex - 1].worldPosition = worldPosition;
            grid.kuboGrid[currentNode.nodeIndex - 1].worldRotation = worldRotation;
            grid.kuboGrid[currentNode.nodeIndex - 1].facingDirection = facingDirection;
            grid.kuboGrid[currentNode.nodeIndex - 1].cubeLayers = cubeLayers;
            grid.kuboGrid[currentNode.nodeIndex - 1].cubeType = cubeTypes;
        }

        //set the node's information relevant to the cube type, then send the Transform information to the cube
        void SetCubeInfo(_CubeBase cube, CubeLayers cubeLayers, CubeTypes cubeTypes, bool _static)
        {
            cube.myIndex = currentNode.nodeIndex;
            cube.myCubeType = cubeTypes;
            cube.myCubeLayer = cubeLayers;
            cube.isStatic = _static;
            cube.facingDirection = currentNode.facingDirection;

            //sets rotation and position according to the values of the relevant node in kuboGrid
            cube.OnLoadSetTransform();
        }
    }
}