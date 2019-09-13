using UnityEngine;
using System.Collections;

public class PointLight : MonoBehaviour {

    public Color color;
    public Color colorLow;
    
    public Material[] materialList;
    //public Material material;

    void Update() {
	    foreach (Material material in materialList) {
            // Change colour of light depending on height
            if (transform.position.y < 200.0f && transform.position.y >= 0.0f) {
                material.SetColor("_PointLightColor", Color.Lerp(colorLow, color, transform.position.y / 200.0f));
            } else if (transform.position.y < 0.0f) {
                // quickly fade to black
                material.SetColor("_PointLightColor", Color.Lerp(colorLow, Color.black, -(transform.position.y / 200.0f)));
            } else {
                material.SetColor("_PointLightColor", color);
            }
        
            material.SetVector("_PointLightPosition", transform.position);
	    }
    }
}
