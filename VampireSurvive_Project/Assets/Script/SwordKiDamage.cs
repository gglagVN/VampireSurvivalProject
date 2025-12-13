using UnityEngine;

public class SwordKiDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 100f;              // Sát thương mỗi lần chạm
    public float hitInterval = 0.2f;         // Thời gian chờ giữa 2 lần gây damage cho cùng 1 quái
    public float hitEffectDuration = 0.3f;   // Thời gian tồn tại của VFX trúng
    public GameObject hitVFX;                // Hiệu ứng trúng mục tiêu

    // Lưu lại thời điểm quái bị đánh gần nhất
    private readonly System.Collections.Generic.Dictionary<GameObject, float> hitTimes =
        new System.Collections.Generic.Dictionary<GameObject, float>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            TryHitEnemy(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            TryHitEnemy(collision.gameObject);
        }
    }

    private void TryHitEnemy(GameObject enemyObj)
    {
        float lastHitTime;
        hitTimes.TryGetValue(enemyObj, out lastHitTime);

        if (Time.time - lastHitTime >= hitInterval)
        {
            EnemyFollow enemy = enemyObj.GetComponent<EnemyFollow>();
            if (enemy != null)
            {
                enemy.takeDamage(damage);
            }
            hitTimes[enemyObj] = Time.time;
            if (hitVFX != null)
            {
                GameObject fx = Instantiate(hitVFX, enemyObj.transform.position, Quaternion.identity);
                Destroy(fx, hitEffectDuration);
            }
        }
    }
}