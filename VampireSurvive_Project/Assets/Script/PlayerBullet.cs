using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactFX; // Normal bullet impact FX
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float timeDestroy = 2.0f;
    private float damage = 5f; // Default damage
    private bool isExplosionBullet = false;
    private float explosionRadius = 3f;
    private VFXManager explosionVFXManager;

    void Start()
    {
        Destroy(gameObject, timeDestroy);
    }

    void Update()
    {
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.Self);
    }

    public void SetExplosionMode(bool mode)
    {
        isExplosionBullet = mode;
    }

    public void SetExplosionDamageAndRadius(float dmg, float radius)
    {
        damage = dmg;
        explosionRadius = radius;
    }

    public void SetImpactFXManager()
    {
        explosionVFXManager = VFXManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            return;

        if (collision.CompareTag("HealPotion"))
            return;

        if (collision.CompareTag("ManaPotion"))
            return;

        if (collision.CompareTag("ExpItem"))
            return;

        if (isExplosionBullet)
        {
            // Explosion mode: Trigger at bullet position
            Vector2 impactPos = transform.position;
            if (explosionVFXManager != null)
            {
                GameObject explosionFX = explosionVFXManager.GetPooledExplosion(impactPos, Quaternion.identity);
                explosionVFXManager.RecycleExplosion(explosionFX, 1.5f);
            }
            else if (bulletImpactFX != null)
            {
                // Fallback if VFXManager is not set
                GameObject impact = Instantiate(bulletImpactFX, impactPos, Quaternion.identity);
                ParticleSystem ps = impact.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    Destroy(impact, ps.main.duration);
                }
            }

            // Damage all enemies in radius
            Collider2D[] hits = Physics2D.OverlapCircleAll(impactPos, explosionRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Enemy") || hit.CompareTag("Boss"))
                {
                    EnemyFollow enemy = hit.GetComponent<EnemyFollow>();
                    if (enemy != null)
                    {
                        enemy.takeDamage(damage);
                    }
                }
            }
        }
        else
        {
            // Normal mode
            Vector2 impactPos = collision.ClosestPoint(transform.position);
            if (bulletImpactFX != null)
            {
                GameObject impact = Instantiate(bulletImpactFX, impactPos, Quaternion.identity);
                ParticleSystem ps = impact.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    Destroy(impact, ps.main.duration);
                }
            }

            // Damage single enemy
            if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
            {
                EnemyFollow enemy = collision.GetComponent<EnemyFollow>();
                if (enemy != null)
                {
                    enemy.takeDamage(damage);
                }
            }
        }

        Destroy(gameObject);
    }

    // Debug explosion radius
    private void OnDrawGizmos()
    {
        if (isExplosionBullet)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}