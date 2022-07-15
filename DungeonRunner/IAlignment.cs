/* IAlignment interface
 * 
 * A simple way to check whether the target is a friend or foe (or neutral!).
 */
public interface IAlignment
{
    /// <summary>
    /// The basic Alignment of the Unit.
    /// </summary>
    abstract Alignment alignment { get; set; }

    /// <summary>
    /// Should this Unit be hostile to the target?
    /// </summary>
    /// <param name="target">The target to check.</param>
    /// <returns></returns>
    public bool beHostileToTarget(IAlignment target)
    {
        bool beHostile = false;

        if (alignment == Alignment.Good)
            if (target.alignment == Alignment.Bad)
                beHostile = true;

        if (alignment == Alignment.Bad)
            if (target.alignment == Alignment.Good)
                beHostile = true;

        return beHostile;
    }
}

public enum Alignment
{
    Good, Neutral, Bad
}