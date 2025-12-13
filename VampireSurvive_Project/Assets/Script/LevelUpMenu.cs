using UnityEngine;

public class LevelUpMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject menuUI;

    private PlayerMovement player;
    public PlayerStats playerStats;

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        playerStats = player.GetComponent<PlayerStats>();
        menuUI.SetActive(true);
    }

    public void OpenMenu()
    {
        menuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseMenu()
    {
        Time.timeScale = 1f;
        menuUI.SetActive(false);
    }

    // Upgrade HP
    public void UpgradeHealth(int amount = 20)
    {
        playerStats.IncreaseHP(amount);
        player.UpdateHPUI();
        CloseMenu();
    }

    // Upgrade SPEED
    public void UpgradeSpeed(float amount = 1f)
    {
        playerStats.IncreaseSpeed(amount);

        // ReUpdate player move speed
        if (player != null)
            player.activeMoveSpeed = playerStats.baseMoveSpeed;

        CloseMenu();
    }

    // Upgrade STAMINA
    public void UpgradeStamina(float amount = 20f)
    {
        playerStats.IncreaseStamina(amount);
        player.UpdateStaminaUI();
        CloseMenu();
    }
}