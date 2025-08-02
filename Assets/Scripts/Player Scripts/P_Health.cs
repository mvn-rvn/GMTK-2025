using UnityEngine;
using System.Collections;

public class P_Health : MonoBehaviour
{
    public float health = 3f;

    private bool invincible = false;

    [SerializeField]
    private float invincibilityTime = 2f;

    public void Damage(float val)
    {
        if (!invincible)
        {
            health -= val;
            StartCoroutine(InvincibilityPeriod());
        }

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

    IEnumerator InvincibilityPeriod()
    {
        SpriteRenderer renderer = GameObject.Find("Player Animations").GetComponent<SpriteRenderer>();
        invincible = true;
        float elapsedTime = 0f;
        while (elapsedTime < invincibilityTime)
        {
            if (renderer.color == new Color(1f, 1f, 1f, 1f))
            {
                renderer.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            }
            else
            {
                renderer.color = new Color(1f, 1f, 1f, 1f);
            }
            yield return new WaitForSeconds(0.2f);
            elapsedTime += 0.2f;
        }
        renderer.color = new Color(1f, 1f, 1f, 1f);
        invincible = false;
    }
}
