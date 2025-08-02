using UnityEngine;

public class StringCenterHandler : MonoBehaviour
{
    [SerializeField] private float damageMult = 5;
    private StringController stringCont;
    private void Awake()
    {
        stringCont = GetComponentInParent<StringController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        P_AttackHandler playerAtk = collision.GetComponentInParent<P_AttackHandler>();

        if (stringCont.health > playerAtk.GetDamage() * damageMult) stringCont.PlayEffect();
        stringCont.Damage(playerAtk.GetDamage() * damageMult);
    }

}
