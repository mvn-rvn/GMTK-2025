using UnityEngine;

public class P_Health : MonoBehaviour
{
    public float health = 3f;

    public void Damage(float val)
    {
        health -= val;

        if (health <= 0) GameOver();
    }

    public void Heal(float val)
    {
        health = Mathf.Clamp(health + val, 0, 3);
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        Destroy(gameObject);
    }
}
