public interface IMoveable
{
    public void Moving();
}

public interface IGravity
{
    public void HandleGravity();
}

public interface IDamageable
{
    public void HandleDamage(int DamageValue);
}
