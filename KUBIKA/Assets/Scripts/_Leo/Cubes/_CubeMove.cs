using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Kubika.CustomLevelEditor;

namespace Kubika.Game
{
    public enum swipeDirection { Front, Right, Left, Back };

    public class _CubeMove : _CubeScanner
    {
        //FALLING 
        int nbrCubeMouvableBelow;
        public int nbrCubeEmptyBelow;
        int nbrCubeBelow;
        int indexTargetNode;
        public bool thereIsEmpty = false;
        Vector3 nextPosition;

        // BOOL ACTION
        [Space]
        [Header("BOOL ACTION")]
        public bool isCheckingMove;
        public bool isCheckingFall;
        public bool isFalling;
        public bool isMoving;
        public bool isOutside;
        public bool isOverNothing;
        public bool isReadyToMove;
        public bool isMovingAndSTFU;


        //FALL MOVE
        Vector3 currentPos;
        Vector3 basePos;
        float currentTime;
        public float moveTime = 0.5f;
        float time;

        //FALL OUTSIDE
        [Space]
        [Header("OUTSIDE")]
        public int nbrDeCubeFallOutside = 10;
        Vector3 moveOutsideTarget;
        [HideInInspector] public Vector3 moveOutsideTargetCustomVector;
        Vector3 fallOutsideTarget;

        // COORD SYSTEM
        [Space]
        [Header("COORD SYSTEM")]
        public int xCoordLocal;
        public int yCoordLocal;
        public int zCoordLocal;

      
        // MOVE
        [Space]
        [Header("MOVE")]
        public Node soloMoveTarget;
        public Node soloPileTarget;
        public Vector3 outsideMoveTarget;

        //PUSH
        public _CubeMove pushNextNodeCubeMove;

        //PILE
        public _CubeMove pileNodeCubeMove;

        //INPUT
        public bool isSelectable = true;
        public bool isSeletedNow = false;
        public swipeDirection enumSwipe;

        [Space]
        public static swipeDirection worldEnumSwipe;

        // WORLD TO SCREEN
        [HideInInspector] public Vector2 baseCube;
        [HideInInspector] public Vector2 nextCube;

        Vector2 baseSwipePos;
        Vector2 currentSwipePos;

        [HideInInspector] public float distanceBaseToNext;
        [HideInInspector] public float distanceTouch;

        public float angleDirection;
        float inverseAngleDirection;

        // DIRECTION
        public float KUBNord;
        public float KUBWest;
        public float KUBSud;
        public float KUBEst;


        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            _DataManager.instance.EndMoving.AddListener(ResetReadyToMove);
            _DataManager.instance.StartFalling.AddListener(FallMoveFunction);
            _DataManager.instance.EndSwipe.AddListener(ResetOutline);
        }

        // Update is called once per frame
        public override void Update()
        {
            //CheckIfFalling();//grid.kuboGrid[myIndex - 1].cubeLayers = CubeLayers.cubeMoveable;
            base.Update();

            if (Input.GetKeyDown(KeyCode.W))
            {
                // Put Actual Node as Moveable
                myCubeLayer = CubeLayers.cubeMoveable;
                grid.kuboGrid[myIndex - 1].cubeLayers = myCubeLayer;
            }
        }


        #region FALL


        public void CheckIfFalling()
        {
            if (!isStatic)
            {
                isCheckingFall = true;
                thereIsEmpty = false;
                nbrCubeMouvableBelow = 0;
                nbrCubeEmptyBelow = 0;
                indexTargetNode = 0;

                Fall(1);
            }
        }

        public void Fall(int nbrCubeBelowParam)
        {
            //Debug.Log("Index 1==" + (myIndex + (_DirectionCustom.down * nbrCubeBelow) - 1));
            //Debug.Log("Index 2==" + (myIndex + (_DirectionCustom.down * nbrCubeBelow)));
            //Debug.Log("Index 3==" + (myIndex + (_DirectionCustom.down) - 1));
            //Debug.Log("Index 4==" + (myIndex + (_DirectionCustom.down)));

            Debug.Log("NUMBERS OF KUBE -- " + (grid.kuboGrid[myIndex - 1 + (_DirectionCustom.down * nbrCubeBelowParam)].nodeIndex));


            if (grid.kuboGrid[myIndex - 1 + (_DirectionCustom.down * nbrCubeBelowParam)].cubeLayers == CubeLayers.cubeEmpty)
            {
                if (MatrixLimitCalcul((myIndex + (_DirectionCustom.down * nbrCubeBelowParam)), _DirectionCustom.down))
                {
                    Debug.Log("EmptyDetected --");
                    thereIsEmpty = true;
                    nbrCubeEmptyBelow += 1;
                    Fall(nbrCubeBelowParam + 1);
                }
                else
                {
                    Debug.Log(" FALL OUTSIDE Pos = " + (myIndex + (_DirectionCustom.down * nbrCubeBelowParam)));
                    fallOutsideTarget = new Vector3(grid.kuboGrid[myIndex - 1].xCoord * grid.offset,
                                                     grid.kuboGrid[myIndex - 1].yCoord * grid.offset - ((nbrCubeEmptyBelow + nbrCubeMouvableBelow) * grid.offset),
                                                     grid.kuboGrid[myIndex - 1].zCoord * grid.offset);

                    fallOutsideTarget += (Vector3.down * nbrDeCubeFallOutside);
                    isOutside = true;
                    isCheckingFall = false;
                }
            }
            else if (grid.kuboGrid[myIndex - 1 + (_DirectionCustom.down * nbrCubeBelowParam)].cubeLayers == CubeLayers.cubeMoveable)
            {
                if (MatrixLimitCalcul((myIndex + (_DirectionCustom.down * nbrCubeBelowParam)), _DirectionCustom.down))
                {
                    Debug.Log("MoveDetected --");
                    nbrCubeMouvableBelow += 1;
                    Fall(nbrCubeBelowParam + 1);
                }
                else
                {
                    Debug.Log(" FALL OUTSIDE Pos = " + (myIndex + (_DirectionCustom.down * nbrCubeBelowParam)));
                    fallOutsideTarget = new Vector3(grid.kuboGrid[myIndex - 1].xCoord * grid.offset,
                                                     grid.kuboGrid[myIndex - 1].yCoord * grid.offset - ((nbrCubeEmptyBelow + nbrCubeMouvableBelow) * grid.offset),
                                                     grid.kuboGrid[myIndex - 1].zCoord * grid.offset);

                    fallOutsideTarget += (Vector3.down * nbrDeCubeFallOutside);
                    isOutside = true;
                    isCheckingFall = false;
                }
            }
            else if (grid.kuboGrid[myIndex - 1 + (_DirectionCustom.down * nbrCubeBelowParam)].cubeLayers == CubeLayers.cubeFull)
            {
                if (MatrixLimitCalcul(myIndex, _DirectionCustom.down))
                {
                    Debug.Log("FULLDetected -- | " + (grid.kuboGrid[myIndex - 1 + (_DirectionCustom.down * nbrCubeBelowParam)].nodeIndex));
                    nbrCubeBelow = nbrCubeBelowParam;

                    indexTargetNode = myIndex + (_DirectionCustom.down * nbrCubeBelow) + (_DirectionCustom.up * (nbrCubeMouvableBelow + 1));
                    nextPosition = grid.kuboGrid[indexTargetNode - 1].worldPosition;

                    Debug.Log("FULLDetected -- Final Destination ");
                    isCheckingFall = false;
                }
                else
                {
                    Debug.Log(" FALL OUTSIDE Pos = " + (myIndex + (_DirectionCustom.down * nbrCubeBelowParam)));
                    fallOutsideTarget = new Vector3(grid.kuboGrid[myIndex - 1].xCoord * grid.offset,
                                                     grid.kuboGrid[myIndex - 1].yCoord * grid.offset - ((nbrCubeEmptyBelow + nbrCubeMouvableBelow) * grid.offset),
                                                     grid.kuboGrid[myIndex - 1].zCoord * grid.offset);

                    fallOutsideTarget += (Vector3.down * nbrDeCubeFallOutside);
                    isOutside = true;
                    isCheckingFall = false;
                }

            }


        }

        public void FallMoveFunction()
        {
            Debug.Log("FallMoveFunction + thereIsEmpty = " + thereIsEmpty + " || isOutside = " + isOutside);
            if (thereIsEmpty == true && isOutside == false)
            {
                isFalling = true;
                StartCoroutine(FallMove(nextPosition, nbrCubeEmptyBelow, nbrCubeBelow));
            }
            else if (isOutside == true)
            {
                isFalling = true;
                StartCoroutine(FallFromMap(fallOutsideTarget, nbrDeCubeFallOutside));
            }
        }

        public IEnumerator FallMove(Vector3 fallPosition, int nbrCub, int nbrCubeBelowParam)
        {
            Debug.Log("FallMove");

            grid.kuboGrid[myIndex - 1].cubeOnPosition = null;
            grid.kuboGrid[myIndex - 1].cubeLayers = CubeLayers.cubeEmpty;
            grid.kuboGrid[myIndex - 1].cubeType = CubeTypes.None;

            basePos = transform.position;
            currentTime = 0;

            while (currentTime <= 1)
            {
                currentTime += Time.deltaTime / nbrCub;
                currentTime = (currentTime / moveTime);

                currentPos = Vector3.Lerp(basePos, fallPosition, currentTime);

                transform.position = currentPos;
                yield return transform.position;
            }


            myIndex = indexTargetNode;
            grid.kuboGrid[indexTargetNode - 1].cubeOnPosition = gameObject;
            //set updated index to cubeMoveable
            grid.kuboGrid[indexTargetNode - 1].cubeLayers = CubeLayers.cubeMoveable;
            grid.kuboGrid[indexTargetNode - 1].cubeType = myCubeType;

            xCoordLocal = grid.kuboGrid[indexTargetNode - 1].xCoord;
            yCoordLocal = grid.kuboGrid[indexTargetNode - 1].yCoord;
            zCoordLocal = grid.kuboGrid[indexTargetNode - 1].zCoord;


            isFalling = false;
            Debug.Log("END-FallMove");
        }


        public IEnumerator FallFromMap(Vector3 fallFromMapPosition, int nbrCaseBelow)
        {
            isFalling = true;


            basePos = transform.position;
            currentTime = 0;

            while (currentTime <= 1)
            {
                currentTime += Time.deltaTime / nbrCaseBelow;
                currentTime = (currentTime / moveTime);

                currentPos = Vector3.Lerp(basePos, fallFromMapPosition, currentTime);

                transform.position = currentPos;
                yield return transform.position;
            }


            xCoordLocal = Mathf.RoundToInt(fallFromMapPosition.x / grid.offset);
            yCoordLocal = Mathf.RoundToInt(fallFromMapPosition.y / grid.offset);
            zCoordLocal = Mathf.RoundToInt(fallFromMapPosition.z / grid.offset);

            _DataManager.instance.moveCube.Remove(this);
            _DataManager.instance.baseCube.Remove(this);


            isFalling = false;
        }

        #endregion

        #region MOVE

        public IEnumerator Move(Node nextNode)
        {
            isMoving = true;
            Debug.Log("IS MOVING || isMoving = " + isMoving);

            grid.kuboGrid[myIndex - 1].cubeOnPosition = null;
            grid.kuboGrid[myIndex - 1].cubeLayers = CubeLayers.cubeEmpty;
            grid.kuboGrid[myIndex - 1].cubeType = CubeTypes.None;

            basePos = transform.position;
            currentTime = 0;

            while (currentTime <= 1)
            {
                currentTime += Time.deltaTime;
                currentTime = (currentTime / moveTime);

                currentPos = Vector3.Lerp(basePos, nextNode.worldPosition, currentTime);

                transform.position = currentPos;
                yield return transform.position;
            }

            //Debug.Log(" nextNode.nodeIndex-1 = " + nextNode.nodeIndex + " ||nextNode.cubeLayers " + nextNode.cubeLayers);


            myIndex = nextNode.nodeIndex;
            nextNode.cubeOnPosition = gameObject;
            //set updated index to cubeMoveable
            nextNode.cubeLayers = CubeLayers.cubeMoveable;
            nextNode.cubeType = myCubeType;

            //Debug.Log(" nextNode.nodeIndex-2 = " + nextNode.nodeIndex + " ||nextNode.cubeLayers " + nextNode.cubeLayers);

            xCoordLocal = grid.kuboGrid[nextNode.nodeIndex - 1].xCoord;
            yCoordLocal = grid.kuboGrid[nextNode.nodeIndex - 1].yCoord;
            zCoordLocal = grid.kuboGrid[nextNode.nodeIndex - 1].zCoord;

            isMoving = false;

            if(isSeletedNow) GetBasePoint(); // RESET SWIPE POS

            Debug.Log("END MOVING ");

        }

        public IEnumerator MoveFromMap(Vector3 nextPosition)
        {
            isMoving = true;

            moveOutsideTarget = new Vector3(nextPosition.x * grid.offset, nextPosition.y * grid.offset, nextPosition.z * grid.offset);

            grid.kuboGrid[myIndex - 1].cubeOnPosition = null;
            grid.kuboGrid[myIndex - 1].cubeLayers = CubeLayers.cubeEmpty;
            grid.kuboGrid[myIndex - 1].cubeType = CubeTypes.None;

            basePos = transform.position;
            currentTime = 0;

            while (currentTime <= 1)
            {
                currentTime += Time.deltaTime;
                currentTime = (currentTime / moveTime);

                currentPos = Vector3.Lerp(basePos, moveOutsideTarget, currentTime);

                transform.position = currentPos;
                yield return transform.position;
            }

            myIndex = indexTargetNode;

            xCoordLocal = Mathf.RoundToInt(moveOutsideTarget.x / grid.offset);
            yCoordLocal = Mathf.RoundToInt(moveOutsideTarget.y / grid.offset);
            zCoordLocal = Mathf.RoundToInt(moveOutsideTarget.z / grid.offset);


            fallOutsideTarget = moveOutsideTarget;
            fallOutsideTarget += (Vector3.down * nbrDeCubeFallOutside);

            isMoving = false;

            //_DataManager.instance.EndChecking.Invoke();

        }

        public void MoveToTarget()
        {
            Debug.Log("MoveToTarget-MOVING");
            if (isOutside == false)
            {
                StartCoroutine(Move(soloMoveTarget));
            }
            else
            {
                StartCoroutine(MoveFromMap(moveOutsideTargetCustomVector));
            }
        }

        public void MoveToTargetPile()
        {
            Debug.Log("MoveToTargetPile-MOVING-PILE");
            StartCoroutine(Move(soloPileTarget));
        }

        public void ResetReadyToMove()
        {
            Debug.Log("RESET");
            isReadyToMove = false;
            pileNodeCubeMove = null;
            pushNextNodeCubeMove = null;
            isOverNothing = false;
            isMovingAndSTFU = false;
        }

        public void CheckingMove(int index, int nodeDirection)
        {
            isMoving = true;
            isCheckingMove = true;
            Debug.Log("CheckingMove ");
            _DataManager.instance.StartMoving.AddListener(MoveToTarget);
            CheckSoloMove(index, nodeDirection);
        }
        public void CheckingPile(int index, int nodeDirection)
        {
            isMoving = true;
            isCheckingMove = true;
            Debug.Log("CheckingPike");
            _DataManager.instance.StartMoving.AddListener(MoveToTargetPile);
            CheckPileMove(index, nodeDirection);
        }

        public void CheckSoloMove(int index, int nodeDirection)
        {
            if (isMovingAndSTFU == false)
            {
                if (MatrixLimitCalcul(index, nodeDirection))
                {
                    indexTargetNode = index + nodeDirection;
                    Debug.Log("---CheckSoloMove--- | " + nodeDirection);

                    switch (grid.kuboGrid[indexTargetNode - 1].cubeLayers)
                    {
                        case CubeLayers.cubeFull:
                            {
                                Debug.Log("STUCK ");
                                soloMoveTarget = grid.kuboGrid[myIndex - 1];
                                isCheckingMove = false;
                            }
                            break;
                        case CubeLayers.cubeEmpty:
                            {
                                Debug.Log("EMPTY ");

                                if (grid.kuboGrid[indexTargetNode - 1 + _DirectionCustom.down].cubeLayers == CubeLayers.cubeMoveable || grid.kuboGrid[indexTargetNode - 1 + _DirectionCustom.down].cubeLayers == CubeLayers.cubeFull)
                                {
                                    isReadyToMove = true;
                                    soloMoveTarget = grid.kuboGrid[myIndex + nodeDirection - 1];
                                    Debug.Log("TAAAAAAAREGT MOVE " + soloMoveTarget.nodeIndex + " ||MyIndex " + myIndex);

                                    if (grid.kuboGrid[myIndex - 1 + _DirectionCustom.up].cubeLayers == CubeLayers.cubeMoveable && MatrixLimitCalcul(myIndex, _DirectionCustom.up))
                                    {
                                        pileNodeCubeMove = grid.kuboGrid[myIndex - 1 + _DirectionCustom.up].cubeOnPosition.GetComponent<_CubeMove>();
                                        pileNodeCubeMove.CheckingPile(pileNodeCubeMove.myIndex - 1, nodeDirection);
                                    }
                                    Debug.Log("EMPTY-CAN MOVE-");
                                }
                                else
                                {
                                    isReadyToMove = true;
                                    Debug.Log("EMPTY-CANNOT MOVE-");
                                    soloMoveTarget = grid.kuboGrid[myIndex - 1];
                                }
                                isCheckingMove = false;

                            }
                            break;
                        case CubeLayers.cubeMoveable:
                            {
                                Debug.Log("MOVE ");
                                pushNextNodeCubeMove = grid.kuboGrid[indexTargetNode - 1].cubeOnPosition.GetComponent<_CubeMove>();
                                if (pushNextNodeCubeMove.isReadyToMove == false)
                                {
                                    pushNextNodeCubeMove.CheckingMove(indexTargetNode, nodeDirection);
                                }
                                CheckSoloMove(indexTargetNode, nodeDirection);
                            }
                            break;
                    }

                }
                else
                {
                    Debug.Log("MATRIX LIMIT SOLO");
                    soloMoveTarget = grid.kuboGrid[myIndex - 1];
                    isCheckingMove = false;
                }
            }
            else
            {
                if (MatrixLimitCalcul(index, nodeDirection))
                {
                    indexTargetNode = index + nodeDirection;
                    Debug.Log("---CheckSoloMove--- | " + nodeDirection + " || index : " + index);

                    switch (grid.kuboGrid[indexTargetNode - 1].cubeLayers)
                    {
                        case CubeLayers.cubeFull:
                            {
                                Debug.Log("STUCK ");
                                soloMoveTarget = grid.kuboGrid[myIndex - 1];
                                isCheckingMove = false;
                            }
                            break;
                        case CubeLayers.cubeEmpty:
                            {

                                isReadyToMove = true;
                                soloMoveTarget = grid.kuboGrid[index + nodeDirection - 1];

                                Debug.Log("EMPTY " + soloMoveTarget.nodeIndex);

                                if (grid.kuboGrid[myIndex - 1 + _DirectionCustom.up].cubeLayers == CubeLayers.cubeMoveable && MatrixLimitCalcul(myIndex, _DirectionCustom.up))
                                {
                                    pileNodeCubeMove = grid.kuboGrid[myIndex - 1 + _DirectionCustom.up].cubeOnPosition.GetComponent<_CubeMove>();
                                    pileNodeCubeMove.CheckingPile(pileNodeCubeMove.myIndex - 1, nodeDirection);
                                }
                                Debug.Log("EMPTY-CAN MOVE-");

                                isCheckingMove = false;
                            }
                            break;
                        case CubeLayers.cubeMoveable:
                            {
                                Debug.Log("MOVE ");
                                pushNextNodeCubeMove = grid.kuboGrid[indexTargetNode - 1].cubeOnPosition.GetComponent<_CubeMove>();
                                if (pushNextNodeCubeMove.isReadyToMove == false)
                                {
                                    pushNextNodeCubeMove.CheckingMove(indexTargetNode, nodeDirection);
                                }
                                CheckSoloMove(indexTargetNode, nodeDirection);
                            }
                            break;
                    }

                }
                else
                {
                    Debug.Log("MATRIX LIMIT SOLO");
                    moveOutsideTargetCustomVector = outsideCoord(myIndex, -nodeDirection);
                    isOutside = true;
                    isCheckingMove = false;
                }
            }


        }

         public void CheckPileMove(int index, int nodeDirection)
         {
            if (MatrixLimitCalcul(index, nodeDirection))
            {
                indexTargetNode = index + nodeDirection;
                Debug.Log("---CheckPileMove--- indexTargetNode = " + indexTargetNode + " || myIndex = " + myIndex);

                switch (grid.kuboGrid[indexTargetNode ].cubeLayers)
                {
                    case CubeLayers.cubeFull:
                        {
                            Debug.Log("STUCK-PILE ");
                            soloPileTarget = grid.kuboGrid[myIndex - 1];
                            isCheckingMove = false;
                        }
                        break;
                    case CubeLayers.cubeEmpty:
                        {
                            Debug.Log("EMPTY-PILE " + (indexTargetNode - 1));

                            isReadyToMove = true;
                            soloPileTarget = grid.kuboGrid[myIndex - 1 + nodeDirection];
                            Debug.Log("EMPTY-PILE-CAN-MOVE ");

                            if (grid.kuboGrid[myIndex - 1 + _DirectionCustom.up].cubeLayers == CubeLayers.cubeMoveable && MatrixLimitCalcul(myIndex, _DirectionCustom.up))
                            {
                                Debug.Log("PILE-AGAIN " );
                                pileNodeCubeMove = grid.kuboGrid[myIndex - 1 + _DirectionCustom.up].cubeOnPosition.GetComponent<_CubeMove>();
                                pileNodeCubeMove.CheckingPile(pileNodeCubeMove.myIndex - 1, nodeDirection);
                            }

                            isCheckingMove = false;
                        }
                        break;
                    case CubeLayers.cubeMoveable:
                        {
                            Debug.Log("MOVE-PILE + indexTargetNode = " + indexTargetNode + " || INDEX = " + (myIndex + nodeDirection));
                            CheckPileMove(indexTargetNode, nodeDirection);
                        }
                        break;
                }

            }
            else
            {
                Debug.Log("MATRIX LIMIT PILE");
                soloPileTarget = grid.kuboGrid[myIndex - 1];
                isCheckingMove = false;
            }
        }


        #endregion

        #region INPUT
        
        public void NextDirection()
        {

            if (!isStatic)
            {
                // Calcul the swip angle
                currentSwipePos = _DataManager.instance.inputPosition;

                distanceTouch = Vector3.Distance(baseSwipePos, currentSwipePos);

                angleDirection = Mathf.Abs(Mathf.Atan2(currentSwipePos.y - baseSwipePos.y, baseSwipePos.x - currentSwipePos.x) * 180 / Mathf.PI - 180);


                KUBNord = _InGameCamera.KUBNordScreenAngle;
                KUBWest = _InGameCamera.KUBWestScreenAngle;
                KUBSud = _InGameCamera.KUBSudScreenAngle;
                KUBEst = _InGameCamera.KUBEstScreenAngle;

                // Check in which direction the player swiped 

                if (angleDirection < KUBNord && angleDirection > KUBEst)
                {
                    enumSwipe = swipeDirection.Front;
                }
                else if (angleDirection < KUBWest && angleDirection > KUBNord)
                {
                    enumSwipe = swipeDirection.Left;
                }
                else if (angleDirection < KUBSud && angleDirection > KUBWest)
                {
                    enumSwipe = swipeDirection.Back;
                }
                else if (angleDirection < KUBEst && angleDirection > KUBSud)
                {
                    enumSwipe = swipeDirection.Right;
                }

                else
                {

                    if (angleDirection > 180)
                        inverseAngleDirection = angleDirection - 180;
                    else
                        inverseAngleDirection = angleDirection + 180;


                    if (inverseAngleDirection < KUBNord && inverseAngleDirection > KUBEst)
                    {
                        enumSwipe = swipeDirection.Back;
                    }
                    else if (inverseAngleDirection < KUBWest && inverseAngleDirection > KUBNord)
                    {
                        enumSwipe = swipeDirection.Right;
                    }
                    else if (inverseAngleDirection < KUBSud && inverseAngleDirection > KUBWest)
                    {
                        enumSwipe = swipeDirection.Front;
                    }
                    else if (inverseAngleDirection < KUBEst && inverseAngleDirection > KUBSud)
                    {
                        enumSwipe = swipeDirection.Left;
                    }
                }

                if (distanceTouch > _DataManager.instance.swipeMinimalDistance)
                    CheckDirection(enumSwipe);
            }
        }

        public void GetBasePoint()
        {
            //Reset Base Touch position
            baseSwipePos = _DataManager.instance.inputPosition;
        }

        public void CheckDirection(swipeDirection swipeDir)
        {
            // Check dans quel direction le joueur swipe
            switch (swipeDir)
            {
                case swipeDirection.Front:
                    CheckWhenToMove(_DirectionCustom.left);
                    break;
                case swipeDirection.Right:
                    CheckWhenToMove(_DirectionCustom.forward);
                    break;
                case swipeDirection.Left:
                    CheckWhenToMove(_DirectionCustom.backward);
                    break;
                case swipeDirection.Back:
                    CheckWhenToMove(_DirectionCustom.right);
                    break;
            }
        }

        void CheckWhenToMove(int direction)
        {


            if (MatrixLimitCalcul(myIndex, direction) == true)
            {
                baseCube = _InGameCamera.instance.cam.WorldToScreenPoint(grid.kuboGrid[myIndex - 1].worldPosition);
                nextCube = _InGameCamera.instance.cam.WorldToScreenPoint(grid.kuboGrid[myIndex - 1 + direction].worldPosition);

                distanceBaseToNext = Vector3.Distance(baseCube, nextCube);

                if (distanceTouch > (distanceBaseToNext * 0.5f) && isMoving == false)
                {
                    CheckingMove(myIndex, direction);
                    StartCoroutine(_DataManager.instance.CubesAreCheckingMove());
                }
            }
        }

        #endregion

        #region FEEDBACK

        public virtual void OutlineActive(int isActive)
        {
            meshRenderer.GetPropertyBlock(MatProp);

            if (isActive == 1)
                MatProp.SetFloat("_Outline", 0.1f);
            else if (isActive == 2)
                MatProp.SetFloat("_Outline", 0);

            meshRenderer.SetPropertyBlock(MatProp);
        }

        public virtual void AddOutline()
        {
            OutlineActive(1);
            GetChildRecursive(myIndex) ;
        }

        public void GetChildRecursive(int index)
        {

            if (grid.kuboGrid[index - 1 + _DirectionCustom.up].cubeLayers == CubeLayers.cubeMoveable && MatrixLimitCalcul(index, _DirectionCustom.up))
            {
                grid.kuboGrid[index - 1 + _DirectionCustom.up].cubeOnPosition.GetComponent<_CubeMove>().OutlineActive(1);
                GetChildRecursive(grid.kuboGrid[index + _DirectionCustom.up].nodeIndex - 1);
            }
        }


        public virtual void ResetOutline()
        {
            OutlineActive(2);
        }

        #endregion

        void TEMPORARY______SHIT()
        {
            if (_DataManager.instance.AreCubesEndingToFall(_DataManager.instance.moveCube.ToArray()) == true)
            {
                // X Axis
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    if (isMoving == false)
                    {
                        CheckingMove(myIndex, _DirectionCustom.left);
                        StartCoroutine(_DataManager.instance.CubesAreCheckingMove());
                        /*else
                        {
                            outsideMoveTarget = new Vector3(xCoordLocal, yCoordLocal, zCoordLocal);
                            outsideMoveTarget += _DirectionCustom.vectorLeft;
                            StartCoroutine(MoveFromMap(outsideMoveTarget));
                            Debug.LogError("Z AU BORD");
                        }*/
                    }
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    if (isMoving == false)
                    {
                        // -X Axis
                        CheckingMove(myIndex, _DirectionCustom.right);
                        StartCoroutine(_DataManager.instance.CubesAreCheckingMove());
                        /*
                        else
                        {
                            outsideMoveTarget = new Vector3(xCoordLocal, yCoordLocal, zCoordLocal);
                            outsideMoveTarget += _DirectionCustom.vectorRight;
                            StartCoroutine(MoveFromMap(outsideMoveTarget));
                            Debug.LogError("S AU BORD");
                        }*/
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (isMoving == false)
                    {
                        // -Z Axis
                        CheckingMove(myIndex, _DirectionCustom.backward);
                        StartCoroutine(_DataManager.instance.CubesAreCheckingMove());
                        /*
                        else
                        {
                            outsideMoveTarget = new Vector3(xCoordLocal, yCoordLocal, zCoordLocal);
                            outsideMoveTarget += _DirectionCustom.vectorBack;
                            StartCoroutine(MoveFromMap(outsideMoveTarget));
                            Debug.LogError("Q AU BORD");
                        }*/
                    }
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    if (isMoving == false)
                    {
                        CheckingMove(myIndex, _DirectionCustom.forward);
                        StartCoroutine(_DataManager.instance.CubesAreCheckingMove());
                        /*
                             else
                             {
                                 outsideMoveTarget = new Vector3(xCoordLocal, yCoordLocal, zCoordLocal);
                                 outsideMoveTarget += _DirectionCustom.vectorForward;
                                 StartCoroutine(MoveFromMap(outsideMoveTarget));
                                 Debug.LogError("D AU BORD");
                             }*/
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                // Y Axis
                CheckingMove(myIndex, _DirectionCustom.up);
                StartCoroutine(_DataManager.instance.CubesAreCheckingMove());
                /*
                 else
                 {
                     Debug.LogError("R AU BORD");
                 }*/
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                // -Y Axis
                CheckingMove(myIndex, _DirectionCustom.down);
                StartCoroutine(_DataManager.instance.CubesAreCheckingMove());
                /*
                 else
                 {
                     Debug.LogError("F AU BORD");
                 }*/
            }


        }
    }
}