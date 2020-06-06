using Sirenix.OdinInspector;
using System;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;

namespace Kubika.Game
{
    public class CubePopulatorManager : MonoBehaviour
    {
        public GameObject cubePrefab, universePrefab;

        public GridLayoutGroup staticGridGroup, functionGridGroup, universeGridGroup;
        [Header("DO NOT MOVE ORDER")] //order is based on the order the cubes are declared in
        
        Sprite[] selectedStaticCubesArray;
        [FoldoutGroup("Static Cubes")] public Sprite[] staticCubes1, staticCubes2, staticCubes3, staticCubes4, staticCubes5, staticCubes6;
        public Sprite[] functionCubes, universePack;

        // Start is called before the first frame update
        void Start()
        {
            StaticCubePopulator();
            FunctionCubePopulator();
            UniversePackPopulator();
        }

        private void FunctionCubePopulator()
        {
            functionGridGroup.constraintCount = (int)CubeTypes.Count - (int)CubeTypes.MoveableCube;

            for (int i = (int)CubeTypes.MoveableCube, j = 0; i < (int)CubeTypes.Count; i++, j++)
            {
                GameObject newObj = Instantiate(cubePrefab, functionGridGroup.gameObject.transform);
                newObj.GetComponent<Image>().sprite = functionCubes[j];
                newObj.GetComponent<Image>().SetNativeSize();
                newObj.GetComponent<CubeSelector>().selectedCubeType = (CubeTypes)i;
            }
        }

        private void StaticCubePopulator()
        {
            staticGridGroup.constraintCount = (int)CubeTypes.QuadStaticCube; // 6 

            if ((int)_MaterialCentral.instance.staticIndex == (int)Biomes.Plains) selectedStaticCubesArray = staticCubes1;
            if ((int)_MaterialCentral.instance.staticIndex == (int)Biomes.Mountains) selectedStaticCubesArray = staticCubes2;
            if ((int)_MaterialCentral.instance.staticIndex == (int)Biomes.Underwater) selectedStaticCubesArray = staticCubes3;
            if ((int)_MaterialCentral.instance.staticIndex == (int)Biomes.Ruins) selectedStaticCubesArray = staticCubes4;
            if ((int)_MaterialCentral.instance.staticIndex == (int)Biomes.Statues) selectedStaticCubesArray = staticCubes5;
            if ((int)_MaterialCentral.instance.staticIndex == (int)Biomes.Temple) selectedStaticCubesArray = staticCubes6;

            for (int i = (int)CubeTypes.FullStaticCube; i <= (int)CubeTypes.QuadStaticCube; i++)
            {
                GameObject newObj = Instantiate(cubePrefab, staticGridGroup.gameObject.transform);
                newObj.GetComponent<Image>().sprite = selectedStaticCubesArray[i - 1];
                newObj.GetComponent<Image>().SetNativeSize();
                newObj.GetComponent<CubeSelector>().selectedCubeType = (CubeTypes)i;
            }
        }

        public void UniversePackPopulator()
        {
            universeGridGroup.constraintCount = (int)Biomes.Count;

            for (int i = 0; i <= universePack.Length; i++)
            {
                GameObject newObj = Instantiate(universePrefab, universeGridGroup.gameObject.transform);
                newObj.GetComponent<Image>().sprite = universePack[i];
                newObj.GetComponent<Image>().SetNativeSize();
                newObj.GetComponent<CubeSelector>().biomes = (Biomes)i;
            }
        }
    }
}
