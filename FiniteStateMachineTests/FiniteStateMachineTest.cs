using NUnit.Framework;
using System;
namespace Karzina.Common.FiniteStateMachineTests
{
    [TestFixture]
    public class FiniteStateMachineTest
    {
        public FiniteStateMachineTest()
        {
            log4net.Config.BasicConfigurator.Configure();
        }

        [TestCase("Turnstile")]
        public void TestConstructor(string Name)
        {
            FiniteStateMachine fsm = new FiniteStateMachine(Name);
            Assert.That(fsm.Name, Is.EqualTo(Name));
        }

        [Test]
        public void TestAddState()
        {
            FiniteStateMachine fsm = new FiniteStateMachine("Turnstile");
            FiniteState movingToProcessed = new FiniteState("MovingToProcessed")
            {
                OnEnterAction = () => { }
            };

            FiniteState addedState = fsm.AddState(movingToProcessed);

            Assert.That(addedState, Is.EqualTo(movingToProcessed));
        }

        [Test]
        public void TestRaiseEvent()
        {
            FiniteStateMachine fsm = new FiniteStateMachine("Turnstile");
            FiniteStateEvent actionSucceeded = new FiniteStateEvent("ActionSucceeded");

            fsm.RaiseEvent(actionSucceeded);
        }

        //[Test]
        //public void TestStop()
        //{
        //    FiniteStateMachine fsm = new FiniteStateMachine("Turnstile");

        //    fsm.Stop();
        //}

        [Test]
        public void TestStart()
        {
            FiniteStateMachine fsm = new FiniteStateMachine("Turnstile");
            FiniteState gettingWorkItem = new FiniteState("GettingWorkItem")
            {
                OnEnterAction = () => { }
            };
            FiniteState movingToProcessed = new FiniteState("MovingToProcessed")
            {
                OnEnterAction = () => { }
            };
            FiniteState addedState = fsm.AddState(gettingWorkItem);
            fsm.Start(gettingWorkItem);
            //fsm.Stop();
        }

    //    [Test]
    //    public void TestNoTransitionForEvent()
    //    {
    //        FiniteStateMachineEngine engine = new FiniteStateMachineEngine("Turnstile")
    //        {
    //            OnExceptionAction = (Exception e) => {
    //                Console.WriteLine($"{e.Message} {e.StackTrace}");
    //            }
    //        };
    //        FiniteState gettingWorkItem = new FiniteState("GettingWorkItem")
    //        {
    //            OnEnterAction = () => { }
    //        };
    //        FiniteState movingToProcessed = new FiniteState("MovingToProcessed")
    //        {
    //            OnEnterAction = () => { }
    //        };
    //        FiniteStateEvent actionSucceeded = new FiniteStateEvent("ActionSucceeded");

    //        engine.AddState(gettingWorkItem);
    //        engine.AddState(movingToProcessed);
    //        Assert.That(() =>
    //        {
    //            engine.Start(gettingWorkItem);
    //            engine.RaiseEvent(actionSucceeded);
    //        }, Throws.InstanceOf<ArgumentException>()
    //        .And.Message.EqualTo($"Failed to find a transition for event '{actionSucceeded.Name}' from state '{gettingWorkItem.Name}'."));
    //        engine.Stop();
    //    }

    }
}
