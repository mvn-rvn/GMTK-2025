using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Chkpnt_Ctrl : MonoBehaviour
{

    [SerializeField]
    List<GameObject> checkpoints = new List<GameObject>();

    [Header("In the inspector, this checkpoint should be the starting checkpoint in the game")]
    [SerializeField]
    GameObject current_checkpoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        foreach (GameObject checkpoint in checkpoints)
        {
            DontDestroyOnLoad(checkpoint);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //PLACEHOLDER TO TEST CHECKPOINT FUNCTIONALITY
        if (Input.GetKeyDown(KeyCode.R)) { Reload(); }
    }

    public void SwapCheckpoint(GameObject new_checkpoint)
    {
        current_checkpoint = new_checkpoint;
    }

    public Vector2 GetCheckpointPos()
    {
        return current_checkpoint.transform.position;
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
