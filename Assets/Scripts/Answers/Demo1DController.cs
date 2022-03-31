using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Example
{
    public class Demo1DController : MonoBehaviour
    {
        public ComputeShader m_cs;
        public int m_cubesSize;
        private Vector3[] m_posArray;
        private ComputeBuffer m_posBuffer;
        private Transform[] m_cubes;

        private void Awake()
        {
            m_posArray = new Vector3[m_cubesSize];
            m_cubes = new Transform[m_cubesSize];
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

            m_cs.Dispatch(0, m_cubesSize / 8, 1, 1);
            m_posBuffer.GetData(m_posArray);
            m_posBuffer.Release();

            for (int i = 0; i < m_posArray.Length; i++)
            {
                m_cubes[i].position = m_posArray[i];
            }
        }

    }
}