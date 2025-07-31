using UnityEngine;
using System.Collections.Generic;

public class EnemyHealth : MonoBehaviour
{

    private List<StringController> attachedStrings = new List<StringController>();

    [SerializeField] private float health;

    public void AttachString(StringController sender)
    {
        attachedStrings.Add(sender);
    }

    public void DetachString(StringController sender)
    {
        attachedStrings.Remove(sender);
    }

    public void Damage(float val)
    {
        float count = attachedStrings.Count;

        if (count > 0)
        {
            foreach (StringController str in attachedStrings)
            {
                str.Damage(val / count);
            }
        }
        else
        {
            health -= val;
            if (health <= 0) KillEnemy();
        }
    }

    public void KillEnemy()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Damage(1);
        }
    }
}
