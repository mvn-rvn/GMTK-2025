using UnityEngine;

public class StringController : MonoBehaviour
{

    public GameObject enemy1;
    public GameObject enemy2;

    [SerializeField] private GameObject stringCenter;

    [SerializeField] private GameObject stringEnd1;
    [SerializeField] private GameObject stringEnd2;
    [SerializeField] private GameObject Anchor1;
    [SerializeField] private GameObject Anchor2;

    public float health;

    // Update is called once per frame
    void Update()
    {
        Vector3 endPos = Vector3.Lerp(enemy1.transform.position, enemy2.transform.position, 0.5f);
        stringCenter.transform.position = Vector3.Lerp(stringCenter.transform.position, endPos, 0.01f);

        //String Group 1
        Anchor1.transform.position = enemy1.transform.position;
        stringEnd1.transform.position = stringCenter.transform.position;

        //String Group 2
        Anchor2.transform.position = stringCenter.transform.position;
        stringEnd2.transform.position = enemy2.transform.position;
    }
}
