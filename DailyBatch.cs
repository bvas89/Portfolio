/***
 * DailyBatch.cs
 * 
 * This creates a PHASE of spawning for the DailySpawner to distribute.
 */

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Batch", menuName = "Spawn Methods / Batch", order = 0)]
public class DailyBatch : SpawnMethod
{
    [Tooltip("Does the batch spawn in rank order? Leave unchecked for random dispersement.")]
    public bool isInOrder = false;

    [Tooltip("The ants provided and how many of them should spawn during this phase.")]
    [System.Serializable]
    public class Ants
    {
        public string name;
        public int amount;

        [HideInInspector]
        public int rank;
    }

#if UNITY_EDITOR
    [SerializeField]
    [Tooltip("The grand total of ants to summon in this batch. This number is automatically updated. Editing changes nothing.")]
    private int grandTotal;
    private void OnValidate()
    {
        grandTotal = 0;
        for (int i = 0; i < AntAmounts.Count; i++)
        {
            grandTotal += AntAmounts[i].amount;
        }
    }
#endif


    /// <summary>
    /// The designer input for how many ants to spawn
    /// </summary>
    public List<Ants> AntAmounts = new List<Ants>()
    {
        new Ants{name = "Worker", amount = 5, rank = 0},
        new Ants{name = "Fire", amount = 5, rank = 1},
        new Ants{name = "Termite", amount = 5, rank = 2},
        new Ants{name = "Weevil", amount = 5, rank = 3},
        new Ants{name = "Queen", amount = 5, rank = 4}
    };

    public override List<SpawnMethod.Ant> AntsList()
    {
        //Placeholder List to return.
        List<Ant> FinalList = new List<Ant>();


        //Creates a list of all the Ants requested from AntAmounts
        List<Ant> ExtendedList = new List<Ant>();

        //For each type of ant..
        for (int i = 0; i < AntAmounts.Count; i++)
            //How many are there?
            for (int j = 0; j < AntAmounts[i].amount; j++)
            {
                Ant a = new Ant
                {
                    name = AntAmounts[i].name,
                    rank = AntAmounts[i].rank,
                    //spawnTime = st,
                    spawnPoint = Random.Range(0, 8)
                };

                //Add it to the list.
                ExtendedList.Add(a);
            }

        //Time Between Spawns
        float tbs = duration / ExtendedList.Count;
        //Start Time
        float st = startTime;

        //We have a list with all of the ants. Iterate through or pull random
        if (isInOrder)
        {
            for (int i = 0; i < ExtendedList.Count; i++)
            {
                FinalList.Add(ExtendedList[i]);
            }
        }
        else
        {
            while (ExtendedList.Count > 0)
            {
                int r = Random.Range(0, ExtendedList.Count);
                FinalList.Add(ExtendedList[r]);
                ExtendedList.RemoveAt(r);
                ExtendedList.TrimExcess();
            }
        }

        //Set spawn times for each ant.
        for (int i = 0; i < FinalList.Count; i++)
        {
            FinalList[i].spawnTime = st;
            st += tbs;
        }

        return FinalList;
    }
}