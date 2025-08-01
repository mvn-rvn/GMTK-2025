using UnityEngine;

public class P_ChkpntMgmt : MonoBehaviour
{
    Chkpnt_Ctrl checkpoint_manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        checkpoint_manager = GameObject.Find("Checkpoint Manager").GetComponent<Chkpnt_Ctrl>();

        transform.position = checkpoint_manager.GetCheckpointPos();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (LayerMask.LayerToName(col.gameObject.layer) == "Checkpoint")
        {
            checkpoint_manager.SwapCheckpoint(col.gameObject);
        }
    }
}
