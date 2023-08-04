using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Abilities/New Ability", order = 0)]
public class Ability : ScriptableObject
{
    public GameObject prefab;
    public Sprite sprite;

    // The color for a generic Star - to tell the difference.
    public Color color;
}
