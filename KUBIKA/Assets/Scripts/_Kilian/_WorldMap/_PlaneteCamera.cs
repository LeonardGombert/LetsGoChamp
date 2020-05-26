﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Kubika.Game
{
    public class _PlaneteCamera : MonoBehaviour
    {
        public CinemachineVirtualCamera vCam;
        public int index;
        public ParticleSystem PS;

        [Space]
        [Header("CamLimit")]
        public Transform pivotVCam;
        Vector3 currentPivotPosition;
        public Transform LimitStart;
        public Transform LimitEnd;
        public Transform LimitStartScroll;
        public Transform LimitEndScroll;

        [Space]
        public bool isActive = false;
        public float ScrollPower = 0.1f;

        // TOUCH
        Touch touch;
        Ray ray;
        RaycastHit hit;
        bool hasTouchedLevel = false;

        //SCROLL
        public float distanceLimitCam;
        public float distanceLimitScroll;
        protected Plane Plane;
        protected Vector3 BaseScroll;
        protected Vector3 CurrentScroll;

        protected Vector2 BaseScrollScreen;
        protected Vector2 CurrentScrollScreen;
        protected float CurrentPositionSymbol;
        float actualPosScroll;
        float actualPosCam;

        public void ActivatePSFB()
        {
            PS.Play();
        }

        void Update()
        {
            if(isActive == true)
            {
                touch = Input.GetTouch(0);
                ray = Camera.main.ScreenPointToRay(touch.position);


                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (Physics.Raycast(ray, out hit))
                        {
                            Debug.Log("Touch Hit " + hit.collider.gameObject.name);

                            if (hit.collider.gameObject.GetComponent<_ScriptMatFaceCube>())
                            {
                                Debug.Log("ClickedOnLevel");
                                hasTouchedLevel = true;
                            }
                            else
                            {
                                BaseScroll = hit.point;
                            }
                        }
                        break;
                    case TouchPhase.Moved:
                        if (hasTouchedLevel == false)
                        {
                            ScrollingSimple(touch.deltaPosition.y);
                        }
                        break;
                    case TouchPhase.Ended:
                        hasTouchedLevel = false;
                        break;
                }
            }
        }

        void ScrollingSimple(float FingerPosition)
        {
            if (Physics.Raycast(ray, out hit))
            {
                CurrentScroll = hit.point;

                currentPivotPosition = pivotVCam.transform.position;

                distanceLimitCam = Vector3.Distance(LimitStart.position, LimitEnd.position);
                distanceLimitScroll = Vector3.Distance(LimitStartScroll.position, LimitEndScroll.position);

                BaseScrollScreen = _Planete.instance.MainCam.WorldToScreenPoint(BaseScroll);
                CurrentScrollScreen = _Planete.instance.MainCam.WorldToScreenPoint(CurrentScroll);

                CurrentPositionSymbol = Mathf.Sign(CurrentScrollScreen.y - BaseScrollScreen.y);

                actualPosCam = InverseLerp(LimitStart.position, LimitEnd.position, pivotVCam.transform.position);
                actualPosScroll = (((actualPosCam - ((Vector3.Distance(BaseScroll, CurrentScroll) / distanceLimitScroll) * CurrentPositionSymbol))));

                //Debug.Log("actualPos_TEEEEEEES " + actualPos_TEEEEEEES);


                Debug.Log(" CAM = " + (Vector3.Distance(LimitStartScroll.position, CurrentScroll) / distanceLimitScroll) * 100 +
                " || Scroll2 = " + (Vector3.Distance(LimitStartScroll.position, BaseScroll) / distanceLimitScroll) +
                " || Scroll3 = " + ((Vector3.Distance(LimitStartScroll.position, BaseScroll) / distanceLimitScroll) - ((Vector3.Distance(BaseScroll, CurrentScroll) / distanceLimitScroll) * CurrentPositionSymbol)) +
                " || Scroll4 = " + (Vector3.Distance(LimitStartScroll.position, BaseScroll) / distanceLimitScroll - (Vector3.Distance(BaseScroll, CurrentScroll) / distanceLimitScroll)) +
                " || Scroll = " + actualPosCam);

                currentPivotPosition = Vector3.Lerp(LimitStart.position, LimitEnd.position, Mathf.Abs(actualPosScroll));
                pivotVCam.transform.position = currentPivotPosition;
            }


        }

        public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
        {
            Vector3 AB = b - a;
            Vector3 AV = value - a;
            return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
        }

        protected Vector3 PlanePositionDelta(Touch touch)
        {
            //not moved
            if (touch.phase != TouchPhase.Moved)
                return Vector3.zero;

            //delta
            var rayBefore = _Planete.instance.MainCam.ScreenPointToRay(touch.position - touch.deltaPosition);
            var rayNow = _Planete.instance.MainCam.ScreenPointToRay(touch.position);
            if (Plane.Raycast(rayBefore, out var enterBefore) && Plane.Raycast(rayNow, out var enterNow))
                return rayBefore.GetPoint(enterBefore) - rayNow.GetPoint(enterNow);

            //not on plane
            return Vector3.zero;
        }
    }
}
