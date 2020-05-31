﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetationBrush : MonoBehaviour
{
    public float sizeOfBrush;
    public int numberToPlace;
    public GameObject[] objectsToPlace;
    public bool isPlacing, isDeleting;

    RaycastHit hit;
    GameObject brushViz;
    Vector3 brushOrientation;
    Vector3 hitPosition;

    GameObject hitObject;

    // Start is called before the first frame update
    void Start()
    {
        brushViz = this.gameObject;
        sizeOfBrush = brushViz.GetComponent<Renderer>().bounds.extents.x; //get the size of the object
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            brushOrientation = new Vector3(hit.normal.z * 90, 0, hit.normal.x * 90);

            brushViz.transform.position = hit.point;
            brushViz.transform.rotation = Quaternion.Euler(brushOrientation);

            if (Input.GetMouseButtonDown(0) && isPlacing) Place();
            if (Input.GetMouseButtonDown(0) && isDeleting) Delete();
        }
    }

    void Place()
    {
        hitPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        hitObject = hit.collider.gameObject;

        for (int i = 0; i < numberToPlace; i++)
        {
            GameObject newObj = Instantiate(objectsToPlace[Random.Range(0, objectsToPlace.Length)], hitPosition, Quaternion.Euler(brushOrientation));
            Vector2 randomizedPosition = Random.insideUnitCircle * sizeOfBrush;

            Vector3 randomizedPos = new Vector3(randomizedPosition.x, 0, randomizedPosition.y);

            newObj.transform.position += randomizedPos;
            newObj.transform.parent = hit.transform;
        }
    }

    void Delete()
    {

    }
}
