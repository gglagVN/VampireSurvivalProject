using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnBoss : MonoBehaviour
{

    public AudioSource VFXSoundBoss;

    public AudioClip VFXClipBoss;

    [Header("Boss Prefab")]
    public GameObject bossPrefab;

    [Header("Vị trí xuất hiện Boss")]
    public Transform spawnPoint;

    [Header("Thời gian triệu hồi (giây)")]
    public float summonTime = 30f;

    private bool bossSpawned = false;

    void Start()
    {
        VFXSoundBoss.clip = VFXClipBoss;
        Invoke("SummonBoss", summonTime);
    }

    void SummonBoss()
    {
        if (bossSpawned) return;

        Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
        VFXSoundBoss.Play();
        if (SceneManager.GetActiveScene().name == "Level3")
        {
            GameObject.Find("SPMiniEnemy").SetActive(false);
            GameObject.Find("SPHeavyEnemy").SetActive(false);
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                Destroy(go);
            }
        }
        bossSpawned = true;
        Debug.Log("Boss đã xuất hiện!");
    }
}