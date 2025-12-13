using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;


public class Gun : MonoBehaviour
{
    private float rotateOffset = 180f;
    [Header("Gun Settings")]
    [SerializeField] public Transform firePos;
    [SerializeField] public GameObject bulletPrefabs;
    [SerializeField] public GameObject explosionImpactFX;
    [SerializeField] private float shotDelay = 0.09f;
    private float nextShot;
    public int maxAmmo = 40;
    public int currentAmmo;

    [Header("Reload Settings")]
    [SerializeField] private float reloadDelay = 1f;
    [SerializeField] private GameObject reloadPrefabs;
    private bool isReloading = false;
    private bool isAutoFiring = false;
    [SerializeField] private TMP_Text ammoText;
    public AudioSource VFXSoundClick;
    public AudioClip musicClipSoundClick; 
    void Awake()
    {
        currentAmmo = maxAmmo;
        VFXSoundClick.clip = musicClipSoundClick;
    }

    void Update()
    {
        RotateGun();
        Shoot();
        Reload();
        ammoText.text = $"{currentAmmo} / {maxAmmo}";

    }

    void RotateGun()
    {
        if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
            return;

        Vector3 displacement = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + rotateOffset);

        // Flip hướng súng theo chuột
        if (angle < -90 || angle > 90)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(1, -1, 1);
    }

    // Bắn đạn thường
    void Shoot()
    {
        if (isReloading)
        {
            if (reloadPrefabs != null) reloadPrefabs.SetActive(true);
            return;
        }

        

        if (Input.GetMouseButtonDown(0) && currentAmmo > 0 && Time.time > nextShot)
        {
            nextShot = Time.time + shotDelay;
            VFXSoundClick.Play();
            Instantiate(bulletPrefabs, firePos.position, firePos.rotation);
            currentAmmo--;

            if (currentAmmo <= 0)
                StartCoroutine(ReloadCoroutine());
        }
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(0) && !isReloading)
        {
            if (!isAutoFiring)
            {
                isAutoFiring = true;
                StartCoroutine(AutoFireCoroutine());
            }
        }
        else
        {
            isAutoFiring = false;
        }
    }

    IEnumerator AutoFireCoroutine()
    {
        while (isAutoFiring)
        {
            if (currentAmmo > 0 && Time.time > nextShot)
            {
                nextShot = Time.time + shotDelay;
                
                Instantiate(bulletPrefabs, firePos.position, firePos.rotation);
                VFXSoundClick.Play();
                currentAmmo--;

                if (currentAmmo <= 0)
                {
                    StartCoroutine(ReloadCoroutine());
                    isAutoFiring = false;
                    yield break;
                }
            }
            yield return null;
        }
    }

    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
            StartCoroutine(ReloadCoroutine());
    }

    IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        if (reloadPrefabs != null)
            reloadPrefabs.SetActive(true);

        yield return new WaitForSeconds(reloadDelay);

        currentAmmo = maxAmmo;
        isReloading = false;

        if (reloadPrefabs != null)
            reloadPrefabs.SetActive(false);

        Debug.Log("Reloaded!");
    }
}
