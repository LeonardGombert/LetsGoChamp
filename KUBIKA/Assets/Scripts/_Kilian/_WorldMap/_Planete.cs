using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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
        public Camera MainCam;

        //INPUT
        Touch touch;
        Vector3 mouse;
        RaycastHit hit;
        Ray ray;

        //PLANETE TO FACE VIEW
        bool planeteView;

        //CAMERA MOVE--
        // ZOOM & CAM MOVING
        Touch touch0;
        Vector3 mouse0;
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
        float mediumXMouv;
        float mediumYMouv;
        float baseYRotation;
        float baseXRotation;
        public float mouvPower = 0.1f;

        public static float KUBNordScreenAngle;
        public static float KUBWestScreenAngle;
        public static float KUBSudScreenAngle;
        public static float KUBEstScreenAngle;



        // Start is called before the first frame update
        void Start()
        {
            if (_instance != null && _instance != this) Destroy(gameObject);
            else _instance = this;

            planeteView = true;
            planeteSpeed = rotationSpeed;

            brainVCam = FindObjectOfType<CinemachineBrain>();
        }

        // Update is called once per frame
        void Update()
        {
            if (planeteView == true)
            {
                RotationPlanete();

                if (Input.GetMouseButtonDown(0) || Input.touchCount == 1)
                    CheckTouch();

                if (Application.isMobilePlatform == true)
                    CameraPhoneInput();
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
            if (Application.isMobilePlatform == true)
            {
                touch = Input.GetTouch(0);
                ray = Camera.main.ScreenPointToRay(touch.position);

                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Touch Hit " + hit.collider.gameObject.name);
                    nextFace = hit.collider.gameObject.GetComponent<_PlaneteCamera>();

                    if (nextFace != null && touch.phase == TouchPhase.Ended)
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
            else
            {
                mouse = Input.mousePosition;
                ray = Camera.main.ScreenPointToRay(mouse);

                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Touch Hit " + hit.collider.gameObject.name);
                    nextFace = hit.collider.gameObject.GetComponent<_PlaneteCamera>();

                    if (nextFace != null && Input.GetMouseButtonUp(0))
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
            brainVCam.m_DefaultBlend.m_Time = 2;
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

            planeteView = true;
            currentFace.vCam.Priority = 0;
            baseVCam.Priority = 20;


        }

        public void StartOnFace(int faceEnQuestion)
        {
            brainVCam.m_DefaultBlend.m_Time = 0;
            Debug.Log("After");
            CameraTransition(raycastFaces[faceEnQuestion + 1]);

        }

        public void AfterFace()
        {
            Debug.Log("A");

            if (actualIndex + 1 < raycastFaces.Length)
            {
                Debug.Log("After");
                CameraTransition(raycastFaces[actualIndex + 1]);
            }
        }

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
            if (Input.GetMouseButton(0))
            {
                Debug.Log("mouse0 " + mouse0);
                mouse0 = Input.mousePosition;
                ScrollingSimple(mouse0);
            }

            ZoomingSimple();
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

        void ScrollingSimple(Vector3 touch0Pos)
        {

            mediumYMouv = ((touch0Pos.x));
            mediumXMouv = ((touch0Pos.y));

            baseRotation = pivotMainCamera.eulerAngles;
            baseYRotation = baseRotation.y;
            baseXRotation = baseRotation.x;
            baseRotation.y = baseRotation.y + mediumYMouv;
            baseRotation.x = baseRotation.x - mediumXMouv;
            pivotMainCamera.eulerAngles = baseRotation;
        }

        void ZoomingSimple()
        {
            currentZoommCam = baseVCam.m_Lens.OrthographicSize;
            currentZoommCam = Mathf.Clamp(currentZoommCam + (Input.mouseScrollDelta.y * zoomPower), currentZoommCam - 20, currentZoommCam + 20);
            baseVCam.m_Lens.OrthographicSize = currentZoommCam;
        }
        #endregion
    }
}
