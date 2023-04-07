using UnityEngine;

public class EnemyStateMachine : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyScriptableObject _enemyData;
    [field: SerializeField] public EnemyBaseState CurrentState { get; set; }

    [SerializeField] private int _health;
    // Start is called before the first frame update
    void Start()
    {
        _health = 20;
    }

    // Update is called once per frame
    void Update()
    {
        if(_health <= 0)
            Destroy(this.gameObject);
    }

    public void HandleDamage(int DamageValue)
    {
        _health -= DamageValue;
    }
}
