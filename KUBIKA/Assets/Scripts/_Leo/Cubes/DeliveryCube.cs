using System;
using UnityEngine;

namespace Kubika.Game
{
    public class DeliveryCube : _CubeScanner
    {
        bool touchingVictory;
        private bool locked;

        GameObject LightShaft;
        Vector3 newRotate = new Vector3();

        // VICTORY FX
        _CubeBase victoryCubeTracker;

        // Start is called before the first frame update
        public override void Start()
        {
            //call base.start AFTER assigning the cube's layers
            base.Start();

            _DataManager.instance.EndFalling.AddListener(CheckForVictory);
            _DataManager.instance.EndFalling.AddListener(ScannerSet);

            SetFb();

        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
            //Debug.Log("Cheking in " + myIndex + _DirectionCustom.LocalScanner(facingDirection));
        }

        private void CheckForVictory()
        {
            //touchingVictory = ProximityChecker(_DirectionCustom.fixedUp, CubeTypes.BaseVictory, CubeLayers.None);
            touchingVictory = VictoryChecker(_DirectionCustom.LocalScanner(facingDirection));

            if (touchingVictory && locked == false)
            {
                locked = true;
                LightShaft.GetComponent<FB_Delivry>().ActivatePSFB();
                StartCoroutine(victoryCubeOnPistion.VictoryFX(true));
                victoryCubeTracker = victoryCubeOnPistion;
                VictoryConditionManager.instance.IncrementVictory();
            }

            // flip the bools when the delivery cube loses track of the victory cube
            if (touchingVictory == false && locked == true)
            {
                StartCoroutine(victoryCubeTracker.VictoryFX(false));
                VictoryConditionManager.instance.DecrementVictory();
                locked = false;
            }
        }

        void SetFb()
        {

            switch (facingDirection)
            {
                case FacingDirection.up:
                    LightShaft = Instantiate(_FeedBackManager.instance.Fb_Delivry, transform.position, Quaternion.identity, transform);
                    newRotate.z = 90;
                    LightShaft.transform.eulerAngles = newRotate;
                    break;
                case FacingDirection.down:
                    LightShaft = Instantiate(_FeedBackManager.instance.Fb_Delivry, transform.position, Quaternion.identity, transform);
                    newRotate.z = 270;
                    LightShaft.transform.eulerAngles = newRotate;
                    break;
                case FacingDirection.right:
                    LightShaft = Instantiate(_FeedBackManager.instance.Fb_Delivry, transform.position, Quaternion.identity, transform);
                    LightShaft.transform.eulerAngles = newRotate;
                    break;
                case FacingDirection.left:
                    LightShaft = Instantiate(_FeedBackManager.instance.Fb_Delivry, transform.position, Quaternion.identity, transform);
                    newRotate.z = 180;
                    LightShaft.transform.eulerAngles = newRotate;
                    break;
                case FacingDirection.forward:
                    LightShaft = Instantiate(_FeedBackManager.instance.Fb_Delivry, transform.position, Quaternion.identity, transform);
                    newRotate.y = 270;
                    LightShaft.transform.eulerAngles = newRotate;
                    break;
                case FacingDirection.backward:
                    LightShaft = Instantiate(_FeedBackManager.instance.Fb_Delivry, transform.position, Quaternion.identity, transform);
                    newRotate.y = 90;
                    LightShaft.transform.eulerAngles = newRotate;
                    break;
            }
        }
    }
}