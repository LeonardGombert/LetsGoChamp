using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kubika.Game
{
    public class TimerCube : _CubeScanner
    {
        public int timerValue;
        [SerializeField] bool touchingCube;

        [SerializeField] GameObject touchingGO;
        [SerializeField] GameObject newTouchingGO;
        [SerializeField] List<int> touchingCubeIndex = new List<int>();

        // Start is called before the first frame update
        public override void Start()
        {
            //call base.start AFTER assigning the cube's layers
            base.Start();
            SetScanDirections();
            _DataManager.instance.EndFalling.AddListener(CubeListener);

            ChangeTexture(timerValue);
        }

        public override void UndoProcedure()
        {
            base.UndoProcedure();
            _DataManager.instance.EndFalling.AddListener(CubeListener);
        }

        public override void HideCubeProcedure()
        {
            base.HideCubeProcedure();
            _DataManager.instance.EndFalling.RemoveListener(CubeListener);
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
        }

        private void CubeListener()
        {
            if (!touchingCube)
            {
                //returns the gameobject you are touching
                touchingGO = AnyMoveableChecker(_DirectionCustom.up);

                if (touchingGO != null) touchingCube = true;
            }

            else if (touchingCube)
            {
                newTouchingGO = AnyMoveableChecker(_DirectionCustom.up);

                if (newTouchingGO != touchingGO || newTouchingGO == null)
                {
                    touchingCube = false;
                    newTouchingGO = null;
                    touchingGO = null;
                    timerValue--;
                }
            }

            if (timerValue <= 0) StartCoroutine(PopOut());
        }

        void ChangeTexture(int actualTimerValue)
        {
            switch (actualTimerValue)
            {
                case 9:
                    _MainTex = _MaterialCentral.instance.actualPack._CounterTex9;
                    break;
                case 8:
                    _MainTex = _MaterialCentral.instance.actualPack._CounterTex8;
                    break;
                case 7:
                    _MainTex = _MaterialCentral.instance.actualPack._CounterTex7;
                    break;
                case 6:
                    _MainTex = _MaterialCentral.instance.actualPack._CounterTex6;
                    break;
                case 5:
                    _MainTex = _MaterialCentral.instance.actualPack._CounterTex5;
                    break;
                case 4:
                    _MainTex = _MaterialCentral.instance.actualPack._CounterTex4;
                    break;
                case 3:
                    _MainTex = _MaterialCentral.instance.actualPack._CounterTex3;
                    break;
                case 2:
                    _MainTex = _MaterialCentral.instance.actualPack._CounterTex2;
                    break;
                case 1:
                    _MainTex = _MaterialCentral.instance.actualPack._CounterTex1;
                    break;
            }

            meshRenderer.GetPropertyBlock(MatProp);

            MatProp.SetTexture("_MainTex", _MainTex);

            meshRenderer.SetPropertyBlock(MatProp);
        }

    }
}