using System.Collections;
using UnityEngine;

public class GunPlayer : PlayerMovement
{
    [Header("Gun Reference")]
    public Gun gun; // Assign object with Gun.cs in Inspector

    [Header("Skill E - Explosion Bullet")]
    public float eCooldown = 2f;         // Cooldown for E (seconds)
    private float lastETime;
    public float staminaCostE = 10f;     // Stamina cost for E
    private bool isUsingSkillE = false;
    public float explosionDamageE = 30f; // Damage for explosion bullet
    public float explosionRadiusE = 3f;  // Explosion radius for E

    [Header("Skill Q - Ultimate Explosion")]
    public float qCooldown = 30f;         // Cooldown for Q (seconds, ultimate)
    private float lastQTime;
    public float staminaCostQ = 100f;    // Stamina cost for ultimate
    public float ultDamage = 50f;        // Damage to all enemies (full map)
    private bool isUsingSkillQ = false;

    public AudioSource VFXSoundQ;
    public AudioClip musicClipSoundQ;
    public AudioSource VFXSoundE;
    public AudioClip musicClipSoundE;

    protected override void Start()
    {
        base.Start();
        VFXSoundQ.clip = musicClipSoundQ;
        VFXSoundE.clip = musicClipSoundE;
        lastETime = -eCooldown;
        lastQTime = -qCooldown;
    }

    protected override void Update()
    {
        base.Update();

        // Prevent skill spam
        if (isUsingSkillE || isUsingSkillQ) return;

        // Activate E (Explosion Bullet)
        if (Input.GetKeyDown(KeyCode.E) && Time.time > lastETime + eCooldown)
        {
            UseSkillE();
        }

        // Activate Q (Ultimate Full Screen Explosion)
        if (Input.GetKeyDown(KeyCode.Q) && Time.time > lastQTime + qCooldown)
        {
            UseSkillQ();
        }
    }

    // ================= Implementation E - Explosion Bullet =================
    void UseSkillE()
    {
        if (gun == null)
        {
            Debug.LogWarning("GunPlayer: Gun reference not assigned in Inspector!");
            return;
        }

        // Check stamina
        if (staminaCostE > 0f && currentSta < staminaCostE)
        {
            Debug.Log("Not enough stamina for Skill E!");
            return;
        }

        if (staminaCostE > 0f)
        {
            currentSta -= staminaCostE;
            UpdateStaminaUI();
        }

        lastETime = Time.time;
        StartCoroutine(PerformSkillE());
    }

    IEnumerator PerformSkillE()
    {
        isUsingSkillE = true;

        // Create bullet at gun's fire position
        GameObject bullet = Instantiate(gun.bulletPrefabs, gun.firePos.position, gun.firePos.rotation);

        PlayerBullet pb = bullet.GetComponent<PlayerBullet>();
        if (pb != null)
        {
            pb.SetExplosionMode(true);
            pb.SetExplosionDamageAndRadius(explosionDamageE, explosionRadiusE);
            VFXSoundE.Play();
            pb.SetImpactFXManager(); // Use VFXManager for explosion
        }

        Debug.Log("GunPlayer: Activated Skill E (Explosion Bullet).");

        yield return new WaitForSeconds(0.25f);
        isUsingSkillE = false;
    }

    // ================= Implementation Q - Ultimate =================
    void UseSkillQ()
    {
        // Check stamina
        if (staminaCostQ > 0f && currentSta < staminaCostQ)
        {
            Debug.Log("Not enough stamina for Skill Q!");
            return;
        }

        if (staminaCostQ > 0f)
        {
            currentSta -= staminaCostQ;
            VFXSoundQ.Play();
            UpdateStaminaUI();
        }

        lastQTime = Time.time;
        StartCoroutine(PerformSkillQ());
    }

    IEnumerator PerformSkillQ()
    {
        isUsingSkillQ = true;

        // Trigger VFX via VFXManager
        if (VFXManager.Instance != null)
        {
            VFXManager.Instance.TriggerUltimateVFX();
        }
        else
        {
            Debug.LogWarning("VFXManager not found!");
        }

        // Sync damage with explosions (ultDelay + buffer)
        yield return new WaitForSeconds(0.85f);

        // Damage all enemies
        EnemyFollow[] allEnemies = FindObjectsOfType<EnemyFollow>();
        foreach (EnemyFollow enemy in allEnemies)
        {
            enemy.takeDamage(ultDamage);
        }

        Debug.Log("GunPlayer: Activated Skill Q (Ultimate Explosion) - Damage applied!");

        yield return new WaitForSeconds(0.5f);
        isUsingSkillQ = false;
    }

    // Debug gizmo for explosion radius (E)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, explosionRadiusE);
    }
}