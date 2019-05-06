using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject mapManager;
    public GameObject tile;
    public List<Tile> currentPath;
    public List<Enemy> enemyList;
    public bool gameStarted;

    Pathfinder pathFinder;
    MapGenerator mapGenerator;
    Spawner spawner;
    List<Tile> tempPath;


    private void Awake()
    {
        mapGenerator = mapManager.GetComponent<MapGenerator>();
        pathFinder = mapManager.GetComponent<Pathfinder>();
        spawner = mapManager.GetComponent<Spawner>();
    }

    // Use this for initialization
    void Start () {
        currentPath = pathFinder.path;
        ColorPath();
        gameStarted = true;
        spawner.StartCoroutine(spawner.SpawnEnemy());
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                //Find which Tile was hit, set its walkable state to false and color to red
                Tile tile = mapGenerator.TileFromWorldPoint(hit.point);
                tile.walkable = false;
                tile.GetComponent<MeshRenderer>().material.color = Color.red;
                UpdateEnemyPaths();
                //Check if the path contains the hit tile
                if (pathFinder.path.Contains(tile))
                {
                    //Store the current path and remove the hit tile from it
                    tempPath = pathFinder.path;
                    pathFinder.path.Remove(tile);
                    //Check if PathFinder was able to find a new path
                    if (!pathFinder.FindPath())
                    {
                        //If a new path was not found, restore the path, reset the hit tile to walkable,
                        //default the color, refind the path, store the path
                        pathFinder.path = tempPath;
                        tile.walkable = true;
                        tile.GetComponent<MeshRenderer>().material.color = Color.grey;
                        pathFinder.FindPath();
                        currentPath = pathFinder.path;
                        ColorPath();
                        UpdateEnemyPaths();
                    } else
                    {
                        currentPath = pathFinder.path;
                        ColorPath();
                    }
                }
            }
        }
	}

    void UpdateEnemyPaths()
    {
        foreach (Enemy thisEnemy in enemyList)
        {
            //thisEnemy.UpdatePath(currentPath);
            thisEnemy.UpdatePathFromPosition();
        }
    }

    void ColorPath()
    {
        foreach (Tile thisTile in currentPath)
        {
            thisTile.GetComponent<MeshRenderer>().material.color = Color.blue;
        }
    }
}
