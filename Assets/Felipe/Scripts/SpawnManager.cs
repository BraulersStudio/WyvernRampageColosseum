using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab1;
    public GameObject enemyPrefab2;
    public GameObject enemyPrefab3;
    public GameObject enemyPrefab4;
    public GameObject powerupPrefab1;
    public GameObject powerupPrefab2;
    public GameObject powerupPrefab3;

    private float spawnRangeX = 10;
    private float spawnZMin = 15; // set min spawn Z
    private float spawnZMax = 25; // set max spawn Z

    public int enemyCount, enemySpeed;
    public int waveCount = 1;
    public GameObject player; 

    void Update()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (enemyCount == 0)
        {
            SpawnEnemyWave(waveCount);
        }
        
    }

    Vector3 GenerateSpawnPosition ()
    {
        float xPos = Random.Range(-spawnRangeX, spawnRangeX);
        float zPos = Random.Range(spawnZMin, spawnZMax);
        return new Vector3(xPos, 0, zPos);
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        Vector3 powerupSpawnOffset = new Vector3(0, 0, -15); // make powerups spawn at player end

        // If no powerups remain, spawn a powerup
        //if (GameObject.FindGameObjectsWithTag("Powerup").Length == 0) // check that there are zero powerups
        //{
            //Instantiate(powerupPrefab, GenerateSpawnPosition() + powerupSpawnOffset, powerupPrefab.transform.rotation);
        //}

        // Spawn number of enemy based on wave number
        for (int i = 0; i < waveCount; i++)
        {
            Instantiate(enemyPrefab1, GenerateSpawnPosition(), enemyPrefab1.transform.rotation);
            //enemySpeed+= 10;

        }

        waveCount++;
        ResetPlayerPosition(); // put player back at start

    }

    // Move player back to position 
    void ResetPlayerPosition ()
    {
        player.transform.position = new Vector3(0, 1, -7);
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

    }
}
