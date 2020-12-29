using Moq;
using NUnit.Framework;
using server.Enums;
using server.Scheduler;
using server.Services;
using server.Utilities;
using System;

namespace server.Test
{
    [TestFixture]
    public class LightControllerTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void WhenControllerTurnsOffLights_ThenTheExpectedCommandIsExecuted()
        {
            var mock = new Mock<ICLIService>();
            mock.Setup(m => m.ExecuteCommand(It.IsAny<string>())).Returns("");
            var scheduleManagerMock = new Mock<IScheduleManager>(MockBehavior.Strict);

            var controller = new LightController(mock.Object, scheduleManagerMock.Object);

            controller.SetLightValue(LightPin.WarmWhite, 1);
            controller.SetLightValue(LightPin.CoolWhite, 1);

            Assert.IsTrue(controller.TurnOff());
            mock.Verify(m => m.ExecuteCommand("pigs p 17 0"), Times.Once);
            mock.Verify(m => m.ExecuteCommand("pigs p 22 0"), Times.Once);
        }

        [Test]
        public void WhenControllerSetsChannelValue_ThenTheExpectedCommandIsExecuted()
        {
            var mock = new Mock<ICLIService>();
            mock.Setup(m => m.ExecuteCommand(It.IsAny<string>())).Returns("");
            var scheduleManagerMock = new Mock<IScheduleManager>(MockBehavior.Strict);

            var controller = new LightController(mock.Object, scheduleManagerMock.Object);
            
            Assert.IsTrue(controller.SetLightValue(LightPin.CoolWhite, 17));
            Assert.IsTrue(controller.SetLightValue(LightPin.WarmWhite, 10));
            
            mock.Verify(m => m.ExecuteCommand("pigs p 22 17"), Times.Once);
            mock.Verify(m => m.ExecuteCommand("pigs p 17 10"), Times.Once);
        }


        [Test]
        public void WhenControllerSleeps_ThenTheExpectedCommandIsExecuted()
        {
            var cliMock = new Mock<ICLIService>(MockBehavior.Strict);
            cliMock.Setup(m => m.ExecuteCommand(It.IsAny<string>())).Returns("");

            var scheduleManagerMock = new Mock<IScheduleManager>();

            var controller = new LightController(cliMock.Object, scheduleManagerMock.Object);
            controller.SetLightValue(LightPin.CoolWhite, 2);
            controller.SetLightValue(LightPin.WarmWhite, 2);

            controller.Sleep(30);
            scheduleManagerMock.Raise(s => s.SleepEpoch += null, EventArgs.Empty);
            scheduleManagerMock.Raise(s => s.SleepEpoch += null, EventArgs.Empty);
            scheduleManagerMock.Raise(s => s.SleepEpoch += null, EventArgs.Empty);

            cliMock.Verify(m => m.ExecuteCommand("pigs p 22 0"), Times.Exactly(1));

            cliMock.Verify(m => m.ExecuteCommand("pigs p 17 1"), Times.Exactly(1));
            cliMock.Verify(m => m.ExecuteCommand("pigs p 17 0"), Times.Exactly(1));

            scheduleManagerMock.Verify(s => s.StopTimers());
        }

        [Test]
        public void WhenControllerWakes_ThenTheExpectedCommandIsExecuted()
        {
            var cliMock = new Mock<ICLIService>(MockBehavior.Strict);
            cliMock.Setup(m => m.ExecuteCommand(It.IsAny<string>())).Returns("");

            var scheduleManagerMock = new Mock<IScheduleManager>();

            var controller = new LightController(cliMock.Object, scheduleManagerMock.Object);
            controller.SetLightValue(LightPin.CoolWhite, 0);
            controller.SetLightValue(LightPin.WarmWhite, 0);

            controller.WakeUp(30);
            scheduleManagerMock.Raise(s => s.WakeEpoch += null, EventArgs.Empty);
            scheduleManagerMock.Raise(s => s.WakeEpoch += null, EventArgs.Empty);

            cliMock.Verify(m => m.ExecuteCommand("pigs p 22 1"), Times.Exactly(1));
            cliMock.Verify(m => m.ExecuteCommand("pigs p 17 1"), Times.Exactly(1));
        }

        [Test]
        public void WhenControllerGetsToMaxValueWhilstWaking_ThenWakeIsStopped()
        {
            var cliMock = new Mock<ICLIService>(MockBehavior.Strict);
            cliMock.Setup(m => m.ExecuteCommand(It.IsAny<string>())).Returns("");

            var scheduleManagerMock = new Mock<IScheduleManager>();

            var controller = new LightController(cliMock.Object, scheduleManagerMock.Object);
            controller.SetLightValue(LightPin.CoolWhite, 255);
            controller.SetLightValue(LightPin.WarmWhite, 255);

            controller.WakeUp(30);
            scheduleManagerMock.Raise(s => s.WakeEpoch += null, EventArgs.Empty);

            scheduleManagerMock.Verify(s => s.StopTimers());
        }

        [Test]
        public void WhenControllerGetsToMinValueWhilstSleeping_ThenSleepIsStopped()
        {
            var cliMock = new Mock<ICLIService>(MockBehavior.Strict);
            cliMock.Setup(m => m.ExecuteCommand(It.IsAny<string>())).Returns("");

            var scheduleManagerMock = new Mock<IScheduleManager>();

            var controller = new LightController(cliMock.Object, scheduleManagerMock.Object);
            controller.SetLightValue(LightPin.CoolWhite, 0);
            controller.SetLightValue(LightPin.WarmWhite, 0);

            controller.Sleep(30);
            scheduleManagerMock.Raise(s => s.SleepEpoch += null, EventArgs.Empty);

            scheduleManagerMock.Verify(s => s.StopTimers());
        }

        [TestCase(1, 60000)]
        [TestCase(60, 3600000)]
        [TestCase(30, 1800000)]
        [TestCase(10, 600000)]
        [TestCase(15, 900000)]
        [TestCase(45, 2700000)]
        public void WhenIntervalProvided_ThenReturnsCorrectMS(int minutes, int expected)
        {
            var mock = new Mock<ICLIService>();
            mock.Setup(m => m.ExecuteCommand(It.IsAny<string>())).Returns("");
            var scheduleManagerMock = new Mock<IScheduleManager>();

            var controller = new LightController(mock.Object, scheduleManagerMock.Object);

            Assert.AreEqual(expected, minutes.MinutesToMS());
        }
    }
}