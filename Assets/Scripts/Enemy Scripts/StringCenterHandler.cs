using UnityEngine;

public class StringCenterHandler : MonoBehaviour
{
    [SerializeField] private float slashHealth = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        slashHealth -= 1f;

        if (slashHealth <= 0)
        {
            gameObject.transform.parent.GetComponent<StringController>().SnapString();
            Destroy(this);
        }
        else gameObject.transform.parent.GetComponent<StringController>().PlayEffect();
    }

}
