using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Kubika.CustomLevelEditor;
using System;

namespace Kubika.Game
{
    public enum Platform { PC, Mobile };
    public class _DataManager : MonoBehaviour
    {
        // INSTANCE
        private static _DataManager _instance;
        public static _DataManager instance { get { return _instance; } }

        // MOVEABLE CUBE
        //public _CubeBase[] baseCubeArray;
        //public _CubeMove[] moveCubeArray;
        //public ElevatorCube[] elevatorsArray;

        public List<_CubeBase> baseCubeArray = new List<_CubeBase>();
        public List<_CubeMove> moveCubeArray = new List<_CubeMove>();
        public List<ElevatorCube> elevatorsArray = new List<ElevatorCube>();

        public List<_CubeMove> moveCube = new List<_CubeMove>();
        public List<ElevatorCube> elevators = new List<ElevatorCube>();

        //UNITY EVENT
        public UnityEvent StartChecking;
        public UnityEvent StartMoving;
        public UnityEvent EndMoving;
        public UnityEvent StartFalling;
        public UnityEvent EndFalling;

        public UnityEvent StartSwipe;
        public UnityEvent EndSwipe;

        // _DIRECTION_CUSTOM
        public int actualRotation;

        //INDEX BANK
        [Space]
        [Header("INDEX BANK")]
        public _DataMatrixScriptable indexBankScriptable;
        public List<_CubeBase> baseCube = new List<_CubeBase>();

        ///////////////INPUT
        Platform platform;
        public RaycastHit aimingHit;
        _CubeMove cubeMove;

        //TOUCH INPUT
        [HideInInspector] public Touch touch;
        [HideInInspector] public Vector3 inputPosition;
        Ray rayTouch;
        public int swipeMinimalDistance = 100;

        //PC INPUT
        Ray rayPC;


        private void Awake()
        {
            if (_instance != null && _instance != this) Destroy(this);
            else _instance = this;

            // CAP LE FPS A 60 FPS
            if (Application.isMobilePlatform == true)
            {
                Application.targetFrameRate = 60;
                QualitySettings.vSyncCount = 0;
                platform = Platform.Mobile;
            }
            else
            { platform = Platform.PC; }
        }

        // Start is called before the first frame update

        public void GameSet()
        {
            #region LIST VERSION            
            baseCubeArray = new List<_CubeBase>();
            moveCubeArray = new List<_CubeMove>();
            elevatorsArray = new List<ElevatorCube>();

            baseCube = new List<_CubeBase>();
            moveCube = new List<_CubeMove>();
            elevators = new List<ElevatorCube>();

            foreach (var item in _Grid.instance.kuboGrid)
            {
                if (item.cubeOnPosition != null)
                {
                    _CubeBase baseCube = item.cubeOnPosition.gameObject.GetComponent<_CubeBase>();
                    _CubeMove cubeMove = item.cubeOnPosition.gameObject.GetComponent<_CubeMove>();
                    ElevatorCube elevatorCube = item.cubeOnPosition.gameObject.GetComponent<ElevatorCube>();

                    if (cubeMove != null)
                    {
                        moveCubeArray.Add(cubeMove);
                    }

                    if (baseCube != null)
                    {
                        baseCubeArray.Add(baseCube);
                    }

                    if (elevatorCube != null)
                    {
                        elevatorsArray.Add(elevatorCube);
                    }
                }
            }
            #endregion

            #region ARRAY VERSION
            /*elevators.Clear();
            moveCube.Clear();
            baseCube.Clear();

            Array.Clear(moveCubeArray, 0, moveCubeArray.Length);
            Array.Clear(baseCubeArray, 0, baseCubeArray.Length);
            Array.Clear(elevatorsArray, 0, elevatorsArray.Length);

            moveCubeArray = FindObjectsOfType<_CubeMove>(); // TODO : DEGEULASSE
            baseCubeArray = FindObjectsOfType<_CubeBase>();
            elevatorsArray = FindObjectsOfType<ElevatorCube>(); */
            #endregion


            foreach (ElevatorCube elevator in elevatorsArray)
            {
                elevators.Add(elevator);
            }

            foreach (_CubeMove cube in moveCubeArray)
            {
                moveCube.Add(cube);
            }

            foreach (_CubeBase cube in baseCubeArray)
            {
                baseCube.Add(cube);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (platform == Platform.Mobile)
                PhoneInput();
            else
                PCInput();

            if (Input.GetKeyDown(KeyCode.W))
            {
                GameSet();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                MakeFall();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                _DirectionCustom.rotationState = actualRotation;
            }
        }

        #region INPUT

        void PhoneInput()
        {
            if (Input.touchCount == 1)
            {
                touch = Input.GetTouch(0);
                inputPosition = touch.position;

                rayTouch = _InGameCamera.instance.NormalCam.ScreenPointToRay(touch.position);
                // Handle finger movements based on TouchPhase
                switch (touch.phase)
                {
                    //When a touch has first been detected, change the message and record the starting position
                    case TouchPhase.Began:

                        if (Physics.Raycast(rayTouch, out aimingHit))
                        {
                            if (aimingHit.collider.gameObject.GetComponent<_CubeMove>() == true)
                            {
                                cubeMove = aimingHit.collider.gameObject.GetComponent<_CubeMove>();

                                if (cubeMove.isSelectable == true)
                                {
                                    cubeMove.isSeletedNow = true;
                                    cubeMove.GetBasePoint();
                                    cubeMove.AddOutline();
                                }
                            }
                        }



                        break;

                    case TouchPhase.Moved:
                        //EMouv.Invoke();
                        inputPosition = touch.position;
                        if (cubeMove != null && cubeMove.isSelectable == true)
                        {
                            cubeMove.NextDirection();
                        }

                        break;

                    case TouchPhase.Ended:
                        {
                            EndSwipe.Invoke();
                            cubeMove.isSeletedNow = false;
                            cubeMove = null;
                        }
                        break;
                }


            }

        }
        //__
        void PCInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                rayPC = _InGameCamera.instance.NormalCam.ScreenPointToRay(Input.mousePosition);

                inputPosition = Input.mousePosition;

                if (Physics.Raycast(rayPC, out aimingHit))
                {
                    if (aimingHit.collider.gameObject.GetComponent<_CubeMove>() == true)
                    {
                        cubeMove = aimingHit.collider.gameObject.GetComponent<_CubeMove>();

                        if (cubeMove.isSelectable == true)
                        {
                            cubeMove.isSeletedNow = true;
                            cubeMove.GetBasePoint();
                            cubeMove.AddOutline();
                        }
                    }
                }

            }
            else if (Input.GetMouseButton(0))
            {
                inputPosition = Input.mousePosition;
                if (cubeMove != null && cubeMove.isSelectable == true)
                {
                    cubeMove.NextDirection();
                }

            }
            else if (Input.GetMouseButtonUp(0))
            {
                EndSwipe.Invoke();
                cubeMove.isSeletedNow = false;
                cubeMove = null;
            }

        }

        #endregion


        #region MAKE FALL
        public void MakeFall()
        {
            foreach (_CubeMove cubes in moveCube)
            {
                cubes.CheckIfFalling();
            }
            StartCoroutine(CubesAreCheckingFall());
        }
        #endregion

        #region INDEX RESET

        public void ResetIndex(int rotationState)
        {
            Debug.LogError("RotSte " + rotationState);

            foreach (_CubeBase cBase in baseCube)
            {
                _Grid.instance.kuboGrid[cBase.myIndex - 1].cubeOnPosition = null;
                _Grid.instance.kuboGrid[cBase.myIndex - 1].cubeLayers = CubeLayers.cubeEmpty;
                _Grid.instance.kuboGrid[cBase.myIndex - 1].cubeType = CubeTypes.None;
            }

            /////// DEMON SCRIPT TODO DEGEULASS

            switch (rotationState)
            {
                case 0:
                    {
                        foreach (_CubeBase cBase in baseCube)
                        {
                            cBase.myIndex = indexBankScriptable.indexBank[cBase.myIndex - 1].nodeIndex0;
                            _Grid.instance.kuboGrid[cBase.myIndex - 1].cubeOnPosition = cBase.gameObject;
                            _Grid.instance.kuboGrid[cBase.myIndex - 1].cubeLayers = cBase.myCubeLayer;
                            _Grid.instance.kuboGrid[cBase.myIndex - 1].cubeType = cBase.myCubeType;

                        }
                    }
                    break;
                case 1:
                    {


                        foreach (_CubeBase cBase in baseCube)
                        {                          
                            cBase.myIndex = indexBankScriptable.indexBank[cBase.myIndex - 1].nodeIndex1;
                            _Grid.instance.kuboGrid[cBase.myIndex - 1].cubeOnPosition = cBase.gameObject;
                            _Grid.instance.kuboGrid[cBase.myIndex - 1].cubeLayers = cBase.myCubeLayer;
                            _Grid.instance.kuboGrid[cBase.myIndex - 1].cubeType = cBase.myCubeType;                           

                        }
                    }
                    break;
                case 2:
                    {


                        foreach (_CubeBase cBase in baseCube)
                        {
                            
                            cBase.myIndex = indexBankScriptable.indexBank[cBase.myIndex - 1].nodeIndex2;
                            _Grid.instance.kuboGrid[cBase.myIndex - 1].cubeOnPosition = cBase.gameObject;
                            _Grid.instance.kuboGrid[cBase.myIndex - 1].cubeLayers = cBase.myCubeLayer;
                            _Grid.instance.kuboGrid[cBase.myIndex - 1].cubeType = cBase.myCubeType;
                        }

                    }
                    break;
            }


        }
        #endregion

        #region TIMED EVENT
        public IEnumerator CubesAreCheckingMove()
        {
            while (AreCubesCheckingMove(moveCube.ToArray()) == false)
            {
                yield return null;
            }
            Debug.LogError("DATA- CHECK-END");
            //EndFalling.RemoveAllListeners();
            StartMoving.Invoke();
            StartCoroutine(CubesAreEndingToMove());
        }

        public IEnumerator CubesAndElevatorAreCheckingMove()
        {
            while (AreCubesAndElevatorsCheckingMove(elevators.ToArray(), moveCube.ToArray()) == false)
            {
                yield return null;
            }
            Debug.LogError("DATA- CHECK-END");
            //EndFalling.RemoveAllListeners();
            StartMoving.Invoke();
            StartCoroutine(CubesAndElevatorsAreEndingToMove());
        }

        public IEnumerator CubesAreEndingToMove()
        {
            while (AreCubesEndingToMove(moveCube.ToArray()) == false)
            {
                yield return null;
            }
            Debug.LogError("DATA- MOVE-END");
            StartMoving.RemoveAllListeners();
            EndMoving.Invoke();
            MakeFall();
        }

        public IEnumerator CubesAndElevatorsAreEndingToMove()
        {
            while (AreCubesAndElevatorsMoving(elevators.ToArray(), moveCube.ToArray()) == false)
            {
                yield return null;
            }
            Debug.LogError("DATA- MOVE-END");
            StartMoving.RemoveAllListeners();
            EndMoving.Invoke();
            MakeFall();
        }


        

        public IEnumerator CubesAreCheckingFall()
        {
            while (AreCubesCheckingFall(moveCube.ToArray()) == false)
            {
                yield return null;
            }
            Debug.LogError("DATA- FALLCHECK-END");
            //EndMoving.RemoveAllListeners();
            StartFalling.Invoke();
            StartCoroutine(CubesAreEndingToFall());
        }

        public IEnumerator CubesAreEndingToFall()
        {
            while (AreCubesEndingToFall(moveCube.ToArray()) == false)
            {
                yield return null;
            }
            Debug.LogError("DATA- FALLING-END");
            //StartFalling.RemoveAllListeners();
            EndFalling.Invoke();
        }


        //////////////////////

        public bool AreCubesCheckingMove(_CubeMove[] cubeMove)
        {
            for (int i = 0; i < cubeMove.Length; i++)
            {
                if (cubeMove[i].isCheckingMove == true)
                {
                    Debug.Log("CUBE_CHECKINGmove = TRUE");
                    return false;
                }
            }

            Debug.Log("CUBE_CHECKINGmove = TRUE");
            return true;
        }

        public bool AreCubesCheckingFall(_CubeMove[] cubeMove)
        {
            for (int i = 0; i < cubeMove.Length; i++)
            {
                if (cubeMove[i].isCheckingFall == true)
                {
                    return false;
                }
            }

            return true;
        }


        public bool AreCubesEndingToMove(_CubeMove[] cubeMove)
        {
            Debug.LogError("CUBE-MOVE-LENGTH  = " + cubeMove.Length);

            for (int i = 0; i < cubeMove.Length; i++)
            {
                Debug.LogError("WHO IS MOVING IN LIST" + i);

                if (cubeMove[i].isMoving == true)
                {
                    Debug.LogError("WHO IS MOVING + cubeMove-NAME = " + cubeMove[i].gameObject.name + " || isMoving = " + cubeMove[i].isMoving);
                    return false;
                }
            }

            return true;
        }


        public bool AreCubesEndingToFall(_CubeMove[] cubeMove)
        {
            for (int i = 0; i < cubeMove.Length; i++)
            {
                if (cubeMove[i].isFalling == true)
                {
                    return false;
                }
            }

            return true;
        }


        ///// ELEVATOR

        public bool AreElevatorsCheckingMove(ElevatorCube[] elevators)
        {
            for (int i = 0; i < elevators.Length; i++)
            {
                if (elevators[i].isCheckingMove == true)
                {
                    Debug.Log("ELEVATOR_CHECKINGmove = FALSE");
                    return false;
                }
            }

            Debug.Log("ELEVATOR_CHECKINGmove = TRUE");
            return true;
        }

        public bool AreElevatorsEndingToMove(ElevatorCube[] elevators)
        {
            for (int i = 0; i < elevators.Length; i++)
            {
                if (elevators[i].isMoving == true)
                {
                    Debug.Log("elevators[i].isMoving1_1_1_1_1_1_ = " + elevators[i].isMoving);
                    return false;
                }
            }

            Debug.Log("TRUE 1_1_1_1_1_1_ ");
            return true;
        }

        public bool AreCubesAndElevatorsCheckingMove(ElevatorCube[] elevators, _CubeMove[] cubeMove)
        {
            if(AreElevatorsCheckingMove(elevators) == true && AreCubesCheckingMove(cubeMove) == true)
            {
                Debug.Log("ELE & Cube Checking False");
                return true;
            }
            else
            {
                Debug.Log("ELE & Cube Checking True");
                return false;
            }

        }

        public bool AreCubesAndElevatorsMoving(ElevatorCube[] elevators, _CubeMove[] cubeMove)
        {
            if (AreElevatorsEndingToMove(elevators) == true && AreCubesEndingToMove(cubeMove) == true)
            {
                Debug.Log("ELE & Cube MOving False");
                return true;
            }
            else
            {
                Debug.Log("ELE & Cube MOving True");
                return false;
            }

        }

        #endregion



    }

}
