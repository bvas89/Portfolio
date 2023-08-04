////////
/// GameController.cs
///
/// This script contains all the main functionality derived from GameData.
/// Primarily used for spawning obstacles and collectibles.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameData;

public class GameController : MonoBehaviour
{
    protected List<gdSpawnable> _spawnables = new List<gdSpawnable>();
    List<gdSpawnable> Spawnables = new List<gdSpawnable>();

    private void Start()
    {
        CreateMasterList();
        StartCoroutine(ChooseObjectToSpawn());
    }

    private void Update()
    {
        // Update the multiplier as time goes on
        if (!Data.Main.IsPaused) Data.Main.UpdateMultiplier();
    }

    /// <summary>
    /// Create a Master List of all the "Spawnable" classes.
    /// </summary>
    public void CreateMasterList()
    {
        //TODO Eventually automate this.
        //Create a Master List of Spawnables
        _spawnables.Add(Data.Empty);
        _spawnables.Add(Data.Coins);
        _spawnables.Add(Data.Gaps);
        _spawnables.Add(Data.PowerUps);

        // Fill the malleable list.
        Spawnables = _spawnables;
    }

    /// <summary>
    /// Choose which object to spawn
    /// </summary>
    /// <returns></returns>
    IEnumerator ChooseObjectToSpawn()
    {
        while (true)
        {
            if (!Data.Main.IsPaused)
            {
                // Spawn an object on the screen
                SpawnChosenItem(ChooseSpawnable(Spawnables.ToArray()));

                //Get a point for every beat (cadence) played
                Actions.Game.GetPoint(Data.Points.Cadence);

            }

            //Wait for next spawn
            yield return new WaitForSeconds(Data.Main.Cadence / Data.Main.Multiplier);
        }
    }

    /// Spawns the chosen item
    void SpawnChosenItem(gdSpawnable s)
    {
        StartCoroutine(RemoveObjectFromRotation(s));

        float posY = UnityEngine.Random.Range(-Data.Main.ScreenHeight / 2f, Data.Main.ScreenHeight / 2f);

        //TODO inspect X value of spawn position. May be too close to screen.
        Vector3 spawnPosition = new Vector3(Data.Main.ScreenWidth, posY, transform.position.z);

        var prefab = s.Prefab;

        /*
        // If Ability, choose which
        if (s.GetType() == typeof(gdPowerUps))
        {
            gdPowerUps.puAbilities[] abilities = GameData.PowerUps.Abilities;
            prefab = ChooseSpawnable(abilities).Ability.prefab;
        }
        */
        //if (s.Prefab != null)
        if (prefab != null)
        {
            GameObject o = Instantiate(prefab, spawnPosition, Quaternion.identity);
            o.GetComponent<MoveObject>().InitializeObject(s);
        }
    }

    /// <summary>
    /// Remove the item from the spawn list until its cooldown is over.
    /// </summary>
    /// <param name="s">The object to put on cooldown </param>
    /// <returns></returns>
    IEnumerator RemoveObjectFromRotation(gdSpawnable s)
    {
        // Find Wait Time
        Spawnables.Remove(s);
        yield return new WaitForSeconds(s.Cooldown);
        Spawnables.Add(s);
    }
}