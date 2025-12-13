using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SwordPlayer : PlayerMovement
{
    [Header("VFX Manager")]
    [SerializeField] private SwordVFXManager vfxManager;

    [Header("Skill Q Settings")]
    public float staminaCostQ = 100f;
    public float damagePerSlash = 50f;
    public float knockbackForceQ = 10f;
    public float knockbackDurationQ = 0.25f;
    public bool spawnImpactOnDeath = true;


    public AudioSource VFXSoundQ;
    public AudioClip musicClipSoundQ;
    public AudioSource VFXSoundE;
    public AudioClip musicClipSoundE;

    [Header("Skill E Settings")]
    public float staminaCostE = 60f;
    public float kiRange = 12f;
    public LayerMask enemyLayer = 1 << 6; // Layer "Enemy"

    private bool isUsingSkill = false;

    protected override void Start()
    {
        base.Start();
        VFXSoundQ.clip = musicClipSoundQ;
        VFXSoundE.clip = musicClipSoundE;
        if (vfxManager == null) vfxManager = GetComponent<SwordVFXManager>();
        if (vfxManager == null) vfxManager = FindObjectOfType<SwordVFXManager>();
        if (vfxManager == null) Debug.LogError("CANNOT FIND SwordVFXManager!");
    }

    protected override void Update()
    {
        base.Update();
        if (isUsingSkill) return;
        if (Input.GetKeyDown(KeyCode.Q)) UseSkillQ();
        if (Input.GetKeyDown(KeyCode.E)) UseSkillE();
    }

    void UseSkillQ()
    {
        if (currentSta < staminaCostQ || vfxManager == null) return;
        currentSta -= staminaCostQ;
        VFXSoundQ.Play();
        UpdateStaminaUI();
        StartCoroutine(PerformSkillQ());
    }

    IEnumerator PerformSkillQ()
    {
        isUsingSkill = true;
        vfxManager.StartJudgementCutBackground(); //Multi-Slash

        List<GameObject> enemies = GetAllOnScreenEnemies();

        for (int i = 0; i < vfxManager.numberOfSlashes; i++)
        {
            vfxManager.PlayRandomSlashVFX(); // Play slash VFX

            foreach (GameObject e in enemies.ToArray())
            {
                if (e == null) continue;
                EnemyFollow enemy = e.GetComponent<EnemyFollow>();
                if (enemy == null) continue;

                enemy.currentEnemyHP -= damagePerSlash;
                enemy.StartCoroutine("FlashHit");

                // Spawn impact Per Hit VFX
                if (spawnImpactOnDeath)
                {
                    Vector3 dir = (e.transform.position - transform.position).normalized;
                    vfxManager.PlayImpactVFX(e.transform.position, dir);
                }

                // Knockback
                Rigidbody2D rb = e.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 dir = ((Vector2)(e.transform.position - transform.position)).normalized
                                  + Random.insideUnitCircle * 0.5f;
                    rb.velocity = Vector2.zero;
                    rb.AddForce(dir * knockbackForceQ, ForceMode2D.Impulse);
                    StartCoroutine(ResetVelocity(e, knockbackDurationQ));
                }

                // Die
                if (enemy.currentEnemyHP <= 0f)
                    enemy.Die();
            }

            yield return new WaitForSeconds(vfxManager.slashInterval);
        }

        yield return new WaitForSeconds(0.3f);
        isUsingSkill = false;
        Debug.Log("Q: SCREEN WIPE COMPLETED - 0 MONSTER SURVIVED!");
    }

    // ===== SKILL E =====
    void UseSkillE()
    {
        if (currentSta < staminaCostE || vfxManager == null) return;
        currentSta -= staminaCostE;
        VFXSoundE.Play();
        UpdateStaminaUI();
        StartCoroutine(PerformSkillE());
    }

    IEnumerator PerformSkillE()
    {
        isUsingSkill = true;

        // track mouse position and direction
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
        Vector3 spawnPos = transform.position + (Vector3)direction * 1.2f;

        // Spawn VFX
        vfxManager.PlayKiengKhiVFX(spawnPos, direction);

        // Damage On Path
        StartCoroutine(DamageEnemiesOnKiPath(spawnPos, direction));

        yield return new WaitForSeconds(0.6f);
        isUsingSkill = false;
        Debug.Log("E: KIẾM KHÍ LAUNCHED!");
    }

    IEnumerator DamageEnemiesOnKiPath(Vector3 startPos, Vector2 direction)
    {
        float checkInterval = 0.05f;
        float distanceCovered = 0f;

        while (distanceCovered < kiRange)
        {
            Vector3 checkPos = startPos + (Vector3)direction * distanceCovered;

            // Raycast to detect enemy
            RaycastHit2D[] hits = Physics2D.RaycastAll(checkPos, direction, 1f, enemyLayer);

            foreach (var hit in hits)
            {
                EnemyFollow enemy = hit.collider.GetComponent<EnemyFollow>();
                if (enemy != null && enemy.currentEnemyHP > 0)
                {
                    enemy.currentEnemyHP -= vfxManager.damageKi;
                    enemy.StartCoroutine("FlashHit");
                    vfxManager.PlayImpactVFX(hit.point, direction);
                    if (enemy.currentEnemyHP <= 0f)
                        enemy.Die();
                }
            }

            distanceCovered += checkInterval * vfxManager.kiSpeed;
            yield return new WaitForSeconds(checkInterval);
        }
    }

    IEnumerator ResetVelocity(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;
        }
    }

    List<GameObject> GetAllOnScreenEnemies()
    {
        List<GameObject> list = new List<GameObject>();
        Camera cam = Camera.main;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Vector3 vp = cam.WorldToViewportPoint(go.transform.position);
            if (vp.z > 0 && vp.x > -0.1f && vp.x < 1.1f && vp.y > -0.1f && vp.y < 1.1f)
                list.Add(go);
        }
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        if (boss != null)
        {
            Vector3 vp1 = cam.WorldToViewportPoint(boss.transform.position);
            if (vp1.z > 0 && vp1.x > -0.1f && vp1.x < 1.1f && vp1.y > -0.1f && vp1.y < 1.1f && boss != null)
                list.Add(boss);
        }

        return list;
    }
}