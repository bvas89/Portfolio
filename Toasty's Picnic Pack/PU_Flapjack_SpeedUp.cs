///PU-Flapjack_SpeedUp
/// Moves objects faster (giving the illusing of a faster character).
/// Removes collider while doing so

using UnityEngine;
using static GameData;
/// <summary>
/// Speeds up the world around the Hero for a short time. Disables colliders.
/// </summary>
public class PU_Flapjack_SpeedUp : AbilityEvent
{
    [Tooltip("How much faster object should move.")]
    public float speedMultiplier = 1.5f;

    [Tooltip("How much faster are objects spawned.")]
    public float cadenceMultiplier = 1.5f;

    /// <summary>
    /// The Hero's colider; to turn to trigger.
    /// </summary>
    Collider2D col;

    // References to revert back to.
    float origSpeed;
    float origCadence;

    protected override void Start()
    {
        base.Start();
        col = Data.Main.Hero.GetComponentInChildren<Collider2D>();
        col.isTrigger = true;

        // Set original values
        origSpeed = Data.Main.Speed;
        origCadence = Data.Main.Cadence;

        // Change the universal speed speed
        Data.Main.ChangeSpeed(origSpeed * speedMultiplier);
        Data.Main.ChangeCadence(origCadence * cadenceMultiplier);

        //Change speed for all moving objects currently on screens
        MoveObject[] moveObjects = FindObjectsOfType<MoveObject>();
        foreach (var v in moveObjects) v.ChangeSpeed(Data.Main.Speed);
    }

    private void OnDestroy()
    {
        // Revert to original speed/cadence
        Data.Main.ChangeSpeed(origSpeed);
        Data.Main.ChangeCadence(origCadence);
        MoveObject[] moveObjects = FindObjectsOfType<MoveObject>();
        foreach (var v in moveObjects) v.ChangeSpeed(Data.Main.Speed);
        col.isTrigger = false;
    }
}
