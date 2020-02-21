namespace Karzina.Common
{
    /// <summary>
    /// Represents a finite state transition.
    /// </summary>
    public class FiniteStateTransition
    {
        /// <summary>
        /// The event that will cause this transition to occur.
        /// </summary>
        public FiniteStateEvent ViaEvent { get; private set; }

        /// <summary>
        /// The state of the finite state machine after the transition occurs.
        /// </summary>
        public FiniteState ToState { get; private set; }
        
        /// <summary>
        /// This constructor is hidden from users.
        /// </summary>
        private FiniteStateTransition() { }

        /// <summary>
        /// Creates a transition to another state via an event.
        /// </summary>
        /// <param name="ToState">FiniteState</param>
        /// <param name="ViaEvent">FiniteStateEvent</param>
        public FiniteStateTransition(FiniteStateEvent ViaEvent, FiniteState ToState) : this()
        {
            this.ViaEvent = ViaEvent;
            this.ToState = ToState;
        }
    }
}
