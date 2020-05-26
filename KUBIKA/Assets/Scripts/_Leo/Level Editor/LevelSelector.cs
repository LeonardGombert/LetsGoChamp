using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubika.Game
{
    public class LevelSelector : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            if (LevelsManager.instance.testingKubiCode == "") LevelsManager.instance.testingKubiCode = "Worl101";
        }

        // called when the user presses the "play" button
        public void SelectLevel()
        {
            UIManager.instance.LoadLevelFromWM(LevelsManager.instance.testingKubiCode);
        }
    }
}