using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    // Singleton instance for easy access
    public static VFXManager Instance { get; private set; }

    [Header("Ultimate VFX Settings (Skill Q)")]
    [SerializeField] private GameObject ultMovePrefab; // Assign ultMove.prefab
    [SerializeField] private GameObject explosionImpactFX; // Assign ExplosionBullet.prefab
    [SerializeField] private float ultDelay = 0.75f; // Delay between ultMove and explosions (0.5-1s)
    [SerializeField] private int maxParticlesToEmit = 400; // Number of particles/explosions
    [SerializeField] private float ultScaleFactorMultiplier = 1f; // Scale adjustment for ultMove
    [SerializeField] private int particleSortingOrder = 10; // Sorting order for visibility

    [Header("Object Pooling")]
    [SerializeField] private int poolSize = 500; // Pool size for explosions (covers both Q and E)
    private Queue<GameObject> explosionPool = new Queue<GameObject>();

    [Header("Camera Shake for Ultimate")]
    [SerializeField] private bool cameraShakeEnabled = true;
    [SerializeField] private float shakeDuration = 1.5f;
    [SerializeField] private AnimationCurve shakeCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
    [SerializeField] private float shakeMagnitude = 0.5f;
    [SerializeField] private bool useMainCamera = true;
    private Vector3 originalCamPos;
    private Camera shakeCam;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.parent = null;       // ✅ Tách ra khỏi cha
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Setup camera shake
        if (cameraShakeEnabled && useMainCamera)
        {
            shakeCam = Camera.main;
            if (shakeCam != null)
            {
                originalCamPos = shakeCam.transform.localPosition;
            }
        }

        // Initialize explosion pool
        InitializeExplosionPool();
    }

    private void InitializeExplosionPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject exp = Instantiate(explosionImpactFX);
            exp.SetActive(false);
            explosionPool.Enqueue(exp);
        }
        Debug.Log("Explosion pool initialized with " + poolSize + " objects.");
    }

    public GameObject GetPooledExplosion(Vector3 position, Quaternion rotation)
    {
        if (explosionPool.Count > 0)
        {
            GameObject exp = explosionPool.Dequeue();
            exp.transform.position = position;
            exp.transform.rotation = rotation;
            exp.SetActive(true);

            // Force play particle systems
            ParticleSystem[] ps = exp.GetComponentsInChildren<ParticleSystem>(true);
            foreach (ParticleSystem p in ps)
            {
                p.Clear(true);
                p.Play(true);
            }

            return exp;
        }
        else
        {
            // Fallback: Instantiate new
            GameObject fallback = Instantiate(explosionImpactFX, position, rotation);
            return fallback;
        }
    }

    public void ReturnToPool(GameObject explosionObj)
    {
        if (explosionObj != null)
        {
            explosionObj.SetActive(false);
            explosionPool.Enqueue(explosionObj);
        }
    }

    public void RecycleExplosion(GameObject explosionObj, float delay = 1.5f)
    {
        StartCoroutine(RecycleAfterDelay(explosionObj, delay));
    }

    private IEnumerator RecycleAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(obj);
    }

    // Trigger Ultimate VFX (Skill Q) - FULL SCREEN theo camera
    public void TriggerUltimateVFX()
    {
        StartCoroutine(UltimateVFXCoroutine());
    }

    private IEnumerator UltimateVFXCoroutine()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Main Camera not found!");
            yield break;
        }

        // ✅ TÍNH TOÁN SCREEN BOUNDS THEO VỊ TRÍ CAMERA HIỆN TẠI (FULL SCREEN)
        float orthographicSize = cam.orthographicSize;
        float screenWidth = orthographicSize * cam.aspect * 2f;
        float screenHeight = orthographicSize * 2f;

        // Center theo camera position (particles sẽ random trong viewport của camera)
        Vector3 screenCenter = cam.transform.position;

        Debug.Log($"Camera pos: {screenCenter}, Screen bounds: {screenWidth}x{screenHeight}");

        // Instantiate ultMove tại center của camera
        GameObject ultInstance = Instantiate(ultMovePrefab, screenCenter, Quaternion.identity);
        ultInstance.transform.localScale = new Vector3(ultScaleFactorMultiplier, ultScaleFactorMultiplier, ultScaleFactorMultiplier);

        // Set sorting order
        SetParticleSortingOrder(ultInstance, particleSortingOrder);

        // Configure ultMove particle system để emit FULL SCREEN theo camera
        ParticleSystem ultPS = ultInstance.GetComponent<ParticleSystem>();
        if (ultPS != null)
        {
            var main = ultPS.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World; // Quan trọng: World space
            main.maxParticles = maxParticlesToEmit;

            // ✅ SHAPE BOX COVER TOÀN SCREEN THEO CAMERA
            var shape = ultPS.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(screenWidth * 1.1f, screenHeight * 1.1f, 1f); // Cover full viewport + margin

            // ✅ EMISSION BURST để emit tất cả particles ngay lập tức
            var emission = ultPS.emission;
            emission.enabled = true;
            ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
            bursts[0] = new ParticleSystem.Burst(0f, maxParticlesToEmit);
            emission.SetBursts(bursts);

            // Clear và Play
            ultPS.Clear(true);
            ultPS.Play(true);
            ultPS.Emit(maxParticlesToEmit);

            Debug.Log($"ultMove configured: center={screenCenter}, shape={shape.scale}, particles={maxParticlesToEmit}");
        }

        // Wait for particles to appear (visible across screen)
        yield return new WaitForSeconds(0.1f);

        // Delay trước khi nổ (0.75s)
        yield return new WaitForSeconds(ultDelay);

        // Camera shake khi nổ
        if (cameraShakeEnabled && shakeCam != null)
        {
            StartCoroutine(CameraShake());
        }

        // Tạo explosions tại vị trí particles (số lượng = số hạt ultMove)
        List<GameObject> activeExplosions = new List<GameObject>();
        if (ultPS != null)
        {
            int numParticles = ultPS.particleCount;
            Debug.Log($"Particles alive for explosions: {numParticles}");

            if (numParticles > 0)
            {
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[numParticles];
                int actualParticles = ultPS.GetParticles(particles);

                // Batch spawn để tránh lag (10/frame)
                int batchSize = 10;
                for (int i = 0; i < actualParticles; i += batchSize)
                {
                    int endIndex = Mathf.Min(i + batchSize, actualParticles);
                    for (int j = i; j < endIndex; j++)
                    {
                        Vector3 particlePos = particles[j].position;
                        GameObject exp = GetPooledExplosion(particlePos, Quaternion.identity);
                        SetParticleSortingOrder(exp, particleSortingOrder + 1);
                        activeExplosions.Add(exp);
                    }
                    if (endIndex < actualParticles) yield return null; // Wait 1 frame
                }
            }
            else
            {
                // Fallback: Random trong screen bounds
                Vector3 camPos = cam.transform.position;
                Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
                Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));

                int fallbackCount = maxParticlesToEmit;
                int batchSize = 10;
                for (int i = 0; i < fallbackCount; i += batchSize)
                {
                    int endIndex = Mathf.Min(i + batchSize, fallbackCount);
                    for (int j = i; j < endIndex; j++)
                    {
                        Vector3 randomPos = new Vector3(
                            Random.Range(bottomLeft.x, topRight.x),
                            Random.Range(bottomLeft.y, topRight.y),
                            0
                        );
                        GameObject exp = GetPooledExplosion(randomPos, Quaternion.identity);
                        SetParticleSortingOrder(exp, particleSortingOrder + 1);
                        activeExplosions.Add(exp);
                    }
                    if (endIndex < fallbackCount) yield return null;
                }
            }
        }

        Debug.Log($"Explosions created: {activeExplosions.Count}");

        // Destroy ultMove instance
        if (ultInstance != null) Destroy(ultInstance);

        // Recycle explosions sau 1.5s
        yield return new WaitForSeconds(1.5f);
        foreach (GameObject exp in activeExplosions)
        {
            if (exp != null) ReturnToPool(exp);
        }
        activeExplosions.Clear();
    }

    private IEnumerator CameraShake()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float strength = shakeCurve.Evaluate(elapsed / shakeDuration) * shakeMagnitude;
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;
            shakeCam.transform.localPosition = originalCamPos + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        shakeCam.transform.localPosition = originalCamPos;
    }

    private void SetParticleSortingOrder(GameObject obj, int order)
    {
        ParticleSystemRenderer[] renderers = obj.GetComponentsInChildren<ParticleSystemRenderer>(true);
        foreach (ParticleSystemRenderer r in renderers)
        {
            r.sortingOrder = order;
            r.enabled = true;
        }
        ParticleSystem[] ps = obj.GetComponentsInChildren<ParticleSystem>(true);
        foreach (ParticleSystem p in ps)
        {
            var main = p.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            p.Clear(true);
            p.Play(true);
        }
    }
}