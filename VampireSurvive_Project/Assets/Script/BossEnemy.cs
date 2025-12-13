using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossEnemy : EnemyFollow
{
    public GameObject bulletPrefabs;
    public float speedDan = 20f;
    private PlayerMovement pm;
    new void Start()
    {
        base.Start();
        pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        if (SceneManager.GetActiveScene().name == "Level2")
            StartCoroutine(chooseSkillLevel2());
        else if (SceneManager.GetActiveScene().name == "Level3")
            StartCoroutine(chooseSkill());
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !pm.isImmune)
        {
            pm.TakeDamage();
        }
    }
    private void dichChuyen()
    {
        if (player != null)
        {
            transform.position = player.transform.position;
        }
    }
    private void banDan()
    {
        if (player != null)
        {
            Vector3 directionToPlayer = player.transform.position - transform.position;
            directionToPlayer.Normalize();
            GameObject bullet = Instantiate(bulletPrefabs, transform.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = directionToPlayer * speedDan;
            Destroy(bullet, 3f);
        }
    }
    private void banDanVongTron()
    {
        int bulletCount = 12;
        float angleStep = 360f / bulletCount;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * angleStep;
            float angleRad = angle * Mathf.Deg2Rad;
            Vector2 bulletDirection = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

            GameObject bullet = Instantiate(bulletPrefabs, transform.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = bulletDirection * speedDan;
            Destroy(bullet, 3f);
        }
    }
    IEnumerator chooseSkill()
    {
        yield return new WaitForSeconds(5);
        while (true)
        {
            int randomSkill = Random.Range(0, 3);
            float temp = moveSpeed;
            switch (randomSkill)
            {
                case 0:
                    dichChuyen();
                    break;
                case 1:
                    banDan();
                    break;
                case 2:
                    banDanVongTron();
                    break;
            }
            moveSpeed = 0;
            yield return new WaitForSeconds(1);
            moveSpeed = temp;
            yield return new WaitForSeconds(5);
        }
    }
    IEnumerator chooseSkillLevel2()
    {
        yield return new WaitForSeconds(5);
        while (true)
        {
            int randomSkill = Random.Range(1, 3);
            float temp = moveSpeed;
            switch (randomSkill)
            {
                case 1:
                    banDan();
                    break;
                case 2:
                    banDanVongTron();
                    break;
            }
            moveSpeed = 0;
            yield return new WaitForSeconds(1);
            moveSpeed = temp;
            yield return new WaitForSeconds(5);
        }
    }
}
