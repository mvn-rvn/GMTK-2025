using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CamStaging : MonoBehaviour
{
    [Header("Cam stage size is based on the x-scale of the stage hitbox")]
    [Header("35.5 = default cam size")]

    public List<GameObject> attached_enemies = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        attached_enemies.RemoveAll(enemy => enemy == null);

        if (attached_enemies.Count == 0)
        {
            Destroy(gameObject);
        }
    }

    public void ActivateStage()
    {
        foreach (GameObject enemy in attached_enemies) {
            enemy.GetComponent<EnemyLogic>().ActivateEnemy();
        }
    }
}
