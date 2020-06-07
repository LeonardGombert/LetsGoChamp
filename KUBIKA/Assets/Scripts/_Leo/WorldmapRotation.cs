using Kubika.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldmapRotation : MonoBehaviour
{
    Transform topArrowObj;
    Transform bottomArrowObj;

    public GameObject worldMap;
    public GameObject activeFace;

    public Biomes currentBiome;

    private void Start()
    {

    }

    void RefreshWorldArrowTargets()
    {
        activeFace = worldMap.transform.GetChild(1).transform.GetChild(0).transform.GetChild((int)currentBiome).gameObject;

        topArrowObj = activeFace.transform.GetChild(3).GetChild(0).transform;
        bottomArrowObj = activeFace.transform.GetChild(3).GetChild(1).transform;
    }

    // Update is called once per frame
    void Update()
    {
        RefreshWorldArrowTargets();
        UIManager.instance.bottomArrow.rectTransform.position = Camera.main.WorldToScreenPoint(bottomArrowObj.position);
        UIManager.instance.topArrow.rectTransform.position = Camera.main.WorldToScreenPoint(topArrowObj.position);
    }
}
