using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cubes1DController : MonoBehaviour
{
    public ComputeShader m_cs;
    public int m_cubesSize;
    private Vector3[] m_posArray;
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
    }

}