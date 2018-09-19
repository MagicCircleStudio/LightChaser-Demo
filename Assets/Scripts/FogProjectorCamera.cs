using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class FogProjectorCamera : MonoBehaviour
{
    public float aspect = 1;
    public Shader flatShader;

    private void OnEnable ()
	{
        Camera cam = GetComponent<Camera>();
        cam.SetReplacementShader(flatShader, "");
        cam.aspect = aspect;
	}
}
