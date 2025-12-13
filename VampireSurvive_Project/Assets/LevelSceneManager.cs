using UnityEngine;
using UnityEngine.UI;

public class LevelSceneManager : MonoBehaviour
{
    public Button[] buttons;
    public GameObject[] lockIcons;

    void Start()
    {
        PlayerPrefs.SetInt("Level1", 1);

        for (int i = 0; i < buttons.Length; i++)
        {
            int unlocked = PlayerPrefs.GetInt("Level" + (i + 1), 0);

            if (unlocked == 1)
            {
                buttons[i].interactable = true;
                lockIcons[i].SetActive(false);
            }
            else
            {
                buttons[i].interactable = false;
                lockIcons[i].SetActive(true);
            }
        }
    }
}
