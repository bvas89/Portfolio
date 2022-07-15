using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAttack : Ability
{
    public bool isUsing = true;

    private void Start()
    {
        isUsing = true;
        Cooldown = 5f;
    }

    private void Update()
    {
        if (Time.time > Cooldown)
            print("Done.");
    }
}
