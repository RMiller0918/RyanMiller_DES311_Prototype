using UnityEngine;

public class MeleeColliderScript : MonoBehaviour
{
    private int _damageValue = 20;
    //Check if the bolt has hit the player or anything else. 
    private void OnTriggerEnter(Collider other)
    {
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
