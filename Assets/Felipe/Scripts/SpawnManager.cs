using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab1;
    public GameObject enemyPrefab2;
    public GameObject enemyPrefab3;
    public GameObject enemyPrefab4;
    public GameObject[] powerupPrefab;
    private float spawnRangeX = 10;
    private float spawnZMin = 15; // set min spawn Z
    private float spawnZMax = 25; // set max spawn Z

    public int enemyCount, enemySpeed;
    public int waveCount = 1;
    public GameObject player;

    public Vector3 playerInitialPosition;

    void Awake()
    {
        playerInitialPosition = player.transform.position;
    }

    void Update()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (enemyCount == 0)
        {
            SpawnEnemyWave();
        }

    }

    Vector3 GenerateSpawnPosition()
    {
        float xPos = player.transform.position.x + Random.Range(-spawnRangeX, spawnRangeX);
        float zPos = player.transform.position.z + Random.Range(spawnZMin, spawnZMax);
        if (xPos < -8)
        {
            return GenerateSpawnPosition();
        }
        else
        {
            return new Vector3(xPos, player.transform.position.y, zPos);
        }
    }

    void SpawnEnemyWave()
    {
        Vector3 powerupSpawnOffset = new Vector3(0, 0, 0); // make powerups spawn at player end

        int randomPowerup = Random.Range(0, powerupPrefab.Length);
        Debug.Log(randomPowerup);
        Instantiate(powerupPrefab[randomPowerup], GenerateSpawnPosition() + powerupSpawnOffset, powerupPrefab[randomPowerup].transform.rotation);

        // Spawn number of enemy based on wave number
        Debug.Log("Wave: " + waveCount);
        if (waveCount <= 3)
        {
            Instantiate(enemyPrefab1, GenerateSpawnPosition(), enemyPrefab1.transform.rotation);
        }
        else if (waveCount <= 6)
        {

            Instantiate(enemyPrefab2, GenerateSpawnPosition(), enemyPrefab2.transform.rotation);
        }
        else if (waveCount < 10)
        {
            Instantiate(enemyPrefab3, GenerateSpawnPosition(), enemyPrefab3.transform.rotation);
        }
        else if (waveCount == 10)
        {
            Instantiate(enemyPrefab4, GenerateSpawnPosition(), enemyPrefab4.transform.rotation);
        }


        if (waveCount == 11)
        {

        }
        else
        {
            waveCount++;
        }

        ResetPlayerPosition(); // put player back at start

    }

    // Move player back to position 
    void ResetPlayerPosition()
    {
        player.transform.position = playerInitialPosition;

    }
}
