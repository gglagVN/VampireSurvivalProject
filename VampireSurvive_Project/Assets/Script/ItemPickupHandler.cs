using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ItemPickupHandler : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 1f;
    public float massPickupRadius = 1.5f;
    private PlayerStats player;

    [Header("Visual Effects")]
    public GameObject healEffectPrefab;
    public GameObject manaEffectPrefab;

    private HashSet<GameObject> pickedUpThisFrame = new HashSet<GameObject>();
    private PlayerMovement playerMovement;
    private PlayerLevelSystem playerLevel;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        player = playerMovement.GetComponent<PlayerStats>();
        playerLevel = playerMovement.GetComponent<PlayerLevelSystem>();
    }
    void Update()
    {
        pickedUpThisFrame.Clear();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("HealPotion"))
        {
            HealPotion healPotion = collision.GetComponent<HealPotion>();
            if (healPotion != null)
            {
                player.Heal(healPotion.healAmount);
                collision.gameObject.SetActive(false); // Dùng disable thay vì Destroy
                playerMovement?.UpdateHPUI();
            }
        }
        else if (collision.CompareTag("ManaPotion"))
        {
            ManaPotion manaPotion = collision.GetComponent<ManaPotion>();
            if (manaPotion != null)
            {
                player.RestoreStamina(manaPotion.manaAmount);
                collision.gameObject.SetActive(false);
                playerMovement?.UpdateStaminaUI();
            }
        }
        else if (collision.CompareTag("ExpItem"))
        {
            ExpItem expItem = collision.GetComponent<ExpItem>();
            if (expItem != null)
            {
                playerLevel.GainXP(expItem.expAmount);
                collision.gameObject.SetActive(false);
                playerMovement?.UpdateStaminaUI();
            }
        }
    }




    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, massPickupRadius);
    }
}