﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour
{
    public float waterHeight = 5;

    public int vertexDensity = 100;

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
	plane = new GameObject();
	plane.name = "Water";
	MeshFilter planeMesh = plane.AddComponent(typeof(MeshFilter)) as MeshFilter;
	planeMesh.mesh = createPlaneMesh(vertexDensity, vertexDensity);
	plane.AddComponent(typeof(MeshCollider));
	plane.transform.localScale = terrainData.size/vertexDensity;
	plane.transform.localPosition = new Vector3(0, waterHeight, 0);
	plane.AddComponent<MeshRenderer>();
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
    
    Mesh createPlaneMesh(int xVertices, int yVertices)
    {
	Mesh mesh = new Mesh();
	mesh.name = "WaterMesh";

	Vector3[] vertices = new Vector3[(xVertices + 1) * (yVertices + 1)];
	Vector2[] uvs = new Vector2[(xVertices + 1) * (yVertices + 1)];
	int index = 0;
	for (float y = 0; y < yVertices + 1; y++) {
	    for (float x = 0; x < xVertices + 1; x++) {
		vertices[index] = new Vector3(x, 0, y);
		uvs[index] = new Vector2(x / xVertices, y / yVertices);
		index += 1;
	    }
	}
	
	index = 0;
	int[] triangles = new int[xVertices * yVertices * 6];
	for (int y = 0; y < yVertices; y++) {
	    for (int x = 0; x < xVertices; x++) {
		triangles[index] = (xVertices + 1) * y + x;
		triangles[index + 1] = (xVertices + 1) * (y + 1) + x;
		triangles[index + 2] = (xVertices + 1) * y + x + 1;
		triangles[index + 3] = (xVertices + 1) * (y + 1) + x;
		triangles[index + 4] = (xVertices + 1) * (y + 1) + x + 1;
		triangles[index + 5] = (xVertices + 1) * y + x + 1;
		index += 6;
	    }
	}

	mesh.vertices = vertices;
	mesh.uv = uvs;
	mesh.triangles = triangles;
	mesh.RecalculateNormals();
	mesh.RecalculateBounds();
	return mesh;
    }
}
