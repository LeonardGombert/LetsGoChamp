using Kubika.CustomLevelEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubika.Game
{
    public class LevelSelector : MonoBehaviour
    {
        RaycastHit hit;
        public LayerMask layerMask;

        // Called every frame
        void Update()
        {
            CheckForNodeTouch();
        }

        private void CheckForNodeTouch()
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(LevelEditor.GetUserPlatform()), out hit))//, Mathf.Infinity, LayerMask.NameToLayer("LevelCubes")))
            {
                LevelCube levelCube = hit.collider.gameObject.GetComponent<LevelCube>();
                if (levelCube != null) LevelsManager.instance.SelectLevel(levelCube.kubicode);
            }
        }
    }
}