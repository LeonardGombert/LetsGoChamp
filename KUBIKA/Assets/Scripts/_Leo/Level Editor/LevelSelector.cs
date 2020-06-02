using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubika.Game
{
    public class LevelSelector : MonoBehaviour
    {
        RaycastHit hit;

        // Called every frame
        void Update()
        {
            CheckForNodeTouch();
        }

        // call when the user loads the worlmap, makes the camera focus to last beaten level
        void FocusOnLastBeatenLevel()
        {

        }

        private void CheckForNodeTouch()
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.GetTouch(0).position), out hit))
            {
                LevelCube levelCube = hit.collider.gameObject.GetComponent<LevelCube>();
                if (levelCube != null) LevelsManager.instance.loadToKubicode = levelCube.kubicode;
            }
        }
    }
}