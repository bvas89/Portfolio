/// GameData.cs
/// The Singlteton used for all game data.
/// This allows me to tinker most variables in one script.

using System;
using UnityEditor.Compilation;
using UnityEngine;

[RequireComponent(typeof(GameController))]
public class GameData : MonoBehaviour
{
    // Singleton instance for GameData
    public static GameData Data;
    // Random number generator
    private static System.Random random = new System.Random();

    #region SubClasses

    // Subclass for game header information
    public gdHeader Header;
    [Serializable]
    public class gdHeader
    {
        [Tooltip("The name of the game being played")]
        [SerializeField] private string gameName;
        public string GameName { get { return gameName; } }

        [Tooltip("The call to action to start the game. 'Tap to start'")]
        [SerializeField] private string cta;
        public string CTA { get { return cta; } }
    }

    // Subclass for game points settings
    public gdPoints Points;
    [Serializable]
    public class gdPoints
    {
        [Tooltip("Points gained from collecting a coin")]
        [SerializeField] private int coin;
        public int Coin { get { return coin; } }

        [Tooltip("Points gained from passing through a gap. Probably should be 0")]
        [SerializeField] private int gap;
        public int Gap { get { return gap; } }

        [Tooltip("Points goined each second of gameplay")]
        [SerializeField] private int second;
        public int Second { get { return second; } }

        [Tooltip("Points gained each beat of cadence")]
        [SerializeField] private int cadence;
        public int Cadence { get { return cadence; } }
    }

    // Subclass for main game settings
    public gdMain Main;
    [Serializable]
    public class gdMain
    {
        [Tooltip("The player hero")]
        [SerializeField] private GameObject hero;
        public GameObject Hero { get { return hero; } }

        [Tooltip("The parent that all Spawnable Objects will be housed under.")]
        [SerializeField] private Transform spawnablesParent;
        public Transform SpawnablesParent { get { return spawnablesParent; } }

        [Tooltip("The direction objects will move across the screen")]
        [SerializeField] private ObjectDirection objectDirection = ObjectDirection.Left;
        public ObjectDirection Direction { get { return objectDirection; } }

        [Tooltip("How many times per second does a spawn chance activate.")]
        [SerializeField] private float cadence = 2.5f;
        public float Cadence { get { return 1f / cadence; } }

        [Tooltip("The base speed of all moving objects")]
        [SerializeField] private float speed = 4.5f;
        public float Speed { get { return speed; } }

        [Tooltip("Is the game paused?")]
        [SerializeField] private bool isPaused = true;
        public bool IsPaused { get { return isPaused; } }

        [Tooltip("The universal multiplier for scripts to refer to")]
        [SerializeField] private float multiplier = 1f;
        public float Multiplier { get { return multiplier; } }

        [Tooltip("The amount of seconds until 2x multiplier")]
        [SerializeField] private float rate = 60f;

        // Properties for screen height and width based on camera settings
        public float ScreenHeight { get { return Camera.main.orthographicSize * 1.66f; } }
        public float ScreenWidth { get { return ScreenHeight * Camera.main.aspect; } }

        // Method to update the multiplier over time
        public void UpdateMultiplier() { multiplier += Time.deltaTime / rate; }

        // Methods to handle game pausing, speed, and cadence changes
        public void PauseGame() { isPaused = true; }
        public void UnPauseGame() { isPaused = false; }
        public void ChangeSpeed(float s) { speed = s; }
        public void ChangeCadence(float c) { cadence = c; }
    }

    // Abstract base class for all spawnable objects
    public abstract class gdSpawnable
    {
        public abstract float Weight { get; }
        public abstract GameObject Prefab { get; }
        public abstract float Cooldown { get; }
        public abstract Sprite Sprite { get; }
    }

    // Subclass for coins spawnable
    public gdCoins Coins;
    [System.Serializable]
    public class gdCoins : gdSpawnable
    {
        [Tooltip("The sprite of the item")]
        [SerializeField] private Sprite sprite;
        public override Sprite Sprite { get { return sprite; } }

        [Tooltip("The prefab of the item")]
        [SerializeField] private GameObject prefab;
        public override GameObject Prefab { get { return prefab; } }

        [Tooltip("The weighted chance for this to spawn")]
        [SerializeField] private float weight;
        public override float Weight { get { return weight; } }

        [Tooltip("Amount of seconds until this can be spawned again")]
        [SerializeField] private float cooldown = .5f;
        public override float Cooldown { get { return cooldown; } }
    }

    // Subclass for gaps spawnable (obstacle with gaps)
    public gdGaps Gaps;
    [System.Serializable]
    public class gdGaps : gdSpawnable
    {
        [Header("Main")]
        [Tooltip("The sprite of the item")]
        [SerializeField] private Sprite sprite;
        public override Sprite Sprite { get { return sprite; } }

        [Tooltip("The prefab of the item")]
        [SerializeField] private GameObject prefab;
        public override GameObject Prefab { get { return prefab; } }

        [Tooltip("The weighted chance for this to spawn")]
        [SerializeField] private float weight;
        public override float Weight { get { return weight; } }

        [Header("Gap")]
        [Tooltip("The height of the gap")]
        [SerializeField] private float gapHeight = 2f;
        public float GapHeight { get { return gapHeight; } }

        [Tooltip("The width of the gap")]
        [SerializeField] private float gapWidth = 2f;
        public float GapWidth { get { return gapWidth; } }

        [Header("Obstacle")]
        [Tooltip("The height of an obstacle. Not used in favor of randomizing.")]
        [SerializeField] private float obstacleHeight = 2f;
        public float ObstacleHeight { get { return obstacleHeight; } }

        [Tooltip("The amount of seconds until this can be spawned again")]
        [SerializeField] private float cooldown = 2f;
        public override float Cooldown { get { return cooldown; } }
    }

    // Subclass for power-ups spawnable
    public gdPowerUps PowerUps;
    [System.Serializable]
    public class gdPowerUps : gdSpawnable
    {
        // Nested class for each power-up ability
        public puAbilities[] Abilities;
        [System.Serializable]
        public class puAbilities
        {
            [Tooltip("The abilities able to be spawned in this game")]
            [SerializeField] private Ability ability;
            public Ability Ability { get { return ability; } }

            [Tooltip("The weighted chance of each ability")]
            [SerializeField] private float weight;
            public float Weight { get { return weight; } }
        }

        [Tooltip("The sprite of the item")]
        [SerializeField] private Sprite sprite;
        public override Sprite Sprite { get { return sprite; } }

        [Tooltip("The prefab of the item")]
        [SerializeField] private GameObject prefab;
        public override GameObject Prefab { get { return prefab; } }

        [Tooltip("The weighted chance for this to spawn")]
        [SerializeField] private float weight;
        public override float Weight { get { return weight; } }

        [Tooltip("The amount of seconds until this can be spawned again")]
        [SerializeField] private float cooldown = 5f;
        public override float Cooldown { get { return cooldown; } }
    }

    // Subclass for empty (no spawnable) objects
    public gdEmpty Empty;
    [System.Serializable]
    public class gdEmpty : gdSpawnable
    {
        [Tooltip("The sprite of the item")]
        [SerializeField] private Sprite sprite;
        public override Sprite Sprite { get { return sprite; } }

        [Tooltip("The weighted chance for this to spawn")]
        [SerializeField] private float weight;
        public override float Weight { get { return weight; } }

        [Tooltip("The prefab of the item")]
        [SerializeField] private GameObject prefab = null;
        public override GameObject Prefab { get { return prefab; } }

        [Tooltip("The amount of seconds until this can be spawned again. Should be 0 for empty spaces")]
        [SerializeField] private float cooldown;
        public override float Cooldown { get { return cooldown; } }
    }
    #endregion

    #region Functions

    // Awake method sets the Data instance for GameData (Singleton pattern)
    private void Awake()
    {
        Data = this;
    }

    /// <summary>
    /// Use the weights within the Spawnables to determine which object is chosen.
    /// </summary>
    /// <param name="objects">The list of spawnable objects to iterate over.</param>
    /// <returns>The selected spawnable object based on weights.</returns>
    public static gdSpawnable ChooseSpawnable(gdSpawnable[] objects)
    {
        // Calculate the total weight of potential spawnables
        float totalWeight = 0f;
        foreach (var v in objects)
        {
            totalWeight += v.Weight;
        }

        // Generate a random value between 0 and the total weight
        float randomValue = (float)random.NextDouble() * totalWeight;

        // Iterate through the items and compare with the random value
        float cumulativeWeight = 0f;
        for (int i = 0; i < objects.Length; i++)
        {
            cumulativeWeight += objects[i].Weight;
            if (randomValue <= cumulativeWeight)
            {
                return objects[i];
            }
        }

        // If this point is reached, something went wrong in the random selection.
        throw new ArgumentException("ChooseSpawnable chose a weight outside of the array cumulative. Investigate.");
    }

    // Overloaded method to choose power-up abilities based on weights
    public static gdPowerUps.puAbilities ChooseSpawnable(gdPowerUps.puAbilities[] items)
    {
        float totalWeight = 0f;
        foreach (var v in items)
        {
            totalWeight += v.Weight;
        }
        float randomValue = (float)random.NextDouble() * totalWeight;
        float cumulativeWeight = 0f;
        for (int i = 0; i < items.Length; i++)
        {
            cumulativeWeight += items[i].Weight;
            if (randomValue <= cumulativeWeight)
                return items[i];
        }
        // If this point is reached, something went wrong in the random selection.
        throw new ArgumentException("The length of items array must match the length of weights array.");
    }

    // Generic method to choose weighted random items based on provided weights
    public static T GenericChooseWeightedRandom<T>(T[] items, float[] weights)
    {
        if (items.Length != weights.Length)
        {
            throw new ArgumentException("The length of items array must match the length of weights array.");
        }

        // Calculate the total weight
        float totalWeight = 0f;
        foreach (float weight in weights)
        {
            totalWeight += weight;
        }

        // Generate a random value between 0 and the total weight
        float randomValue = (float)random.NextDouble() * totalWeight;

        // Iterate through the items and compare with the random value
        float cumulativeWeight = 0f;
        for (int i = 0; i < items.Length; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue <= cumulativeWeight)
            {
                return items[i];
            }
        }

        // If this point is reached, something went wrong in the random selection.
        return default(T);
    }

    // Methods to pause and unpause the game using the main class
    public static void PauseGame() { Data.Main.PauseGame(); }
    public static void UnPauseGame() { Data.Main.UnPauseGame(); }

    #endregion
}

public enum ObjectDirection { Left, Right, Up, Down }