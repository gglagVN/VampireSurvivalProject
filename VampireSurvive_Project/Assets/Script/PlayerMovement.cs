using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    // Movement Setting
    private PlayerStats playerStat;
    public Slider sliderHP;
    public Slider sliderSta;
    public int maxHP = 100;
    public int currentHP;
    public float moveSpeed = 5f;

    // Dash Setting
    public float activeMoveSpeed;
    public float dashSpeed = 15f;
    public float dashLength = 0.3f;
    public float dashCooldown = 0.2f;
    protected float dashCounter;
    public bool isImmune = false;
    protected float dashCooldownCounter;
    private Color originalColor;
    private SpriteRenderer sr;

    // Stamina Settings
    public float maxSta = 100f;
    public float currentSta;
    public int dashCost = 20;
    public float regenSta = 10f;

    // Game Over
    public static bool isGameOver = false;
    private bool isDead = false;   // 👈 Chặn điều khiển sau khi chết
    public Lose loseScript;       // 👈 Tham chiếu đến Lose.cs

    protected Rigidbody2D rb;
    protected Vector2 moveInput;
    private Animator anim;
    private RectTransform rtHP;
    private RectTransform rtST;
    private WinManager wm;

    protected virtual void Start()
    {
        isGameOver = false;
        isDead = false;
        wm = GameObject.Find("WinManager").GetComponent<WinManager>();
        rtHP = sliderHP.GetComponent<RectTransform>();
        rtST = sliderSta.GetComponent<RectTransform>();
        playerStat = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        loseScript = GameObject.Find("WinManager").GetComponent<Lose>();
        originalColor = sr.color;
        currentHP = maxHP;
        currentSta = maxSta;

        sliderHP = GameObject.Find("HPSlider")?.GetComponent<Slider>();
        sliderSta = GameObject.Find("StaSlider")?.GetComponent<Slider>();

        UpdateUI();
        activeMoveSpeed = moveSpeed;
    }

    protected virtual void Update()
    {
        if (isDead)
        {
            return;
        }

        UpdateAnimation();

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        if (moveInput.x < 0)
            sr.flipX = false;
        else if (moveInput.x > 0)
            sr.flipX = true;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(1))
        {
            if (dashCooldownCounter <= 0 && dashCounter <= 0 && currentSta >= dashCost)
            {
                activeMoveSpeed = dashSpeed;
                dashCounter = dashLength;
                currentSta -= dashCost;
            }
        }

        if (dashCounter > 0)
        {
            dashCounter -= Time.deltaTime;
            isImmune = true;

            if (dashCounter <= 0)
            {
                isImmune = false;
                activeMoveSpeed = playerStat.baseMoveSpeed;
                dashCooldownCounter = dashCooldown;
            }
        }

        if (dashCooldownCounter > 0)
            dashCooldownCounter -= Time.deltaTime;

        if (currentSta < playerStat.baseMaxSta)
        {
            currentSta += regenSta * Time.deltaTime;
            if (currentSta > playerStat.baseMaxSta) currentSta = playerStat.baseMaxSta;
        }

        UpdateStaminaUI();
    }


    public void UpdateUI()
    {
        UpdateHPUI();
        UpdateStaminaUI();
    }

    public void UpdateHPUI()
    {
        if (sliderHP != null)
        {
            rtHP.sizeDelta = new Vector2(playerStat.baseMaxHP * 2, rtHP.sizeDelta.y);
            sliderHP.maxValue = playerStat.baseMaxHP;
            sliderHP.value = currentHP;
        }
    }

    public void UpdateStaminaUI()
    {
        if (sliderSta != null)
        {
            rtST.sizeDelta = new Vector2(playerStat.baseMaxSta * 2, rtST.sizeDelta.y);
            sliderSta.maxValue = playerStat.baseMaxSta;
            sliderSta.value = currentSta;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (isDead)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        rb.velocity = moveInput * activeMoveSpeed;
    }
    public void TakeDamage()
    {
        currentHP -= 10;
        StartCoroutine(FlashHit());
        isImmune = true;
        StartCoroutine(ResetImmune(0.3f));
        UpdateHPUI();
        if (currentHP <= 0)
        {
            GetComponent<Collider2D>().enabled = false;
            HandleDeath();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Enemy") && !isImmune)
        {
            TakeDamage();
        }
        if (collision.gameObject.CompareTag("ItemWin"))

        {
            wm.ShowWin();
        }
    }

    private void HandleDeath()
    {
        if (isDead) return;

        Debug.Log("Game Over!");
        isDead = true;
        isGameOver = true;
        rb.velocity = Vector2.zero;
        anim.SetTrigger("isDead");
        StartCoroutine(ShowLoseUI());
    }

    private IEnumerator ShowLoseUI()
    {
        yield return new WaitForSecondsRealtime(2f);
        loseScript.ShowLoseScreen();
        Time.timeScale = 0;
    }

    private IEnumerator ResetImmune(float delay)
    {
        yield return new WaitForSeconds(delay);
        isImmune = false;
    }

    IEnumerator FlashHit()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sr.color = originalColor;
    }

    private void UpdateAnimation()
    {
        bool isMoveMan = Mathf.Abs(rb.velocity.x) > 0.1f || Mathf.Abs(rb.velocity.y) > 0.1f;
        anim.SetBool("isMoveMan", isMoveMan);
    }
}
