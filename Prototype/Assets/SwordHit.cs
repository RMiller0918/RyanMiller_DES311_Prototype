using UnityEngine;

public class SwordHit : MonoBehaviour
{
    private int _damageValue = 50;
    private void OnTriggerEnter(Collider other) //Checks if the enemy sword has hit anything that is not another Enemy.
    {
        Debug.Log(other);
        if (other.gameObject.tag == "Enemy") return;
        Debug.Log(other.gameObject);
        CheckForDamageable(other.gameObject);
    }

    //Checks for a damageable component on the object it has hit. If an component is returned then the object runs it's own HandleDamage function. Use of IDamageable interface.
    private void CheckForDamageable(GameObject other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        Debug.Log(damageable);
        if (damageable != null)
        {
            Debug.Log("Handling Damage");
            damageable.HandleDamage(_damageValue);
        }
    }
}
