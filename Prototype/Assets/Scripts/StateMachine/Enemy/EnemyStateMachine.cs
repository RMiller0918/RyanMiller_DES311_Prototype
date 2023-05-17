using UnityEngine;

public class EnemyStateMachine : MonoBehaviour, IDamageable, ILightable
{
    [SerializeField] private EnemyScriptableObject _enemyData;
    [field: SerializeField] public EnemyBaseState CurrentState { get; set; }
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _health;

    [field:Header("Lighting")]
    [field: SerializeField] public bool Lit { get; private set; }

    [Header("Health Bar")]
    [SerializeField] private HealthBar _healthBar;

    private void Awake()
    {
        _healthBar = GetComponentInChildren<HealthBar>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _health = _enemyData.Health;
        _maxHealth = _health;
    }

    // Update is called once per frame
    void Update()
    {
        if(_health <= 0)
            Destroy(this.gameObject);
    }

    public void HandleDamage(int damageValue)
    {
        _health -= damageValue;
        _healthBar.UpdateHealthBar(_maxHealth, _health);
    }

    public void HandleHitByLight(int lightValue)
    {
        Debug.Log(lightValue);
        Lit = lightValue > 25;
    }
}
