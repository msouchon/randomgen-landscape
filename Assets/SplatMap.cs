using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplatMap : MonoBehaviour
{
    public bool blend = true;
    public Material material;

    // Start is called before the first frame update
    void Start()
    {
        Terrain terrain = GetComponent<Terrain>();
	TerrainData terrainData = terrain.terrainData;

	float[,,] splatMap = new float[terrainData.alphamapWidth,
		                       terrainData.alphamapHeight,
				       terrainData.alphamapLayers];

	for (int y = 0; y < terrainData.alphamapHeight; y++) {
	    for (int x = 0; x < terrainData.alphamapWidth; x++) {
		
		float yNorm = (float) y / (float) terrainData.alphamapHeight;
		float xNorm = (float) x / (float) terrainData.alphamapWidth;
		float height = terrainData.GetInterpolatedHeight(yNorm, xNorm);

		Vector3 normal = terrainData.GetInterpolatedNormal(yNorm, xNorm);
		float steepness = terrainData.GetSteepness(yNorm, xNorm);

		float[] splatWeights = new float[terrainData.alphamapLayers];

		// exposed rock
		splatWeights[2] = Mathf.Clamp01(steepness*steepness/terrainData.heightmapHeight) * 1.8f;
		// snow
		if (height > 40 + Random.value*8) splatWeights[3] = 2f;
		// sand
		else if (height < 7 + Random.value*3) splatWeights[1] = 2f;
		// grass
		else splatWeights[0] = (1f - Mathf.Clamp01(steepness*steepness/(terrainData.heightmapHeight))) * 5f;

		float z = 0;
		float max = 0;
		int maxi = 0;

		for (int i = 0; i < splatWeights.Length; i++) {
		    z += splatWeights[i];
		    if (max < splatWeights[i]) {
			    maxi = i;
			    max = splatWeights[i];
		    }

		}
		splatWeights[maxi] += 8f;
		z += 8f;

		for (int i = 0; i < splatWeights.Length; i++) {
		    splatWeights[i] /= z;
		    splatMap[x, y, i] = splatWeights[i];
		}
	    }
	}
	terrainData.SetAlphamaps(0, 0, splatMap);
    }

    // Update is called once per frame
    void Update()
    {
        if(blend) {
            material.SetFloat("_Blend", 1);
        } else {
            material.SetFloat("_Blend", 0);
        }
    }
}
