using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraScroll : MonoBehaviour
{
    public float scrollSpeed = 15f;            
    public float fastScrollMultiplier = 2f;

    public float endY = -25f;                  
    public string menuSceneName = "Menu";

    void Start()
    {
        Time.timeScale = 1f;

        Vector3 pos = transform.position;
        pos.y = 20f;
        transform.position = pos;
    }

    void Update()
    {
        float currentSpeed = scrollSpeed;

        if (Input.GetMouseButton(0) || Input.touchCount > 0)
            currentSpeed *= fastScrollMultiplier;

        transform.Translate(Vector3.down * currentSpeed * Time.deltaTime);

        if (transform.position.y < endY)
            SceneManager.LoadScene(menuSceneName);
    }
}