public interface IDamageable
{
    /// <summary>
    /// The current HP of the Object
    /// </summary>
    int HP { get; set; }

    /// <summary>
    /// Take Damage from a source.
    /// </summary>
    /// <param name="amount">The amount of damage to take.</param>
    abstract void TakeDamage(float amount);
}
