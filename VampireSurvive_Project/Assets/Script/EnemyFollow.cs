using UnityEngine;
using System.Collections;
using System;

public class EnemyFollow : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float knockbackForce = 3f;
    public float knockbackDuration = 0.2f;
    public float enemyMaxHP = 30;
    public float currentEnemyHP;
    public float damage = 10;
    public int point = 0;
    private Color originalColor;
    private SpriteRenderer sr;
    public Transform player;
    private Rigidbody2D rb;
    private bool isKnockback = false;

    private ItemDropSystem dropSystem;

    public enum EnemyType { Mini, Heavy, Boss }
    public EnemyType type;
    public PlayerLevelSystem lvUP;
    public GameObject winItemPrefab;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentEnemyHP = enemyMaxHP;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        lvUP = player.GetComponent<PlayerLevelSystem>();
        dropSystem = FindObjectOfType<ItemDropSystem>();

        if (type == EnemyType.Mini) point = 5;
        else if (type == EnemyType.Heavy) point = 10;
        else if (type == EnemyType.Boss) point = 100;
    }

    void FixedUpdate()
    {
        if (!isKnockback && player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            if (direction.x > 0)
                sr.flipX = false;
            else if (direction.x < 0)
                sr.flipX = true;
            Vector2 newPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
    }

    public void pushBackCalculation()
    {
        Vector2 pushBackDir = (transform.position - player.position).normalized;
        StartCoroutine(Knockback(pushBackDir));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (type == EnemyType.Boss)
            return;
        if (collision.gameObject.CompareTag("Player"))
        {
            pushBackCalculation();
            takeDamage(damage);
        }
    }

    public void takeDamage(float damageAttack)
    {
        currentEnemyHP -= damageAttack;
        StartCoroutine(FlashHit());
        if (currentEnemyHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        ScoreManagement.Instance.addScore(point);

        // Logic Drop Item
        if (dropSystem != null)
        {
            dropSystem.DropItems(transform.position, type);
        }
        if (type == EnemyType.Mini)
            lvUP.GainXP(5);
        else if (type == EnemyType.Heavy)
            lvUP.GainXP(10);
        else if (type == EnemyType.Boss)
        {
            lvUP.GainXP(100);
            if (winItemPrefab != null)
            {
                Instantiate(winItemPrefab, transform.position, Quaternion.identity);
                winItemPrefab.SetActive(true);
            }
        }
        Destroy(gameObject);
    }

    IEnumerator Knockback(Vector2 pushDir)
    {
        isKnockback = true;
        rb.AddForce(pushDir * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        rb.velocity = Vector2.zero;
        isKnockback = false;
    }

    IEnumerator FlashHit()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sr.color = originalColor;
    }
}
