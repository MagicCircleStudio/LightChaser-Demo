using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject airWallPrefab;
    public GameObject groundPrefab;
    public GameObject stonePrefab;

    public int sizeX = 8;
    public int sizeZ = 8;
    public int stoneCount = 5;
    public int highLevelCount = 10;
    [HideInInspector]
    private MapData mapData;

    private GameObject getPrefabType(ECellType cellType)
    {
        switch (cellType)
        {
            case ECellType.AirWall:
                return airWallPrefab;
            case ECellType.Air:
                return null;
            case ECellType.Ground:
                return groundPrefab;
            case ECellType.Stone:
                return stonePrefab;
            default:
                return null;
        }
    }

    private void generateMap()
    {
        // Add random stones
        var counter = stoneCount;
        while (counter > 0)
        {
            var i = Random.Range(0, sizeX) + 1;
            var j = Random.Range(0, sizeZ) + 1;

            if (mapData.Cells[j][i] == 0)
            {
                mapData.Cells[j][i] = ECellType.Stone;
                counter--;
            }
        }

        // Add random level
        counter = highLevelCount;
        while (counter > 0)
        {
            var i = Random.Range(0, sizeX) + 1;
            var j = Random.Range(0, sizeZ) + 1;

            mapData.Level[j][i]++;
            counter--;
        }

        // Generate object
        for (int j = 0; j < sizeZ + 2; j++)
        {
            for (int i = 0; i < sizeX + 2; i++)
            {
                var prefab = getPrefabType(mapData.Cells[j][i]);
                // Generate high level ground
                for (int k = 0; k <= mapData.Level[j][i]; k++)
                    Instantiate(getPrefabType(ECellType.Ground), new Vector3(i, k - 1, j), Quaternion.identity, transform);
                // Generate object on the ground
                if (prefab != null)
                    Instantiate(prefab, new Vector3(i, mapData.Level[j][i], j), Quaternion.identity, transform);
            }
        }
    }

    public MapData Generate()
    {
        mapData = new MapData(sizeX, sizeZ);

        generateMap();

        return mapData;
    }

}
