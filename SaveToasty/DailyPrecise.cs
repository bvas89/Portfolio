/* DailyPrecise : SpawnMethod
 * 
 * Gives the designer direct control over which Ant to spawn, what time, and which spawnpoint.
 * 
 */
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Precise", menuName = "Spawn Methods / Precise", order = 3)]
public class DailyPrecise : SpawnMethod
{
    [System.Serializable]
    public class IndividualAnt
    {
        public enum AntRank { Worker, Fire, Termite, Weevil, Queen };

        [SerializeField]
        public AntRank rank;

        public float spawnTime;

        [Range(0,7)]
        public int spawnPoint;
    }

    public IndividualAnt[] Ants;

    public override List<Ant> AntsList()
    {
        List<Ant> ExtendedList = new List<Ant>();
        float st = startTime;

        for(int i = 0; i < Ants.Length; i++)
        {
            Ant a = new Ant
            {
                name = Ants[i].rank.ToString(),
                rank = (int)Ants[i].rank,
                spawnPoint = Ants[i].spawnPoint,
                spawnTime = st + Ants[i].spawnTime
            };

            ExtendedList.Add(a);
        }

        return ExtendedList;
    }
}
