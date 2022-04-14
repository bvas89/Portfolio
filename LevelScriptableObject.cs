/*LevelScriptableObject.css - Scriptable Object
 * 
 *  This script allows the designer to create 5 different types of levels to be
/// uploaded to the Spawner component.
///
/// 1. Batch: Takes a batch of Ants and spawns them over the given duration.
/// 2. Individual: Gives the designer direct control over each Ant.
/// 3. TrueRandom: Spawns randomly chosen Ants at the given interval.
/// 4. Wave: Spawns a group of ants at the given interval
///             (TODO: Incorporate "When X Ants remain."
/// 5. WeightedRandom: Randomly chooses an Ant, based on the given weights.
 */

using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Level
{
    [CreateAssetMenu(fileName = "NewLevel", menuName = "Level", order = 0)]
    public class LevelScriptableObject : ScriptableObject
    {
        public Method Method;
        public Settings Settings;
    }

    public enum Method
    {
        Batch,
        Individual,
        TrueRandom,
        Wave,
        WeightedRandom
    }

    [System.Serializable]
    public class Settings
    {
        [Tooltip("Takes the sum of ants and disperses them over a duration.")]
        public Batch Batch;
        [Tooltip("Allows the Designer direct control over each ant.")]
        public Individual Individual;
        [Tooltip("Sets random ants to spawn.")]
        public TrueRandom TrueRandom;
        [Tooltip("How to spawn waves of ants.")]
        public Wave Wave;
        [Tooltip("How to handle ants weighted chance to spawn.")]
        public WeightedRandom WeightedRandom;
    }

    [System.Serializable]
    public class Batch
    {
        [Tooltip("The length of time the ants will spawn over.")]
        public float Duration = 30.0f;

        [Tooltip("Denotes whether to spawn the bugs in order, or at random.")]
        public bool isInOrder = false;

        [System.Serializable]
        public class Ant
        {
            public string name;
            public int amount;
        }

        public List<Ant> Ants = new List<Ant>()
        {
            new Ant{name = "Worker", amount = 5},
            new Ant{name = "Fire", amount = 5},
            new Ant{name = "Termite", amount = 5},
            new Ant{name = "Weevil", amount = 5},
            new Ant{name = "Queen", amount = 5},
        };
    }

    [System.Serializable]
    public class Individual
    {
        public Ant[] ants;

        //The Ant's settings to use.
        [System.Serializable]
        public class Ant
        {
            [Tooltip("What rank is the ant?")]
            public AntType antType;

            [Tooltip("The elapsed time before the ant will spawn.")]
            public float spawnTime;

            [Tooltip("The SpawnPoint that the ant will spawn at. " +
                "0 is equal to 12 o'clock")]
            [Range(0,7)]
            public int spawnPoint;

            //Parameter to spawn at a certan time.
            [HideInInspector]
            public bool hasSpawned = false;

            public enum AntType
            {
                Worker, Fire, Termite, Weevil, Queen
            }
        }
    }

    [System.Serializable]
    public class TrueRandom
    {
        [Tooltip("The amount of time between spawns; in seconds.")]
        public float SpawnTime = 1.0f;
    }

    [System.Serializable]
    public class Wave
    {
        [Tooltip("When to spawn a new wave." +
            "\n " +
            "\nRepeatEvery: Uses 'Amount' as the timer." +
            "\nBuffer: Uses 'Time' in the Wave as the timer." +
            "\nAtTime: Uses 'Time' in the Wave as the timer." +
            "\nAntsRemaning: Uses 'Amount' as how many Ants remain.")]
        public DelayMethod delayMethod;
        [Tooltip("Represents the variable for the WaveDelay in Time or Ants.")]
        public float Amount = 1.0f;
        [Tooltip("Each wave to be progammed.")]
        public Waves[] waves = new Waves[5];

        /// <summary>
        /// Sets the SpawnMethod to come in Waves. This can be set to Repeat every X seconds,
        /// at set times, or when Y ants remain alive.
        /// </summary>
        [System.Serializable]
        public class Waves
        {
            [Tooltip("When the next wave is spawned." +
                "\n\n ONLY WORKS if DelayMethod is set to Buffer or AtTime.")]
            public float _time;

            [System.Serializable]
            public class AntWeights
            {
                public string name;
                public int value;
            }

            public List<AntWeights> AntAmounts = new List<AntWeights>()
            {
                new AntWeights{name = "Worker", value = 1},
                new AntWeights{name = "Fire", value = 0},
                new AntWeights{name = "Termite", value = 0},
                new AntWeights{name = "Weevil", value = 0},
                new AntWeights{name = "Queen", value = 0}
            };
        }

    }
        public enum DelayMethod { RepeatEvery, BufferBeforeNext/*AntsRemaning, AtTime*/}
    
    [System.Serializable]
    public class WeightedRandom
    {
        [Tooltip("The amount of time between spawns; in seconds.")]
        public float SpawnTime = 1.0f;

        //Variables
        private int[] _weights;
        private int rnd;

        //The Name/Weight of the Ants.
        [System.Serializable]
        public class AntWeights
        {
            public string name;
            public int value;
        }

        public List<AntWeights> antWeights = new List<AntWeights>()
        {
            new AntWeights{name = "Worker", value = 40},
            new AntWeights{name = "Fire", value = 20},
            new AntWeights{name = "Termite", value = 10},
            new AntWeights{name = "Weevil", value = 5},
            new AntWeights{name = "Queen", value = 1}
        };

        private static Dictionary<string, int> antDictionary
            = new Dictionary<string, int>();

        /// <summary>
        /// Populates the Dictionary. MUST be called.
        /// </summary>
        public void SetDictionary()
        {
            //Makes a Dictionary of the List of AntWeights
            foreach (var w in antWeights)
                antDictionary.Add(w.name, w.value);
            _weights = new int[antDictionary.Count];
            antDictionary.Values.CopyTo(_weights, 0);
        }

        /// <summary>
        /// The sum of the values in the array
        /// </summary>
        /// <param name="i">the array to be summed up.</param>
        /// <returns></returns>
        public int SumOfWeights(int[] i)
        {
            int j = 0;
            foreach (int k in i)
                j += k;
            return j;
        }

        /// <summary>
        /// Chooses which ant to spawn.
        /// </summary>
        /// <returns>The rank of the ant to spawn.</returns>
        public int ChooseWeightedRandom()
        {
            int r = Random.Range(0, SumOfWeights(_weights));
            rnd = r;
            for (int i = 0; i < _weights.Length; i++)
            {
                if (r < _weights[i])
                {
                    return i;
                }
                r -= _weights[i];
            }
            return rnd;
        }
    }
}