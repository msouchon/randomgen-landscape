using UnityEngine;
using System.Collections;

public class PointLight : MonoBehaviour {

    public Color color;
    public Color colorLow;

    public float colorChangePoint = 200.0f;
    
    public Material[] materialList;

    void Update() {
	foreach (Material material in materialList) {
	    // If the sun is below a certain point, allow a new color to
	    // be gradually introduced. Used for sunset/sunrise
	    if (transform.position.y < colorChangePoint) {
		material.SetColor("_PointLightColor", Color.Lerp(colorLow, color, transform.position.y / colorChangePoint));
	    } else {
		material.SetColor("_PointLightColor", color);
	    }
	    // Update the position of the light to the materials
	    material.SetVector("_PointLightPosition", transform.position);
	}
    }
}
