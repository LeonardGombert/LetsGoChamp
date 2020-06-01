using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kubika.Game
{
    public class TimerCube : _CubeScanner
    {
        public int timerValue;
        bool touchedCube;

        public List<int> touchingCubeIndex = new List<int>();
        private bool hasCubes;

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

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
        }

        private void CubeListener()
        {
            //if the timer already has cubes it is following
            if (hasCubes)
            {
                Debug.Log("looking out for my cubes");

                // check each registered index to make sure the cube is still there
                foreach (int index in touchingCubeIndex)
                {
                    // if one or more of the cubes have moved, reset the bools
                    if (grid.kuboGrid[index - 1].cubeOnPosition == null)
                    {
                        //reset find cube variables
                        touchedCube = false;
                        hasCubes = false;

                        //decrement the value by 1 for the next pass
                        Debug.Log("Man down !");
                        timerValue--;

                        ChangeTexture(timerValue);
                    }
                }
            }

            if (timerValue > 0)
            {
                // forget the cubes you've already registered (in case only 1 moves)
                touchingCubeIndex.Clear();

                // check in every "direction"
                foreach (int index in indexesToCheck)
                {
                    touchedCube = AnyMoveableChecker(index);

                    // if you touch a cube
                    if (touchedCube)
                    {
                        // save that cube to the list of "registered" cubes
                        touchingCubeIndex.Add(myIndex + index);

                        // set your state to "has registered cubes"
                        hasCubes = true;
                    }
                }
            }

            else PopOut();
        }

        void ChangeTexture(int actualTimerValue)
        {

            switch(actualTimerValue)
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