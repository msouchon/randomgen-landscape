using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour
{
    public float waterHeight = 5;

    public Material waterMaterial;

    public float Kd = 1;
    public float Ks = 1;
    public float specN = 1;
    public float Ka = 1;
    public float fAtt = 1;
    
    private GameObject plane;

    void Start()
    {
	TerrainData terrainData = GetComponent<Terrain>().terrainData;
        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
	plane.transform.localScale = terrainData.size/10;
	plane.transform.localPosition = new Vector3(terrainData.size.x/2, waterHeight, terrainData.size.z/2);
	plane.GetComponent<Renderer>().material = waterMaterial;
    }

    void Update()
    {
        waterMaterial.SetFloat("_Ks", Ks);
        waterMaterial.SetFloat("_specN", specN);
        waterMaterial.SetFloat("_Kd", Kd);
        waterMaterial.SetFloat("_Ka", Ka);
        waterMaterial.SetFloat("_fAtt", fAtt);
    }
}
