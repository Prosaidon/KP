using UnityEngine;

public class EnemyProjectile : EnemyDamage
{
    private float speed = 10;
    [SerializeField] private float resetTime;
    [SerializeField] private LayerMask enemyLayer;
    private float lifetime;
    private Animator anim;
    private BoxCollider2D coll;

    private bool hit;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
    }

    public void ActivateProjectile()
    {
        hit = false;
        lifetime = 0;
        gameObject.SetActive(true);
        coll.enabled = true;
    }
    private void Update()
    {
        if (hit) return;

        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > resetTime)
            gameObject.SetActive(false);
    }

    private new void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyLayer == (enemyLayer | (1 << collision.gameObject.layer)))
        {
            // Collision detected with enemy layer, but we don't want any reaction
            return;
        }
        
        hit = true;
        base.OnTriggerEnter2D(collision); // Execute logic from parent script first
        coll.enabled = false;

        if (anim != null)
            gameObject.SetActive(false); // When this hits any object deactivate arrow
    }
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}