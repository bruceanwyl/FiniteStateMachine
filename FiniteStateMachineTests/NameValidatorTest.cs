using System;
using NUnit.Framework;

namespace Karzina.Common.FiniteStateMachineTests
{
    [TestFixture]
    public class NameValidatorTest
    {
        readonly string errMessageNonEmpty = "You must provide a non-empty value for Name.";
        readonly string errMessageStartEnd = "The value for Name cannot start or end with white space.";

        [TestCase("")]
        [TestCase(" ")]
        public void TestConstructorNonEmptyName(string Name)
        {
            Assert.That(() => { NameValidator nv = new NameValidator(Name); }, 
            Throws.InstanceOf<ArgumentException>().And.Message.EqualTo(errMessageNonEmpty));
        }

        [Test]
        public void TestConstructorNonEmptyName()
        {
            Assert.That(() => {
                NameValidator nv = new NameValidator(string.Empty);
            }, Throws.InstanceOf<ArgumentException>().And.Message.EqualTo(errMessageNonEmpty));
        }

        [TestCase("GettingWorkItem ")]
        [TestCase(" GettingWorkItem")]
        [TestCase(" GettingWorkItem ")]
        public void TestConstructorNameIsNotValid(string Name)
        {
            Assert.That(() => {
                NameValidator nv = new NameValidator(Name);
            }, Throws.InstanceOf<ArgumentException>().And.Message.EqualTo(errMessageStartEnd));
        }

        [TestCase("GettingWorkItem")]
        public void TestConstructorNameIsValid(string Name)
        {
            NameValidator nv = new NameValidator(Name);
            Assert.That(nv.Name, Is.EqualTo(Name));
        }
    }
}
