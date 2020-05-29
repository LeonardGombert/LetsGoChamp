using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Kubika.Game
{
    public class _InGameCamera : MonoBehaviour
    {
        private static _InGameCamera _instance;
        public static _InGameCamera instance { get { return _instance; } }

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
        //
        Vector3 pointCenterRef;
        Vector3 pointNordRef;
        Vector3 pointWestRef;
        Vector3 pointSudRef;
        Vector3 pointEstRef;
        public static float KUBNordScreenAngle;
        public static float KUBWestScreenAngle;
        public static float KUBSudScreenAngle;
        public static float KUBEstScreenAngle;

        [Space]
        [Header("CINEMACHINE")]
        public float ShakeDuration = 0.5f;          // Time the Camera Shake effect will last
        public float ShakeAmplitude = 1.5f;         // Cinemachine Noise Profile Parameter
        public float ShakeFrequency = 2.0f;         // Cinemachine Noise Profile Parameter
        private float ShakeElapsedTime = 0f;
        public CinemachineVirtualCamera cam;
        public Camera NormalCam;
        private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

        [Space]
        public Transform pointCenter;
        public Transform pointNord;
        public Transform pointWest;
        public Transform pointSud;
        public Transform pointEst;

        public LayerMask RotatiponTorus;


        // Start is called before the first frame update
        void Start()
        {
            if (_instance != null && _instance != this) Destroy(gameObject);
            else _instance = this;

            if (cam != null)
                virtualCameraNoise = cam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

            GetScreenSwipeAngle();
        }


        // Update is called once per frame
        void Update()
        {
            if (Application.isMobilePlatform == true)
                CameraPhoneInput();


            if (cam != null && virtualCameraNoise != null)
            {
                // If Camera Shake effect is still playing
                if (ShakeElapsedTime > 0)
                {
                    // Set Cinemachine Camera Noise parameters
                    virtualCameraNoise.m_AmplitudeGain = ShakeAmplitude;
                    virtualCameraNoise.m_FrequencyGain = ShakeFrequency;

                    // Update Shake Timer
                    ShakeElapsedTime -= Time.deltaTime;
                }
                else
                {
                    // If Camera Shake effect is over, reset variables
                    virtualCameraNoise.m_AmplitudeGain = 0f;
                    ShakeElapsedTime = 0f;
                }
            }
        }

        public void ScreenShake()
        {
            ShakeElapsedTime = ShakeDuration;
        }

        void CameraPhoneInput()
        {
            if (Input.touchCount == 1)
            {
                //touch0 = Input.GetTouch(0);
                //GetScreenSwipeAngle();
                //ScrollingSimple(touch0.deltaPosition);


            }
            else if (Input.touchCount == 2)
            {
                touch0 = Input.GetTouch(0);
                touch1 = Input.GetTouch(1);

                touch0PrevPos = touch0.position - touch0.deltaPosition;
                touch1PrevPos = touch1.position - touch1.deltaPosition;

                prevMagn = (touch0PrevPos - touch1PrevPos).magnitude;
                currentMagn = (touch0.position - touch1.position).magnitude;

                difference = currentMagn - prevMagn;

                GetScreenSwipeAngle();
                Zooming(difference);
                Scrolling(touch0.deltaPosition, touch1.deltaPosition);


            }
        }

        void CameraPCInput()
        {
            //mouse0 = Input.mousePosition;
            //GetScreenSwipeAngle();
            //ScrollingSimple(mouse0);
        }

        void Zooming(float difference)
        {
            currentCamPos = cam.transform.localPosition;
            currentCamZPos = currentCamPos.z;
            currentCamPos.z = Mathf.Clamp(currentCamZPos + (difference * zoomPower), currentCamZPos - 2, currentCamZPos + 2);
            cam.transform.localPosition = currentCamPos;
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

        void ScrollingSimple(Vector2 touch0Pos)
        {

            mediumYMouv = ((touch0Pos.x)) * mouvPower;
            mediumXMouv = ((touch0Pos.y)) * mouvPower;

            baseRotation = pivotMainCamera.eulerAngles;
            baseYRotation = baseRotation.y;
            baseXRotation = baseRotation.x;
            baseRotation.y = baseRotation.y + mediumYMouv;
            baseRotation.x = baseRotation.x - mediumXMouv;
            pivotMainCamera.eulerAngles = baseRotation;
        }

        public void GetScreenSwipeAngle()
        {
            // Convert SwipePosition Points in Screen Space
            pointCenterRef = NormalCam.WorldToScreenPoint(pointCenter.position);
            pointNordRef = NormalCam.WorldToScreenPoint(pointNord.position);
            pointWestRef = NormalCam.WorldToScreenPoint(pointWest.position);
            pointSudRef = NormalCam.WorldToScreenPoint(pointSud.position);
            pointEstRef = NormalCam.WorldToScreenPoint(pointEst.position);

            // Calcul the angle between the two SwipePosition
            KUBNordScreenAngle = Mathf.Abs(Mathf.Atan2(pointNordRef.y - pointCenterRef.y, pointCenterRef.x - pointNordRef.x) * 180 / Mathf.PI - 180);
            KUBWestScreenAngle = Mathf.Abs(Mathf.Atan2(pointWestRef.y - pointCenterRef.y, pointCenterRef.x - pointWestRef.x) * 180 / Mathf.PI - 180);
            KUBSudScreenAngle = Mathf.Abs(Mathf.Atan2(pointSudRef.y - pointCenterRef.y, pointCenterRef.x - pointSudRef.x) * 180 / Mathf.PI - 180);
            KUBEstScreenAngle = Mathf.Abs(Mathf.Atan2(pointEstRef.y - pointCenterRef.y, pointCenterRef.x - pointEstRef.x) * 180 / Mathf.PI - 180);

        }
    }
}
