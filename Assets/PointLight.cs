using UnityEngine;
using System.Collections;

public class PointLight : MonoBehaviour {

    public Color color;
    public Color colorLow;
    public Material material;

    void Update() {
        if(transform.position.y < 200.0f) {
            material.SetColor("_PointLightColor", Color.Lerp(colorLow, color, transform.position.y / 200.0f));
        }else {
            material.SetColor("_PointLightColor", color);
        }
        
        material.SetVector("_PointLightPosition", transform.position);
    }
}
