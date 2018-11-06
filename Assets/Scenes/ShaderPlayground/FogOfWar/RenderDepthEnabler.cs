using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderDepthEnabler : MonoBehaviour {

    // Use this for initialization
    void Start ()
    {
        Camera camera = GetComponent<Camera>();
        camera.depthTextureMode = DepthTextureMode.Depth;
    }
    
    // Update is called once per frame
    void Update ()
    {
        
    }
}
