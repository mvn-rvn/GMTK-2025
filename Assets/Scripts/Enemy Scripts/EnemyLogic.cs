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
    [SerializeField] private bool manuallyActivated;
    [SerializeField] private float attackCooldown = 5f;

    private GameObject player;

    private enum State
    {
        Inactive, // not calculating logic
        Idle, // calculating logic but no player in range
        Alerted // player in range
    }

    private State awareness;
    private bool tracking;
    private Vector3 distance;

    private float storedY;
    [SerializeField] private float variationY;
    private float lerpY = 0f;
    private float lerpDir = 1;
    [SerializeField] private float lerpSpeed = 0.05f;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        health = gameObject.GetComponent<EnemyHealth>();
        player = FindAnyObjectByType<P_Movement>().gameObject;
        storedY = gameObject.transform.position.y;
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

        if (Vector3.Magnitude(distance) > activationDistance || (manuallyActivated && awareness == State.Inactive))
        {
            awareness = State.Inactive;
            return;
        }
        else if (Vector3.Magnitude(distance) < spotDistance) awareness = State.Alerted;
        else awareness = State.Idle;

        if (awareness == State.Alerted && !tracking) StartCoroutine(ShootLoop());


        if (lerpY == 1) lerpDir = -1f;
        else if (lerpY == 0) lerpDir = 1f;
        lerpY = Mathf.Clamp(lerpY + Time.deltaTime * lerpSpeed * lerpDir, 0, 1);
        gameObject.transform.position = new Vector2(gameObject.transform.position.x, Mathf.SmoothStep(storedY - variationY, storedY + variationY, lerpY));
    }

    private IEnumerator ShootLoop()
    {
        tracking = true;
        while (awareness == State.Alerted)
        {
            if (!ObstructionCheck())
            {
                Debug.Log("shooting");
                ProjectileHandler proj = Instantiate(projectile, transform.position, transform.rotation);
                proj.SetDirection(Vector3.Normalize(distance));
                Debug.Log(proj);
            }
            yield return new WaitForSeconds(attackCooldown);
        }
        tracking = false;
    }

    private bool ObstructionCheck()
    {
        RaycastHit2D castHit = Physics2D.Raycast(transform.position, Vector3.Normalize(distance), Vector3.Magnitude(distance), LayerMask.GetMask("Walls"));
        if(castHit)
        {
            return true;
        }
        return false;
    }

    public void ActivateEnemy()
    {
        awareness = State.Idle;
    }

    public void DeactivateEnemy()
    {
        awareness = State.Inactive;
    }
}
