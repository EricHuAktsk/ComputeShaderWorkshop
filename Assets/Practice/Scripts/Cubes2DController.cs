using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cubes2DController : MonoBehaviour
{
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
    }

    private void OnDisable()
    {
        //m_texture.Release();
    }
}