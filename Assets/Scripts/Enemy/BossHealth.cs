using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; } // Merubah "currentHealt" menjadi "currentHealth"
    private Animator anim;
    private bool dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;
    
    [Header("Component")]
    [SerializeField] private Behaviour[] components;
   
    private bool invulnerable;
    AudioManager audioManager;
    public Healtbar healtBar;
    public RangedBoss rangedBoss;
    public float originalAttackCooldown;

    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        healtBar = FindObjectOfType<Healtbar>();
        spriteRend = GetComponent<SpriteRenderer>();
        rangedBoss = GetComponentInChildren<RangedBoss>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public void TakeDamage(float _damage)
    {
        if (invulnerable) return;
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
            StartCoroutine(Invulnerability());
            healtBar.Bar(currentHealth);
            ReduceCooldownOnHit(); // Panggil fungsi untuk mengurangi cooldown
        
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        if (!dead)
        {
            anim.SetTrigger("die");
            foreach(Behaviour component in components)
                component.enabled = false;
            dead = true;
            audioManager.PlaySFX(audioManager.death);
        }

        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.EnemyKilled();
        }

        gameObject.SetActive(false);
    }

    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }

    public void Respawn()
    {
        AddHealth(startingHealth);
        anim.ResetTrigger("die");
        anim.Play("IdlePlayer"); 
        StartCoroutine(Invulnerability());

        foreach (Behaviour component in components)
            component.enabled = true;
        dead = false;
    }

    private IEnumerator Invulnerability()
    {
        invulnerable = true;
        Physics2D.IgnoreLayerCollision(10, 11, true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(10, 11, false);
        invulnerable = false;
    }
    public void ReduceCooldownOnHit()
    {
        if (!invulnerable && rangedBoss != null && rangedBoss.attackCooldown > 0f) 
        {
            rangedBoss.attackCooldown *= 0.5f; // Reduksi 2x dari cooldown
            StartCoroutine(TemporarilyIncreaseCooldown());
        }
    }
    private IEnumerator TemporarilyIncreaseCooldown()
    {
        yield return new WaitForSeconds(5f);
        rangedBoss.attackCooldown = originalAttackCooldown;
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
