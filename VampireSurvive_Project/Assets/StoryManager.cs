using UnityEngine;
using TMPro;
using System.Collections;

public class StoryManager : MonoBehaviour
{
    public CanvasGroup cg;
    public TMP_Text storyText;

    [TextArea(4, 10)]
    public string[] storyLines;

    public float fadeSpeed = 1f;
    int index = 0;
    bool isTransitioning = false; // Ngăn spam

    void Start()
    {
        cg.alpha = 0;
        StartCoroutine(FadeChangeText(storyLines[index]));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isTransitioning)
        {
            NextStory();
        }
    }

    void NextStory()
    {
        index++;
        if (index < storyLines.Length)
        {
            StartCoroutine(FadeChangeText(storyLines[index]));
        }
        else
        {
            Debug.Log("Story done.");
            // Bạn có thể load scene
            // SceneManager.LoadScene("GameScene");
        }
    }

    IEnumerator FadeChangeText(string newText)
    {
        isTransitioning = true;

        // Fade Out
        while (cg.alpha > 0)
        {
            cg.alpha -= fadeSpeed * Time.deltaTime;
            yield return null;
        }

        storyText.text = newText;

        // Fade In
        while (cg.alpha < 1)
        {
            cg.alpha += fadeSpeed * Time.deltaTime;
            yield return null;
        }

        isTransitioning = false;
    }
}
