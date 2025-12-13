using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinManager : MonoBehaviour
{
    public GameObject winPanel;
    private WinCondition wc;
    public float winTime = 300f;

    public bool gameEnded = false;

    void Start()
    {
        wc = GameObject.Find("WinController").GetComponent<WinCondition>();
        if (winPanel != null)
            winPanel.SetActive(false);

        StartCoroutine(ShowWinAfterTime());
    }

    IEnumerator ShowWinAfterTime()
    {
        yield return new WaitForSeconds(winTime);
        wc.Level1Win();
    }

    public void ShowWin()
    {
        if (gameEnded) return;

        gameEnded = true;
        Time.timeScale = 0f;

        if (winPanel != null)
            winPanel.SetActive(true);

        Debug.Log("YOU WIN!");
    }
}
