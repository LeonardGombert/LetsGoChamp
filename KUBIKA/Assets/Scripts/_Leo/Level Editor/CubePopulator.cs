using System;
using UnityEngine;
using UnityEngine.UI;

namespace Kubika.Game
{
    public class CubePopulator : MonoBehaviour
    {
        public GameObject prefab;
        GridLayoutGroup gridGroup;
        public bool isFunctionCubeSelector, isStaticCubeSelector, isUniversePackSelector;

        // Start is called before the first frame update
        void Start()
        {
            gridGroup = GetComponent<GridLayoutGroup>();

            if (isStaticCubeSelector) StaticCubePopulator();
            if (isFunctionCubeSelector) FunctionCubePopulator();
            if (isUniversePackSelector) UniversePackPopulator();
        }

        private void FunctionCubePopulator()
        {
            GameObject newObj;

            gridGroup.constraintCount = (int)CubeTypes.Count - (int)CubeTypes.MoveableCube; // as of yet, 17

            for (int i = (int)CubeTypes.MoveableCube; i < (int)CubeTypes.Count; i++)
            {
                newObj = Instantiate(prefab, transform);
                newObj.GetComponent<Image>().color = UnityEngine.Random.ColorHSV();
                newObj.GetComponent<CubeSelector>().selectedCubeType = (CubeTypes)i;
            }
        }

        private void StaticCubePopulator()
        {
            GameObject newObj;

            gridGroup.constraintCount = (int)CubeTypes.QuadStaticCube; // 6 

            for (int i = (int)CubeTypes.FullStaticCube; i <= (int)CubeTypes.QuadStaticCube; i++)
            {
                newObj = Instantiate(prefab, transform);
                newObj.GetComponent<Image>().color = UnityEngine.Random.ColorHSV();
                newObj.GetComponent<CubeSelector>().selectedCubeType = (CubeTypes)i;
            }
        }

        public void UniversePackPopulator()
        {
            GameObject newObj;

            gridGroup.constraintCount = (int)Biomes.Count - 1;

            for (int i = 1; i < (int)Biomes.Count; i++)
            {
                newObj = Instantiate(prefab, transform);
                newObj.GetComponent<Image>().color = UnityEngine.Random.ColorHSV();
                newObj.GetComponent<CubeSelector>().biomes = (Biomes)i;
            }
        }
    }
}
