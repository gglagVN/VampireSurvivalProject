using UnityEngine;
using UnityEngine.SceneManagement;

public class Lose : MonoBehaviour
{
    [Header("Panel hiển thị khi chết")]
    public GameObject losePanel;

    private void Start()
    {
        losePanel.SetActive(false);
    }

    public void ShowLoseScreen()
    {
        losePanel.SetActive(true);
        Time.timeScale = 0;
    }
}
