using UnityEngine;
using System.Collections;

public class PointLight : MonoBehaviour {

    public Color color;
    public float Kd = 1;
    public float Ks = 1;
    public float specN = 1;
    public float Ka = 1;
    public float fAtt = 1;

    public Material material;

    void Update() {
        material.SetColor("_PointLightColor", color);
        material.SetVector("_PointLightPosition", transform.position);
        material.SetFloat("_Ks", Ks);
        material.SetFloat("_specN", specN);
        material.SetFloat("_Kd", Kd);
        material.SetFloat("_Ka", Ka);
        material.SetFloat("_fAtt", fAtt);
    }
}
