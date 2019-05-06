using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour {

    MapGenerator mapGen;
    public Tile startTile;
    public Tile endTile;
    int currentDistance;

    public GameManager gameManager;
    public List<Tile> path;

    private void Awake()
    {
        path = new List<Tile>();
        mapGen = GetComponent<MapGenerator>();
    }

    public bool FindPath()
    {
        if (path.Count > 0)
        {
            for (int i = 0; i < path.Count; i++)
            {
                MeshRenderer renderer = path[i].GetComponent<MeshRenderer>();
                renderer.material.color = Color.grey;
            }
            path.Clear();
        }
        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();
        startTile = mapGen.startTile;
        endTile = mapGen.endTile;
        openSet.Add(startTile);
        while (openSet.Count > 0)
        {
            Tile tile = openSet[0];
            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < tile.fCost || openSet[i].fCost == tile.fCost)
                {
                    if (openSet[i].hCost < tile.hCost)
                    {
                        tile = openSet[i];
                    }
                }
            }
            openSet.Remove(tile);
            closedSet.Add(tile);
            if (tile == endTile)
            {
                RetracePath(startTile, endTile);
                return true;
            }
            foreach (Tile neighborTile in mapGen.GetNeighbors(tile))
            {
                if (!neighborTile.walkable || closedSet.Contains(neighborTile))
                {
                    continue;
                }
                int neighborCost = tile.gCost + GetDistance(tile, neighborTile);
                if (neighborCost < neighborTile.gCost || !openSet.Contains(neighborTile))
                {
                    neighborTile.gCost = neighborCost;
                    neighborTile.hCost = GetDistance(neighborTile, endTile);
                    neighborTile.parent = tile;
                    if(!openSet.Contains(neighborTile))
                    {
                        openSet.Add(neighborTile);
                    }
                }
            }
        }
        Debug.Log("Path failed");
        return false;
    }

    int GetDistance (Tile a, Tile b)
    {
        int distanceX = Mathf.Abs(a.gridX - b.gridX);
        int distanceZ = Mathf.Abs(a.gridZ - b.gridZ);
        return distanceX + distanceZ;
    }

    void RetracePath(Tile startTile, Tile endTile)
    {
        Tile currentTile = endTile;
        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }
        path.Reverse();
        gameManager.currentPath = path;
        foreach (Tile thisTile in path)
        {
            thisTile.GetComponent<MeshRenderer>().material.color = Color.blue;
        }
    }
}
