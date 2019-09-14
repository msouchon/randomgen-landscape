using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls how textures are mapped onto the terrain.
// This is done by a splatmap which allows textures to be
// placed on the terrain with precision and using the shader
public class SplatMap : MonoBehaviour
{
    // Blending of textures in the shader
    public bool blend = true;
    [Range(0.01f, 1f)]
    public float blendAmount = 0.1f;

    public Material material;

    // Shader phong values
    public float Kd = 1;
    public float Ks = 1;
    public float specN = 1;
    public float Ka = 1;
    public float fAtt = 1;

    // Values for all the textures
    public int sandIndex = 0;
    public float sandStrength = 2f;
    public float sandRandomStrength = 3f;
    public float sandHeight = 7f;

    public int rockIndex = 1;
    public float rockStrength = 1.8f;

    public int snowIndex = 2;
    public float snowStrength = 2f;
    public float snowRandomStrength = 8f;
    public float snowHeight = 40f;

    public int grassIndex = 3;
    public float grassStrength = 5f;

    // How much the highest scoring texture is increased by
    public float highlightStrength = 8f;
    
    void Start()
    {
        Terrain terrain = GetComponent<Terrain>();
	TerrainData terrainData = terrain.terrainData;

	float[,,] splatMap = new float[terrainData.alphamapWidth,
		                       terrainData.alphamapHeight,
				       terrainData.alphamapLayers];

	for (int y = 0; y < terrainData.alphamapHeight; y++) {
	    for (int x = 0; x < terrainData.alphamapWidth; x++) {
		
		// Get normalised values for coordinates
		float yNorm = (float) y / (float) terrainData.alphamapHeight;
		float xNorm = (float) x / (float) terrainData.alphamapWidth;

		// Get attributes of each position to help with texture choice
		float height = terrainData.GetInterpolatedHeight(yNorm, xNorm);
		Vector3 normal = terrainData.GetInterpolatedNormal(yNorm, xNorm);
		float steepness = terrainData.GetSteepness(yNorm, xNorm);

		float[] splatWeights = new float[terrainData.alphamapLayers];

		// Rock
		// Placed on surfaces with a high steepness
		splatWeights[rockIndex] = Mathf.Clamp01(steepness*steepness/terrainData.heightmapHeight) * rockStrength;

		// Snow
		// Placed on surface above an elevation
		// has randomness to create scatter
		if (height > snowHeight + Random.value * snowRandomStrength) splatWeights[snowIndex] = snowStrength;

		// Sand
		// as above but below an elevation
		else if (height < sandHeight + Random.value * sandRandomStrength) splatWeights[sandIndex] = sandStrength;

		// Grass
		// Placed on surfaces that have a low steepness
		else splatWeights[grassIndex] = (1f - Mathf.Clamp01(steepness*steepness/(terrainData.heightmapHeight))) * grassStrength;

		// Highlight highest texture
		float total = 0;
		float max = 0;
		int maxi = 0;
		for (int i = 0; i < splatWeights.Length; i++) {
		    total += splatWeights[i];
		    if (max < splatWeights[i]) {
			    maxi = i;
			    max = splatWeights[i];
		    }
		}
		splatWeights[maxi] += highlightStrength;
		total += highlightStrength;

		// Normalise all values between 0-1
		for (int i = 0; i < splatWeights.Length; i++) {
		    splatWeights[i] /= total;
		    splatMap[x, y, i] = splatWeights[i];
		}
	    }
	}
	terrainData.SetAlphamaps(0, 0, splatMap);
    }

    void Update()
    {
	// Update shader values
        if(blend) {
            material.SetFloat("_Blend", 1);
        } else {
            material.SetFloat("_Blend", 0);
        }
        material.SetFloat("_BlendAmount", blendAmount);

        material.SetFloat("_Ks", Ks);
        material.SetFloat("_specN", specN);
        material.SetFloat("_Kd", Kd);
        material.SetFloat("_Ka", Ka);
        material.SetFloat("_fAtt", fAtt);
    }
}
