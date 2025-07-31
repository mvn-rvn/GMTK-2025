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
        transform.localScale = new Vector2(scale_original.x * p_movement.direction, scale_original.y);
        transform.localPosition = new Vector2(position_original.x * p_movement.direction, position_original.y);

        if (p_movement.input_horizontal != 0f)
        {
            animator.SetBool("Standing", false);
            animator.SetBool("Running", true);
        }
        else
        {
            animator.SetBool("Standing", true);
            animator.SetBool("Running", false);
        }
    }
}
