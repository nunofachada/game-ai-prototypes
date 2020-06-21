using UnityEngine;

public class ProcGen : MonoBehaviour
{
    [SerializeField]
    private float tileSize = 10f;

    [SerializeField]
    [Range(0,1)]
    private float maxAltitude = 0.1f;

    private void Awake()
    {
        Terrain terrain = GetComponent<Terrain>();
        int width = terrain.terrainData.heightmapWidth;
        int height = terrain.terrainData.heightmapHeight;

        float[,] heights = new float[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                heights[i, j] = maxAltitude* Mathf.PerlinNoise(
                    tileSize * i / (float)width, tileSize * (float)j / height);
            }
        }
        terrain.terrainData.SetHeights(0, 0, heights);
        //terrain.terrainData.SyncHeightmap();
    }

}
