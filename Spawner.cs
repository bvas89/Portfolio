/* Spawner.cs
 * 
 * This script is a component for the Spawner gameobject. 
 * It takes a LevelScriptableObject as input to determine how to spawn enemy ants.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Level;

public class Spawner : MonoBehaviour
{
    [Tooltip("The encapsulating parent Spawn Point. " +
    "Should contain children eiligible to spawn ants.")]
    public Transform SpawnPoint;
    private Transform[] _spawnPoints;

    [Tooltip("The level to be loaded")]
    public LevelScriptableObject _level;

    public GameObject AntPrefab;
    public Ant.Ant[] AntSettings;

    private float currentTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Gets the spawnpoints from the SpawnPoint Parent
        _spawnPoints = SpawnPoint.GetComponentsInChildren<Transform>();

        SetSpawnMethod();
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
    }

    //Chooses which Coroutine to run.
    void SetSpawnMethod()
    {
        switch(_level.Method)
        {
            case Method.Batch:
                StartCoroutine(Batch());
                break;
            case Method.Individual:
                StartCoroutine(Individual());
                break;
            case Method.TrueRandom:
                StartCoroutine(TrueRandom());
                break;
            case Method.Wave:
                StartCoroutine(Wave());
                break;
            case Method.WeightedRandom:
                StartCoroutine(WeightedRandom());
                break;
            default:
                break;
        }
    }

    IEnumerator Batch()
    {
        //Use our own list.
        List<Batch.Ant> ants = new List<Batch.Ant>();
        foreach (var v in _level.Settings.Batch.Ants)
            ants.Add(v);

        //To simplify, convert ants to their ranks.
        List<int> ranks = new List<int>();

        //For each Ant Rank, add that amount to the Ranks list.
        for(int i = 0; i < ants.Count; i++)
            for (int j = 0; j < ants[i].amount; j++)
                ranks.Add(i);

        //How often between spawns.
        float t = _level.Settings.Batch.Duration / ranks.Count;
        print(t);

        GameObject a = AntPrefab;

        //Spawn the Ants
        //Gets the ants in order.
        if (_level.Settings.Batch.isInOrder)
        {
            for (int i = 0; i < ranks.Count; i++)
            {
                //Get a random Spawn Point.
                int s = Random.Range(0, _spawnPoints.Length);
                Vector3 spawnPoint = _spawnPoints[s].transform.position;

                //Spawn the Ant
                a.GetComponent<Ant.AntController>().ant = AntSettings[ranks[i]];
                Instantiate(a, spawnPoint, Quaternion.identity);
                yield return new WaitForSeconds(t);
            }
        }
        else //If spawning is Random (as it should be).
        {
            while(true)
            {
                //Get a random ant in the List
                int r = Random.Range(0, ranks.Count);

                //Get Random Spawn Point.
                int s = Random.Range(0, _spawnPoints.Length);
                Vector3 spawnPoint = _spawnPoints[s].transform.position;

                //Instantiate it
                a.GetComponent<Ant.AntController>().ant = AntSettings[ranks[r]];
                Instantiate(a, spawnPoint, Quaternion.identity);

                //Clean up List
                ranks.RemoveAt(r);
                ranks.TrimExcess();

                //Wait
                yield return new WaitForSeconds(t);
            }
        }
    }

    /// <summary>
    /// Spawns ants according to the Designer's specifications.
    /// </summary>
    IEnumerator Individual()
    {
        //Get the array of ants.
        Individual.Ant[] ants = _level.Settings.Individual.ants;
 
        while (true)
        {
            //Get all the spawn times.
            foreach (var v in ants)
            {
                //If is ready and has not yet spawned
                if (v.hasSpawned == false && currentTime >= v.spawnTime)
                {
                    //Initialize the Ant
                    GameObject a = AntPrefab;
                    int rank = (int)v.antType;
                    Vector3 spawnPoint =
                        _spawnPoints[v.spawnPoint].transform.position;

                    //Spawn the Ant
                    a.GetComponent<Ant.AntController>().ant = AntSettings[rank];
                    Instantiate(a, spawnPoint, Quaternion.identity);

                    v.hasSpawned = true;
                }
            }

            //Repeat check every x second
            yield return new WaitForSeconds(1.0f);
        }
    }

    /// <summary>
    /// Spawns a random ant every X seconds.
    /// </summary>
    IEnumerator TrueRandom()
    {
        GameObject a = AntPrefab;
        while(true)
        {
            //Get a random SpawnPoint and AntRank.
            int s = Random.Range(0, _spawnPoints.Length);
            int r = Random.Range(0, AntSettings.Length);

            //Set the Ant Rank and instantiate it.
            a.GetComponent<Ant.AntController>().ant = AntSettings[r];
            Instantiate(a, _spawnPoints[s].transform.position, Quaternion.identity);

            yield return new WaitForSeconds(_level.Settings.TrueRandom.SpawnTime);
        }
    }

    /// <summary>
    /// Spawns a weighted random Ant once every x seconds.
    /// </summary>
    IEnumerator WeightedRandom()
    {
        _level.Settings.WeightedRandom.SetDictionary();
        GameObject a = AntPrefab;

        while (true)
        {
            //Random Spawn Point
            int s = Random.Range(0, _spawnPoints.Length);
            //Get the Rank of the ant.
            int r = _level.Settings.WeightedRandom.ChooseWeightedRandom();

            a.GetComponent<Ant.AntController>().ant = AntSettings[r];
            Instantiate(a, _spawnPoints[s].transform.position, Quaternion.identity);

            yield return new WaitForSeconds(_level.Settings.WeightedRandom.SpawnTime);
        }
    }

    /// <summary>
    /// Spawns waves of ants.
    /// </summary>
    IEnumerator Wave()
    {
        GameObject a = AntPrefab;

        //For every Wave..
        for (int i = 0; i < _level.Settings.Wave.waves.Length; i++)
        {
            //Get Ant amounts
            foreach (var j in _level.Settings.Wave.waves[i].AntAmounts)
            {
                //Initialize each ant.
                for (int k = 0; k < j.value; k++)
                {
                    //the Rank of the ant; this is set to the index of its name.
                    int r = _level.Settings.Wave.waves[i].AntAmounts.IndexOf(j);

                    //Random Spawn Point
                    int rand = Random.Range(0, _spawnPoints.Length);

                    //Sets the rank and spaws the ant.
                    a.GetComponent<Ant.AntController>().ant = AntSettings[r];
                    Instantiate(a, _spawnPoints[rand].transform.position, Quaternion.identity);
                }
            }

            //Sets how long to wait.
            float t = 1.0f;
            switch (_level.Settings.Wave.delayMethod)
            {
                case DelayMethod.RepeatEvery:
                    t = _level.Settings.Wave.Amount;
                    break;
                case DelayMethod.BufferBeforeNext:
                    t = _level.Settings.Wave.waves[i]._time;
                    break;
                default:
                    t = 1.0f;
                    break;
            }
            yield return new WaitForSeconds(t);
        }
    }
}