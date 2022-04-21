using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FrustrumCulling
{
    public static Vector4 GetPlane(Vector3 normal, Vector3 point)
    {
        return new Vector4(normal.x, normal.y, normal.z, -Vector3.Dot(normal, point));
    }

    public static Vector4 GetPlane(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
        return GetPlane(normal, a);
    }

    public static Vector3[] GetCameraFarClipPlanePoint(Camera camera)
    {
        Vector3[] points = new Vector3[4];
        Transform transform = camera.transform;
        float distance = camera.farClipPlane;
        float halfFovRad = Mathf.Deg2Rad * camera.fieldOfView * 0.5f;
        float upLen = distance * Mathf.Tan(halfFovRad);
        float rightLen = upLen * camera.aspect;
        Vector3 farCenterPoint = transform.position + distance * transform.forward;
        Vector3 up = upLen * transform.up;
        Vector3 right = rightLen * transform.right;
        points[0] = farCenterPoint - up - right;//left-bottom
        points[1] = farCenterPoint - up + right;//right-bottom
        points[2] = farCenterPoint + up - right;//left-up
        points[3] = farCenterPoint + up + right;//right-up
        return points;
    }

    public static Vector4[] GetFrustumPlane(Camera camera)
    {
        Vector4[] planes = new Vector4[6];
        Transform transform = camera.transform;
        Vector3 cameraPosition = transform.position;
        Vector3[] points = GetCameraFarClipPlanePoint(camera);

        planes[0] = GetPlane(cameraPosition, points[0], points[2]);//left
        planes[1] = GetPlane(cameraPosition, points[3], points[1]);//right
        planes[2] = GetPlane(cameraPosition, points[1], points[0]);//bottom
        planes[3] = GetPlane(cameraPosition, points[2], points[3]);//up
        planes[4] = GetPlane(-transform.forward, transform.position + transform.forward * camera.nearClipPlane);//near
        planes[5] = GetPlane(transform.forward, transform.position + transform.forward * camera.farClipPlane);//far
        return planes;
    }
}

public class IndirectDrawCulling : MonoBehaviour
{
    public struct MeshData
    {
        public Vector3 Position;
    }

    public Mesh Mesh;
    public Material InstanceMaterial;
    public int WorldSize;
    public int MeshCount;
    public bool EnabledCulling;
    public ComputeShader FrustrumCullCS;
    private ComputeBuffer m_meshDatasBuffer;
    private ComputeBuffer m_cullResultBuffer;
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
        m_cullResultBuffer = new ComputeBuffer(MeshCount, sizeof(float) * 3, ComputeBufferType.Append);

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
        Vector4[] planes = FrustrumCulling.GetFrustumPlane(Camera.main);

        FrustrumCullCS.SetFloat("_InstanceCount", MeshCount);
        m_meshDatasBuffer.SetData(m_meshDatas);
        FrustrumCullCS.SetMatrix("_MatrixVP", GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false) * Camera.main.worldToCameraMatrix);
        FrustrumCullCS.SetBuffer(0, "_Inputs", m_meshDatasBuffer);
        m_cullResultBuffer.SetCounterValue(0);
        FrustrumCullCS.SetBool("_EnabledCull", EnabledCulling);
        FrustrumCullCS.SetBuffer(0, "_CullResult", m_cullResultBuffer);
        FrustrumCullCS.SetVectorArray("_Planes", planes);
        FrustrumCullCS.SetVector("_CameraPosWS", Camera.main.transform.position);
        FrustrumCullCS.Dispatch(0, Mathf.CeilToInt(MeshCount / 32f), 1, 1);
        ComputeBuffer.CopyCount(m_cullResultBuffer, m_argsBuffer, sizeof(uint));
        InstanceMaterial.SetBuffer("meshDataBuffer", m_cullResultBuffer);
        InstanceMaterial.SetFloat("_WorldSize", WorldSize);
        Graphics.DrawMeshInstancedIndirect(Mesh, 0, InstanceMaterial, new Bounds(Vector3.zero, new Vector3(WorldSize, WorldSize, WorldSize)), m_argsBuffer);
    }

    private void OnDestroy()
    {
        m_meshDatasBuffer.Release();
        m_cullResultBuffer.Release();
        m_argsBuffer.Release();
    }
}
