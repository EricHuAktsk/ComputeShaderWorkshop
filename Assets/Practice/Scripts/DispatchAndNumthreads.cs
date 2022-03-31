using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispatchAndNumthreads : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform[] m_cubestTransformArray;
    public Vector3Int m_cubeCounts;
    public float m_scale = 1f;

    private void Awake()
    {
        m_cubestTransformArray = new Transform[m_cubeCounts.x * m_cubeCounts.y * m_cubeCounts.z];
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

    private void OnSetCubeColor()
    {

    }

}