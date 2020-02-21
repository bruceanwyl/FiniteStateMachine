using System;
using NUnit.Framework;

namespace Karzina.Common.FiniteStateMachineTests
{
    [TestFixture]
    public class FiniteStateMachineEngineTest
    {
        [TestCase("Turnstile")]
        public void TestConstructorWithValidName(string Name)
        {
            FiniteStateMachineEngine engine = new FiniteStateMachineEngine(Name);
            Assert.That(engine.Name, Is.EqualTo(Name));
            Assert.That(engine.ManagedStates.Count, Is.EqualTo(0));
            Assert.That(engine.PendingEvents.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestAddState()
        {
            FiniteStateMachineEngine engine = new FiniteStateMachineEngine("Turnstile");
            FiniteState movingToProcessed = new FiniteState("MovingToProcessed")
            {
                OnEnterAction = () => { }
            };

            FiniteState addedState = engine.AddState(movingToProcessed);

            Assert.That(addedState, Is.EqualTo(movingToProcessed));
            Assert.That(engine.ManagedStates.Count, Is.EqualTo(1));
            Assert.That(engine.ManagedStates.ContainsKey(movingToProcessed.Name));
        }

        [Test]
        public void TestAddDuplicateState()
        {
            FiniteStateMachineEngine engine = new FiniteStateMachineEngine("Turnstile");
            FiniteState movingToProcessed = new FiniteState("MovingToProcessed")
            {
                OnEnterAction = () => { }
            };

            Assert.That(() =>
            {
                FiniteState addedState = engine.AddState(movingToProcessed);
                addedState = engine.AddState(movingToProcessed);
            }, Throws.InstanceOf<ArgumentException>().And.Message.EqualTo($"Attempt to add a duplicate state '{movingToProcessed.Name}'."));

        }

        [Test]
        public void TestRaiseEvent()
        {
            FiniteStateMachineEngine engine = new FiniteStateMachineEngine("Turnstile");
            FiniteStateEvent actionSucceeded = new FiniteStateEvent("ActionSucceeded");

            engine.RaiseEvent(actionSucceeded);
            Assert.That(engine.PendingEvents.Count, Is.EqualTo(1));
            Assert.That(engine.PendingEvents.Contains(actionSucceeded), Is.True);
        }

        [Test]
        public void TestNoTransitionForEvent()
        {
            FiniteStateMachineEngine engine = new FiniteStateMachineEngine("Turnstile");
            FiniteState gettingWorkItem = new FiniteState("GettingWorkItem")
            {
                OnEnterAction = () => { }
            };
            FiniteState movingToProcessed = new FiniteState("MovingToProcessed")
            {
                OnEnterAction = () => { }
            };
            FiniteStateEvent actionSucceeded = new FiniteStateEvent("ActionSucceeded");

            engine.AddState(gettingWorkItem);
            engine.AddState(movingToProcessed);
            engine.SetCurrentState(gettingWorkItem);
            engine.RaiseEvent(actionSucceeded);
            Assert.That(() =>
            {
                engine.HandlePendingEvents();
            }, Throws.InstanceOf<ArgumentException>().And.Message.EqualTo($"Failed to find a transition for event '{actionSucceeded.Name}' from state '{gettingWorkItem.Name}'."));

        }

        [Test]
        public void TestSetCurrentState()
        {
            FiniteStateMachineEngine engine = new FiniteStateMachineEngine("Turnstile");
            FiniteState gettingWorkItem = new FiniteState("GettingWorkItem")
            {
                OnEnterAction = () => { }
            };
            FiniteState movingToProcessed = new FiniteState("MovingToProcessed")
            {
                OnEnterAction = () => { }
            };

            engine.AddState(gettingWorkItem);
            engine.AddState(movingToProcessed);

            Assert.That(engine.ManagedStates.Count, Is.EqualTo(2));
            Assert.That(engine.ManagedStates.ContainsKey(gettingWorkItem.Name));
            Assert.That(engine.ManagedStates.ContainsKey(movingToProcessed.Name));

            engine.SetCurrentState(gettingWorkItem);
            Assert.That(engine.CurrentState, Is.EqualTo(gettingWorkItem));
            engine.SetCurrentState(movingToProcessed);
            Assert.That(engine.CurrentState, Is.EqualTo(movingToProcessed));
        }

        [Test]
        public void TestSetCurrentStateToNonManagedState()
        {
            FiniteStateMachineEngine engine = new FiniteStateMachineEngine("Turnstile");
            FiniteState gettingWorkItem = new FiniteState("GettingWorkItem")
            {
                OnEnterAction = () => { }
            };
            FiniteState movingToProcessed = new FiniteState("MovingToProcessed")
            {
                OnEnterAction = () => { }
            };

            engine.AddState(gettingWorkItem);
            // do not add the second one
            //engine.AddState(movingToProcessed);

            //engine.SetCurrentState(gettingWorkItem);
            //Assert.That(engine.CurrentState, Is.EqualTo(gettingWorkItem));

            Assert.That(() =>
            {
                engine.SetCurrentState(movingToProcessed);
            }, Throws.InstanceOf<ArgumentException>().And.Message.EqualTo($"Attempted to set current state to '{movingToProcessed.Name}' but no state of that name is managed by this state machine."));

        }

    }
}
