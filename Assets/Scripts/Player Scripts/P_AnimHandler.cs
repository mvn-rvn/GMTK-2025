using UnityEngine;

public class P_AnimHandler : MonoBehaviour
{

    P_Movement p_movement;

    Animator animator;

    Vector2 scale_original;
    Vector2 position_original;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        p_movement = transform.parent.gameObject.GetComponent<P_Movement>();

        animator = GameObject.Find("Player Animations").GetComponent<Animator>();

        scale_original = transform.localScale;
        position_original = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Player Attack Manager").GetComponent<P_AttackHandler>().flipping)
        {
            transform.localScale = new Vector2(scale_original.x * p_movement.direction, scale_original.y);
            transform.localPosition = new Vector2(position_original.x * p_movement.direction, position_original.y);
        }

        if (p_movement.input_horizontal != 0)
            {
                animator.SetBool("Standing", false);
                animator.SetBool("Running", true);
            }
            else
            {
                animator.SetBool("Standing", true);
                animator.SetBool("Running", false);
            }

        animator.SetBool("Grounded", p_movement.grounded);

        if (p_movement.jumping && !GameObject.Find("Player Attack Manager").GetComponent<P_AttackHandler>().attacking)
        {
            if (p_movement.double_jumped)
            {
                animator.Play("JumpPrototype", -1, 0f);
            }
            animator.Play("JumpPrototype");
        }
    }
}
