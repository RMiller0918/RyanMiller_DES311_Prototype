using UnityEngine;

public class MeleeColliderScript : MonoBehaviour
{
    private int _damageValue = 50;
    //check that the melee collider has hit anything that is not the player. 
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.gameObject.tag == "Player") return;
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
