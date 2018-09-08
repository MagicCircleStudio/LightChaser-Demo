using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public MapData mapData;
    public GameObject testingObjects;
    public GameObject playerPrefab;
    [HideInInspector]
    public PlayerController playerController;

    private void Awake()
    {
        testingObjects = GameObject.Find("Testing Objects");
        testingObjects.SetActive(false);

        if (!mapGenerator)
            mapGenerator = GameObject.Find("Map Generator").GetComponent<MapGenerator>();
        if (!mapGenerator)
            Debug.LogWarning("Can't find 'Map Generator'");
    }

    private void Start()
    {
        mapData = mapGenerator.Generate();

        // Add player
        
        var playerExists = false;
        while (!playerExists)
        {
            var i = Random.Range(0, mapData.SizeX) + 1;
            var j = Random.Range(0, mapData.SizeZ) + 1;

            if (mapData.Cells[j][i] == 0)
            {
                playerExists = true;
                var player = Instantiate(playerPrefab, new Vector3(i, mapData.Level[j][i], j), Quaternion.identity, transform);
                playerController = player.GetComponent<PlayerController>();
            }
        }
    }
}
