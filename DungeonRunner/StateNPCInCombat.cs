/*  StateNPCInCombat
 * This houses the 4 Major states of an NPC being in combat
 * 
 * -MakeDecision - Acts as an idle state. Chooses the next attack and where to position itself
 * -MovingToPosition - Moves to where the attack needs it to be.
 * -UseAttack - Uses the chosen attack
 * -RunToPreCombatPosition - Hastily runs back to where it was before combat
 */
using UnityEngine;

public class StateNPCInCombat
{
    /// <summary>
    /// Combat Idle State, Chooses next Ability
    /// </summary>
    public class MakeDecision : FSMState<NPCController>
    {
        public MakeDecision(StateFactory<NPCController> fac, FiniteStateMachine<NPCController> mac) : base(fac, mac) { Factory = fac; Factory.StateMachine = mac; }

        public override void Enter(NPCController t)
        {
            t.animator.SetFloat("Speed", 0);
            if (!t.animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))
                t.animator.Play("Locomotion");

            //TODO: Make a more robust timer.
            //The time this unit spends deciding what to do next.
            timer = 1f;

            // If entering combat, set Pre-Combat position
            if (!t.IsInCombat)
            {
                t.IsInCombat = true;
                t.PositionBeforeCombat = t.transform.position;
            }

            //Check for Hostile targets.
            if (t.target == null)
            {
                GameObject go = Extensions.CheckForHostiles(t, t.LineOfSiteLength, t.LineOfSiteAngle, t.AggroRadius, true, false, typeof(IAlignment));

                if(go == null)
                    ChangeState(States.NPC.OutOfCombat.Idle);
                else
                t.target = go;
            }
        }

        public override void Execute(NPCController t)
        {
            //Cooldown
            if (timer > 0)
                timer -= Time.deltaTime;
            else //When decision has been made
            {
                // If all attacks are on Cooldown, Make another decision
                int j = 0;
                for (int i = 0; i < t.atks.Count; i++)
                {
                    Debug.Log("INSIDE");
                    if (t.atks[i].isCoolingDown == false)
                        j++;

                    if (j > 0) break;
                }

                //If at least one ability is ready
                if (j > 0)
                {
                    //Gets the index of the ability to be used
                    Extensions.WeightedRandom(t.atks, out int index);
                    base.index = index;

                    //Only continue if a target is had
                    if (t.target != null)
                    { ChangeState(States.NPC.Combat.MovingToPosition); }
                    else //If no target, run back
                    { ChangeState(States.NPC.Combat.RunToPreCombatPosition); }
                }
                else //Otherwise make another decision (Wait.)
                {
                    ChangeState(States.NPC.Combat.MakeDecision);
                }
            }
        }

        public override void Exit(NPCController t)
        {
        }
    }

    /// <summary>
    /// Move close enough to attack.
    /// </summary>
    public class MovingToPosition : FSMState<NPCController>
    {
        public MovingToPosition(StateFactory<NPCController> sf, FiniteStateMachine<NPCController> snpc) : base(sf, snpc) { Factory = sf; Factory.StateMachine = snpc; }

        public override void Enter(NPCController t)
        {
            // Play Locomotion animation if not already.
            if (!t.animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))
                t.animator.Play("Locomotion");
        }

        public override void Execute(NPCController t)
        {
            // Takes the next ability and decides where to go
            t.agent.SetDestination(Extensions.AbilityPosition(t, t.atks[index].atk));
            t.animator.SetFloat("Speed", t.agent.velocity.magnitude);

            // Use the attack it had ready.
            if (t.agent.remainingDistance < .1f)
                ChangeState(States.NPC.Combat.UseAttack);
        }

        public override void Exit(NPCController t)
        {
        }
    }

    /// <summary>
    /// Uses the determined Ability
    /// </summary>
    public class UseAttack : FSMState<NPCController>
    {
        public UseAttack(StateFactory<NPCController> sf, FiniteStateMachine<NPCController> snpc) : base(sf, snpc) { Factory = sf; Factory.StateMachine = snpc; }

        public override void Enter(NPCController t)
        {
            // How long to stay in this State
            // Will probably be overhauled later
            timer = t.atks[index].atk.Clip.length;

            // Create new AOC to include modular Attack animation clip.
            AnimatorOverrideController aoc = new AnimatorOverrideController(t.animator.runtimeAnimatorController);
            t.animator.runtimeAnimatorController = aoc;

            aoc["Attack"] = t.atks[index].atk.Clip;

            if (!t.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                t.animator.SetTrigger("Attack");

            // Use the selected Attack/Ability
            t.UseAttack(t.atks[index]);
        }

        public override void Execute(NPCController t)
        {
            // Go back to MakeDecision once the timer has expired.
            if (timer > 0) timer -= Time.deltaTime;
            else ChangeState(States.NPC.Combat.MakeDecision);
        }

        public override void Exit(NPCController t)
        {
            //Reset AnimatorController to the original
            //Not sure if necessary. Will play around with this later.
            t.animator.runtimeAnimatorController = t.AnimatorOverrideController;
        }
    }

    /// <summary>
    /// Runs back where it was before combat - I.E. No more enemies around.
    /// </summary>
    public class RunToPreCombatPosition : FSMState<NPCController>
    {
        public RunToPreCombatPosition(StateFactory<NPCController> sf, FiniteStateMachine<NPCController> snpc) : base(sf, snpc) { Factory = sf; Factory.StateMachine = snpc; }

        // References to increase velocity
        float speed;
        float accel;

        public override void Enter(NPCController t)
        {
            //Ascertain there is no target, for sake of accidents
            t.target = null;

            // Set velocity references
            speed = t.agent.speed;
            accel = t.agent.acceleration;

            //Set a brief pause
            timer = 2f;
        }

        public override void Execute(NPCController t)
        {
            if (timer > 0)
            {
                //A brief pause
                t.agent.SetDestination(t.transform.position);
                timer -= Time.deltaTime;
            }
            else
            {
                // Hastily run back to position
                t.agent.SetDestination(t.PositionBeforeCombat);
                t.agent.speed = speed * 3;
                t.agent.acceleration = accel * 3;

                // Change to Idle state
            if (Vector3.Distance(t.agent.destination,t.transform.position) < .1f)
                ChangeState(States.NPC.OutOfCombat.Idle);
            }
        }

        public override void Exit(NPCController t)
        {
            // Revert references
            t.agent.speed = speed; t.agent.acceleration = accel;
            t.IsInCombat = false;
        }
    }
}