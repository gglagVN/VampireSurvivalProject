using System.Collections;

using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    public int damage = 10;
    [HideInInspector] public bool canAttack = false;

    // Danh sách các enemy đã bị đánh trong 1 đòn
    private HashSet<EnemyFollow> enemiesHit = new HashSet<EnemyFollow>();

    // Thêm tham chiếu đến prefab impact cho VFX khi hit enemy
    [SerializeField] private GameObject swordImpactPrefab;

    void OnTriggerStay2D(Collider2D collision)
    {
        if (canAttack && collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            EnemyFollow enemy = collision.GetComponent<EnemyFollow>();
            if (enemy != null && !enemiesHit.Contains(enemy))
            {
                enemy.takeDamage(damage);
                enemy.pushBackCalculation();

                // Spawn sword impact VFX tại vị trí enemy khi hit
                if (swordImpactPrefab != null)
                {
                    GameObject impact = Instantiate(swordImpactPrefab, collision.transform.position, Quaternion.identity);
                    // Tùy chỉnh rotation nếu cần, ví dụ: dựa trên hướng chém
                    Destroy(impact, 1f); // Tự hủy sau thời gian phù hợp với effect
                }

                // Đánh dấu enemy này đã bị đánh
                enemiesHit.Add(enemy);
            }
        }
    }

    // Hàm reset khi bắt đầu đòn mới
    public void ResetHitList()
    {
        enemiesHit.Clear();
    }

    // Setter cho impact prefab (gọi từ PlayerAttack khi ult)
    public void SetSwordImpactPrefab(GameObject prefab)
    {
        swordImpactPrefab = prefab;
    }
}