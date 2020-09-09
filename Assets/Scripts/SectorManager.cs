using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SectorManager : MonoBehaviour
{
    [Header("External Assets")]
    public GameObject sectorPrefab;

    [Header("Perlin Noise")]
    public Vector2 perlinPosition;
    public float zoom = 10;
    public int octaves = 5;

    [Header("Mesh Generator")]
    public float height = 15;
    public Gradient vertexGradient;
    public float localHeightMin = -1;
    public float localHeightMax = 2.2f;
    public float localHeightZoom = 2;
    public float yMultiplierUnder0 = 1;
    public float yAdditionUnder0;

    [Header("Sector Manager")]
    public Transform[] trackedEntities;
    public int renderDistance = 1;

    float localHeightRange;
    Dictionary<Vector2Int, WorldSector> loadedSectors;

    bool isCheckingSectors;

    private void Awake()
    {
        localHeightRange = localHeightMax - localHeightMin;
        loadedSectors = new Dictionary<Vector2Int, WorldSector>();
    }

    private void Start()
    {
        LoadSector(Vector2Int.zero);
    }

    private void Update()
    {
        if (!isCheckingSectors)
            StartCoroutine(CheckLoadedSectors());
    }

    private void LoadSector(Vector2Int position)
    {
        WorldSector sector = Instantiate(sectorPrefab, new Vector3(position.x * 255, 0, position.y * 255), Quaternion.identity, transform)
            .GetComponent<WorldSector>();
        sector.sectorPosition = position;
        Vector2 localHeightPosition = perlinPosition + position;
        localHeightPosition /= localHeightZoom;
        float newLocalHeight = Mathf.PerlinNoise(localHeightPosition.x, localHeightPosition.y);
        newLocalHeight = newLocalHeight * localHeightRange + localHeightMin;
        if (newLocalHeight < 0)
            newLocalHeight *= yMultiplierUnder0;
        sector.localHeight = newLocalHeight;
        loadedSectors[position] = sector;

        sector.height = height;
        if (newLocalHeight < 0)
            sector.yAdjust = yAdditionUnder0;

        sector.perlinPosition = perlinPosition + position * 255;
        sector.zoom = zoom;
        sector.octaves = octaves;

        float[][] borders = new float[4][] { null, null, null, null };
        Mesh borderMesh;
        if (loadedSectors.ContainsKey(position + Vector2Int.up))
        {
            borders[0] = new float[256];
            borderMesh = loadedSectors[position + Vector2Int.up].GetComponent<MeshFilter>().sharedMesh;
            for (int i = 0; i < 256; i++)
            {
                borders[0][i] = borderMesh.vertices[65280 + i].y;
            }
        }
        if (loadedSectors.ContainsKey(position + Vector2Int.left))
        {
            borders[1] = new float[256];
            borderMesh = loadedSectors[position + Vector2Int.left].GetComponent<MeshFilter>().sharedMesh;
            for (int i = 0; i < 256; i++)
            {
                borders[1][i] = borderMesh.vertices[256 * i].y;
            }
        }
        if (loadedSectors.ContainsKey(position + Vector2Int.right))
        {
            borders[2] = new float[256];
            borderMesh = loadedSectors[position + Vector2Int.right].GetComponent<MeshFilter>().sharedMesh;
            for (int i = 0; i < 256; i++)
            {
                borders[2][i] = borderMesh.vertices[255 + 256 * i].y;
            }
        }
        if (loadedSectors.ContainsKey(position + Vector2Int.down))
        {
            borders[3] = new float[256];
            borderMesh = loadedSectors[position + Vector2Int.down].GetComponent<MeshFilter>().sharedMesh;
            for (int i = 0; i < 256; i++)
            {
                borders[3][i] = borderMesh.vertices[i].y;
            }
        }

        sector.GenerateSectorMesh(borders);
    }

    private void UnloadSector(Vector2Int position)
    {
        WorldSector sector = loadedSectors[position];
        Destroy(sector.gameObject);
        loadedSectors.Remove(position);
    }

    private IEnumerator CheckLoadedSectors()
    {
        isCheckingSectors = true;

        List<Vector2Int> shouldBeLoaded = new List<Vector2Int>();
        foreach (Transform entity in trackedEntities)
        {
            Vector2Int currentSector = (entity.position.FlattenToTopDown() / 255).Truncate();
            for (int x = currentSector.x - renderDistance; x <= currentSector.x + renderDistance; x++)
            {
                for (int z = currentSector.y - renderDistance; z <= currentSector.y + renderDistance; z++)
                {
                    shouldBeLoaded.Add(new Vector2Int(x, z));
                }
            }
        }

        List<Vector2Int> areLoaded = new List<Vector2Int>(loadedSectors.Keys);
        foreach (Vector2Int toUnload in areLoaded.Except(shouldBeLoaded))
        {
            yield return null;
            UnloadSector(toUnload);
        }
        foreach (Vector2Int toLoad in shouldBeLoaded.Except(areLoaded))
        {
            yield return null;
            LoadSector(toLoad);
        }

        isCheckingSectors = false;
    }
}
