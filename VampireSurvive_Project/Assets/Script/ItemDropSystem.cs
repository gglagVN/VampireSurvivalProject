using UnityEngine;

public class ItemDropSystem : MonoBehaviour
{
    [Header("Drop Prefabs")]
    public GameObject healPotionPrefab;
    public GameObject manaPotionPrefab;
    public GameObject expItemPrefab;

    [Header("Drop Chances (0-100)")]
    public float healDropChance = 30f;
    public float manaDropChance = 30f;
    public float expDropChance = 50f;

    [Header("Drop Settings")]
    public float dropForce = 2f;
    public float dropRadius = 0.5f;

    public void DropItems(Vector3 position, EnemyFollow.EnemyType enemyType)
    {
        if (Random.Range(0f, 100f) <= healDropChance)
        {
            DropItem(healPotionPrefab, position);
        }
        if (Random.Range(0f, 100f) <= manaDropChance)
        {
            DropItem(manaPotionPrefab, position);
        }
        if (Random.Range(0f, 100f) <= expDropChance)
        {
            GameObject expItem = DropItem(expItemPrefab, position);
        }
    }

    private GameObject DropItem(GameObject itemPrefab, Vector3 position)
    {
        if (itemPrefab == null) return null;

        // random pos 
        Vector2 randomOffset = Random.insideUnitCircle * dropRadius;
        Vector3 dropPosition = position + new Vector3(randomOffset.x, randomOffset.y, 0);

        // Spawn item
        GameObject item = Instantiate(itemPrefab, dropPosition, Quaternion.identity);

        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            rb.AddForce(randomDirection * dropForce, ForceMode2D.Impulse);
        }

        return item;
    }
}
