using Kubika.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubika.Game
{
    public class SwitchButton : _CubeScanner
    {
        SwitchCube[] switchCubes;
        bool isPressed;
        private bool locked;

        // Start is called before the first frame update
        public override void Start()
        {
            //call base.start AFTER assigning the cube's layers
            base.Start();
            _DataManager.instance.EndFalling.AddListener(CheckIfPressed);
        }

        void OnEnable()
        {
            switchCubes = FindObjectsOfType<SwitchCube>();
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
        }

        private void CheckIfPressed()
        {
            isPressed = AnyMoveableChecker(_DirectionCustom.up);
            Debug.DrawRay(transform.position, Vector3.up, Color.green);

            if (isPressed == true && locked == false)
            {
                locked = true;
                ActivateSwitches();
            }

            if(isPressed == false && locked == true)
            {
                locked = false;
                DeactivateSwitches();
            }
        }

        private void ActivateSwitches()
        {
            Debug.Log("A Cube is pressing down on me, I'm activating switch cubes");

            foreach (SwitchCube cube in switchCubes) cube.isActive = true;
        }

        private void DeactivateSwitches()
        {
            Debug.Log("The Cube left, I'm deactivating all switch cubes");

            foreach (SwitchCube cube in switchCubes) cube.isActive = false;
        }
    }
}