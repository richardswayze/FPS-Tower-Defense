using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour {

    public static MapGenerator mapGenerator;

    public int width;
    public int depth;
    public Tile tilePrefab;
    public Tile[,] tiles;
    public Tile startTile;
    public Tile endTile;
    public List<Tile> path;
    public GameObject buildingPrefab;

    float tileDiameter;
    float scale;
    int gridSizeX, gridSizeZ;
    int magicNumber = 10; //Figure out why this is 10
    Pathfinder pathFinder;
    List<Tile> tilesToSelect = new List<Tile>();
    List<GameObject> buildings = new List<GameObject>();
    List<Tile> startingPath = new List<Tile>();

    [SerializeField]
    int buildingGenerationChance;

    private void Awake()
    {
        if (mapGenerator == null)
        {
            mapGenerator = this;
        }
        else if  (mapGenerator != null)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start() {
        tileDiameter = tilePrefab.transform.localScale.x;
        scale = tileDiameter * magicNumber;
        gridSizeX = Mathf.RoundToInt(width / tileDiameter);
        gridSizeZ = Mathf.RoundToInt(depth / tileDiameter);
        pathFinder = GetComponent<Pathfinder>();
        GenerateTiles();
    }

    void GenerateTiles()
    {
        tiles = new Tile[width, depth];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector3 spawnLocation = new Vector3(x * scale, 0f, z * scale);
                Tile tile = Instantiate(tilePrefab, transform);
                tile.transform.position = spawnLocation;
                tiles[x, z] = tile;
                tile.gridX = x;
                tile.gridZ = z;
            }
        }
        foreach (Tile thisTile in tiles)
        {
            tilesToSelect.Add(thisTile);
        }
        GenerateEdgeBuildings();
    }

    void GenerateEdgeBuildings()
    {
        int randomScaleY;
        foreach (Tile thisTile in tiles)
        {
            if (TileEdgeCheck(thisTile))
            {
                GameObject newBuilding = Instantiate(buildingPrefab);
                randomScaleY = Random.Range(1, 11);
                newBuilding.transform.localScale = new Vector3(newBuilding.transform.localScale.x, randomScaleY, newBuilding.transform.localScale.z);
                newBuilding.transform.position = thisTile.transform.position;
                thisTile.walkable = false;
                tilesToSelect.Remove(thisTile);
                buildings.Add(newBuilding);
            }
        }
        SelectStartEndTiles();
    }

    void SelectStartEndTiles()
    {
        startTile = tiles[1, 1];
        endTile = tiles[width - 2, depth - 2];
        tilesToSelect.Remove(startTile);
        tilesToSelect.Remove(endTile);
        startTile.GetComponent<MeshRenderer>().material.color = Color.green;
        endTile.GetComponent<MeshRenderer>().material.color = Color.red;
        startingPath = FindStartPath(startTile);
        GenerateBuildings();
    }

    void GenerateBuildings()
    {
        int generateCheck;
        List<Tile> didNotGenerate = new List<Tile>();
        foreach (Tile thisTile in tilesToSelect)
        {
            if (!startingPath.Contains(thisTile) && (tilesToSelect.IndexOf(thisTile)) % 2 == 0)
            {
                generateCheck = Random.Range(0, 100);
                if (TileEdgeCheck(thisTile) || buildingGenerationChance >= generateCheck)
                {
                    GameObject newBuilding = Instantiate(buildingPrefab);
                    newBuilding.transform.position = thisTile.transform.position;
                    thisTile.walkable = false;
                    buildings.Add(newBuilding);
                } else
                {
                    didNotGenerate.Add(thisTile);
                }
            } else
            {
                didNotGenerate.Add(thisTile);
            }  
        }
        foreach (Tile thisTile in didNotGenerate)
        {
            int unwalkableNeighbors = 0;
            foreach (Tile neighbor in GetNeighbors(thisTile))
            {
                if (!neighbor.walkable)
                {
                    unwalkableNeighbors += 1;
                }

            }
            if (unwalkableNeighbors >= 3)
            {
                GameObject newBuilding = Instantiate(buildingPrefab);
                newBuilding.transform.position = thisTile.transform.position;
                thisTile.walkable = false;
                buildings.Add(newBuilding);
            }
        }
        pathFinder.FindPath();
    }

    public Tile TileFromWorldPoint(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / magicNumber);
        int z = Mathf.RoundToInt(worldPosition.z / magicNumber);
        return tiles[x, z];
    }

    public List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0 || x == -1 && z == 1 || x == -1 && z == -1 || x == 1 && z == 1 || x == 1 && z == -1)
                {
                    continue;
                }
                int checkX = tile.gridX + x;
                int checkZ = tile.gridZ + z;
                if (checkX >= 0 && checkX < gridSizeX && checkZ >= 0 && checkZ < gridSizeZ)
                {
                    neighbors.Add(tiles[checkX, checkZ]);
                }
            }
        }
        return neighbors;
    }

    public bool TileEdgeCheck(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0 || x == -1 && z == 1 || x == -1 && z == -1 || x == 1 && z == 1 || x == 1 && z == -1)
                {
                    continue;
                }
                int checkX = tile.gridX + x;
                int checkZ = tile.gridZ + z;
                if (checkX >= 0 && checkX < gridSizeX && checkZ >= 0 && checkZ < gridSizeZ)
                {
                    neighbors.Add(tiles[checkX, checkZ]);
                }
            }
        }
        return neighbors.Count < 4;
    }

    List<Tile> FindStartPath(Tile tile)
    {
        Tile nextTile = tile;
        List<Tile> startPath = new List<Tile>();
        List<Tile> openTiles = new List<Tile>();
        openTiles.Add(nextTile);
        startPath.Add(nextTile);
        HashSet<Tile> visitedTiles = new HashSet<Tile>();
        visitedTiles.Add(nextTile);
        while (openTiles.Count > 0)
        {
            openTiles.Clear();
            foreach (Tile neighbor in GetNeighbors(nextTile))
            {
                if (!visitedTiles.Contains(neighbor) && neighbor.walkable)
                {
                    openTiles.Add(neighbor);
                }
            }
            if (openTiles.Count == 0)
            {
                break;
            }
            nextTile = openTiles[Random.Range(0, openTiles.Count)];
            startPath.Add(nextTile);
            visitedTiles.Add(nextTile);
            foreach (Tile leftoverTile in openTiles)
            {
                visitedTiles.Add(leftoverTile);
            }
            if (nextTile == endTile)
            {
                return startPath;
            }
        }
        return startPath;
    }
}
