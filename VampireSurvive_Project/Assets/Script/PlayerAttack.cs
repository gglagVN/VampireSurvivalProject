using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject attackRange;
    private AttackRange atk;
    public float attackDuration = 0.2f;
    private bool coolDown;
    public float timeCoolDown = 1f;


    public AudioSource VFXSoundClick;
    public AudioClip musicClipSoundClick; 

    private bool isAttackingNow; // 🔹 biến theo dõi trạng thái đang tấn công

    void Awake()
    {
        atk = attackRange.GetComponent<AttackRange>();
        coolDown = false;
        isAttackingNow = false;
        VFXSoundClick.clip = musicClipSoundClick;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !coolDown)
        {
            StartCoroutine(DoAttack(atk));

        }
    }

    public bool isAttacking()
    {
        return isAttackingNow;
    }

    IEnumerator DoAttack(AttackRange atk)
    {
        coolDown = true;
        isAttackingNow = true;   // 🔹 bật trạng thái đang tấn công
        atk.ResetHitList();
        atk.canAttack = true;

        yield return new WaitForSeconds(attackDuration);

        VFXSoundClick.Play();

        atk.canAttack = false;
        isAttackingNow = false;  // 🔹 tắt trạng thái tấn công

        yield return new WaitForSeconds(timeCoolDown);
        coolDown = false;
    }
}
