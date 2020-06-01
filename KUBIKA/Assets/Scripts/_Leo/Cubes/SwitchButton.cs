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
        GameObject Button;
        _BoutonFB BoutonScript;
        Vector3 newRotate;
        bool isPressed;
        private bool locked;

        // Start is called before the first frame update
        public override void Start()
        { 
            //call base.start AFTER assigning the cube's layers
            base.Start();
            _DataManager.instance.EndFalling.AddListener(CheckIfPressed);
            switchCubes = FindObjectsOfType<SwitchCube>();
            SpawnButton();
        }

        public override void UndoProcedure()
        {
            base.UndoProcedure();
            _DataManager.instance.EndFalling.AddListener(CheckIfPressed);
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
        }

        private void CheckIfPressed()
        {
            isPressed = AnyMoveableChecker(_DirectionCustom.LocalScanner(facingDirection)); // doont use up, because it needds to be LOCAL up
            
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

            foreach (SwitchCube cube in switchCubes)
            {
                cube.isActive = true;
                cube.StatusUpdate();
            }
        }

        private void DeactivateSwitches()
        {
            Debug.Log("The Cube left, I'm deactivating all switch cubes");

            foreach (SwitchCube cube in switchCubes)
            {
                cube.isActive = false;
                cube.StatusUpdate();
            }
        }

        void SpawnButton()
        {
            switch (facingDirection)
            {
                case FacingDirection.up:
                    Button = Instantiate(_BoutonManager.instance.SwitchButton, transform.position, Quaternion.identity, transform);
                    BoutonScript = Button.GetComponentInChildren<_BoutonFB>();
                    newRotate.z = 90;
                    Button.transform.eulerAngles = newRotate;
                    break;

                case FacingDirection.down:
                    Button = Instantiate(_BoutonManager.instance.SwitchButton, transform.position, Quaternion.identity, transform);
                    BoutonScript = Button.GetComponentInChildren<_BoutonFB>();
                    newRotate.z = 270;
                    Button.transform.eulerAngles = newRotate;
                    break;

                case FacingDirection.right:
                    Button = Instantiate(_BoutonManager.instance.SwitchButton, transform.position, Quaternion.identity, transform);
                    BoutonScript = Button.GetComponentInChildren<_BoutonFB>();
                    newRotate.z = 180;
                    Button.transform.eulerAngles = newRotate;
                    break;

                case FacingDirection.left:
                    Button = Instantiate(_BoutonManager.instance.SwitchButton, transform.position, Quaternion.identity, transform);
                    BoutonScript = Button.GetComponentInChildren<_BoutonFB>();
                    Button.transform.eulerAngles = newRotate;
                    break;

                case FacingDirection.forward:
                    Button = Instantiate(_BoutonManager.instance.SwitchButton, transform.position, Quaternion.identity, transform);
                    BoutonScript = Button.GetComponentInChildren<_BoutonFB>();
                    newRotate.y = 90;
                    Button.transform.eulerAngles = newRotate;
                    break;

                case FacingDirection.backward:
                    Button = Instantiate(_BoutonManager.instance.SwitchButton, transform.position, Quaternion.identity, transform);
                    BoutonScript = Button.GetComponentInChildren<_BoutonFB>();
                    newRotate.y = 270;
                    Button.transform.eulerAngles = newRotate;
                    break;
            }
        }
    }
}