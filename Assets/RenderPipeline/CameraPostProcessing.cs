using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode] 
public class CameraPostProcessing : MonoBehaviour
{
    public Material FXMaterial;
    void OnRenderImage(RenderTexture src, // Everything the camera renders, automaticallly assigned to _MainTex
        RenderTexture dst // The renderTarget of this camera. Usually the display. Got bugs when rendered to texture
        )
    {
        RenderTexture verticalBlurred = RenderTexture.GetTemporary(src.width, src.height);
        Graphics.Blit(src, verticalBlurred, FXMaterial, 0);
        Graphics.Blit(verticalBlurred, dst, FXMaterial, 1);
        verticalBlurred.Release(); // clear the buffer to 0 when done
        // use next shader with empty render texture here
    }
}
