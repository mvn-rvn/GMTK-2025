using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Handles hitting the player
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<P_Health>().Damage(1f);
        }
    }
}
