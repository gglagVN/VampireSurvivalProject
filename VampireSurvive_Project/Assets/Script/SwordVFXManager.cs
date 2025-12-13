using System.Collections;
using UnityEngine;

public class SwordVFXManager : MonoBehaviour
{
    [Header("VFX Prefabs")]
    public GameObject slashPrefab;
    public GameObject multiSlashPrefab;
    public GameObject swordImpactPrefab;
    public GameObject swordKiPrefab;

    [Header("References")]
    public Transform cameraTransform;
    public SpriteRenderer srSword;

    [Header("Judgement Cut Settings")]
    public int numberOfSlashes = 10;
    public float slashInterval = 0.04f;
    public int numberOfMultiSlashes = 5;
    public float multiSlashSpawnDelay = 0.01f;
    public float multiSlashScaleVariation = 0.2f;
    public float shakeMagnitude = 0.8f;
    public float shakeDuration = 0.8f;

    [Header("SwordKI E")]
    public float kiSpeed = 28f;
    public float kiLifetime = 1.4f;
    public float kiWidth = 4.0f;
    public float kiShakeMagnitude = 0.45f;
    public float kiShakeDuration = 0.25f;
    public float damageKi = 120f;

    public static bool isJudgementActive = false;

    private void Awake()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
        if (srSword == null) srSword = GetComponentInChildren<SpriteRenderer>(true);
    }

    // ===== SwordKI E =====
    public void PlayKiengKhiVFX(Vector2 startPos, Vector2 direction)
    {
        if (swordKiPrefab == null)
        {
            Debug.LogError("swordKiPrefab (Slash_D) NOT ASSIGNED!");
            return;
        }

        GameObject ki = Instantiate(swordKiPrefab, startPos, Quaternion.identity);

        // Rotate 
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        ki.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        ki.transform.localScale = new Vector3(kiWidth, 1.5f, 1f);

        // Particle System setup
        ParticleSystem ps = ki.GetComponent<ParticleSystem>();
        if (ps != null) SetupEnhancedKiParticleSystem(ps);

        // Add Rigidbody2D 
        Rigidbody2D rb = ki.GetComponent<Rigidbody2D>();
        if (rb == null) rb = ki.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.drag = 0.3f;
        rb.velocity = direction.normalized * kiSpeed;
        rb.freezeRotation = true;

        // Bigger collidor
        BoxCollider2D col = ki.GetComponent<BoxCollider2D>();
        if (col == null) col = ki.AddComponent<BoxCollider2D>();
        col.size = new Vector2(kiWidth * 0.9f, 1.2f); // Height collider
        col.isTrigger = true;

        // Screen shake
        //StartCoroutine(ScreenShake(kiShakeDuration, kiShakeMagnitude));

        // Auto destroy
        Destroy(ki, kiLifetime);
    }

    //ParticleSystem setup for enhanced effect
    void SetupEnhancedKiParticleSystem(ParticleSystem ps)
    {
        var main = ps.main;
        main.startLifetime = kiLifetime * 1.2f;
        main.startSpeed = kiSpeed * 0.8f;
        main.startSize = 0.5f;
        main.maxParticles = 250;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.white, 0f),
                new GradientColorKey(Color.white, 0.5f),
                new GradientColorKey(new Color(1f, 1f, 1f, 0f), 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 0.5f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        col.color = grad;

        // EMISSION 
        var emission = ps.emission;
        emission.rateOverTime = 150f;

        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.x = new ParticleSystem.MinMaxCurve(10f, 15f);
        velocity.y = new ParticleSystem.MinMaxCurve(-4f, 4f);

        // NOISE MODULE 
        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 3f;
        noise.frequency = 0.6f;
        noise.scrollSpeed = 2f;
        noise.quality = ParticleSystemNoiseQuality.High;

        // SIZE OVER LIFETIME 
        var size = ps.sizeOverLifetime;
        size.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 0.2f);
        sizeCurve.AddKey(0.5f, 1.5f);
        sizeCurve.AddKey(1f, 0.1f);
        size.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        // RENDERER 
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.sortingLayerName = "VFX";
        renderer.sortingOrder = 15;
        renderer.renderMode = ParticleSystemRenderMode.Stretch;
        renderer.lengthScale = 6f;
        renderer.velocityScale = 0.15f;
    }

    public void PlaySlashVFX(Vector3 pos, Quaternion rot, bool flipX, float length = 1f)
    {
        if (slashPrefab == null) return;
        GameObject s = Instantiate(slashPrefab, pos, rot);
        float sx = srSword ? srSword.bounds.size.x * 1.2f * length : 2f * length;
        float sy = srSword ? srSword.bounds.size.y * 1.2f : 2f;
        s.transform.localScale = new Vector3(sx, sy, 1f);

        SpriteRenderer sr = s.GetComponentInChildren<SpriteRenderer>();
        if (sr) { sr.flipX = flipX; sr.sortingLayerName = "VFX"; sr.sortingOrder = 10; }

        Destroy(s, 0.5f);
    }

    public void PlayRandomSlashVFX()
    {
        Vector3 pos = RandomScreenPos();
        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        PlaySlashVFX(pos, rot, Random.value > 0.5f, 1.5f);
    }

    // Imact VFX Method
    public void PlayImpactVFX(Vector3 pos, Vector3 dir)
    {
        if (swordImpactPrefab == null) return;
        GameObject i = Instantiate(swordImpactPrefab, pos, Quaternion.identity);
        i.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, Vector3.forward);

        foreach (ParticleSystem p in i.GetComponentsInChildren<ParticleSystem>())
        {
            var r = p.GetComponent<ParticleSystemRenderer>();
            if (r) { r.sortingLayerName = "VFX"; r.sortingOrder = 10; }
            p.Play();
        }
        Destroy(i, 0.6f);
    }

    // Judgement Cut effect
    public void StartJudgementCutBackground()
    {
        isJudgementActive = true;
        StartCoroutine(SpawnMultiSlashClean());
        StartCoroutine(ScreenShake(shakeDuration, shakeMagnitude));
    }

    public IEnumerator ScreenFlash()
    {
        yield return new WaitForSeconds(0.1f);
        isJudgementActive = false;
    }

    Vector3 RandomScreenPos()
    {
        Camera c = cameraTransform.GetComponent<Camera>();
        Vector3 b = c.ViewportToWorldPoint(new Vector3(0, 0, c.nearClipPlane));
        Vector3 t = c.ViewportToWorldPoint(new Vector3(1, 1, c.nearClipPlane));
        return new Vector3(Random.Range(b.x, t.x), Random.Range(b.y, t.y), 0);
    }

    // Spawn multi-slash judgement cut VFX across the screen
    IEnumerator SpawnMultiSlashClean()
    {
        Camera c = cameraTransform.GetComponent<Camera>();
        float h = c.orthographicSize * 2f;
        float w = h * c.aspect;
        float bx = w / 7f, by = h / 7f;

        for (int i = 0; i < numberOfMultiSlashes; i++)
        {
            Vector3 p = RandomScreenPos();
            GameObject m = Instantiate(multiSlashPrefab, p, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
            float s = 1f + Random.Range(-multiSlashScaleVariation, multiSlashScaleVariation);
            m.transform.localScale = new Vector3(bx * s, by * s, 1f);

            foreach (ParticleSystem ps in m.GetComponentsInChildren<ParticleSystem>())
            {
                var r = ps.GetComponent<ParticleSystemRenderer>();
                if (r) { r.sortingLayerName = "VFX"; r.sortingOrder = 10; }
                ps.Play();
            }
            Destroy(m, 0.8f);
            yield return new WaitForSeconds(multiSlashSpawnDelay);
        }
    }

    IEnumerator ScreenShake(float d, float m)
    {
        Vector3 o = cameraTransform.localPosition;
        float e = 0;
        while (e < d)
        {
            cameraTransform.localPosition = o + Random.insideUnitSphere * m;
            e += Time.deltaTime;
            yield return null;
        }
        cameraTransform.localPosition = o;
    }
}