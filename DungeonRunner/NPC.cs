using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPC : ScriptableObject
{
    [Header("NPC Stats")]
    [SerializeField] protected int _maxHP = 3;
    [SerializeField] protected float _waitTime = 2.5f;
    [Range(0,1)]
    [SerializeField] protected float _waitTimeModifier = 0f;
    [SerializeField] protected float _moveSpeed = 1f;
    [SerializeField] protected float _baseAttack = 3f;
    [SerializeField] public abstract bool trackTargetPos();
    [Tooltip("How far this unit can detect targets when it sees them.")]
    [SerializeField] protected float _lineOfSightLength = 5f;
    [SerializeField] protected float _lineOfSightAngle = 120f;
    [Tooltip("How close to check for non-seen targets; i.e., enemies behind the unit.")]
    [SerializeField] protected float _shortRadiusLength = 2.5f;
    [SerializeField] protected Ability[] _attacks;
    [SerializeField] protected AnimatorOverrideController _animatorOverrideController;
    [SerializeField] protected float _wanderRadius;

    public int MaxHP() { return _maxHP; }
    public float MoveSpeed() { return _moveSpeed; }
    public float LineOfSightLength() { return _lineOfSightLength; }
    public float LineOfSightAngle() { return _lineOfSightAngle; }
    public float ShortRadiusLength() { return _shortRadiusLength; }
    public float BaseAttack() { return _baseAttack; }
    public Ability[] Attacks() { return _attacks; }
    public AnimatorOverrideController AnimatorOverrideController() { return _animatorOverrideController; }
    public float WanderRadius() { return _wanderRadius; }


    public float WaitTime()
    {
        float low = _waitTime - (_waitTime * _waitTimeModifier);
        float high = _waitTime + (_waitTime * _waitTimeModifier);
        float waitTime = Random.Range(low, high);
        return waitTime;
    }

}
