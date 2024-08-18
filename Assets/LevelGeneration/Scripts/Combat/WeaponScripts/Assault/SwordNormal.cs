using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwordNormal : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private Transform slashAnimSpawnPoint;
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private int staminaCost;

    [SerializeField] private int damageAmount;

    [SerializeField] private Transform weaponCollider;
    private Animator animator;
    private Charecter charecter;

    private GameObject slashAnim;

    public AudioClip slashSound1;
    public AudioClip slashSound2;

    enum AbilityState
    {
        ready,
        active,
        charge
    }
    AbilityState state = AbilityState.charge;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        charecter = FindObjectOfType<Charecter>();
    }

    private void Start()
    {
        weaponCollider.gameObject.GetComponent<SwordDamageSouce>().damageAmount = this.damageAmount;
        slashAnimSpawnPoint = GameObject.Find("SlashSpawnPoint").transform;
    }

    private void OnDestroy()
    {
        animator.ResetTrigger("SkillThrow");
        animator.ResetTrigger("CancelSkill");
        weaponCollider.gameObject.GetComponent<SwordDamageSouce>().chargeAmount = 0;
    }

    private void Update()
    {
        MouseFollowWithOffset();
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }

    public void Attack()
    {
        if (charecter.stamina.currVal >= staminaCost && !PlayerController.instance.isMenuActive)
        {
            animator.SetTrigger("Attack");
            SoundManager.instance.RandomizeSfx(slashSound1, slashSound2);
            weaponCollider.gameObject.SetActive(true);
            slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
            slashAnim.transform.parent = this.transform.parent;
            weaponCollider.gameObject.GetComponent<DamageSouce>();
            StartCoroutine(ReduceStaminaRoutine());
        }
    }

    private IEnumerator ReduceStaminaRoutine()
    {
        charecter.ReduceStamina(staminaCost);
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator SkillRoutine()
    {
        yield return new WaitForSeconds(5);
    }

    public void DoneAttackingAnimEvent()
    {
        weaponCollider.gameObject.SetActive(false);
    }

    public void SwingUpFlipAnimEvent()
    {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(-180, 0, 0);

        if (PlayerController.instance.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void SwingDownFlipAnimEvent()
    {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (PlayerController.instance.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    private void MouseFollowWithOffset()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(PlayerController.instance.transform.position);

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        if (mousePos.x < playerScreenPoint.x)
        {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, -180, angle);
        }
        else
        {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
