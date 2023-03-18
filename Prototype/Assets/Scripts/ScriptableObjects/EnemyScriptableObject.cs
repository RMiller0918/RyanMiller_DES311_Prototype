using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Configuration",menuName = "Enemy Scriptable Object")]
public class EnemyScriptableObject : ScriptableObject
{
    public enum EnemyType
    {
        Melee,
        Ranged,
        LightBearer
    }

    [SerializeField][Range(50,500)] private int _health;
    public int Health => Health;
    [SerializeField] private int _id;
    public int ID => _id;
    [SerializeField] private string _enemyName;
    public string EnemyName => _enemyName;

    public EnemyType _enemyType;
}
