/* StateNPCNoCombat
 * NPC States to use outside of combat
 * 
 * Idle state
 * Moving state
 */
using UnityEngine;

public class StateNPCNoCombat
{
    /// <summary>
    /// The Unit's idle state.
    /// </summary>
    public class Idle : FSMState<NPCController>
    {
        // All states require references to the factor and machine
        public Idle(StateFactory<NPCController> sf, FiniteStateMachine<NPCController> snpc) : base(sf, snpc)
        { Factory = sf; Factory.StateMachine = snpc; }

        // When entering the state
        public override void Enter(NPCController t)
        {            
            // Unit leaves combat
            t.IsInCombat = false;

            // Idle in position
            t.agent.SetDestination(t.transform.position);

            // Animate
            t.animator.Play("Locomotion");
            t.animator.SetFloat("Speed", 0);

            // The amount of time to wait before moving.
            timer = Time.time + t.WaitTime;
        }

        // During the state
        public override void Execute(NPCController t)
        {
            // Always be checking for hostile targets
            GameObject go = Extensions.CheckForHostiles(t, t.LineOfSiteLength, t.LineOfSiteAngle, t.AggroRadius, true, false, typeof(IAlignment));

            // If HOSTILE target is found, enter Combat
            if (go != null)
            {
                t.target = go;
                ChangeState(States.NPC.Combat.MakeDecision);
            }

            // Move after waiting
            if (Time.time > timer)
                ChangeState(States.NPC.OutOfCombat.Moving);
        }

        // When exiting the state.
        public override void Exit(NPCController t)
        {
        }
    }

    /// <summary>
    /// The Unit's Moving state.
    /// </summary>
    public class Moving : FSMState<NPCController>
    {
        public Moving(StateFactory<NPCController> sf, FiniteStateMachine<NPCController> snpc) 
            : base(sf, snpc) { Factory = sf; Factory.StateMachine = snpc; }

        public override void Enter(NPCController t)
        {
            // Set animation
            t.animator.SetFloat("Speed", t.agent.velocity.magnitude);
            if (!t.animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))
                t.animator.Play("Locomotion");

            // Set the next destination to wander to
            t.agent.SetDestination(Extensions.WanderVision(t));
        }

        public override void Execute(NPCController t)
        {
            // Animate
            t.animator.SetFloat("Speed", t.agent.velocity.magnitude);

            // Always be checking for hostiles
            GameObject go = Extensions.CheckForHostiles(t, t.LineOfSiteLength, t.LineOfSiteAngle, t.AggroRadius, true, false, typeof(IAlignment));

            // If target is found, enter Combat
            if (go != null)
            {
                t.target = go;
                ChangeState(States.NPC.Combat.MakeDecision);
            }

            //Idle if at position, idle
            if (Vector3.Distance(t.agent.destination, t.transform.position) < .1f)
                ChangeState(States.NPC.OutOfCombat.Idle);
        }
        public override void Exit(NPCController t)
        {
        }
    }
}
