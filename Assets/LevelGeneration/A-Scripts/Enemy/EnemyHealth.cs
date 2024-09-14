using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] public int maxHealth;
    [SerializeField] public int currentHealth;
    [SerializeField] private GameObject deathVFXPrefab;
    [SerializeField] private float knockBackThrust = 15f;

    private Knockback knockback;
    private Flash flash;

    [SerializeField] StatusBar hpBar;

    public AudioClip hitSound1;
    public AudioClip hitSound2;

    private void Awake()
    {
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHpBar();
    }

    public void UpdateHpBar()
    {
        hpBar.Set(currentHealth, maxHealth);
        SoundManager.instance.RandomizeSfx2(hitSound1, hitSound2);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHpBar();
        knockback.GetKnockedBack(PlayerController.instance.transform, knockBackThrust);
        StartCoroutine(flash.FlashRoutine());
        StartCoroutine(CheckDetectDeathRoutine());
    }

    public void TakeDamageNoKnockBack(int damage)
    {
        currentHealth -= damage;
        UpdateHpBar();
        StartCoroutine(flash.FlashRoutine());
        StartCoroutine(CheckDetectDeathRoutine());
    }

    private IEnumerator CheckDetectDeathRoutine()
    {
        yield return new WaitForSeconds(flash.GetRestoreMatTime());
        DetectDeath();
    }

    public void DetectDeath()
    {
        if (currentHealth <= 0)
        {

            if (GetComponent<PickupSpawner>())
            {
                GetComponent<PickupSpawner>().DropItems();
            }

            Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
