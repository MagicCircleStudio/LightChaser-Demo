using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace FogOfWarPlayground {
    public class FogOfWarCamera : MonoBehaviour {
        public RenderTexture fovMaskTexture;
        public Color clearColor = Color.black;

        // The command buffer for clearing RT.
        private CommandBuffer commandBuffer = null;
        private Camera rigCamera;

        // Use this for initialization
        void Start() {
            rigCamera = GetComponent<Camera>();
            rigCamera.forceIntoRenderTexture = true;
            rigCamera.targetTexture = fovMaskTexture;
            rigCamera.clearFlags = CameraClearFlags.Nothing;

            commandBuffer = new CommandBuffer();
            commandBuffer.SetRenderTarget(fovMaskTexture);
            commandBuffer.ClearRenderTarget(true, true, clearColor, 1.0f);

            rigCamera.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, commandBuffer);
            StartCoroutine(RemoveCommandBufferIE());
        }

        IEnumerator RemoveCommandBufferIE() {
            yield return new WaitForEndOfFrame();
            rigCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardAlpha, commandBuffer);
        }

        // Update is called once per frame
        void Update() {

        }
    }

}