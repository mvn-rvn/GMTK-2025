using UnityEngine;

public class CamMovement : MonoBehaviour
{
    public GameObject target;
    public float smoothing_time = 0.3f;
    Vector2 movement_velocity = Vector2.zero;
    float size_velocity = 0f;

    float original_z;
    float original_size;

    GameObject staging_area;
    Camera cam;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        original_z = transform.position.z;

        cam = gameObject.GetComponent<Camera>();
        original_size = cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        if (staging_area == null)
        {
            transform.position = Vector2.SmoothDamp(transform.position, target.transform.position, ref movement_velocity, smoothing_time);
            transform.position = new Vector3(transform.position.x, transform.position.y, original_z);

            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, original_size, ref size_velocity, smoothing_time);
        }
        
        else
        {

        }
    }
}
