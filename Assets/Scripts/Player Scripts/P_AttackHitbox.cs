using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class P_AttackHitbox : MonoBehaviour
{
    CapsuleCollider2D hitbox;
    Vector2 original_hitbox_offset;

    public float attack_delay = 0.2f;
    public float attack_duration = 0.2f;
    public float attack_recovery = 0.2f;

    [HideInInspector]
    public bool attacking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hitbox = gameObject.GetComponent<CapsuleCollider2D>();
        original_hitbox_offset = hitbox.offset;
    }

    // Update is called once per frame
    void Update()
    {
        hitbox.offset = new Vector2(
            original_hitbox_offset.x * GameObject.Find("Player").GetComponent<P_Movement>().direction,
            original_hitbox_offset.y
        );

        if (!attacking && InputSystem.actions.FindAction("Attack").WasPressedThisFrame())
        {
            StartCoroutine(Attack(attack_delay, attack_duration, attack_recovery));
        }
    }

    IEnumerator Attack(float delay, float duration, float recovery)
    {
        attacking = true;
        float elapsed_time = 0f;
        while (elapsed_time < delay)
        {
            elapsed_time += Time.deltaTime;
            yield return null;
        }
        elapsed_time = 0f;
        hitbox.enabled = true;
        while (elapsed_time < duration)
        {
            elapsed_time += Time.deltaTime;
            yield return null;
        }
        elapsed_time = 0f;
        hitbox.enabled = false;
        while (elapsed_time < recovery)
        {
            elapsed_time += Time.deltaTime;
            yield return null;
        }
        attacking = false;
    }
}
