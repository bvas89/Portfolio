/***
 * DailyWave.cs
 * 
 * This creates a PHASE of spawning for the DailySpawner to distribute.
 */
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "Spawn Methods / Wave", order = 2)]
public class DailyWave : SpawnMethod
{
    [SerializeField]
    public Wave[] Waves = new Wave[1];

    public override List<Ant> AntsList()
    {
        float st = startTime;
        float tbs = duration / Waves.Length;

        //Creates a list of all of the ants.
        List<Ant> ExtendedList = new List<Ant>();
        for (int i = 0; i < Waves.Length; i++)
        {
            for(int j = 0; j < Waves[i].AntAmounts.Count; j++)
            {
                Ant a = new Ant
                {
                    name = Waves[i].AntAmounts[j].name,
                    rank = Waves[i].AntAmounts[j].rank,
                    spawnPoint = Random.Range(0, 8),

                    //Sets the timer to the wave.
                    spawnTime = st,
                };

                ExtendedList.Add(a);
            }

            st += tbs;
        }

        return ExtendedList;
    }

    //A wave of ants
    [System.Serializable]
    public class Wave
    {
        [HideInInspector]
        public string name = "Wave";
        //Value = How many of each ant to spawn for the wave.
        [System.Serializable]
        public class Ants
        {
            [HideInInspector]
            public string name;
            public int value;

            [HideInInspector]
            public int rank;
        }

        
        public List<Ants> AntAmounts = new List<Ants>()
        {
            new Ants{name = "Worker", value = 1, rank = 0},
            new Ants{name = "Fire", value = 1, rank = 1},
            new Ants{name = "Termite", value = 1, rank = 2},
            new Ants{name = "Weevil", value = 1, rank = 3},
            new Ants{name = "Queen", value = 1, rank = 4},
        };
    }
}
