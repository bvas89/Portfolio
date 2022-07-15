
public class States
{
    StateFactory<NPCController> _factory;
    FiniteStateMachine<NPCController> _machine;
    public States(StateFactory<NPCController> factory, FiniteStateMachine<NPCController> machine)
    { _factory = factory; _machine = machine; }

     //SubClasses
    public _NPC<NPCController> NPC => new _NPC<NPCController>(_factory, _machine);

    // The NPC Classes to reference.
    public class _NPC<T> where T : NPCController
    {
        StateFactory<NPCController> f;
        FiniteStateMachine<NPCController> m;
        public _NPC(StateFactory<NPCController> curTX_, FiniteStateMachine<NPCController> curMach) { f = curTX_; m = curMach; }

        public _OutOfCombat<T> OutOfCombat => new _OutOfCombat<T>(f, m);
        public _Combat<T> Combat => new _Combat<T>(f, m);
        public _PostCombat<T> PostCombat => new _PostCombat<T>(f, m);


        public class _OutOfCombat<T> where T : NPCController
        {
            StateFactory<NPCController> fac;
            FiniteStateMachine<NPCController> mac;
            public _OutOfCombat(StateFactory<NPCController> curTX_, FiniteStateMachine<NPCController> curMach) { fac = curTX_; mac = curMach; }

            public StateNPCNoCombat.Idle Idle => new StateNPCNoCombat.Idle(fac, mac);
            public StateNPCNoCombat.Moving Moving => new StateNPCNoCombat.Moving(fac, mac);
        }

        public class _Combat<T> where T : NPCController
        {
            StateFactory<NPCController> fac;
            FiniteStateMachine<NPCController> mac;
            public _Combat(StateFactory<NPCController> curTX_, FiniteStateMachine<NPCController> curMach) { fac = curTX_; mac = curMach; }

            public StateNPCInCombat.MakeDecision MakeDecision => new StateNPCInCombat.MakeDecision(fac, mac);

            public StateNPCInCombat.MovingToPosition MovingToPosition => new StateNPCInCombat.MovingToPosition(fac, mac);
            public StateNPCInCombat.UseAttack UseAttack => new StateNPCInCombat.UseAttack(fac, mac);
            public StateNPCInCombat.RunToPreCombatPosition RunToPreCombatPosition => new StateNPCInCombat.RunToPreCombatPosition(fac, mac);
        }

        public class _PostCombat<T> where T : NPCController
        {
            StateFactory<NPCController> fac;
            FiniteStateMachine<NPCController> mac;
            public _PostCombat(StateFactory<NPCController> curTX_, FiniteStateMachine<NPCController> curMach) { fac = curTX_; mac = curMach; }

            public StateNPCPostCombat.PostCombatDecisionMaker MakeDecision => 
                new StateNPCPostCombat.PostCombatDecisionMaker(fac, mac);

            public StateNPCPostCombat.RunBackToLastPosition RunToLastPosition =>
                new StateNPCPostCombat.RunBackToLastPosition(fac, mac);
        }
    }
}
