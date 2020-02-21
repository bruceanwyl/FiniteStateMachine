using System;

namespace Karzina.Common
{
    /// <summary>
    /// Represents a very simple finite state machine.
    /// 
    /// A complete implementation would support:
    ///  * guarded transitions
    ///  * an OnEntry event for each state
    ///  * an OnExit event handler for each state
    ///  * an OnTransition event handler for each transition
    /// 
    /// This implementation supports:
    ///   * an OnEntry event for each state
    ///   
    /// Even so, you can use this component to build an application using the (finite) state pattern.
    /// 
    /// You can:
    ///   * Add the states that represent your finite state machine. See <see cref="AddState(FiniteState)"/>.
    ///   * Start the finite state machine in it's initial state. See <see cref="Start(FiniteState)"/>.
    ///   * Raise a finite state event. See <see cref="RaiseEvent(FiniteStateEvent)"/>
    /// </summary>
    public class FiniteStateMachine
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private FiniteStateMachineEngine engine;

        /// <summary>
        /// Gets the name of the finite state machine instance.
        /// </summary>
        public string Name
        {
            get { return engine.Name; }
        }

        /// <summary>
        /// The default constructor is hidden from users because we insist on a name for our state machine.
        /// </summary>
        private FiniteStateMachine() { }

        /// <summary>
        /// Creates a finite state machine object with the given name.
        /// </summary>
        /// <param name="Name">A descriptive name for this state machine.</param>
        /// <exception cref="ArgumentException">Thrown if the value for Name is null, empty or consists only of white space.</exception>
        /// <exception cref="ArgumentException">Thrown if the value for Name starts or ends with white space.</exception>
        public FiniteStateMachine(string Name) : this()
        {
            engine = new FiniteStateMachineEngine(Name);
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

                return engine.AddState(StateToAdd);
            }
            finally
            {
                if (log.IsDebugEnabled)
                    log.Debug($"{methodName}: exit");
            }
        }

        /// <summary>
        /// Create and add a new FiniteState to the collection of states managed by this state machine.
        /// </summary>
        /// <param name="Name">The name finite state to create and add.</param>
        /// <returns>A reference to the finite state that was just created and added.</returns>
        /// <exception cref="ArgumentException">Thrown if you try to add a finite state that has already been added.</exception>
        public FiniteState AddState(string Name)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            try
            {
                if (log.IsDebugEnabled)
                    log.Debug($"{methodName}: enter");
                FiniteState stateToAdd = new FiniteState(Name);
                return engine.AddState(stateToAdd);
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

            engine.RaiseEvent(e);

            if (log.IsDebugEnabled)
                log.Debug($"{methodName}: exit");
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
        /// </summary>
        /// <param name="InitialState">The finite state in which this state machine starts.</param>
        public void Start(FiniteState InitialState)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (log.IsDebugEnabled)
                log.Debug($"{methodName}: enter");

            engine.Start(InitialState);

            if (log.IsDebugEnabled)
                log.Debug($"{methodName}: exit");

        }

        ///// <summary>
        ///// Provides a mechanism to stop the finite state machine.
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

        //    engine.Stop();

        //    if (log.IsDebugEnabled)
        //        log.Debug($"{methodName}: exit");
        //}

    }
}
