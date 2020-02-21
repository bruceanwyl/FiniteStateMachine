using NUnit.Framework;

namespace Karzina.Common.FiniteStateMachineTests
{
    public class FiniteStateEventTest
    {
        [TestCase("ActionFailed")]
        public void TestConstructor(string Name)
        {
            FiniteStateEvent fse = new FiniteStateEvent(Name);
            Assert.That(fse.Name, Is.EqualTo(Name));
        }
    }
}
