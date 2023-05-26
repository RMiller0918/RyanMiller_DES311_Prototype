//Interfaces that are used throughout scripts.

public interface IMoveable //Called only by scripts that need them, not called by external classes.
{
    public void Moving();
}

public interface IGravity //Called only by scripts that need them, not called by external classes.
{
    public void HandleGravity();
}

public interface IDamageable //Used when scripts need to damage an object.
{
    public void HandleDamage(int damageValue);
}

public interface ILightable //Called by the Light Manager
{
    public void HandleHitByLight(int lightValue);
}