using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [SerializeField] float moveSpeed;
    [SerializeField] float health;

    List<Tile> path;
    List<Tile> currentPath;
    GameManager gameManager;
    MapGenerator mapGenerator;
    Tile startTile;
    Tile endTile;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        mapGenerator = FindObjectOfType<MapGenerator>();
        path = new List<Tile>();
        currentPath = new List<Tile>();
        //path = gameManager.currentPath;
    }

    // Use this for initialization
    void Start () {
        gameManager.enemyList.Add(this);
        UpdatePathFromPosition();
	}
	
	// Update is called once per frame
	void Update () {
        if (currentPath.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentPath[0].transform.position, moveSpeed * Time.deltaTime);
            if (transform.position == currentPath[0].transform.position)
            {
                currentPath.Remove(currentPath[0]);
            }
        }         
    }

    //public void UpdatePath(List<Tile> currentPath)
    //{
    //    path = currentPath;
    //    Tile currentTile = mapGenerator.TileFromWorldPoint(transform.position);

    //    int index = path.IndexOf(currentTile);
    //    for (int i = 0; i < index; i++)
    //    {
    //        path.Remove(path[i]);
    //    }
    //}

    public void UpdatePathFromPosition()
    {
        startTile = mapGenerator.TileFromWorldPoint(transform.position);
        endTile = mapGenerator.endTile;
        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();
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
            }
            foreach (Tile neighborTile in mapGenerator.GetNeighbors(tile))
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
                    if (!openSet.Contains(neighborTile))
                    {
                        openSet.Add(neighborTile);
                    }
                }
            }
        }
    }

    int GetDistance(Tile a, Tile b)
    {
        int distanceX = Mathf.Abs(a.gridX - b.gridX);
        int distanceZ = Mathf.Abs(a.gridZ - b.gridZ);
        return distanceX + distanceZ;
    }

    void RetracePath(Tile startTile, Tile endTile)
    {
        if (path.Count > 0)
        {
            path.Clear();
        }
        Tile currentTile = endTile;
        while (currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }
        path.Reverse();
        currentPath = path;
    }

    public void TakeDamage(float amountToDamage)
    {
        health -= amountToDamage;
        if (health <= 0)
        {
            Spawner.enemyList.Remove(this.transform);
            gameManager.enemyList.Remove(this);
            Destroy(gameObject);
        }
    }
}
