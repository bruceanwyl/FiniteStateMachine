using System;

namespace Karzina.Common
{
    /// <summary>
    /// Performs the common validation of a Name.
    /// Used in:
    ///  * <see cref="FiniteState(string)"/>
    ///  * <see cref="FiniteStateEvent(string)"/>
    ///  * <see cref="FiniteStateMachine(string)"/>
    /// </summary>
    public class NameValidator
    {
        /// <summary>
        /// Gets the value of the Name value passed in the constructor.
        /// It will only contain a value if the validation checks pass.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Creates a NameValidator object with the given name.
        /// Validity is checked within the constructor.
        /// Name cannot be:
        ///  * a zero length string
        ///  * only white space characters
        ///  * string.Empty
        ///  * start or end with white space
        /// </summary>
        /// <param name="Name">A descriptive name.</param>
        /// <exception cref="ArgumentException">Thrown if the value for Name is null, empty or consists only of white space.</exception>
        /// <exception cref="ArgumentException">Thrown if the value for Name starts or ends with white space.</exception>
        public NameValidator(string Name) 
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("You must provide a non-empty value for Name.");

            if (!Name.Equals(Name.Trim()))
                throw new ArgumentException("The value for Name cannot start or end with white space.");

            this.Name = Name;
        }

    }
}
