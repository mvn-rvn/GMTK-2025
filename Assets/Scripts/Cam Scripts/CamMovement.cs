using UnityEngine;

public class CamMovement : MonoBehaviour
{
    public GameObject target;
    public float smoothing_time = 0.3f;
    Vector2 velocity = Vector2.zero;
    float original_z;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        original_z = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        transform.position = Vector2.SmoothDamp(transform.position, target.transform.position, ref velocity, smoothing_time);
        transform.position = new Vector3(transform.position.x, transform.position.y, original_z);
    }
}
