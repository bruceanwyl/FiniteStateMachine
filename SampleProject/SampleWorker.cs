using System;

namespace Karzina.Common
{

    public class SampleWorker : ThreadedWorker
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private int counter = 0;

        private StateMachineContainer fsm = new StateMachineContainer();
        private class StateMachineContainer
        {
            private readonly FiniteStateMachine engine = new FiniteStateMachine("DoorManager");

            public FiniteState OpeningDoor { get; private set; }
            public FiniteState ClosingDoor { get; private set; }
            public FiniteState Stopping { get; private set; }

            public FiniteStateEvent DoorOpened { get; private set; }
            public FiniteStateEvent DoorClosed { get; private set; }
            public FiniteStateEvent ActionFailed { get; private set; }

            public StateMachineContainer()
            {
                //
                //  The underlying finite state machine engine needs a list of all 
                //  the states. It gets them as we "Add" them here.
                //
                OpeningDoor = engine.AddState("OpeningDoor");
                ClosingDoor = engine.AddState("ClosingDoor");
                Stopping = engine.AddState("Stopping");
                //
                //  Alternatively
                //OpeningDoor = engine.AddState(new FiniteState("OpeningDoor"));
                //ClosingDoor = engine.AddState(new FiniteState("ClosingDoor"));
                //  Alternatively (more)
                //Stopping = new FiniteState("Stopping");
                //engine.AddState(Stopping);

                //
                //  Events are not "added" to the state machine directly.
                //  However, they are effectively added to each state when state transitions are defined.
                //
                DoorOpened = new FiniteStateEvent("DoorOpened");
                DoorClosed = new FiniteStateEvent("DoorClosed");
                ActionFailed = new FiniteStateEvent("ActionFailed");

            }

            public void despatch(FiniteStateEvent e)
            {
                engine.RaiseEvent(e);
            }

            public void start(FiniteState startState)
            {
                engine.Start(startState);
            }

        }

        private void initialiseStateMachine()
        {
            //
            //  OnEnterAction assignments must be done here because the handler
            //  methods are out of scope in the StateMachineContainer class.
            //
            fsm.OpeningDoor.OnEnterAction = OpeningDoorHandler;
            fsm.ClosingDoor.OnEnterAction = ClosingDoorHandler;
            fsm.Stopping.OnEnterAction = StoppingHandler;

            //
            //  Transitions are added to the individual FiniteState objects and so, can
            //  be defined either here or inside the StateMachineContainer constructor.
            //
            fsm.ClosingDoor.AddTransition(fsm.DoorClosed, fsm.OpeningDoor);
            fsm.OpeningDoor.AddTransition(fsm.DoorOpened, fsm.ClosingDoor);
            fsm.OpeningDoor.AddTransition(fsm.ActionFailed, fsm.Stopping);
        }

        public override void DoWork()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (log.IsDebugEnabled)
                log.DebugFormat("{0}: enter", methodName);

            initialiseStateMachine();
            fsm.start(fsm.OpeningDoor);

            if (log.IsDebugEnabled)
                log.DebugFormat("{0}: exit", methodName);
        }

        private void OpeningDoorHandler()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (log.IsDebugEnabled)
                log.DebugFormat("{0}: enter", methodName);

            if (counter++ < 10)
            {
                fsm.despatch(fsm.DoorOpened);
            }
            else
            {
                fsm.despatch(fsm.ActionFailed);
            }

            if (log.IsDebugEnabled)
                log.DebugFormat("{0}: exit", methodName);
        }
        private void ClosingDoorHandler()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (log.IsDebugEnabled)
                log.DebugFormat("{0}: enter", methodName);

            fsm.despatch(fsm.DoorClosed);

            if (log.IsDebugEnabled)
                log.DebugFormat("{0}: exit", methodName);
        }
        private void StoppingHandler()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (log.IsDebugEnabled)
                log.DebugFormat("{0}: enter", methodName);

            //  Does not despatch an event so the statemachine exits

            if (log.IsDebugEnabled)
                log.DebugFormat("{0}: exit", methodName);
        }
    }
}
