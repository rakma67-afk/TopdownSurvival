using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 1.5f;
    public float firstSpawnDelay = 1f;

    [Header("Random Settings")]
    public int minHealth = 1;
    public int maxHealth = 5;
    public float minSpeed = 2f;
    public float maxSpeed = 10f;

    private void Start()
    {
        StartCoroutine(
            SpawnLoop()
        );
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(
            firstSpawnDelay
        );

        while (GameManager.Instance != null &&
               !GameManager.Instance.IsGameOver)
        {
            SpawnEnemy();

            yield return new WaitForSeconds(
                spawnInterval
            );
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints == null ||
            spawnPoints.Length == 0)
        {
            return;
        }

        int randomIndex =
            Random.Range(
                0,
                spawnPoints.Length
            );

        Transform spawnPoint =
            spawnPoints[randomIndex];

        GameObject spawnedEnemy = Instantiate(
            enemyPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        //if (spawnedEnemy.TryGetComponent<EnemyController>(out EnemyController enemy))
        //    int randomHealth = Random.Range(minHealth, maxHealth + 1); // สุ่ม 1 ถึง 5
        //    float randomSpeed = Random.Range(minSpeed, maxSpeed);     // สุ่ม 2 ถึง 10
        //
        //    enemy.SetStats(randomHealth, randomSpeed);
        //}
    }
}
