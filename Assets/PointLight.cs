using UnityEngine;
using System.Collections;

public class PointLight : MonoBehaviour {

    public Color color;
    public Material material;

    void Update() {
        material.SetColor("_PointLightColor", color);
        material.SetVector("_PointLightPosition", transform.position);
    }
}
