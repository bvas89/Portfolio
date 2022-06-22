/* Ability ScriptableObject
 * 
 * The base class of PowerUps/Abilities.
 */
using UnityEngine;

    public abstract class Ability : ScriptableObject
    {
        [Tooltip("The prefab of the ability.")]
        public GameObject prefab;

        [Tooltip("The icon of the ability.")]
        public Sprite Icon;

        [Tooltip("The cooldown of the ability.")]
        public float Cooldown = 30.0f;

        [Tooltip("Is the ability ready to be instantiated?")]
        public bool isReady = false;

    [Tooltip("The emoji associated with the ability.")]
    [HideInInspector]
    public abstract string SetEmoji();

        /// <summary>
        /// The ability PRE-USE finger movement.
        /// </summary>
        public abstract void Gesture();

        /// <summary>
        /// The command sent to the AbilityController to Use the ability.
        /// </summary>
        public abstract void TriggerAbility();
    }
