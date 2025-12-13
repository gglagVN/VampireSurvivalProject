using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;


public class PlayerLevelSystem : MonoBehaviour
{
    [Header("Menu")]
    public LevelUpMenu levelUpMenu;
    [Header("Level Settings")]
    public int level = 1;
    public float currentXP = 0;
    public float requiredXP = 100;

    [Header("UI")]
    public Slider xpSlider;
    public TextMeshProUGUI levelText;

    void Start()
    {
        UpdateXPUI();
    }

    public void GainXP(float amount)
    {
        currentXP += amount;
        if (currentXP >= requiredXP)
        {
            LevelUp();
        }
        UpdateXPUI();
    }

    void LevelUp()
    {
        level++;
        currentXP -= requiredXP;
        requiredXP *= 1.25f;
        UpdateXPUI();
        levelUpMenu.OpenMenu();
    }

    void UpdateXPUI()
    {
        if (xpSlider != null)
        {
            xpSlider.maxValue = requiredXP;
            xpSlider.value = currentXP;
        }

        if (levelText != null)
        {
            levelText.text = "Lv " + level.ToString();
        }
    }
}
