using System;

namespace Karzina.Common
{
    /// <summary>
    /// Represents a finite state event. Something that happens to cause your finite state machine to transition from the current state to another state.
    /// </summary>
    /// <remarks>
    /// Currently just an object container for a string.
    /// </remarks>
    public class FiniteStateEvent
    {
 
        /// <summary>
        /// The name of the event.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// This constructor is private so that users cannot create an instance without a name.
        /// </summary>
        private FiniteStateEvent() { }

        /// <summary>
        /// Create a FiniteStateEvent with a specific name.
        /// </summary>
        /// <param name="Name">The name of the event.</param>
        /// <exception cref="ArgumentException">Thrown if the value for Name is null, empty or consists only of white space.</exception>
        /// <exception cref="ArgumentException">Thrown if the value for Name starts or ends with white space.</exception>
        public FiniteStateEvent(string Name) : this()
        {
            this.Name = (new NameValidator(Name)).Name;
        }
    }
}
