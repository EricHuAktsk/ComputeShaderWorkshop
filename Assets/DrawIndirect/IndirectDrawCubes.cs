using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndirectDrawCubes : MonoBehaviour
{
    public struct MeshData
    {
        public Vector3 Position;
    }
    public Mesh Mesh;
    public Material InstanceMaterial;
    public int WorldSize;
    public int MeshCount;
    private ComputeBuffer m_meshDatasBuffer;
    private ComputeBuffer m_argsBuffer;
    private uint[] m_args = new uint[5] { 0, 0, 0, 0, 0 };
    private MeshData[] m_meshDatas;
    // Start is called before the first frame update
    void Start()
    {

        m_meshDatas = new MeshData[MeshCount];
        for (int i = 0; i < MeshCount; i++)
        {
            m_meshDatas[i].Position = new Vector3(Random.Range(-WorldSize, WorldSize), Random.Range(-WorldSize, WorldSize), Random.Range(-WorldSize, WorldSize));
        }

        m_meshDatasBuffer = new ComputeBuffer(MeshCount, sizeof(float) * 3);
        m_meshDatasBuffer.SetData(m_meshDatas);
        InstanceMaterial.SetBuffer("meshDataBuffer", m_meshDatasBuffer);
        InstanceMaterial.SetFloat("_WorldSize", WorldSize);
        m_argsBuffer = new ComputeBuffer(1, m_args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        if (Mesh != null)
        {
            m_args[0] = (uint)Mesh.GetIndexCount(0);
            m_args[1] = (uint)MeshCount;
            m_args[2] = (uint)Mesh.GetIndexStart(0);
            m_args[3] = (uint)Mesh.GetBaseVertex(0);
        }
        else
        {
            m_args[0] = m_args[1] = m_args[2] = m_args[3] = 0;
        }
        m_argsBuffer.SetData(m_args);
    }

    // Update is called once per frame
    void Update()
    {
        Graphics.DrawMeshInstancedIndirect(Mesh, 0, InstanceMaterial, new Bounds(Vector3.zero, new Vector3(WorldSize, WorldSize, WorldSize)), m_argsBuffer);
    }
}
