﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

namespace Kubika.Game
{
    public class _Planete : MonoBehaviour
    {
        private static _Planete _instance;
        public static _Planete instance { get { return _instance; } }


        // ROTATION
        public Transform planeteTrans;
        public float rotationSpeed;
        public float facesSpeed;
        public float planeteSpeed;

        public _PlaneteCamera[] raycastFaces;
        public int actualIndex;
        public _PlaneteCamera currentFace;
        public _PlaneteCamera nextFace;
        public _PlaneteCamera oldFace;

        //BASE CAMERA
        public CinemachineVirtualCamera baseVCam;
        public CinemachineBrain brainVCam;
        public CinemachineBlenderSettings blenderSettingsVCam; // the resulting settings
        public CinemachineBlenderSettings blenderCUTSettingsVCam; // for a cut transition
        public Camera MainCam;

        //INPUT
        Touch touch;
        Vector3 mouse;
        RaycastHit hit;
        Ray ray;

        //PLANETE TO FACE VIEW
        public bool planeteView;

        //CAMERA MOVE--
        // ZOOM & CAM MOVING
        Touch touch0;
        Vector3 mouse0;
        Vector3 mouse0LastPos;
        Touch touch1;
        Vector2 touch0PrevPos;
        Vector2 touch1PrevPos;
        float prevMagn;
        float currentMagn;
        float difference;
        [Space(10)] public float zoomPower;
        //
        Vector3 currentCamPos;
        float currentZoommCam;
        float currentCamZPos;
        //MOUV
        public Transform pivotMainCamera;
        Vector3 baseRotation;
        Vector3 currentRotation;
        float mediumXMouv;
        float mediumYMouv;
        float baseYRotation;
        float baseXRotation;
        public float mouvPower = 0.1f;

        public static float KUBNordScreenAngle;
        public static float KUBWestScreenAngle;
        public static float KUBSudScreenAngle;
        public static float KUBEstScreenAngle;

        // TRANSITION TO NEXT LEVEL
        public LevelCube[] levelCubes;
        public GameObject targetLevel;

        float time;
        float changeX, changeY, changeZ;
        float startValueX, startValueY, startValueZ;
        float targetValueX, targetValueY, targetValueZ;
        float tweenDuration = 1f;

        // PLATFORM
        public bool isMobilePlatform = false;

        // MOON
        public _MOON moonScript;


        // Start is called before the first frame update
        void Start()
        {
            if (_instance != null && _instance != this) Destroy(gameObject);
            else _instance = this;

            planeteView = true;
            planeteSpeed = rotationSpeed;

            levelCubes = FindObjectsOfType<LevelCube>();

            if (_CheckCurrentPlatform.platform == RuntimePlatform.Android || _CheckCurrentPlatform.platform == RuntimePlatform.IPhonePlayer)
                isMobilePlatform = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (planeteView == true)
            {
                RotationPlanete();

                if (Input.GetMouseButtonUp(0) || Input.touchCount == 1)
                    CheckTouch();

                if (isMobilePlatform == true)
                    CameraPhoneInput();
                else
                    CameraPCInput();
            }
            else
            {

            }
            //else
            //CameraPCInput();
        }


        void RotationPlanete()
        {
            planeteTrans.transform.Rotate(0, Time.deltaTime * rotationSpeed, 0);
        }

        void CheckTouch()
        {
            if (isMobilePlatform == true)
            {
                touch = Input.GetTouch(0);
                ray = Camera.main.ScreenPointToRay(touch.position);

                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Touch Hit Mobile " + hit.collider.gameObject.name);
                    nextFace = hit.collider.gameObject.GetComponent<_PlaneteCamera>();                                               // it returns which biome is being selected base on the name of the face
                    
                    if (hit.collider.gameObject.GetComponent<_PlaneteCamera>() == true)
                    {
                        nextFace = hit.collider.gameObject.GetComponent<_PlaneteCamera>();
                    }
                    else if(hit.collider.gameObject.tag == "Moon")
                    {
                        planeteView = false;
                        _MOON.instance.MoveToMoon();
                        return;
                    }

                    if (nextFace != null && touch.phase == TouchPhase.Ended)
                    {
                        WorldmapManager.instance.currentBiome = (Biomes)int.Parse(hit.collider.gameObject.name.Replace("Face", "")) - 1; // cursed memes for depressed programming teens
                        
                        if (planeteView == true)
                        {
                            nextFace.isActive = true;
                            nextFace.ActivatePSFB();
                            foreach (_PlaneteCamera faces in raycastFaces)
                            {
                                faces.gameObject.GetComponent<Collider>().enabled = false;
                            }

                            facesSpeed = rotationSpeed * 0.33f;
                            rotationSpeed = facesSpeed;
                        }

                        CameraTransition(nextFace);
                        planeteView = false;

                        StartCoroutine(UIManager.instance.ZoomedWorldMapPriority());
                    }
                }
            }
            else
            {
                mouse = Input.mousePosition;
                ray = Camera.main.ScreenPointToRay(mouse);

                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Touch Hit " + hit.collider.gameObject.name);

                    if (hit.collider.gameObject.GetComponent<_PlaneteCamera>() == true)
                    {
                        nextFace = hit.collider.gameObject.GetComponent<_PlaneteCamera>();
                    }
                    else if (hit.collider.gameObject.tag == "Moon")
                    {
                        planeteView = false;
                        _MOON.instance.MoveToMoon();
                        return;
                    }

                    if (nextFace != null)
                    {
                        if (planeteView == true)
                        {
                            nextFace.isActive = true;
                            nextFace.ActivatePSFB();
                            foreach (_PlaneteCamera faces in raycastFaces)
                            {
                                faces.gameObject.GetComponent<Collider>().enabled = false;
                            }

                            facesSpeed = rotationSpeed * 0.33f;
                            rotationSpeed = facesSpeed;
                        }

                        CameraTransition(nextFace);
                        planeteView = false;
                    }
                }
            }



        }

        void CameraTransition(_PlaneteCamera next)
        {
            actualIndex = next.index;

            if (oldFace != null) oldFace.vCam.Priority = 0;
            currentFace = next;
            currentFace.vCam.Priority = 20;
            baseVCam.Priority = 0;
            oldFace = currentFace;
        }

        public void MainPlaneteView()
        {
            Debug.Log("BackToMainVeiw");

            if (planeteView == false)
            {
                foreach (_PlaneteCamera faces in raycastFaces)
                {
                    faces.gameObject.GetComponent<Collider>().enabled = true;
                    faces.isActive = false;
                }

                rotationSpeed = planeteSpeed;
            }

            _MOON.instance.isMoonView = false;
            _MOON.instance.BaseView.Priority = 0;
            planeteView = true;
            currentFace.vCam.Priority = 0;
            baseVCam.Priority = 20;
        }

        //used to start the player's cam on the face after beating a level
        public IEnumerator StartOnFace(int faceEnQuestion)
        {
            brainVCam.m_CustomBlends = blenderCUTSettingsVCam;

            Debug.Log("After");

            planeteView = false;
            raycastFaces[faceEnQuestion + 1].isActive = true;
            foreach (_PlaneteCamera faces in raycastFaces)
            {
                faces.gameObject.GetComponent<Collider>().enabled = false;
            }
            facesSpeed = rotationSpeed * 0.33f;
            rotationSpeed = facesSpeed;

            CameraTransition(raycastFaces[faceEnQuestion + 1]);

            StartCoroutine(FocusOnNextLevel(LevelsManager.instance._Kubicode, faceEnQuestion + 1));

            yield return null;
        }

        public IEnumerator FocusOnNextLevel(string _Kubicode, int faceEnQuestion)
        {
            Transform faceRotationPoint = gameObject.transform.GetChild(1).transform.GetChild(0).
                                          transform.GetChild(faceEnQuestion).transform.GetChild(1).transform;

            foreach (var item in levelCubes)
            {
                if (item.kubicode == _Kubicode)
                {
                    targetLevel = item.gameObject;
                    break;
                }
                else continue;
            }

            currentFace.PutCameraInfrontOfCube(targetLevel.transform.position);

            yield return null;
        }

        //for rotation
        public void AfterFace()
        {
            Debug.Log("A");

            if (actualIndex + 1 < raycastFaces.Length)
            {
                Debug.Log("After");
                CameraTransition(raycastFaces[actualIndex + 1]);
            }
        }

        //for rotation
        public void BeforeFace()
        {
            Debug.Log("B");

            if (actualIndex - 1 >= 0)
            {
                Debug.Log("Before");
                CameraTransition(raycastFaces[actualIndex - 1]);
            }
        }


        //CAMERA-----------------------------------------------------

        void CameraPhoneInput()
        {
            if (Input.touchCount == 2)
            {
                touch0 = Input.GetTouch(0);
                touch1 = Input.GetTouch(1);

                touch0PrevPos = touch0.position - touch0.deltaPosition;
                touch1PrevPos = touch1.position - touch1.deltaPosition;

                prevMagn = (touch0PrevPos - touch1PrevPos).magnitude;
                currentMagn = (touch0.position - touch1.position).magnitude;

                difference = currentMagn - prevMagn;
                Zooming(difference);
                Scrolling(touch0.deltaPosition, touch1.deltaPosition);


            }
        }

        void CameraPCInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouse0LastPos = Input.mousePosition;
                baseRotation = currentRotation = pivotMainCamera.eulerAngles;
            }
            else if (Input.GetMouseButton(0))
            {
                mouse0 = Input.mousePosition;
                Debug.Log("mouse0 " + mouse0);
                ScrollingSimple(mouse0, mouse0LastPos);
            }

            ZoomingPC(Input.mouseScrollDelta.y);
        }

        void ZoomingPC(float scrollValue)
        {
            currentZoommCam = baseVCam.m_Lens.OrthographicSize;
            currentZoommCam = Mathf.Clamp(currentZoommCam - (scrollValue * zoomPower * 10), currentZoommCam - 20, currentZoommCam + 20);
            baseVCam.m_Lens.OrthographicSize = currentZoommCam;
        }

        void ScrollingSimple(Vector2 touch0Pos, Vector2 base0Pos)
        {

            mediumYMouv = ((touch0Pos.x - base0Pos.x)) * mouvPower;
            mediumXMouv = ((touch0Pos.y - base0Pos.y)) * mouvPower;

            baseYRotation = baseRotation.y + mediumYMouv;
            baseXRotation = baseRotation.x - mediumXMouv;
            currentRotation.y = baseYRotation;
            currentRotation.x = baseXRotation;
            pivotMainCamera.eulerAngles = currentRotation;
        }

        #region SCROLL & ZOOM
        void Zooming(float difference)
        {
            currentZoommCam = baseVCam.m_Lens.OrthographicSize;
            currentZoommCam = Mathf.Clamp(currentZoommCam - (difference * zoomPower), currentZoommCam - 20, currentZoommCam + 20);
            baseVCam.m_Lens.OrthographicSize = currentZoommCam;
        }

        void Scrolling(Vector2 touch0Pos, Vector2 touch1Pos)
        {

            mediumYMouv = ((touch0Pos.x + touch1Pos.x) * 0.5f) * mouvPower;
            mediumXMouv = ((touch0Pos.y + touch1Pos.y) * 0.5f) * mouvPower;

            baseRotation = pivotMainCamera.eulerAngles;
            baseYRotation = baseRotation.y;
            baseXRotation = baseRotation.x;
            baseRotation.y = baseYRotation + mediumYMouv;
            baseRotation.x = baseXRotation - mediumXMouv;
            pivotMainCamera.eulerAngles = baseRotation;
        }
        #endregion
    }
}
