using UnityEngine;

public class EnemyAnimationScript : MonoBehaviour
{
    private BoxCollider _weaponHitBox;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _weaponHitBox = GetComponentInChildren<BoxCollider>();
        _weaponHitBox.enabled = false;
    }


    //Animation event for enemy attack enables the Hit box for the enemy baton/sword
    public void EnableSwing()
    {
        _weaponHitBox.enabled = true;
    }

    //Animation event for Enemy Attack disables the hit box for enemy baton/Sword
    private void DisableSwing()
    {
        _weaponHitBox.enabled = false;
    }
}
