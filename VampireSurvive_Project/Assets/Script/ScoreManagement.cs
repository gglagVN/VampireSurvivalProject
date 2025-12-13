using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreManagement : MonoBehaviour
{
    public static ScoreManagement Instance; // Tạo 1 Singleton Pattern, dễ dàng sử dụng cho các lớp Enemy
    public TextMeshProUGUI scoreText; // UI hiển thị điểm

    private int score = 0;

    void Awake()
    {
        if (Instance == null) Instance = this; // singleton để gọi dễ dàng
        else Destroy(gameObject);
    }
    void Start()
    {
        UpdateUI();
    }

    public void addScore(int point)
    {
        score += point;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public int GetScore()
    {
        return score;
    }
}
