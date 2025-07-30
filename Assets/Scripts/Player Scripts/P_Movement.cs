using UnityEngine;

public class P_Movement : MonoBehaviour
{

    Rigidbody2D rb;
    BoxCollider2D bounding_box;
    CapsuleCollider2D hitbox;

    float input_horizontal;
    public float move_speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        bounding_box = gameObject.GetComponent<BoxCollider2D>();
        hitbox = gameObject.GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        input_horizontal = Input.GetAxis("Horizontal");

    }

    void FixedUpdate()
    {
        float move_velocity = input_horizontal * move_speed * Time.fixedDeltaTime;
        rb.linearVelocityX = move_velocity;
    }
}
