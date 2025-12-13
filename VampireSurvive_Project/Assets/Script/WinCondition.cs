using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    public TimeManagement time;
    public GameObject winItem;
    public WinManager winManager;

    void Start()
    {
        time = GameObject.Find("TimeText").GetComponent<TimeManagement>();
        winItem.SetActive(false);

    }
    public void Level1Win()
    {
        winItem.SetActive(true);
    }
}
