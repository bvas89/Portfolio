using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour, IDamageable, IAlignment
{    
    [Tooltip("The NPC to draw stats from.")]
    [SerializeField] protected NPC _ReferenceNPC;

    [Tooltip("The alignment of the unit.")]
    public Alignment Alignment = Alignment.Bad;

    [Tooltip("The unit's target.")]
    public GameObject target;

    [Tooltip("The abilities this unit can use.")]
    public Ability[] Abilities;

    //The unit's abilities' stats (Cooldown, etc).
    public List<ATK> atks;

    [Header("Stats")]
    public int MaxHealth;
    public int CurHealth;
    public float WaitTime => _ReferenceNPC.WaitTime();
    public bool IsInCombat = false;
    public float LineOfSiteLength;
    public float LineOfSiteAngle;
    public float AggroRadius;
    public Vector3 StartPosition, PositionBeforeCombat;
    public float WanderRadius;
    public float BaseAttack;

    //Components
    public IAlignment iAlignment;
    public StateFactory<NPCController> sFactory;
    public FiniteStateMachine<NPCController> sMachine;
    public States States;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public AnimatorOverrideController AnimatorOverrideController;

    // Interfaces
    public int HP { get { return CurHealth; } set { CurHealth = value; } }
    public Alignment alignment { get { return Alignment; } set { Alignment = value; } }

    private void Awake()
    {
        // Get components
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {        
        SetStats();
        InitializeStateFactory();

        // Pull from Ability references.
        atks = new List<ATK>();
        foreach (var v in Abilities)
        {
            ATK k = new ATK();
            k.atk = v;
            k.cd = v.Cooldown;
            k.prio = v.Priority;
            atks.Add(k);
        }

        // Set alignment
        iAlignment = this;
    }
    private void Update()
    {
        sFactory.Update();
        UpdateAbilityCooldowns();
    }

    //TODO: Overhaul how abilities are Stored/Called
    //TODO: ALL TIMERS: Set expiration time, rather than Counting Down.
    /// <summary>
    /// Runs cooldown timers for each ability.
    /// </summary>
    void UpdateAbilityCooldowns()
    {
        foreach (var v in atks)
        {
            if (v.isCoolingDown)
                v.cd -= Time.deltaTime;

            if (v.cd <= 0)
            {
                v.isCoolingDown = false;
                v.cd = v.atk.Cooldown;
            }
        }
    }

    /// <summary>
    /// Set this unit's stats to the NPC scriptable object
    /// </summary>
    void SetStats()
    {
        MaxHealth = _ReferenceNPC.MaxHP();
        CurHealth = MaxHealth;
        LineOfSiteAngle = _ReferenceNPC.LineOfSightAngle();
        LineOfSiteLength = _ReferenceNPC.LineOfSightLength();
        AggroRadius = _ReferenceNPC.ShortRadiusLength();
        BaseAttack = _ReferenceNPC.BaseAttack();
        StartPosition = transform.position;
        AnimatorOverrideController = _ReferenceNPC.AnimatorOverrideController();
        animator.runtimeAnimatorController = AnimatorOverrideController;
    }

    /// <summary>
    /// Initializes the Finite State Factory.
    /// </summary>
    void InitializeStateFactory()
    {
        sFactory = new StateFactory<NPCController>(new FiniteStateMachine<NPCController>());
        States = new States(sFactory, sFactory.StateMachine);
        sFactory.Initialize(this, States);
        sFactory.ChangeState(States.NPC.OutOfCombat.Idle);
    }
    
    // Take damage. May move to an NPC Combat script.
    public void TakeDamage(float amount)
    {
        HP -= (int)amount;
    }

    // Deal damage.
    public void DealDamage(IDamageable target)
    {

    }

    /// <summary>
    /// Uses the chosen ability.
    /// </summary>
    /// <param name="attack">The ability/attack being used.</param>
    public void UseAttack(ATK attack)
    {
        Ability a = attack.atk;
        attack.isCoolingDown = true;

        a.Owner = this;
        a.Alignment = Alignment;
        Quaternion q = transform.rotation;
        Instantiate(a,transform.position,q);
    }    

    private void OnDrawGizmos()
    {
        //Wander Radius
       // Gizmos.DrawWireSphere(StartPosition, WanderRadius);

        //Combat Radius/FoV indicators
        {
            /*
          //  Gizmos.DrawWireSphere(transform.position, _ReferenceNPC.LineOfSightLength());
            Gizmos.DrawWireSphere(transform.position, _ReferenceNPC.ShortRadiusLength());

            Quaternion ang = Quaternion.AngleAxis(-LineOfSiteAngle / 2, new Vector3(0, 1, 0));
            Vector3 na = ang * Vector3.forward * 10;
            Vector3 da = transform.TransformDirection(na);

            Quaternion ang1 = Quaternion.AngleAxis(LineOfSiteAngle / 2, new Vector3(0, 1, 0));
            Vector3 na1 = ang1 * Vector3.forward * 10;
            Vector3 da1 = transform.TransformDirection(na1);

            Quaternion ang2 = Quaternion.AngleAxis(0, new Vector3(0, 1, 0));
            Vector3 na2 = ang2 * Vector3.forward * 10;
            Vector3 da2 = transform.TransformDirection(na2);

            Ray rayL = new Ray(transform.position, da);
            Debug.DrawRay(rayL.origin, rayL.direction * 10);

            Ray rayR = new Ray(transform.position, da1);
            Debug.DrawRay(rayR.origin, rayR.direction * 10);

            Ray rayF = new Ray(transform.position, da2);
            Debug.DrawLine(rayF.GetPoint(10), rayR.GetPoint(10));
            Debug.DrawLine(rayF.GetPoint(10), rayL.GetPoint(10));
            */
        }
    }
}

// Relevent information for the abilities
public class ATK
{
    public Ability atk;
    public bool isCoolingDown;
    public float cd;
    public int prio;
}