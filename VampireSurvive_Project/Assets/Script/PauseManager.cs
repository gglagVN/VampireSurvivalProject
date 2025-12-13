using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    private WinManager winManager;
    private bool isPaused;

    void Awake()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pausePanel.SetActive(false);
        winManager = GameObject.Find("WinManager").GetComponent<WinManager>();
    }

    void Update()
    {
        // ✅ Nếu game đã over thì không cho pause
        if (PlayerMovement.isGameOver)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (PlayerMovement.isGameOver || winManager.gameEnded == true) return;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
}
