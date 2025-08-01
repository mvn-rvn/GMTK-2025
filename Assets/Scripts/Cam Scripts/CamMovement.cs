using UnityEngine;

public class CamMovement : MonoBehaviour
{
    public GameObject target;
    public float smoothing_time = 0.3f;

    Vector2 movement_velocity = Vector2.zero;
    float size_velocity = 0f;
    Vector2 scale_velocity = Vector2.zero;

    float original_z;
    float original_size;
    Vector2 original_scale;

    [HideInInspector]
    public GameObject staging_area;
    Camera cam;

    public GameObject left_bounds_object;
    public GameObject right_bounds_object;
    BoxCollider2D left_bounds;
    BoxCollider2D right_bounds;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        original_z = transform.position.z;

        cam = gameObject.GetComponent<Camera>();
        original_size = cam.orthographicSize;

        original_scale = transform.localScale;

        left_bounds = left_bounds_object.GetComponent<BoxCollider2D>();
        right_bounds = right_bounds_object.GetComponent<BoxCollider2D>();
        left_bounds.enabled = false;
        right_bounds.enabled = false;

        transform.position = GameObject.Find("Checkpoint Manager").GetComponent<Chkpnt_Ctrl>().GetCheckpointPos();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.Log("No target to follow");
            return;
        }

        if (staging_area == null)
        {
            left_bounds.enabled = false;
            right_bounds.enabled = false;

            transform.position = Vector2.SmoothDamp(transform.position, target.transform.position, ref movement_velocity, smoothing_time);
            transform.position = new Vector3(transform.position.x, transform.position.y, original_z);

            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, original_size, ref size_velocity, smoothing_time);

            transform.localScale = Vector2.SmoothDamp(transform.localScale, original_scale, ref scale_velocity, smoothing_time);
        }

        else
        {
            left_bounds.enabled = true;
            right_bounds.enabled = true;

            transform.position = Vector2.SmoothDamp(transform.position, staging_area.transform.position, ref movement_velocity, smoothing_time);
            transform.position = new Vector3(transform.position.x, transform.position.y, original_z);

            float adjusted_size = staging_area.transform.localScale.x / 3.55f;

            cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, adjusted_size, ref size_velocity, smoothing_time);

            Vector2 adjusted_scale = new Vector2(
                adjusted_size,
                adjusted_size
            );

            transform.localScale = Vector2.SmoothDamp(transform.localScale, adjusted_scale, ref scale_velocity, smoothing_time);
        }
    }
}
