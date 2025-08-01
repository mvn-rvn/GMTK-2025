using UnityEngine;

public class P_Health : MonoBehaviour
{
    public float health = 3f;

    public void Damage(float val)
    {
        health -= val;

        if (health <= 0) GameOver();
    }

    private void GameOver()
    {
        Debug.Log("Game Over");
        Destroy(gameObject);
    }
}
