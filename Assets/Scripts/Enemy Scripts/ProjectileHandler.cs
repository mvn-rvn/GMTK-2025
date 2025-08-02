using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    public void SetDirection(Vector3 dir, float angle)
    {
        rb.linearVelocity = dir * speed;
        
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<P_Health>().Damage(1f);
        }
        Destroy(gameObject);
    }
}
