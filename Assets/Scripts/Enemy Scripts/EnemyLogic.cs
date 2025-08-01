using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyLogic : MonoBehaviour
{
    [SerializeField] private float knockbackMultiplier;
    [SerializeField] private float drag;

    private Rigidbody2D rb;
    private EnemyHealth health;

    [SerializeField] private ProjectileHandler projectile;
    [SerializeField] private float spotDistance;
    [SerializeField] private float activationDistance;
    [SerializeField] private float attackCooldown = 5f;

    private GameObject player;

    private enum State
    {
        Inactive,
        Idle,
        Alerted
    }

    private State awareness;
    private bool tracking;
    private Vector3 distance;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        health = gameObject.GetComponent<EnemyHealth>();
        player = FindAnyObjectByType<P_Movement>().gameObject;
    }

    private void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude > 0)
        {
            float velMod = 1 - drag;
            rb.linearVelocity *= velMod;
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

    private void Update()
    {
        distance = player.transform.position - gameObject.transform.position;

        if (Vector3.Magnitude(distance) > activationDistance)
        {
            awareness = State.Idle;
            return;
        }
        else if (Vector3.Magnitude(distance) < spotDistance) awareness = State.Alerted;
        else awareness = State.Inactive;

        if (awareness == State.Alerted && !tracking) StartCoroutine(ShootLoop());
    }

    private IEnumerator ShootLoop()
    {
        tracking = true;
        while (awareness == State.Alerted)
        {
            if (!ObstructionCheck())
            {
                ProjectileHandler proj = Instantiate(projectile, transform.position, transform.rotation);
                proj.SetDirection(Vector3.Normalize(distance));
            }
            yield return new WaitForSeconds(attackCooldown);
        }
        tracking = false;
    }

    private bool ObstructionCheck()
    {
        RaycastHit2D castHit = Physics2D.Raycast(transform.position, Vector3.Normalize(distance), Vector3.Magnitude(distance), LayerMask.GetMask("Walls", "Cam Bounds"));
        if(castHit)
        {
            return true;
        }
        return false;
    }
}
