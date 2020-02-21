using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Karzina.Common
{
    /// <summary>
    /// Creates the underlying engine of the finite state machine.
    /// This class contains what was the private functionality of the
    /// <see cref="FiniteStateMachine"/> class. It is exposed publicly
    /// by this class so that it can be tested.
    /// The private functionality of <see cref="FiniteStateMachine"/> is delegated to
    /// a private instance of this class.
    /// </summary>
    public class FiniteStateMachineEngine
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string START_STATE_NAME = "FiniteStateMachine.StartState";
        private const string START_EVENT_NAME = "FiniteStateMachine.StartEvent";

        //private bool stopRequested = false;
        private bool handlingEvent = false;
        private readonly object locker = new object();

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="e"></param>
        //public delegate void FiniteStateMachineExceptionEvent(Exception e);

        ///// <summary>
        ///// 
        ///// </summary>
        //public FiniteStateMachineExceptionEvent OnExceptionAction { get; set; }

        /// <summary>
        /// Gets the collection of FiniteStates managed by this class.
        /// </summary>
        public Dictionary<string, FiniteState> ManagedStates { get; private set; }

        /// <summary>
        /// Gets the current state of the finite state machine engine.
        /// </summary>
        public FiniteState CurrentState { get; private set; }

        /// <summary>
        /// Gets the collection of PendingEvents managed by this class.
        /// </summary>
        public Queue<FiniteStateEvent> PendingEvents { get; private set; }

        /// <summary>
        /// Gets the Name of this object.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// We hide this constructor because we insist on a name. 
        /// However, other constructors must call this one.
        /// </summary>
        private FiniteStateMachineEngine()
        {
            ManagedStates = new Dictionary<string, FiniteState>();
            PendingEvents = new Queue<FiniteStateEvent>();
        }

        /// <summary>
        /// Creates a FiniteStateMachineEngine with the given name.
        /// 
        /// The name will be used to identify the Thread in which the finite state machine event handlers run.
        /// </summary>
        /// <param name="Name">A descriptive name for this state machine.</param>
        /// <exception cref="ArgumentException">Thrown if the value for Name is null, empty or consists only of white space.</exception>
        /// <exception cref="ArgumentException">Thrown if the value for Name starts or ends with white space.</exception>
        public FiniteStateMachineEngine(string Name) : this()
        {
            this.Name = (new NameValidator(Name)).Name;
        }

        /// <summary>
        /// Add a new FiniteState to the collection of states managed by this state machine.
        /// </summary>
        /// <param name="StateToAdd">The finite state to add.</param>
        /// <returns>A reference to the finite state that was just added.</returns>
        /// <exception cref="ArgumentException">Thrown if you try to add a finite state that has already been added.</exception>
        public FiniteState AddState(FiniteState StateToAdd)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            try
            {
                if (log.IsDebugEnabled)
                    log.Debug($"{methodName}: enter");

                if (ManagedStates.ContainsKey(StateToAdd.Name))
                {
                    throw new ArgumentException($"Attempt to add a duplicate state '{StateToAdd.Name}'.");
                }

                if (log.IsInfoEnabled)
                    log.Info($"{methodName}: [{StateToAdd.Name}]");

                ManagedStates.Add(StateToAdd.Name, StateToAdd);
                return StateToAdd;
            }
            finally
            {
                if (log.IsDebugEnabled)
                    log.Debug($"{methodName}: exit");
            }
        }

        /// <summary>
        /// Raises an event in the finite state machine.
        /// </summary>
        /// <param name="e">The instance of the event to raise</param>
        public void RaiseEvent(FiniteStateEvent e)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (log.IsDebugEnabled)
                log.Debug($"{methodName}: enter");
            try
            {
                if (log.IsInfoEnabled)
                    log.Info($"{methodName}: [{e.Name}]");
                //
                //  Use a queue so that event handlers can raise events.
                //
                lock (PendingEvents)
                {
                    PendingEvents.Enqueue(e);

                    if (log.IsDebugEnabled)
                        log.Debug($"{methodName}: handlingEvent = {handlingEvent}");

                    if (handlingEvent)
                    {
                        return;
                    }
                    handlingEvent = true;
                }
                HandlePendingEvents();
                // eliminate race for the flag by using the same lock object
                lock (PendingEvents)
                {
                    handlingEvent = false;
                }
            }
            finally
            {
                if (log.IsDebugEnabled)
                    log.Debug($"{methodName}: exit");
            }
        }

        /// <summary>
        /// Starts the state machine. 
        /// 
        /// Every state diagram has a start state with a single transition to some initial state in the diagram via a start event.
        /// This method performs the following actions:
        ///  * creates the start state
        ///  * creates the start event
        ///  * adds a transition from the start state, via the start event to InitialState, the finite state passed as a parameter
        ///  * sets the current state to be the start state
        ///  * raises the start event
        ///    The OnEntry event handler for InitialState will be executed.
        ///    The state machine is now sitting in it's initial state and waiting for an event to occur. 
        /// 
        /// On a technical note, the underlying Thread, in which the event handlers are run, is also created and started here.
        /// </summary>
        /// <param name="InitialState">The finite state in which this state machine starts.</param>
        /// <exception cref="ArgumentException">Thrown if the parameter InitialState is a finite state that has not been added by <see cref="AddState(FiniteState)"/></exception>
        public void Start(FiniteState InitialState)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (log.IsDebugEnabled)
                log.Debug($"{methodName}: enter");

            //if (OnExceptionAction == null)
            //{
            //    throw new ArgumentException($"The state machine must have an OnExceptionAction before you can start it.");
            //}

            //
            //  Perform sanity checks
            //
            foreach(KeyValuePair<string, FiniteState> kvp in ManagedStates)
            {
                //
                //  1. Make sure each finite state has an OnEnterAction
                //
                FiniteState finiteState = kvp.Value;
                if (finiteState.OnEnterAction == null)
                {
                    throw new Exception($"The finite state, '{finiteState.Name}' does not have an OnEnterAction.");
                }
                //
                //  2. Make sure the target state in every transition has been added to the state machine.
                //
                foreach(KeyValuePair<string, FiniteStateTransition> transitionKvp in finiteState.Transitions)
                {
                    FiniteStateTransition fst = transitionKvp.Value;
                    string stateName = fst.ToState.Name;
                    if (!ManagedStates.ContainsKey(stateName))
                    {
                        throw new Exception($"The finite state, '{stateName}' has not been added to the state machine.");
                    }
                }
            }

            FiniteState startState = new FiniteState(START_STATE_NAME);
            FiniteStateEvent startEvent = new FiniteStateEvent(START_EVENT_NAME);

            startState.OnEnterAction = () => { }; // needs an empty handler
            AddState(startState);
            startState.AddTransition(startEvent, InitialState);
            SetCurrentState(startState);
            RaiseEvent(startEvent);

            //Thread runner = new Thread(RunStateMachine)
            //{
            //    Name = Name
            //};
            //runner.Start();

            if (log.IsDebugEnabled)
                log.Debug($"{methodName}: exit");
        }

        ///// <summary>
        ///// Provides a mechanism to stop the finite state machine.
        ///// It will cause the method <see cref="RunStateMachine()"/> to complete so that the thread in which it runs can stop too.
        ///// 
        ///// Note that for finite state machine designs that raise events in the event handlers,
        ///// you must have a Stopping event handler as part of your state machine design.
        ///// The code in the Stopping event must not raise any events and Stop() should be called from within that handler.
        ///// </summary>
        //public void Stop()
        //{
        //    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //    if (log.IsDebugEnabled)
        //        log.Debug($"{methodName}: enter");

        //    stopRequested = true;
        //    lock (locker)
        //    {
        //        Monitor.Pulse(locker); // Wakes up the state machine thread if it is sleeping.
        //    }

        //    if (log.IsDebugEnabled)
        //        log.Debug($"{methodName}: exit");
        //}

        /// <summary>
        /// Sets the current state of the finite state machine.
        /// </summary>
        /// <param name="toState">The finite state that we want to become the current state.</param>
        /// <exception cref="ArgumentException">Thrown if the parameter toState is a finite state that has not been added by <see cref="AddState(FiniteState)"/></exception>
        public void SetCurrentState(FiniteState toState)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (log.IsDebugEnabled)
                log.Debug($"{methodName}: enter");

            if (!ManagedStates.TryGetValue(toState.Name, out FiniteState currentStateCandidate))
            {
                throw new ArgumentException($"Attempted to set current state to '{toState.Name}' but no state of that name is managed by this state machine.");
            }
            CurrentState = currentStateCandidate;

            if (log.IsInfoEnabled)
                log.Info($"{methodName}: changed to [{CurrentState.Name}]");

            if (log.IsDebugEnabled)
                log.Debug($"{methodName}: exit");
        }

        ///// <summary>
        ///// RunStateMachine is the heart of the engine that runs the finite state machine. It is executed in a  
        ///// separate thread that is started when you <see cref="Start(FiniteState)"/> the state machine.
        ///// All events that have been queued by <see cref="RaiseEvent(FiniteStateEvent)"/>, are processed here.
        ///// The method runs in an infinite loop that only exits when <see cref="Stop()"/> is called. 
        ///// When it has processed all of the events in the queue, it will sleep until it is pulsed 
        ///// by either <see cref="RaiseEvent(FiniteStateEvent)"/> or <see cref="Stop()"/>
        ///// </summary>
        //public void RunStateMachine()
        //{
        //    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //    if (log.IsDebugEnabled)
        //        log.Debug($"{methodName}: enter");
        //
        //    try
        //    {
        //        while (!stopRequested)
        //        {
        //            HandlePendingEvents();
        //            if (!stopRequested)
        //            {
        //                lock (locker)
        //                {
        //                    if (log.IsDebugEnabled)
        //                        log.Debug($"{methodName}: Putting to sleep the state machine thread.");
        //
        //                    Monitor.Wait(locker); // Nothing to do, so go to sleep until we are pulsed by RaiseEvent or Stop
        //
        //                    if (log.IsDebugEnabled)
        //                        log.Debug($"{methodName}: Just woke up :-)");
        //                }
        //            }
        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        log.Error($"{e.Message}");
        //        //OnExceptionAction?.Invoke(e);
        //    }
        //    finally
        //    {
        //        if (log.IsDebugEnabled)
        //            log.Debug($"{methodName}: exit");
        //    }
        //}
        //

        /// <summary>
        /// This function is called during the first call to RaiseEvent() in Start() because at that point,
        /// we are not handling a previous event. From then on, every event handler raises an event.
        /// </summary>
        public void HandlePendingEvents()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (log.IsDebugEnabled)
                log.Debug($"{methodName}: enter");

            try
            {
                while (PendingEvents.Count > 0)
                {
                    if (log.IsDebugEnabled)
                        log.Debug($"{methodName}: PendingEvents.Count={PendingEvents.Count}");
                    FiniteStateEvent ev = null;
                    lock (PendingEvents)
                    {
                        ev = PendingEvents.Dequeue();
                    }
                    if (CurrentState.Transitions.TryGetValue(ev.Name, out FiniteStateTransition transition))
                    {
                        if (log.IsDebugEnabled)
                            log.Debug($"{methodName}: transition {CurrentState.Name} -> {ev.Name} ->{transition.ToState.Name}");
                        SetCurrentState(transition.ToState);
                        if (log.IsDebugEnabled)
                            log.Debug($"{methodName}: before invoke, PendingEvents.Count={PendingEvents.Count}");
                        CurrentState.OnEnterAction?.Invoke();
                        if (log.IsDebugEnabled)
                            log.Debug($"{methodName}: after invoke, PendingEvents.Count={PendingEvents.Count}");
                    }
                    else
                    {
                        throw new ArgumentException($"Failed to find a transition for event '{ev.Name}' from state '{CurrentState.Name}'.");
                    }
                }
            }
            finally
            {
                if (log.IsDebugEnabled)
                    log.Debug($"{methodName}: exit");
            }
        }
    }

}
