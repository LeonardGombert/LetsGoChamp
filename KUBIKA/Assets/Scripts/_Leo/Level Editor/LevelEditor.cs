using Kubika.Gam;
using Kubika.Game;
using Kubika.Saving;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Kubika.CustomLevelEditor
{
    public class LevelEditor : MonoBehaviour
    {
        private static LevelEditor _instance;
        public static LevelEditor instance { get { return _instance; } }

        RaycastHit hit;
        int hitIndex;
        int moveWeight;
        _Grid grid;

        _CubeBase currentHitCube;

        public CubeTypes currentCube;
        public FacingDirection facingDirection;

        List<RaycastHit> placeHits = new List<RaycastHit>();
        List<RaycastHit> deleteHits = new List<RaycastHit>();

        //PLACING CUBES
        private bool placeMultiple = true;
        Vector3 userInputPosition;

        public LevelSetup levelSetup;
        public List<TextAsset> prefabLevels = new List<TextAsset>();

        public bool isPlacing, isDeleting, isRotating;

        // 1 = highly sensitive, 2 = less, etc.
        public int rotationDampen;
        private bool placingButton;

        private void Awake()
        {
            if (_instance != null && _instance != this) Destroy(this);
            else _instance = this;

            currentCube = CubeTypes.FullStaticCube;
        }

        private void Start()
        {
            grid = _Grid.instance;
        }

        private void Update()
        {
            EditorLoop();
        }

        private void EditorLoop()
        {
            //make sure the user isn't interacting with a UI element
            if (!EventSystem.current.IsPointerOverGameObject()
                || !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                if (ScenesManager.isLevelEditor || ScenesManager.isDevScene)
                {
                    DetectInputs();
                    if (!placingButton) CubeSelection();
                }
            }
            else return;
        }

        #region PLACE AND REMOVE CUBES
        public void SwitchAction(string received)
        {
            isPlacing = false;
            isDeleting = false;
            isRotating = false;

            switch (received)
            {
                case "isPlacing":
                    isPlacing = true;
                    break;

                case "isDeleting":
                    isDeleting = true;
                    break;

                case "isRotating":
                    isRotating = true;
                    break;

                default:
                    break;
            }
        }
        private void DetectInputs()
        {
            /*
            //add the cubes you hit to a list of RaycastHits
            if (Input.GetMouseButton(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary)
            {
                CheckUserPlatform();

                if (Physics.Raycast(Camera.main.ScreenPointToRay(userInputPosition), out hit))
                    if (!placeHits.Contains(hit)) placeHits.Add(hit);
            }

            //when the user releases the mouse, place all the cubes at once
            if (Input.GetMouseButtonUp(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                foreach (RaycastHit hit in placeHits) PlaceCube(hit);
                placeHits.Clear();
            }

            //add the cubes you hit to a list of RaycastHits
            if (Input.GetMouseButton(1))
            {
                CheckUserPlatform();

                if (Physics.Raycast(Camera.main.ScreenPointToRay(userInputPosition), out hit))
                    if (!deleteHits.Contains(hit)) deleteHits.Add(hit);
            }

            //when the user releases the mouse, delete alll cubes at once
            if (Input.GetMouseButtonUp(1) || Input.touchCount > 1 && Input.GetTouch(1).phase == TouchPhase.Ended)
            {
                foreach (RaycastHit hit in deleteHits) DeleteCube(hit);
                deleteHits.Clear();
            }*/

            if (isPlacing)
            {
                //single click and place
                if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    GetUserPlatform();
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(userInputPosition), out hit)) PlaceCube(hit);
                    hitIndex = 0;
                }
            }

            if (isDeleting)
            {
                //single click and delete
                if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    GetUserPlatform();
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(userInputPosition), out hit)) DeleteCube(hit);
                    hitIndex = 0;
                }
            }

            if (isRotating)
            {
                //select cube and set the rotation to the difference between cube and finger w/ a preview
                if (Input.GetMouseButton(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    GetUserPlatform();
                    Camera.main.ScreenPointToRay(userInputPosition);
                }

                // one release, set the rotation
                if (Input.GetMouseButtonUp(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(userInputPosition), out hit)) RotateCube(hit);
                    hitIndex = 0;
                }
            }
        }

        private void PlaceCube(RaycastHit hit)
        {
            //get the index of the cube you just hit
            currentHitCube = hit.collider.gameObject.GetComponent<_CubeBase>();
            hitIndex = currentHitCube.myIndex;

            //calculate where you're placing the new cube
            CubeOffset(hit.normal);

            //if the current node doesn't have a cube on it, place a new cube
            if (currentCube != CubeTypes.None && IndexIsEmpty())
            {
                //create a new Cube and add the CubeObject component to store its index
                GameObject newCube = Instantiate(SaveAndLoad.instance.cubePrefab);
                OnPlaceCube(newCube);
            }
        }

        private void DeleteCube(RaycastHit hit)
        {
            //get the index of the cube you just hit
            currentHitCube = hit.collider.gameObject.GetComponent<_CubeBase>();
            hitIndex = currentHitCube.myIndex;

            moveWeight = 0; //why is this here ? 

            //if there is a cube
            if (!IndexIsEmpty())
            {
                currentHitCube.ResetCubeInfo();
                //reset the grid info
                Destroy(currentHitCube.gameObject);
            }

            grid.placedCubes.Remove(hit.collider.gameObject);
            //if there are no more gridObjects, redraw the grid
            if (grid.placedCubes.Count == 0) grid.RefreshGrid();
        }

        private void RotateCube(RaycastHit hit)
        {
            //if you haven't hit a Timer Cube
            if(!IncrementTimer(hit))
            {
                currentHitCube = hit.collider.gameObject.GetComponent<_CubeBase>();
                hitIndex = currentHitCube.myIndex;

                Quaternion newRotation;

                #region rotation
                /*if (hitIndex != 0 && hit.collider.gameObject != null)
                {
                    int rotationX = (int)CustomScaler.Scale((int)userInputPosition.x, 0, Camera.main.pixelWidth, -360, 360);
                    rotationX = rotationX / 10;

                    int rotationY = (int)CustomScaler.Scale((int)userInputPosition.y, 0, Camera.main.pixelHeight, -360, 360);
                    rotationY = rotationY / 10;

                    if (rotationX % 9 * rotationDampen == 0)
                    {
                        Vector3 rotationVector = new Vector3(rotationX * 10,
                            hit.collider.gameObject.transform.rotation.y,
                            hit.collider.gameObject.transform.rotation.z);

                        newRotation = Quaternion.Euler(rotationVector);

                        Debug.Log(rotationVector);
                        hit.collider.gameObject.transform.rotation = newRotation;
                    }

                    if (rotationY % 9 * rotationDampen == 0)
                    {
                        Vector3 rotationVector = new Vector3(hit.collider.gameObject.transform.rotation.y,
                            rotationY * 10,
                            hit.collider.gameObject.transform.rotation.z);

                        newRotation = Quaternion.Euler(rotationVector);

                        Debug.Log(rotationVector);
                        hit.collider.gameObject.transform.rotation = newRotation;
                    }
                }*/
                #endregion

                if (hitIndex != 0 && hit.collider.gameObject != null)
                {
                    if (currentHitCube.myCubeType == CubeTypes.CornerStaticCube)
                    {
                        //increment the enum if it isn't the last one, else reset it to the first
                        if (currentHitCube.facingDirection < FacingDirection.downright) currentHitCube.facingDirection++;
                        else currentHitCube.facingDirection = FacingDirection.up;
                    }

                    else
                    {
                        //increment the enum if it isn't the last one, else reset it to the first
                        if (currentHitCube.facingDirection < FacingDirection.left) currentHitCube.facingDirection++;
                        else currentHitCube.facingDirection = FacingDirection.up;
                    }


                    //returns the coordinates that the cube should adopt according to its enum
                    Vector3 rotationVector = CubeFacingDirection.CubeFacing(currentHitCube.facingDirection);

                    //convert the coordinates to a euler angle
                    newRotation = Quaternion.Euler(rotationVector);

                    //assign the quaternion to the cube's transform
                    hit.collider.gameObject.transform.rotation = newRotation;

                    //set the rotation info in node grid
                    currentHitCube.SetRelevantNodeInfo();
                }
            }
        }

        bool IncrementTimer(RaycastHit hit)
        {
            TimerCube timerCube = hit.collider.gameObject.GetComponent<TimerCube>();

            if (timerCube != null)
            {
                if(timerCube.myCubeType == CubeTypes.TimerCube9)
                {
                    timerCube.myCubeType = CubeTypes.TimerCube1;
                    timerCube.timerValue++;
                }

                timerCube.myCubeType++;
                timerCube.timerValue++;
                
                timerCube.SetRelevantNodeInfo();
                return true;
            }

            else return false;
        }
        #endregion

        #region GENERATE BASE GRID
        public void GenerateBaseGrid()
        {
            switch (levelSetup)
            {
                case LevelSetup.none:
                    string emptyGridJson = prefabLevels.Find(item => item.name.Contains("Empty")).ToString();
                    LevelEditorData emptyGrid = JsonUtility.FromJson<LevelEditorData>(emptyGridJson);
                    SaveAndLoad.instance.ExtractAndRebuildLevel(emptyGrid);
                    break;

                case LevelSetup.BaseGrid:
                    string baseGridJson = prefabLevels.Find(item => item.name.Contains("Base")).ToString();
                    LevelEditorData baseGrid = JsonUtility.FromJson<LevelEditorData>(baseGridJson);
                    SaveAndLoad.instance.ExtractAndRebuildLevel(baseGrid);
                    break;

                case LevelSetup.Plane:
                    string planeJson = prefabLevels.Find(item => item.name.Contains("Plane")).ToString();
                    LevelEditorData planeData = JsonUtility.FromJson<LevelEditorData>(planeJson);
                    SaveAndLoad.instance.ExtractAndRebuildLevel(planeData);
                    break;

                case LevelSetup.RightDoublePlane:
                    string rightJson = prefabLevels.Find(item => item.name.Contains("Right")).ToString();
                    LevelEditorData rightData = JsonUtility.FromJson<LevelEditorData>(rightJson);
                    SaveAndLoad.instance.ExtractAndRebuildLevel(rightData);
                    break;

                case LevelSetup.LeftDoublePlane:
                    string leftJson = prefabLevels.Find(item => item.name.Contains("Left")).ToString();
                    LevelEditorData leftData = JsonUtility.FromJson<LevelEditorData>(leftJson);
                    SaveAndLoad.instance.ExtractAndRebuildLevel(leftData);
                    break;

                default:
                    break;
            }
        }

        #endregion

        #region CHANGE CUBE SELECTION
        private void CubeSelection()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                if (currentCube == CubeTypes.FullStaticCube) currentCube = CubeTypes.ChaosBall;
                else currentCube--;
            }

            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (currentCube == CubeTypes.ChaosBall) currentCube = CubeTypes.FullStaticCube;
                else currentCube++;
            }
        }

        // Set all of the cube's information when it is placed
        private void OnPlaceCube(GameObject newCube)
        {
            grid.placedCubes.Add(newCube);

            switch (currentCube)
            {
                case CubeTypes.FullStaticCube:
                    newCube.AddComponent(typeof(StaticCube));
                    StaticCube staticCube = newCube.GetComponent<StaticCube>();
                    SendInfoToCube(staticCube as _CubeBase, CubeTypes.FullStaticCube, CubeLayers.cubeFull, true);
                    break;

                case CubeTypes.EmptyStaticCube:
                    newCube.AddComponent(typeof(StaticCube));
                    StaticCube emptyStaticCube = newCube.GetComponent<StaticCube>();
                    SendInfoToCube(emptyStaticCube as _CubeBase, CubeTypes.EmptyStaticCube, CubeLayers.cubeFull, true);
                    break;

                case CubeTypes.TopStaticCube:
                    newCube.AddComponent(typeof(StaticCube));
                    StaticCube topStaticCube = newCube.GetComponent<StaticCube>();
                    SendInfoToCube(topStaticCube as _CubeBase, CubeTypes.TopStaticCube, CubeLayers.cubeFull, true);
                    break;

                case CubeTypes.CornerStaticCube:
                    newCube.AddComponent(typeof(StaticCube));
                    StaticCube cornerStaticCube = newCube.GetComponent<StaticCube>();
                    SendInfoToCube(cornerStaticCube as _CubeBase, CubeTypes.CornerStaticCube, CubeLayers.cubeFull, true);
                    break;

                case CubeTypes.TripleStaticCube:
                    newCube.AddComponent(typeof(StaticCube));
                    StaticCube tripleStaticCube = newCube.GetComponent<StaticCube>();
                    SendInfoToCube(tripleStaticCube as _CubeBase, CubeTypes.TripleStaticCube, CubeLayers.cubeFull, true);
                    break;

                case CubeTypes.QuadStaticCube:
                    newCube.AddComponent(typeof(StaticCube));
                    StaticCube quadStaticCube = newCube.GetComponent<StaticCube>();
                    SendInfoToCube(quadStaticCube as _CubeBase, CubeTypes.QuadStaticCube, CubeLayers.cubeFull, true);
                    break;

                case CubeTypes.MoveableCube:
                    newCube.AddComponent(typeof(MoveableCube));
                    MoveableCube moveableCube = newCube.GetComponent<MoveableCube>();
                    SendInfoToCube(moveableCube as _CubeBase, CubeTypes.MoveableCube, CubeLayers.cubeMoveable, false);
                    break;

                case CubeTypes.BaseVictoryCube:
                    newCube.AddComponent(typeof(BaseVictoryCube));
                    BaseVictoryCube victoryCube = newCube.GetComponent<BaseVictoryCube>();
                    SendInfoToCube(victoryCube as _CubeBase, CubeTypes.BaseVictoryCube, CubeLayers.cubeMoveable, false);
                    break;

                case CubeTypes.ConcreteVictoryCube:
                    newCube.AddComponent(typeof(ConcreteVictoryCube));
                    ConcreteVictoryCube concreteVictoryCube = newCube.GetComponent<ConcreteVictoryCube>();
                    SendInfoToCube(concreteVictoryCube as _CubeBase, CubeTypes.ConcreteVictoryCube, CubeLayers.cubeMoveable, false);
                    break;

                case CubeTypes.BombVictoryCube:
                    newCube.AddComponent(typeof(BombVictoryCube));
                    BombVictoryCube bombVictoryCube = newCube.GetComponent<BombVictoryCube>();
                    SendInfoToCube(bombVictoryCube as _CubeBase, CubeTypes.BombVictoryCube, CubeLayers.cubeMoveable, false);
                    break;

                case CubeTypes.SwitchVictoryCube:
                    newCube.AddComponent(typeof(SwitchVictoryCube));
                    SwitchVictoryCube switchVictoryCube = newCube.GetComponent<SwitchVictoryCube>();
                    SendInfoToCube(switchVictoryCube as _CubeBase, CubeTypes.SwitchVictoryCube, CubeLayers.cubeFull, true);
                    break;

                case CubeTypes.DeliveryCube:
                    newCube.AddComponent(typeof(DeliveryCube));
                    DeliveryCube deliveryCube = newCube.GetComponent<DeliveryCube>();
                    SendInfoToCube(deliveryCube as _CubeBase, CubeTypes.DeliveryCube, CubeLayers.cubeFull, true);
                    break;

                case CubeTypes.BlueElevatorCube:
                    newCube.AddComponent(typeof(ElevatorCube));
                    ElevatorCube blueElevatorCube = newCube.GetComponent<ElevatorCube>();
                    blueElevatorCube.isGreen = false;
                    SendInfoToCube(blueElevatorCube as _CubeBase, CubeTypes.BlueElevatorCube, CubeLayers.cubeFull, false);
                    break;

                case CubeTypes.GreenElevatorCube:
                    newCube.AddComponent(typeof(ElevatorCube));
                    ElevatorCube greenElevatorCube = newCube.GetComponent<ElevatorCube>();
                    greenElevatorCube.isGreen = true;
                    SendInfoToCube(greenElevatorCube as _CubeBase, CubeTypes.GreenElevatorCube, CubeLayers.cubeFull, false);
                    break;

                case CubeTypes.ConcreteCube:
                    newCube.AddComponent(typeof(ConcreteCube));
                    ConcreteCube concreteCube = newCube.GetComponent<ConcreteCube>();
                    SendInfoToCube(concreteCube as _CubeBase, CubeTypes.ConcreteCube, CubeLayers.cubeMoveable, false);
                    break;

                case CubeTypes.BombCube:
                    newCube.AddComponent(typeof(BombCube));
                    BombCube mineCube = newCube.GetComponent<BombCube>();
                    SendInfoToCube(mineCube as _CubeBase, CubeTypes.BombCube, CubeLayers.cubeMoveable, false);
                    break;

                case CubeTypes.TimerCube:
                    newCube.AddComponent(typeof(TimerCube));
                    TimerCube timerCube = newCube.GetComponent<TimerCube>();
                    SendInfoToCube(timerCube as _CubeBase, CubeTypes.TimerCube1, CubeLayers.cubeFull, true); //spawn TimerCube1 as it is base 
                    timerCube.timerValue = 1;
                    break;

                case CubeTypes.SwitchCube:
                    newCube.AddComponent(typeof(SwitchCube));
                    SwitchCube switchCube = newCube.GetComponent<SwitchCube>();
                    SendInfoToCube(switchCube as _CubeBase, CubeTypes.SwitchCube, CubeLayers.cubeFull, true);
                    break;

                case CubeTypes.SwitchButton:
                    placingButton = false;
                    newCube.AddComponent(typeof(SwitchButton));
                    SwitchButton switchButton = newCube.GetComponent<SwitchButton>();
                    SendInfoToCube(switchButton as _CubeBase, CubeTypes.SwitchButton, CubeLayers.cubeFull, true);
                    break;

                case CubeTypes.RotatorLeftTurner:
                    newCube.AddComponent(typeof(RotateLeftCube));
                    RotateLeftCube rotateLeftCube = newCube.GetComponent<RotateLeftCube>();
                    SendInfoToCube(rotateLeftCube as _CubeBase, CubeTypes.RotatorLeftTurner, CubeLayers.cubeFull, true);
                    break;

                case CubeTypes.RotatorRightTurner:
                    newCube.AddComponent(typeof(RotateRightCube));
                    RotateRightCube rotateRightCube = newCube.GetComponent<RotateRightCube>();
                    SendInfoToCube(rotateRightCube as _CubeBase, CubeTypes.RotatorRightTurner, CubeLayers.cubeFull, true);
                    break;

                case CubeTypes.RotatorLocker:
                    newCube.AddComponent(typeof(RotatorLocker));
                    RotatorLocker rotatorLocker = newCube.GetComponent<RotatorLocker>();
                    SendInfoToCube(rotatorLocker as _CubeBase, CubeTypes.RotatorLocker, CubeLayers.cubeFull, true);
                    break;

                case CubeTypes.ChaosBall:
                    newCube.AddComponent(typeof(ChaosBall));
                    ChaosBall chaosBall = newCube.GetComponent<ChaosBall>();
                    SendInfoToCube(chaosBall as _CubeBase, CubeTypes.ChaosBall, CubeLayers.cubeMoveable, false);
                    break;

                default:
                    break;
            }
        }

        void SendInfoToCube(_CubeBase cubeBase, CubeTypes cubeType, CubeLayers cubeLayer, bool _static)
        {
            //Cube info
            if (cubeBase.gameObject.GetComponent<_CubeScanner>() != null)
                cubeBase.facingDirection = FacingDirection.forward;

            cubeBase.myIndex = GetCubeIndex();
            cubeBase.isStatic = _static;
            cubeBase.myCubeLayer = cubeLayer;
            cubeBase.myCubeType = cubeType;

            cubeBase.gameObject.transform.position = GetCubePosition();
            cubeBase.gameObject.transform.parent = grid.transform;

            cubeBase.SetRelevantNodeInfo();
        }

        #endregion

        #region GET OFFSET AND CHECK INDEX STATE
        void CubeOffset(Vector3 cubeNormal)
        {
            if (cubeNormal == Vector3.up) moveWeight = _DirectionCustom.up;  //+ 1
            if (cubeNormal == Vector3.down) moveWeight = _DirectionCustom.down;  //- 1
            if (cubeNormal == Vector3.right) moveWeight = _DirectionCustom.right; //+ the grid size
            if (cubeNormal == Vector3.left) moveWeight = _DirectionCustom.left; //- the grid size
            if (cubeNormal == Vector3.forward) moveWeight = _DirectionCustom.forward; //+ the grid size squared
            if (cubeNormal == Vector3.back) moveWeight = _DirectionCustom.backward; //- the grid size squared
        }

        //get the position of the cube you are placing and set the cubeOnPosition
        Vector3 GetCubePosition()
        {
            return grid.kuboGrid[hitIndex - 1 + moveWeight].worldPosition;
        }

        int GetCubeIndex()
        {
            return grid.kuboGrid[hitIndex - 1 + moveWeight].nodeIndex;
        }

        Vector3 GetUserPlatform()
        {
            userInputPosition = Vector3.zero;

            if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) userInputPosition = Input.mousePosition;

            if (Input.touchCount == 1) userInputPosition = Input.GetTouch(0).position;
            else if (Input.touchCount == 2) userInputPosition = Input.GetTouch(1).position;

            return userInputPosition;
        }

        bool IndexIsEmpty()
        {
            if (grid.kuboGrid[hitIndex - 1 + moveWeight].cubeOnPosition == null) return true;
            else return false;
        }
        #endregion
    }
}
