using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class _DecorMaterialHeightPreset : MonoBehaviour
{
    MeshRenderer meshrender;
    MaterialPropertyBlock matProp;
    public float maxHeight;

    [SerializeField] Color ColorMin = Color.black;
    [SerializeField] Color ColorMax;
    public float ColorLerp;

    void Start()
    {      
        AddRandomHeight();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            AddRandomHeight(); 
        }
            
    }

    void AddRandomHeight()
    {
        meshrender = GetComponent<MeshRenderer>();
        matProp = new MaterialPropertyBlock();

        meshrender.GetPropertyBlock(matProp);
        matProp.SetFloat("_VertexHeight", (Random.Range(0, 1) * 2 - 1) *  Random.Range(0, maxHeight)); // Get random negative / positive mul random number

        matProp.SetColor("_ColorMin", ColorMin); 
        matProp.SetColor("_ColorMax", ColorMax); 
        matProp.SetFloat("_ColorLerp", Random.Range(0f,1f)); 

        meshrender.SetPropertyBlock(matProp);
    }


}
