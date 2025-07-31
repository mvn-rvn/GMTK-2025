using UnityEngine;

public class StringCenterHandler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameObject.transform.parent.GetComponent<StringController>().SnapString();
    }

}
