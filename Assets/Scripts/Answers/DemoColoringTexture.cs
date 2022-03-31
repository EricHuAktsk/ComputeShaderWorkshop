using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Example
{

    public class DemoColoringTexture : MonoBehaviour
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
            var colors = new Color[TextureSize * TextureSize];
            for (int x = 0; x < TextureSize; x++)
            {
                for (int y = 0; y < TextureSize; y++)
                {
                    colors[x + y * TextureSize] = new Color(x / (float)TextureSize, y / (float)TextureSize, 0, 1);
                }
            }
            m_textureObject = new Texture2D(TextureSize, TextureSize);
            m_textureObject.SetPixels(colors);
            m_textureObject.Apply();
            m_renderer.material.mainTexture = m_textureObject;
        }

        [ContextMenu("Color by GPU")]
        private void ColoringTextureGPU()
        {
            m_colorTexturecCS.SetFloat("_TextureSize", (float)TextureSize);
            m_colorTexturecCS.SetTexture(0, "_Result", m_renderTexture);
            m_colorTexturecCS.Dispatch(0, TextureSize / 8, TextureSize / 8, 1);
            m_renderer.material.mainTexture = m_renderTexture;
        }



        private void OnDestroy()
        {
            if (m_textureObject != null)
                DestroyImmediate(m_textureObject);
        }
    }

}