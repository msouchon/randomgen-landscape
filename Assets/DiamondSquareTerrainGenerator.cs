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

    public bool laplaceFilter = true;

    [Range (0, 10)] public int laplaceFilterPasses = 10;

    void Start()
    {
	// Calculate a valid resolution that is permitted by the algorithm
	int resolution = (int) Mathf.Pow(2, resolutionFactor) + 1;

	// Set up terrain
        Terrain terrain = GetComponent<Terrain>();
	terrain.terrainData.heightmapResolution = resolution + 1;
	terrain.terrainData.size = new Vector3(actualSize, height, actualSize);
	float[,] heightmap = GenerateHeightmap(resolution);

	// Run filters
	if (tanhFilter) heightmap = TanhFilter(heightmap, resolution);
	if (laplaceFilter) {
	    for (int i = 0; i < laplaceFilterPasses; i++) {
	        heightmap = LaplaceFilter(heightmap, resolution);
	    }
	}

	terrain.terrainData.SetHeights(0, 0, heightmap);
    }

    // Given a size for a heightmap use the diamond square algorithm to build it
    float[,] GenerateHeightmap(int size) {
	float[,] heightmap = new float[size, size];

	// First Step:
	// Randomise corner values
	// They are the same as it allows this terrain to be placed
	// alongside itself
	float cornerHeight = Random.value;
	heightmap[0, 0] = cornerHeight;
	heightmap[0, size-1] = cornerHeight;
	heightmap[size-1, 0] = cornerHeight;
	heightmap[size-1, size-1] = cornerHeight;

	float average;
	int step, x, y;

	// Half step size each time both steps are completed
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

		    // Clamp the value to make sure it doesnt go below 0
		    // Unity terrain heightmaps only indicate difference between 0 and 1
		    heightmap[x + step/2, y + step/2] = Mathf.Clamp01(average);
		}
	     }
	     // Square Step
	     for (x = 0; x < size-1; x += step/2) {
		for (y = (x + step/2) % step; y < size-1; y += step) {
		    // As square steps can rely on data outside of the heightmap
		    // wrap around.
		    // This allows the heightmap to be tiled
		    average = (heightmap[x, (y + step/2) % (size - 1)] +
			       heightmap[x, (y - step/2 + size - 1) % (size - 1)] +
			       heightmap[(x + step/2) % (size - 1), y] +
			       heightmap[(x - step/2 + size - 1) % (size - 1), y]
			      ) / 4.0f;
		    average += (Random.value - 0.5f) * variation;
		    heightmap[x, y] = Mathf.Clamp01(average);

		    // If setting the value of an edge point, set it also
		    // on the wrap around
		    if (x == 0) heightmap[size - 1, y] = Mathf.Clamp01(average);
		    if (y == 0) heightmap[x, size - 1] = Mathf.Clamp01(average);
		}
	     }
	     // After each set of steps reduce the randomness to smooth out the terrain
	     variation *= smoothness;
	}

	return heightmap;
    }

    // Using the tanh function, applies it across the heightmap
    // to change its values
    float[,] TanhFilter(float[,] heightmap, int size) {
	for (int x = 0; x < size; x++) {
	    for (int y = 0; y < size; y++) {
		heightmap[x, y] = (float) (System.Math.Tanh(heightmap[x, y] * Mathf.PI / 2) + 1)/2;
	    }
	}
	return heightmap;
    }

    // This filter is made for smoothing of polygonal meshes
    // This can smooth a heightmap out after creation
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
