using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class AmmoTextManager : MonoBehaviour
{
    private Gun gun;
    [SerializeField] private TMP_Text ammoText;
    void Start()
    {
        int index = PlayerPrefs.GetInt("CharacterIndex", 0);
        if (index == 0)
        {
            ammoText.GameObject().SetActive(false);
        }

    }
}
