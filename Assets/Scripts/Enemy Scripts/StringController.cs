using UnityEngine;
using System.Collections;

public class StringController : MonoBehaviour
{

    public GameObject enemy1;
    public GameObject enemy2;

    [SerializeField] private GameObject stringCenter;

    [SerializeField] private GameObject[] strings = new GameObject[4];
    [SerializeField] private GameObject stringEnd1;
    [SerializeField] private GameObject stringEnd2;
    [SerializeField] private GameObject Anchor1;
    [SerializeField] private GameObject Anchor2;

    public float health;
    private bool snapped = false;

    [SerializeField] private float snapSpeed = 0.3f;
    [SerializeField] private float fadeSpeed = 0.9f;
    [SerializeField] private float deathTimer = 0.4f;

    [SerializeField] private GameObject particleEffect;

    private void Start()
    {
        enemy1.GetComponent<EnemyHealth>().AttachString(this);
        enemy2.GetComponent<EnemyHealth>().AttachString(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (snapped)
        {
            if (enemy1 != null) Anchor1.transform.position = Vector3.Lerp(Anchor1.transform.position, enemy1.transform.position, snapSpeed);
            if (enemy2 != null) Anchor2.transform.position = Vector3.Lerp(Anchor2.transform.position, enemy2.transform.position, snapSpeed);

            //can't be called string bc its a type
            foreach (GameObject str in strings)
            {
                Color stringColor = str.GetComponent<SpriteRenderer>().color;
                str.GetComponent<SpriteRenderer>().color = new Color(stringColor.r, stringColor.b, stringColor.g, stringColor.a * fadeSpeed);
            }

            deathTimer -= Time.deltaTime;
            if (deathTimer <= 0) Destroy(gameObject);

            return;
        }

        Vector3 endPos = Vector3.Lerp(enemy1.transform.position, enemy2.transform.position, 0.5f);
        stringCenter.transform.position = Vector3.Lerp(stringCenter.transform.position, endPos, 0.01f);

        //String Group 1
        Anchor1.transform.position = stringCenter.transform.position;
        stringEnd1.transform.position = enemy1.transform.position;

        //String Group 2
        Anchor2.transform.position = stringCenter.transform.position;
        stringEnd2.transform.position = enemy2.transform.position;

        if (health <= 0) SnapString();
    }

    public void Damage(float val)
    {
        health -= val;
    }

    public void SnapString()
    {
        if (enemy1 != null) enemy1.GetComponent<EnemyHealth>().DetachString(this);
        if (enemy2 != null) enemy2.GetComponent<EnemyHealth>().DetachString(this);
        PlayEffect();

        FindAnyObjectByType<P_Health>().Heal(1);
        snapped = true;
    }

    public void PlayEffect()
    {
        Instantiate(particleEffect, stringCenter.transform, false);
    }
}
