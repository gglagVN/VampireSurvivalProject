using UnityEngine;

public class BasicEnemy : EnemyFollow
{

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            pushBackCalculation();
            takeDamage(damage);
        }
    }
}
