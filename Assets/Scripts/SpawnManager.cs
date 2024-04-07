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

    public int enemyCount;
    public int waveCount = 1;
    public GameObject player;
    public PlayerController plyCtrl;
    public GameManager gameManager;

    private float spawnRangeX = 22f;
    private float spawnRangeZ = 17f;
    private Vector3 spawnPivot = new Vector3(35, 0, 35);

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
        float xPos = Random.Range(-spawnRangeX, spawnRangeX);
        float zPos = Random.Range(-spawnRangeZ, spawnRangeZ);
        return new Vector3(xPos, player.transform.position.y, zPos) + spawnPivot;
    }

    private void SpawnPowerup()
    {
        Vector3 powerupSpawnOffset = new Vector3(0, 2, 0);

        int randomPowerup = Random.Range(0, powerupPrefab.Length);
        Debug.Log(randomPowerup);
        Instantiate(powerupPrefab[randomPowerup], GenerateSpawnPosition() + powerupSpawnOffset, powerupPrefab[randomPowerup].transform.rotation);
    }

    void SpawnEnemyWave()
    {
        for (int i = 0; i < powerupPrefab.Length -1; i++)
        {

            SpawnPowerup();
        }

        // Spawn number of enemy based on wave number
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

        // Increase wave count up to 11, in 11 should win
        if (waveCount == 11)
        {
            gameManager.VictoryScene();
        }
        else
        {
            waveCount++;
        }
    }
}
