using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    public GameObject[] panelChoose;
    public GameObject panelSetting;
    void Start()
    {
        if (panelSetting != null)
            panelSetting.SetActive(false);
        if (panelChoose == null || panelChoose.Length == 0)
            return;
        foreach (GameObject panel in panelChoose)
        {
            panel.SetActive(false);
        }
    }
    public void ChoosePlayer(int index)
    {
        PlayerPrefs.SetInt("CharacterIndex", index);
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void EnablePanelChoosePlayer(int i)
    {
        panelChoose[i].SetActive(true);
    }
    public void DisablePanelChoosePlayer(int i)
    {
        panelChoose[i].SetActive(false);
    }
    public void PlayNewGame()
    {
        PlayerPrefs.DeleteKey("Level2");
        PlayerPrefs.DeleteKey("Level3");
        PlayerPrefs.Save();
        LoadScene("LevelSelect");
    }
    public void OpenSetting()
    {
        panelSetting.SetActive(true);
    }
    public void OffSetting()
    {
        panelSetting.SetActive(false);
    }
    public void NextLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        int currentLevel = int.Parse(sceneName.Replace("Level", ""));
        int nextLevel = currentLevel + 1;
        PlayerPrefs.SetInt("Level" + nextLevel, 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Level" + nextLevel);
    }
}