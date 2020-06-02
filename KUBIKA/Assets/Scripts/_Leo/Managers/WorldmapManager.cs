using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldmapManager : MonoBehaviour
{
    private static WorldmapManager _instance;
    public static WorldmapManager instance { get { return _instance; } }

    public List<GameObject> levelCubes = new List<GameObject>();

    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(this);
        else _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
