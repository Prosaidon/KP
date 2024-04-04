using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEnemy : MonoBehaviour
{
    // Start is called before the first frame update
 [Header("Health")]
    public float startingHealthForEnemy = 3f; // Menetapkan nilai awal kesehatan untuk musuh
    public float currentHealth { get; private set; }
    private Animator anim;
    private bool dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;

    [Header("Component")]
    [SerializeField] private Behaviour[] components;
    private bool invulnerable;

    [Header("Death Sound")]
    [SerializeField] private AudioClip deathSound;

    private void Awake()
    {
        if (!gameObject.CompareTag("Enemy"))
        {
            enabled = false; // Menonaktifkan skrip jika bukan objek musuh
            return;
        }

        currentHealth = startingHealthForEnemy; // Set kesehatan awal untuk musuh
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    // Metode TakeDamage dan fungsi lainnya tetap sama

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
