using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
    [Header("Boss để theo dõi")]
    public EnemyFollow boss;  // sẽ được gán khi Boss spawn
    public Image hpBar;
    void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<EnemyFollow>();
    }

    void Update()
    {
        if (boss == null || hpBar == null) return;

        // cập nhật fillAmount
        hpBar.fillAmount = boss.currentEnemyHP / (float)boss.enemyMaxHP;
        // ẩn thanh máu nếu Boss đã chết
        if (boss.currentEnemyHP <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}