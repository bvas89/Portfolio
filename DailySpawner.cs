/* DailySpawner
 * 
 * Takes a DailyLevel scriptable object and creates a Master List of ants to spawn.
 * 
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(ScoreAndTimeManager))]
public class DailySpawner : MonoBehaviour
{
    [Tooltip("The Daily Level to pull from.")]
    public DailyLevel Level;

    //The List of ants, times, and locations
    public List<SpawnMethod.Ant> MasterList;

    [HideInInspector]
    public Ant.Ant[] AntSettings;
    [HideInInspector]
    public GameObject AntPrefab;
    [HideInInspector]
    public Transform SpawnPointMaster;
    [HideInInspector]
    public AbilityController abilityController;


    private ScoreAndTimeManager timeManager;
    private DailyAbilityContainer dac;

    private LevelSelector_Date levelSelector;

    //A generic failsafe Level.
    [HideInInspector] public DailyLevel defaultLevel;

    //Stops spawning if Toasty has died.
    private bool toastyIsAlive = true;

    private void Awake()
    {
        timeManager = GetComponent<ScoreAndTimeManager>();
        dac = GetComponent<DailyAbilityContainer>();
        levelSelector = GetComponent<LevelSelector_Date>();
    }
    void Start()
    {
        Level = levelSelector.GetTodaysLevel();
        timeManager.duration = Level.TotalDuration;
        SetAbilities();
    }

    private void OnEnable()
    {
        Actions.Toasty.HasDied += OnToastyDeath;
        Actions.Toasty.IsReadyToPlay += StartGame;
    }

    private void OnDisable()
    {
        Actions.Toasty.HasDied -= OnToastyDeath;
        Actions.Toasty.IsReadyToPlay -= StartGame;
    }

    //Sets the abilities accordingly.
    void SetAbilities()
    {
        for (int i = 0; i < Level.Abilities.Length; i++)
        {
            string s = Level.Abilities[i].ToString();

            switch (s)
            {
                case "None":
                    abilityController.abilities[i] = null;
                    break;
                case "IceCube":
                    abilityController.abilities[i] = dac.Abilities[0];
                    break;
                case "Orange":
                    abilityController.abilities[i] = dac.Abilities[1];
                    break;
                case "MagGlass":
                    abilityController.abilities[i] = dac.Abilities[2];
                    break;
                case "Nuke":
                    abilityController.abilities[i] = dac.Abilities[3];
                    break;
                case "HoneyPot":
                    abilityController.abilities[i] = dac.Abilities[4];
                    break;
                case "AntTrap":
                    abilityController.abilities[i] = dac.Abilities[5];
                    break;
                case "Balloon":
                    abilityController.abilities[i] = dac.Abilities[6];
                    break;
                case "Carrot":
                    abilityController.abilities[i] = dac.Abilities[7];
                    break;
                case "FlySwatter":
                    abilityController.abilities[i] = dac.Abilities[8];
                    break;
                case "Jelly":
                    abilityController.abilities[i] = dac.Abilities[9];
                    break;
                case "PeanutButter":
                    abilityController.abilities[i] = dac.Abilities[10];
                    break;
                case "Squeegee":
                    abilityController.abilities[i] = dac.Abilities[11];
                    break;
            }
        }
        abilityController.SetUI();
    }

    void StartGame()
    {
        Actions.Toasty.IsReadyToPlay -= StartGame;
        MasterList = new List<SpawnMethod.Ant>();

        //Creates a Master List of all the ants in the Level's methods.
        if (Level.Methods.Length != 0)
            MasterList = Level.ConvertToList();
        else
            MasterList = defaultLevel.ConvertToList();
        
        MasterList.Sort((p1, p2) => p1.spawnTime.CompareTo(p2.spawnTime)); //This isn't necessary.

        //Add time so that Counter starts when Toasty is ready (Falls to the board).
        foreach (var v in MasterList)
        {
            print(v.name + " " + v.spawnTime);
            v.spawnTime += Time.time;
        }
    }

    void OnToastyDeath()
    {
        toastyIsAlive = false;
    }

    void Update()
    {
        //Check to spawn ants in list
        if (toastyIsAlive && MasterList != null)
        {
            foreach (var v in MasterList)
            {
                if (v.hasBeenSpawned == false && Time.time > v.spawnTime)
                {
                    v.hasBeenSpawned = true;

                    //Prepare the Ant
                    GameObject a = AntPrefab;
                    a.GetComponent<Ant.AntController>().ant = AntSettings[v.rank];
                    Vector3 spawnPoint = SpawnPointMaster.GetChild(v.spawnPoint).position;

                    Instantiate(a, spawnPoint, Quaternion.identity);
                }
            }
        }
    }
}
