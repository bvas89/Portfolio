/* AbilityMagGlass : Ability
 * 
 * Initializes the Magnifying Glass when its button has been clicked.
 */

using UnityEngine;

[CreateAssetMenu(fileName = "New Magnifying Glass", menuName = "Abilities/Magnifying Glass")]
public class AbilityMagGlass : Ability
{
    [Tooltip("How long the player has use this ability.")]
    public float totalDuration = 5.0f;

    [Tooltip("How long each point in the line lasts before being removed.")]
    public float pointDuration = 2.5f;

    [Tooltip("How much damage this does per tick.")]
    public float damage = 1.0f;

    public override void Gesture()
    {
        isReady = true;
    }

    public override string SetEmoji()
    {
        return Emoji.magGlass;
    }

    public override void TriggerAbility()
    {
        GameObject go = Instantiate(prefab);
        go.GetComponent<MagnifyingGlassController>().Initialize(pointDuration, totalDuration, damage);
    }
}
