/* SpawnMethod.cs ScriptableObject
 * 
 * This is the base class for creating phases (or methods) of spawning enemies.
 * DailyLevel.cs uses these methods for spawning ants accordingly.
 * 
 */

using System.Collections.Generic;
using UnityEngine;


public abstract class SpawnMethod : ScriptableObject
{
    [Tooltip("How long into the game to start the Method.")]
    public float startTime = 0.0f;

    [Tooltip("How long the method lasts. Other methods can run congruently with this.")]
    public float duration = 30f;

    public class Ant
    {
        public string name;
        public int rank;
        public float spawnTime;
        public int spawnPoint;
        public bool hasBeenSpawned = false;
    }

    /// <summary>
    /// The list of ants to send to the DailySpawner
    /// </summary>
    /// <returns></returns>
    public abstract List<Ant> AntsList();
}
