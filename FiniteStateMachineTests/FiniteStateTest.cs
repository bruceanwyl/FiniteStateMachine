using NUnit.Framework;
using System;

namespace Karzina.Common.FiniteStateMachineTests
{
    [TestFixture]
    public class FiniteStateTest
    {
        [TestCase("GettingWorkItem")]
        public void TestConstructorNameIsValid(string Name)
        {
            FiniteState fs = new FiniteState(Name);
            Assert.That(fs.Name, Is.EqualTo(Name));
        }

        [Test]
        public void TestConstructorInitialisesTransitionsCollection()
        {
            FiniteState fs = new FiniteState("GettingWorkItem");
            Assert.That(fs.Transitions.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestAddOneTransition()
        {
            FiniteState gettingWorkItem = new FiniteState("GettingWorkItem");
            FiniteState movingToErrors = new FiniteState("MovingToErrors");

            FiniteStateEvent actionFailed = new FiniteStateEvent("ActionFailed");

            gettingWorkItem.AddTransition(actionFailed, movingToErrors);

            Assert.That(gettingWorkItem.Transitions.Count, Is.EqualTo(1));
            Assert.That(gettingWorkItem.Transitions.ContainsKey("ActionFailed"));
        }

        [Test]
        public void TestAddTwoTransitions()
        {
            FiniteState gettingWorkItem = new FiniteState("GettingWorkItem");
            FiniteState movingToErrors = new FiniteState("MovingToErrors");
            FiniteState movingToProcessed = new FiniteState("MovingToProcessed");

            FiniteStateEvent actionFailed = new FiniteStateEvent("ActionFailed");
            FiniteStateEvent actionSucceeded = new FiniteStateEvent("ActionSucceeded");

            gettingWorkItem.AddTransition(actionSucceeded, movingToProcessed);
            gettingWorkItem.AddTransition(actionFailed, movingToErrors);

            Assert.That(gettingWorkItem.Transitions.Count, Is.EqualTo(2));
            Assert.That(gettingWorkItem.Transitions.ContainsKey("ActionFailed"));
            Assert.That(gettingWorkItem.Transitions.ContainsKey("ActionSucceeded"));
        }

        [Test]
        public void TestAddDuplicateTransition()
        {
            FiniteState gettingWorkItem = new FiniteState("GettingWorkItem");
            FiniteState movingToErrors = new FiniteState("MovingToErrors");
            FiniteState movingToProcessed = new FiniteState("MovingToProcessed");

            FiniteStateEvent actionSucceeded = new FiniteStateEvent("ActionSucceeded");

            Assert.That( () => {
                gettingWorkItem.AddTransition(actionSucceeded, movingToProcessed);
                gettingWorkItem.AddTransition(actionSucceeded, movingToErrors);
            }, Throws.InstanceOf<ArgumentException>().And.Message.EqualTo($"Duplicate event name='{actionSucceeded.Name}' used to create a transition from state name='{gettingWorkItem.Name}'."));

        }

        //  a global variable manipulated by MovingToErrorsHandler function below
        int callCount = 0;
        [Test]
        public void TestAddOnEnterAction()
        {
            FiniteState movingToErrors = new FiniteState("MovingToErrors");

            Assert.That(movingToErrors.OnEnterAction, Is.Null);

            movingToErrors.OnEnterAction = MovingToErrorsHandler;
            Assert.That(movingToErrors.OnEnterAction, Is.InstanceOf<FiniteStateAction>());

            //
            //  Now check that the OnEnterAction is callable via the delegate.
            //  It manipulates the global int "callCount".
            //
            Assert.That(callCount, Is.EqualTo(0));
            movingToErrors.OnEnterAction();
            Assert.That(callCount, Is.EqualTo(1));
        }
        // OnEnterAction delegate for TestAddOnEnterAction
        private void MovingToErrorsHandler()
        {
            callCount++;
        }
    }
}
