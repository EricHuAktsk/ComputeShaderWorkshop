using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoAppendBufferConditioning : MonoBehaviour
{
    public RenderTexture m_texture;
    public ComputeShader m_cs;
    public Vector3Int m_cubesSize;
    private Vector3[] m_posArray;
    private ComputeBuffer m_posBuffer;
    private uint[] m_farIdArray;
    private ComputeBuffer m_farPosIdAppendBuffer;
    private Transform[] m_cubes;

    private void Awake()
    {
        m_farIdArray = new uint[m_cubesSize.x * m_cubesSize.y * m_cubesSize.z];
        m_posArray = new Vector3[m_cubesSize.x * m_cubesSize.y * m_cubesSize.z];
        m_cubes = new Transform[m_cubesSize.x * m_cubesSize.y * m_cubesSize.z];
        for (int i = 0; i < m_cubes.Length; i++)
        {
            m_cubes[i] = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        }
    }

    //[ContextMenu("Run Compute Shader")]
    private void Update()
    {
        m_posBuffer = new ComputeBuffer(m_posArray.Length, sizeof(float) * 3);
        m_farPosIdAppendBuffer = new ComputeBuffer(m_posArray.Length, sizeof(uint), ComputeBufferType.Append);
        m_farPosIdAppendBuffer.SetCounterValue(0);

        m_posBuffer.SetData(m_posArray);
        m_cs.SetBuffer(0, "PosWS", m_posBuffer);
        m_cs.SetVector("CameraPos", Camera.main.transform.position);
        m_cs.SetBuffer(0, "FarPosId", m_farPosIdAppendBuffer);
        m_cs.Dispatch(0, m_cubes.Length / 64, 1, 1);

        m_posBuffer.GetData(m_posArray);
        for (int i = 0; i < m_posArray.Length; i++)
        {
            m_cubes[i].position = m_posArray[i];
            m_cubes[i].localScale = Vector3.one;
        }

        var countBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.IndirectArguments);
        ComputeBuffer.CopyCount(m_farPosIdAppendBuffer, countBuffer, 0);
        var counter = new int[1] { 0 };
        countBuffer.GetData(counter);
        var count = counter[0];
        m_farIdArray = new uint[count];
        m_farPosIdAppendBuffer.GetData(m_farIdArray);
        //Debug.Log("append data size = " + count);
        for (int i = 0; i < m_farIdArray.Length; i++)
        {
            m_cubes[m_farIdArray[i]].localScale = Vector3.zero;
        }
        m_posBuffer.Release();
        m_farPosIdAppendBuffer.Release();
        countBuffer.Release();
    }
}
