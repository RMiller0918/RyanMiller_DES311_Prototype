using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Configuration",menuName = "Enemy Scriptable Object")]
public class EnemyScriptableObject : ScriptableObject
{
    //Stores Enemy Data. Planned to be used to allow for enemy types.
    public enum EnemyType
    {
        Melee,
        Ranged,
        LightBearer
    }

    [SerializeField][Range(50,500)] private int _health;
    public int Health => _health;
    [SerializeField] private int _id;
    public int ID => _id;
    [SerializeField] private string _enemyName;
    public string EnemyName => _enemyName;

    public EnemyType _enemyType;
}
