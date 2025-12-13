using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class ChoosePlayer : MonoBehaviour
{
    public GameObject[] characters;
    public Transform spawnPoint;
    public CinemachineVirtualCamera vCam;

    void Start()
    {
        int index = PlayerPrefs.GetInt("CharacterIndex", 0);
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            index = 0;
        }
        if (index >= 0 && index < characters.Length)
        {
            GameObject newPlayer = Instantiate(characters[index], spawnPoint.position, Quaternion.identity);
            if (vCam != null)
            {
                vCam.Follow = newPlayer.transform;
            }
        }
    }
}