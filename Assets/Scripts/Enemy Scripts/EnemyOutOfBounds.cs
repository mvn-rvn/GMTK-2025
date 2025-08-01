using UnityEngine;

public class EnemyOutOfBounds : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "OOB")
        {
            gameObject.GetComponent<EnemyHealth>().Damage(999f);
        }
    }
}
