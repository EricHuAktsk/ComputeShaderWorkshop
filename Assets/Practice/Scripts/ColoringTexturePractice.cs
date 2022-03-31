using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ColoringTexturePractice : MonoBehaviour
{
    /*
        Texture : represent the GPU (graphic card memory) side of a Texture

        RenderTexture : based on Texture, add the shader/material management

        Texture2D : based on Texture, add a CPU (processor, ram) side management. It handle the Hard Drive loading/saving of the pnj/jpg/etc
    */

    public int TextureSize;
    //CPU START
    private Texture2D m_textureObject;
    //CPU END

    //GPU START
    private RenderTexture m_renderTexture;
    public ComputeShader m_colorTexturecCS;
    //GPU END
    private Renderer m_renderer;
    // Start is called before the first frame update
    void Start()
    {
        m_renderTexture = new RenderTexture(TextureSize, TextureSize, 0, RenderTextureFormat.ARGB32);
        m_renderTexture.enableRandomWrite = true;
        m_renderer = GetComponent<Renderer>();
    }

    [ContextMenu("Color by CPU")]
    private void ColoringTextureCPU()
    {

    }

    [ContextMenu("Color by GPU")]
    private void ColoringTextureGPU()
    {

    }



    private void OnDestroy()
    {
        if (m_textureObject != null)
            DestroyImmediate(m_textureObject);
    }
}