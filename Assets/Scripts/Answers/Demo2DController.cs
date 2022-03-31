using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Example
{
    public class Demo2DController : MonoBehaviour
    {
        // Start is called before the first frame update
        public Texture m_worldNoise;
        public float m_noiseScale;
        public RenderTexture m_texture;
        public ComputeShader m_cs;
        public Renderer m_renderer;
        public Vector3Int m_cubesSize;
        private Vector3[] m_posArray;
        private ComputeBuffer m_posBuffer;
        private Transform[] m_cubes;

        private void Awake()
        {
            m_posArray = new Vector3[m_cubesSize.x * m_cubesSize.y * m_cubesSize.z];
            m_cubes = new Transform[m_cubesSize.x * m_cubesSize.y * m_cubesSize.z];
            for (int i = 0; i < m_cubes.Length; i++)
            {
                m_cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            }
        }

        private void Update()
        {
            m_posBuffer = new ComputeBuffer(m_posArray.Length, sizeof(float) * 3);
            m_posBuffer.SetData(m_posArray);
            m_cs.SetBuffer(0, "PosWS", m_posBuffer);
            m_cs.SetTexture(0, "WorldNoise", m_worldNoise);
            m_cs.SetFloat("NoiseScale", m_noiseScale);
            m_cs.SetFloat("Time", Time.time * 0.02f);

            m_cs.Dispatch(0, m_cubesSize.x / 64, 1, 1);
            m_posBuffer.GetData(m_posArray);
            m_posBuffer.Release();
            for (int i = 0; i < m_posArray.Length; i++)
            {
                m_cubes[i].position = m_posArray[i];
            }
        }

        private void OnDisable()
        {
            //m_texture.Release();
        }

    }
}