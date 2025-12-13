using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    private PlayerMovement player;

    [Header("Base Stats")]
    public int baseMaxHP;
    public float baseMoveSpeed;
    public float baseMaxSta;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
        baseMaxHP = player.maxHP;
        baseMaxSta = player.maxSta;
        baseMoveSpeed = player.moveSpeed;

    }

    public void IncreaseHP(int amount)
    {
        baseMaxHP += amount;
        if (player.currentHP < baseMaxHP) player.currentHP = baseMaxHP;
        Debug.Log(player.currentHP);
    }

    public void IncreaseSpeed(float amount)
    {
        baseMoveSpeed += amount;
    }

    public void IncreaseStamina(float amount)
    {
        baseMaxSta += amount;
    }

    public void Heal(int amount)
    {
        player.currentHP = Mathf.Min(player.currentHP + amount, baseMaxHP);
        Debug.Log(player.currentHP);
    }

    public void RestoreStamina(float amount)
    {
        player.currentSta = Mathf.Min(player.currentSta + amount, baseMaxSta);
    }
}