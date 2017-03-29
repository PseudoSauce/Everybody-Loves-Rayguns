using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Vingette : MonoBehaviour {
    [SerializeField]
    private Shader _shader;
    //[Range(0,1)]
    private float MinRadius = 1.0f;
    //[Range(0, 1)]
    private float MaxRadius = 1.0f;
    //[Range(0, 1)]
    private float Saturation = 1.0f;

    Material _material;

    void OnEnable() {
        _material = new Material(_shader);
    }

    //public void SetVignette(float minRad, float maxRad, float sat) {
    //    MinRadius = minRad;
    //    Saturation = sat;
    //}

    public void ConvergeVignette(bool isConverge, float vigStep) {
        if (isConverge) {
            if (MinRadius >= 0 && Saturation >= 0) {
                MinRadius -= vigStep;
                Saturation -= vigStep;
            }
        } else {
            if (MinRadius < 1.0f - vigStep && Saturation < 1.0f - vigStep) {
                MinRadius += vigStep;
                Saturation += vigStep;
            }
        }
    }

    public void ResetVignette() {
        Saturation = 1.0f;
        MinRadius = 1.0f;
    }

    public void OnRenderImage(RenderTexture src, RenderTexture dst) {
        _material.SetFloat("_MinRadius", MinRadius);
        _material.SetFloat("_MaxRadius", MaxRadius);
        _material.SetFloat("_Saturation", Saturation);

        Graphics.Blit(src, dst, _material, 0);
    }
}
