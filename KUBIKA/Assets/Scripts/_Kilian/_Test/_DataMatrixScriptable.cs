using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kubika.CustomLevelEditor;

namespace Kubika.Game
{
    [CreateAssetMenu(fileName = "DataBank", menuName = "DataBank", order = 1)]
    public class _DataMatrixScriptable : ScriptableObject
    {
        public Node[] indexBank;

        //prevents values from being updated when changing scenes
        private void OnEnable()
        {
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }
    }
}
