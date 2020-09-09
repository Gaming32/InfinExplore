using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSector : MonoBehaviour
{
    [Header("Perlin Noise")]
    public Vector2 perlinPosition;
    public float zoom = 10;
    public int octaves = 5;

    [Header("Mesh Generator")]
    public float height = 15;
    public Gradient vertexGradient;
    public float yAdjust;

    [Header("Sector Manager")]
    public Vector2Int sectorPosition;
    public bool generateOnStart = true;
    public float localHeight = 1;

    void OnValidate()
    {
        if (generateOnStart)
        {
            GenerateSectorMesh();
        }
    }

    public void GenerateSectorMesh()
    {
        GenerateSectorMesh(null, null, null, null);
    }

    public void GenerateSectorMesh(int[] edgeForward, int[] edgeLeft, int[] edgeRight, int[] edgeBack)
    {
        Mesh mesh = new Mesh();
        mesh.name = $"Sector {sectorPosition.x}x{sectorPosition.y}";
        mesh = CreateSectorMesh(mesh, new int[][] { edgeForward, edgeLeft, edgeRight, edgeBack});
        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private (float, float) CalculateMinMaxY(IEnumerable<Vector3> array)
    {
        IEnumerator<Vector3> enumerator = array.GetEnumerator();
        Vector3 current = enumerator.Current;
        float min = current.y, max = current.y;
        while (enumerator.MoveNext())
        {
            current = enumerator.Current;
            if (current.y < min)
            {
                min = current.y;
            }
            if (current.y > max)
            {
                max = current.y;
            }
        }
        return (min, max);
    }

    private Mesh CreateSectorMesh(Mesh mesh, int[][] edges)
    {
        mesh.Clear();

        List<Vector3> vertices = new List<Vector3>();
        float mulValue = height;
        float addValue = localHeight / 2;
        for (int x = 0; x < 256; x++)
        {
            for (int z = 0; z < 256; z++)
            {
                float baseY = Perlin.Fbm((perlinPosition.x + x) / zoom * (localHeight / 2), (perlinPosition.y + z) / zoom * (localHeight / 2), octaves);
                //float y = (baseY + addValue) * mulValue + addValue;
                float y = (baseY + addValue) * mulValue;
                y += yAdjust;
                vertices.Add(new Vector3(x, y, z));
            }
        }
        mesh.vertices = vertices.ToArray();

        (float minY, float maxY) = CalculateMinMaxY(vertices);
        List<Color> colors = new List<Color>();
        foreach (Vector3 vertex in vertices)
        {
            Color color;
            if (vertex.y < 0)
            {
                color = vertexGradient.Evaluate(0);
            }
            else
            {
                color = vertexGradient.Evaluate(Mathf.InverseLerp(minY, height, vertex.y));
            }
            colors.Add(color);
        }
        mesh.colors = colors.ToArray();

        List<int> triangles = new List<int>();
        for (int x = 0, vertex = 0; x < 255; x++)
        {
            for (int z = 0; z < 255; z++)
            {
                triangles.Add(vertex);
                triangles.Add(vertex + 1);
                triangles.Add(vertex + 256);
                triangles.Add(vertex + 1);
                triangles.Add(vertex + 257);
                triangles.Add(vertex + 256);
                vertex++;
            }
            vertex++;
        }
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    void OnNODrawGizmos()
    {
        Gizmos.color = Color.white;
        foreach (Vector3 vertex in GetComponent<MeshFilter>().sharedMesh.vertices)
        {
            Gizmos.DrawSphere(vertex, .25f);
        }
    }
}
