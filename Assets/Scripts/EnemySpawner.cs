using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 1f; // สุ่มเกิดทุก ๆ 1 วินาที
    public float timeBetweenWaves = 3f; // เวลาพักเตรียมตัวก่อนเริ่มระลอกใหม่

    [Header("Random Settings")]
    public int minHealth = 1;
    public int maxHealth = 25;
    public float minSpeed = 2f;
    public float maxSpeed = 10f;

    private int enemiesToSpawn = 2; // ระลอกแรกมีศัตรู 2 ตัว

    private void Start()
    {
        StartCoroutine(WaveSpawnerLoop());
    }

    private IEnumerator WaveSpawnerLoop()
    {
        yield return new WaitForSeconds(timeBetweenWaves);

        while (GameManager.Instance != null && !GameManager.Instance.IsGameOver)
        {
            // อัปเดตเลขเวฟบนหน้าจอ UI
            GameManager.Instance.UpdateWaveText();

            // ทยอยปล่อยศัตรูตามจำนวนที่กำหนดไว้ในระลอกนี้
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                if (GameManager.Instance.IsGameOver) yield break;

                SpawnEnemy();
                GameManager.Instance.enemiesAlive++; // แจ้งระบบว่าศัตรูเกิดเพิ่ม 1 ตัว

                yield return new WaitForSeconds(spawnInterval);
            }

            // รอจนกว่าผู้เล่นจะเคลียร์ศัตรูในระลอกนี้ตายหมด (enemiesAlive เป็น 0)
            while (GameManager.Instance.enemiesAlive > 0 && !GameManager.Instance.IsGameOver)
            {
                yield return new WaitForSeconds(0.5f);
            }

            // เตรียมคำนวณจำนวนศัตรูสำหรับระลอกถัดไป
            GameManager.Instance.currentWave++;

            // เพิ่มจำนวนศัตรู 50% และปัดเศษขึ้น (2 -> 3 -> 5 -> 8)
            enemiesToSpawn = Mathf.CeilToInt(enemiesToSpawn * 1.5f);

            // พักหายใจก่อนเวฟถัดไปเริ่ม
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        if (spawnedEnemy.TryGetComponent<EnemyController>(out EnemyController enemy))
        {
            float randomHealth = Random.Range(minHealth, maxHealth + 1);
            float randomSpeed = Random.Range(minSpeed, maxSpeed);
            float armorValue = 0;

            enemy.SetStats(randomHealth, randomSpeed, armorValue);
        }
    }
}