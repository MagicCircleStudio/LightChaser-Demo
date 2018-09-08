using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ECellType
{
    AirWall = -1,
    Air = 0,
    Ground = 1,
    Stone = 2
}

public class MapData {
    public int SizeX;
    public int SizeZ;

    public ECellType[][] Cells;
    public int[][] Level;

    private void FullFillMapData(ECellType cellType)
    {
        Cells = new ECellType[SizeZ + 2][];
        Level = new int[SizeZ + 2][];

        for (int j = 0; j < SizeZ + 2; j++)
        {
            Cells[j] = new ECellType[SizeX + 2];
            Level[j] = new int[SizeX + 2];
            for (int i = 0; i < SizeX + 2; i++)
            {
                Cells[j][i] = cellType;
                Level[j][i] = 0;
            }
        }
    }

    private void InitializeBorder()
    {
        for (int i = 0; i < SizeX + 2; i++)
        {
            // Cells[0][i] = ECellType.AirWall;
            Level[0][i] = -1;
            // Cells[SizeZ + 1][i] = ECellType.AirWall;
            Level[SizeZ + 1][i] = -1;
        }

        for (int j = 0; j < SizeZ + 2; j++)
        {
            // Cells[j][0] = ECellType.AirWall;
            Level[j][0] = -1;
            // Cells[j][SizeX + 1] = ECellType.AirWall;
            Level[j][SizeX + 1] = -1;
        }
    }

    public MapData(int sizeX, int sizeY, ECellType fullFillCellType = ECellType.Air)
    {
        SizeX = sizeX;
        SizeZ = sizeY;

        FullFillMapData(fullFillCellType);
        InitializeBorder();
    }
}
