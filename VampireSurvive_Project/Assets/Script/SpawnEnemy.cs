using UnityEngine;
using System.Collections;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemyPrefab;   // Prefab của Enemy
    public int enemyCount = 5;       // Số lượng Enemy muốn spawn
    public float delay = 10f;        // Thời gian giữa các đợt spawn
    public Vector2 spawnCenter;      // Tâm của khu vực spawn
    public Vector2 spawnSize;        // Kích thước khu vực spawn
    public Transform player;         // Tham chiếu đến Player
    public float safeRadius = 3f;    // Bán kính an toàn quanh Player
    public int maxEnemyQuantity = 50;

    void Start()
    {

        StartCoroutine(SpawnEnemyLoop());
    }

    IEnumerator SpawnEnemyLoop()
    {
        yield return new WaitForSeconds(5);
        while (true)
        {
            int enemyQuantity = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if (enemyQuantity < maxEnemyQuantity)
                SpawnEnemies();

            yield return new WaitForSeconds(delay);
        }
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector2 randomPos;
            do
            {
                randomPos = new Vector2(
                    Random.Range(spawnCenter.x - spawnSize.x / 2, spawnCenter.x + spawnSize.x / 2),
                    Random.Range(spawnCenter.y - spawnSize.y / 2, spawnCenter.y + spawnSize.y / 2)
                );
            }
            while (Vector2.Distance(randomPos, player.position) < safeRadius);

            Instantiate(enemyPrefab, randomPos, Quaternion.identity);
        }
    }


    // Vẽ vùng spawn + safe zone
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(spawnCenter, spawnSize);

        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.position, safeRadius);
        }
    }
}
