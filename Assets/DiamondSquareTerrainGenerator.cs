using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DiamondSquareTerrainGenerator : MonoBehaviour
{

    public int actualSize = 256;

    public int resolutionFactor = 7;

    public int height = 200;

    public float variation = 1.0f;

    [Range (0, 1)] public float smoothness;

    public bool tanhFilter = false;

    public bool laplaceFilter = false;

    [Range (0, 10)] public int laplaceFilterPasses = 1;

    // Start is called before the first frame update
    void Start()
    {
	int resolution = (int) Mathf.Pow(2, resolutionFactor) + 1;
        Terrain terrain = GetComponent<Terrain>();
	terrain.terrainData.heightmapResolution = resolution + 1;
	terrain.terrainData.size = new Vector3(actualSize, height, actualSize);
	float[,] heightmap = GenerateHeightmap(resolution);
	if (tanhFilter) heightmap = SmoothingFilter(heightmap, resolution);
	if (laplaceFilter) {
	    for (int i = 0; i < laplaceFilterPasses; i++) {
	        heightmap = LaplaceFilter(heightmap, resolution);
	    }
	}
	terrain.terrainData.SetHeights(0, 0, heightmap);
    }
    float[,] GenerateHeightmap(int size) {
	float[,] heightmap = new float[size, size];

	float cornerHeight = Random.value;
	heightmap[0, 0] = cornerHeight;
	heightmap[0, size-1] = cornerHeight;
	heightmap[size-1, 0] = cornerHeight;
	heightmap[size-1, size-1] = cornerHeight;

	float average = 1.0f;
	int step, x, y;

	for (step = (size-1)/2; step > 1; step /= 2) {
	     // Diamond Step
	     for (x = 0; x < size-1; x += step) {
		for (y = 0; y < size-1; y += step) {
		    average = (heightmap[x, y] +
			       heightmap[x, y + step] +
			       heightmap[x + step, y] +
			       heightmap[x + step, y + step]
			      ) / 4.0f;
		    average += (Random.value - 0.5f) * variation;
		    heightmap[x + step/2, y + step/2] = Mathf.Clamp01(average);
		}
	     }
	     // Square Step
	     for (x = 0; x < size-1; x += step/2) {
		for (y = (x + step/2) % step; y < size-1; y += step) {
		    average = (heightmap[x, (y + step/2) % (size - 1)] +
			       heightmap[x, (y - step/2 + size - 1) % (size - 1)] +
			       heightmap[(x + step/2) % (size - 1), y] +
			       heightmap[(x - step/2 + size - 1) % (size - 1), y]
			      ) / 4.0f;
		    average += (Random.value - 0.5f) * variation;
		    heightmap[x, y] = Mathf.Clamp01(average);
		    if (x == 0) heightmap[size - 1, y] = Mathf.Clamp01(average);
		    if (y == 0) heightmap[x, size - 1] = Mathf.Clamp01(average);
		}
	     }
	     variation *= smoothness;
	}

	return heightmap;
    }
    /*
    void DiamondStep(float[,] heightmap, int curr_size, int x, int y) {
	Debug.Log(curr_size);
	float average = (heightmap[x - curr_size, y - curr_size] + // Bottom Left
		         heightmap[x - curr_size, y + curr_size] + // Top Left
		         heightmap[x + curr_size, y - curr_size] + // Bottom Right
		         heightmap[x + curr_size, y + curr_size] // Top Right
			) / 4.0f;
	heightmap[x, y] = average + Random.value;
	if (curr_size >= 2) {
	    SquareStep(heightmap, curr_size, x, y - curr_size);
	    SquareStep(heightmap, curr_size, x, y + curr_size);
	    SquareStep(heightmap, curr_size, x - curr_size, y);
	    SquareStep(heightmap, curr_size, x + curr_size, y);
	}
    }
    void SquareStep(float[,] heightmap, int curr_size, int x, int y) {
	float average = (heightmap[(x - curr_size + size) % size, y] +
			 heightmap[(x + curr_size) % size, y] +
			 heightmap[x, (y - curr_size + size) % size] +
			 heightmap[x, (y + curr_size) % size]
			) / 4.0f;
	heightmap[x, y] = average + Random.value;
	if (curr_size >= 3) {
	    curr_size = (curr_size-1)/2;
	    DiamondStep(heightmap, curr_size, (x + curr_size) % size, (y + curr_size) % size);
	}
    }*/

    float[,] SmoothingFilter(float[,] heightmap, int size) {
	for (int x = 0; x < size; x++) {
	    for (int y = 0; y < size; y++) {
		heightmap[x, y] = (float) (System.Math.Tanh(heightmap[x, y] * Mathf.PI / 2) + 1)/2;
	    }
	}
	return heightmap;
    }

    float[,] LaplaceFilter(float[,] heightmap, int size) {
	for (int x = 0; x < size; x++) {
	    for (int y = 0; y < size; y++) {
		heightmap[x, y] = (heightmap[(x-1 + size) % size, y] +
				   heightmap[(x+1) % size, y] +
				   heightmap[x, (y-1+ size) % size] +
				   heightmap[x, (y+1) % size])/4;
	    }
	}
	return heightmap;
    }
}
