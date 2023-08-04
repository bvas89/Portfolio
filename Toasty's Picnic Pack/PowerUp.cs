using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GameData;

public class PowerUp : Collectible
{
    // The ability to be used
    Ability ability;

    public void Start()
    {
        // Get which ability this power up is and attach the appropriate sprite
        ability = ChooseSpawnable(Data.PowerUps.Abilities).Ability;
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        sr.sprite = ability.sprite;
        sr.color = ability.color;
    }

    public override void Collect()
    {
        //Instantiate(ability.prefab, GameData.instance.transform); ;
        Instantiate(ability.prefab, Data.transform);
        Destroy(gameObject);
    }

}
