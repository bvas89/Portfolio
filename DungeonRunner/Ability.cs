using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public NPCController Owner;
    public Alignment Alignment;
    public AnimationClip Clip;
    public float Cooldown = 1f;
    public bool isCoolingDown = false;
    public float MaxLength;
    public float MinLength;
    public float Radius;
    public int Priority;

    private void Start()
    {
        //Animate
    }

    private void Update()
    {

    }
}
