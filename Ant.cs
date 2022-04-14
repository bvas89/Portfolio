/* Ant.cs - Scriptable Object
 * 
 * Contains all of the stats to assign to the ant on instantiation.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Experimental.U2D.Animation;

namespace Ant
{
    [CreateAssetMenu(fileName = "AntStats", menuName = "ScriptableObjects/Ant", order = 1)]
    public class Ant : ScriptableObject
    {  
        [TextArea]
        public string Note = "Notes";

        [Tooltip("The main stats of the Ant")]
        public Stats stats;
        [Tooltip("The movement variables of the Ant")]
        public Movement movement;
        [Tooltip("The combat stats of the ant.")]
        public Combat combat;
        [Tooltip("The visuals of the Ant.")]
        public Visuals visuals;

        [System.Serializable]
        public class Stats
        {
            [Tooltip("The name of the unit")]
            public string name;
            [Tooltip("The rank of the unit")]
            public int rank;
            [Tooltip("HitPoints")]
            public int hp;
        }

        [System.Serializable]
        public class Movement
        {
            [Tooltip("How fast the ant moves.")]
            public float speed = 5;

            [Tooltip("How quickly the ant turns. deg/sec.")]
            public float rotationSpeed = 3600f;

            [Tooltip("The maximum acceleration of the unit.")]
            public float acceleration = 25f;

            [Tooltip("Base time to wait before moving again.")]
            public float waitTimeAlpha = 1f;

            [Tooltip("Percentage of Alpha to add/subtract from Wait Time." +
                "\n(i.e. A WaitTimeAlpha of 1 and Fuzz of 0.1 will yield a random" +
                "WaitTime between 0.9 and 1.1 seconds.)")]
            [Range(0,1)]
            public float waitTimeFuzz = .5f;

            /// <summary>
            /// Computes the amount of time to wait.
            /// </summary>
            /// <returns>The weighted amount of time to wait.</returns>
            public float WaitTime()
            {
                float rand = Random.Range(-waitTimeFuzz, waitTimeFuzz);
                return waitTimeAlpha + (waitTimeAlpha * rand);
            }

            [Tooltip("The radius that the ant will detect something yummy.")]
            public float searchRadius = 2;


            [Tooltip("The radius the ant will move within.")]
            public float movementRadius = 5;

            [Tooltip("The handicap given to assist in finding Toasty." +
                "\n\n A gravity of 0.1 takes a random point and nudges it 10% " +
                "closer in Toasty's direction. This is to pull ants toward Toasty " +
                "while still allowing them to move Randomly.")]
            [Range(0, 1)]
            public float gravity = 0.1f;

            [Tooltip("How often a new pheramone is dropped.")]
            public float pheramoneSpawnTime = 1f;

            [Tooltip("How long until the pheramone dissipates.")]
            public float pheramoneDuration = 0.5f;
        }

        [System.Serializable]
        public class Combat
        {
            [Tooltip("The amount of damage the ant deals.")]
            public float damage = 3;

            [Tooltip("The amount of time it takes the ant to bite")]
            public float biteTimeAlpha = 0.5f;

            [Tooltip("Percentage of Alpha to add/subtract from Wait Time." +
                "\n(i.e. A BiteTimeAlpha of 0.5 and Fuzz of 0.5 will yield a random" +
                "WaitTime between 0.25 and 0.75 seconds.)")]
            [Range(0,1)]
            public float biteTimeFuzz = .5f;

            /// <summary>
            /// Computes the amount of time it takes to bite.
            /// </summary>
            /// <returns>The weighted amount of bite time.</returns>
            public float BiteTime()
            {
                float rand = Random.Range(-biteTimeFuzz, biteTimeFuzz);
                return biteTimeAlpha + (biteTimeAlpha * rand);
            }

            [Header("Not Yet Implemented")]
            [Tooltip("Critical Strike Chance")]
            public float criticalChance = 0.05f;

            [Tooltip("Critical Strike Damage Multiplier")]
            [Range(1,3)]
            public float criticalDamageMultiplier = 1.5f;

            /// <summary>
            /// Determiens how much damage a critical strike should do.
            /// </summary>
            /// <returns>The amount of damage a critical strike does.</returns>
            public float CriticalStrikeDamage()
            {
                return damage * criticalDamageMultiplier;
            }
        }

        [System.Serializable]
        public class Visuals
        {
            public Sprite sprite;
            public SpriteLibraryAsset spriteLibraryAsset;
        }
    }
}