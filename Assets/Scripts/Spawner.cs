using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject enemyPrefab;
    public GameManager gameManager;
    public static List<Transform> enemyList;

    [SerializeField] float spawnRate;

    MapGenerator mapGenerator;
    Vector3 spawnLocation;


    private void Awake()
    {
        mapGenerator = GetComponent<MapGenerator>();
        enemyList = new List<Transform>();
    }

    private void Start()
    {
        spawnLocation = mapGenerator.startTile.transform.position;
    }

    public IEnumerator SpawnEnemy()
    {
        while (true)
        {
            if (gameManager.gameStarted)
            {
                yield return new WaitForSeconds(spawnRate);
                GameObject enemy = Instantiate(enemyPrefab);
                enemy.transform.position = spawnLocation;
                enemyList.Add(enemy.transform);
            }
        }
    }
}
