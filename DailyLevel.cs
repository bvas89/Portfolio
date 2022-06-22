/*  DailyLevel.cs
 * 
 * Creates a single Level object to be sent to the DailySpawner.
 * Relays which powerups to send.
 * Returns a list of ants to spawn.
 * 
 */

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Daily Level", menuName = "Daily Level", order = 0)]
public class DailyLevel : ScriptableObject
{
    [Tooltip("How long the level lasts in seconds.")]
    public float TotalDuration = 215f;

    [Tooltip("The 3 abilities selected for the player today. Will eventually be procedural.")]
    public PowerUp[] Abilities = new PowerUp[3];

    [Tooltip("The methods to construct the Ant List from.")]
    [SerializeField] public SpawnMethod[] Methods;


    /// <summary>
    /// Returns a list of all the ants in each Method.
    /// </summary>
    public List<SpawnMethod.Ant> ConvertToList()
    {
        List<SpawnMethod.Ant> list = new List<SpawnMethod.Ant>();

        for (int i = 0; i < Methods.Length; i++)
        {
            for (int j = 0; j < Methods[i].AntsList().Count; j++)
            {
                list.Add(Methods[i].AntsList()[j]);
            }
        }
        return list;
    }
}

//Available PowerUps to choose from.
public enum PowerUp
{
   // None,
    IceCube,
    Orange,
    MagGlass,
    Nuke,
    HoneyPot,
    AntTrap,
    Balloon,
   // Carrot,
    FlySwatter,
    //Jelly,
    PeanutButter,
   // Squeegee
}