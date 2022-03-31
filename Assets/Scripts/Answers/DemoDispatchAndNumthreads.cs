using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Example
{
    public class DemoDispatchAndNumthreads : MonoBehaviour
    {
        private struct CubeData
        {
            public Color color;
        }

        // Start is called before the first frame update
        private Transform[] m_cubestTransformArray;
        private CubeData[] m_cubeDatas;
        public Vector3Int m_cubeCounts;
        public Vector3Int m_groupsSize;
        public int m_kernal;
        public float m_scale = 1f;
        [Range(0.01f, 1f)]
        public float m_noiseScale = 1f;
        public ComputeShader m_cs;

        private void Awake()
        {
            m_cubestTransformArray = new Transform[m_cubeCounts.x * m_cubeCounts.y * m_cubeCounts.z];
            m_cubeDatas = new CubeData[m_cubeCounts.x * m_cubeCounts.y * m_cubeCounts.z];
        }
        void Start()
        {
            for (int x = 0; x < m_cubeCounts.x; x++)
            {
                for (int y = 0; y < m_cubeCounts.y; y++)
                {
                    for (int z = 0; z < m_cubeCounts.z; z++)
                    {
                        var index = z * m_cubeCounts.y * m_cubeCounts.x + y * m_cubeCounts.x + x;
                        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        m_cubestTransformArray[index] = obj.transform;
                        m_cubestTransformArray[index].position = new Vector3(x, y, z) * m_scale;
                    }
                }
            }
            OnSetCubeColor();
        }

        private void OnValidate()
        {
            if (m_cubestTransformArray == null || m_cubeDatas == null)
                return;
            OnSetCubeColor();
        }

        private void OnSetCubeColor()
        {
            var buffer = new ComputeBuffer(m_cubeDatas.Length, sizeof(float) * 4);
            buffer.SetData(m_cubeDatas);
            m_cs.SetBuffer(m_kernal, "CubeDatas", buffer);
            m_cs.Dispatch(m_kernal, m_groupsSize.x, m_groupsSize.y, m_groupsSize.z);

            buffer.GetData(m_cubeDatas);
            for (int i = 0; i < m_cubeDatas.Length; i++)
            {
                m_cubestTransformArray[i].GetComponent<Renderer>().material.color = m_cubeDatas[i].color;
            }
            buffer.Dispose();
        }
    }
}
