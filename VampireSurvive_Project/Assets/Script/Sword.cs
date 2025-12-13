using UnityEngine;

public class Sword : MonoBehaviour
{
    private Animator anim;
    private PlayerAttack atk;
    [SerializeField] public SpriteRenderer srSword; // Đổi thành public để PlayerAttack truy cập

    void Awake()
    {
        anim = GetComponent<Animator>();
        srSword = GetComponent<SpriteRenderer>();
        atk = GetComponentInParent<PlayerAttack>();
    }

    void LateUpdate()
    {
        UpdateAnimation();

    }

    private void UpdateAnimation()
    {
        if (atk != null && anim != null)
        {
            bool attacking = atk.isAttacking();
            anim.SetBool("isAttackSword", attacking);
        }
    }


}