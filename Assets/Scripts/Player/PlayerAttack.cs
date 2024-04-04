using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireballs; // Menggunakan prefab tunggal untuk fireball
    //[SerializeField] private AudioClip fireballSound;
    AudioManager audioManager;
    private Dialog dialog;
    private Animator anim;
    private Player player;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        player = GetComponent<Player>();
        dialog = FindObjectOfType<Dialog>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Update()
    {
        if (!dialog.isActiveAndEnabled)
        {
            if (Input.GetMouseButtonDown(0) && cooldownTimer > attackCooldown && player.canAttack() && Time.timeScale > 0)
            {
                Attack();
            }
        }
        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        if (!dialog.isActiveAndEnabled) // Memastikan dialog tidak aktif sebelum menyerang
        {
            audioManager.PlaySFX(audioManager.bullet);
            anim.SetTrigger("attack");
            cooldownTimer = 0;

            int fireballIndex = FindFireball();
            if (fireballIndex != -1)
            {
                fireballs[fireballIndex].transform.position = firePoint.position;
                fireballs[fireballIndex].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
            }
        }
    }

    private int FindFireball()
    {
        for(int i = 0; i < fireballs.Length; i++)
        {
            if(!fireballs[i].activeInHierarchy)
                return i;
        }
        return 0;
    }
}
