using System.Collections;
using TMPro;
using UnityEngine;

public class TimeManagement : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public float countdownTime;
    public float currentTime;

    void Awake()
    {
        currentTime = countdownTime;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime; // trừ dần theo thời gian thực
            if (currentTime < 0) currentTime = 0; // tránh số âm
        }

        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
