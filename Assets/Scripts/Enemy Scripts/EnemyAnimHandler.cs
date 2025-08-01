using UnityEngine;

public class EnemyAnimHandler : MonoBehaviour
{
    private EnemyLogic logic;

    private Animator animator;

    Vector2 scale_original;

    void Awake()
    {
        logic = gameObject.GetComponent<EnemyLogic>();

        animator = gameObject.GetComponent<Animator>();

        scale_original = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector2(scale_original.x * -logic.DirectionFacing(), scale_original.y);

        if (logic.isWalking)
        {
            animator.SetBool("Standing", false);
            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Standing", true);
            animator.SetBool("Walking", false);
        }
    }
}
