using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    [SerializeField] private float knockbackMultiplier;
    [SerializeField] private float drag;

    private Rigidbody2D rb;
    private EnemyHealth health;
    

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        health = gameObject.GetComponent<EnemyHealth>();
    }

    private void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude > 0)
        {
            float velMod = 1 - drag;
            rb.linearVelocity *= new Vector2(velMod, velMod);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Handles getting hit by the player
        P_AttackHandler playerAtk = collision.GetComponentInParent<P_AttackHandler>();
        float knockback = Vector3.Normalize(gameObject.transform.position - playerAtk.gameObject.transform.position).x * knockbackMultiplier;
        rb.linearVelocity = Vector3.right * knockback;

        health.Damage(playerAtk.GetDamage());
    }
}
