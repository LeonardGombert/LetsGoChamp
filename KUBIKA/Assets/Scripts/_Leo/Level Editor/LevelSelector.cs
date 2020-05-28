﻿using System;
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

        private void CheckForNodeTouch()
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.GetTouch(0).position), out hit))
            {
                LevelNode levelNode = hit.collider.gameObject.GetComponent<LevelNode>();
                if (levelNode != null) UIManager.instance.loadToKubiCode = levelNode.kubiCode;
            }
        }
    }
}