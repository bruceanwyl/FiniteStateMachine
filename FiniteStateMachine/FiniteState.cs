using System;
using System.Collections.Generic;

namespace Karzina.Common
{
    /// <summary>
    /// Represents a finite state.
    /// </summary>
    public class FiniteState
    {
        /// <summary>
        /// The name of this finite state.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The function delegate to execute when a transition is made to this state.
        /// </summary>
        public FiniteStateAction OnEnterAction { get; set; }

        /// <summary>
        /// Provides readonly access to the collection of transitions out of this finite state.
        /// </summary>
        public Dictionary<string, FiniteStateTransition> Transitions { get; private set; }

        /// <summary>
        /// The default constructor is hidden from users of this class.
        /// </summary>
        private FiniteState() { }

        /// <summary>
        /// Create a finite state with a given name. The name is used as a key in the collection
        /// of finite states managed by the finite state machine and hence, must be unique.
        /// </summary>
        /// <param name="Name">A descriptive name for this finite state.</param>
        /// <exception cref="ArgumentException">Thrown if the value for Name is null, empty or consists only of white space.</exception>
        /// <exception cref="ArgumentException">Thrown if the value for Name starts or ends with white space.</exception>
        public FiniteState(string Name) : this()
        {
            this.Name = (new NameValidator(Name)).Name;
            Transitions = new Dictionary<string, FiniteStateTransition>();
        }

        /// <summary>
        /// Create a finite state with a given name and OnEnter action. 
        /// The name is used as a key in the collection of finite states managed by the finite state machine
        /// and hence, must be unique.
        /// </summary>
        /// <param name="Name">A descriptive name for this finite state.</param>
        /// <param name="OnEnterAction">The function delegate to execute when a transition is made to this state.</param>
        /// <exception cref="ArgumentException">Thrown if the value for Name is null, empty or consists only of white space.</exception>
        /// <exception cref="ArgumentException">Thrown if the value for Name starts or ends with white space.</exception>
        /// <exception cref="ArgumentException">Thrown if the value for OnEnterAction is null.</exception>
        public FiniteState(string Name, FiniteStateAction OnEnterAction) : this()
        {
            this.Name = (new NameValidator(Name)).Name;
            this.OnEnterAction = OnEnterAction ?? throw new ArgumentException("OnEnterAction cannot be null");
            Transitions = new Dictionary<string, FiniteStateTransition>();
        }

        /// <summary>
        /// Add a new transition to the collection of transitions from this state that is created from an event and a state.
        /// </summary>
        /// <param name="ViaEvent">The event that will trigger this transition.</param>
        /// <param name="ToState">The state that is the destination of the transition.</param>
        /// <exception cref="ArgumentException">Thrown if the event in TransitionToAdd has already been used in a transition from this state.</exception>
        public void AddTransition(FiniteStateEvent ViaEvent, FiniteState ToState)
        {
            AddTransition(new FiniteStateTransition(ViaEvent, ToState));
        }

        /// <summary>
        /// Add a new transition to the collection of transitions from this state.
        /// </summary>
        /// <param name="TransitionToAdd">The transition to add.</param>
        /// <exception cref="ArgumentException">Thrown if the event in TransitionToAdd has already been used in a transition from this state.</exception>
        public void AddTransition(FiniteStateTransition TransitionToAdd)
        {
            string key = TransitionToAdd.ViaEvent.Name;
            if (Transitions.ContainsKey(key))
            {
                throw new ArgumentException($"Duplicate event name='{key}' used to create a transition from state name='{Name}'.");
            }
            Transitions.Add(key, TransitionToAdd);
        }

    }
}
