using Moq;
using NUnit.Framework;
using server.Channel;
using server.Enums;
using server.Scheduler;
using server.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace server.Test
{
    [TestFixture]
    class FunctionalityTests
    {
        private Mock<ICLIService> _cliMock;
        private Mock<IScheduleManager> _scheduleManagerMock;


        [SetUp]
        public void BeforeEach()
        {
            _cliMock = new Mock<ICLIService>();
            _cliMock.Setup(m => m.ExecuteCommand(It.IsAny<string>())).Returns("");

            _scheduleManagerMock = new Mock<IScheduleManager>(MockBehavior.Strict);
            _scheduleManagerMock.Setup(m => m.StartSleep(It.IsAny<int>()));
            _scheduleManagerMock.Setup(m => m.StopTimers());
            _scheduleManagerMock.Setup(m => m.StartWithDelay(It.IsAny<int>(), It.IsAny<int>()));
        }

        [TearDown]
        public void AfterEach()
        {
            _cliMock.Reset();
            _scheduleManagerMock.Reset();
        }

        [Test]
        public void AsAUser_IWantToBeAbleToTurnTheLightsOffInstantly()
        {
            var controller = new LightController(_cliMock.Object, _scheduleManagerMock.Object);

            controller.SetLightValue(LightPin.WarmWhite, 1);
            controller.SetLightValue(LightPin.CoolWhite, 1);

            Assert.IsTrue(controller.TurnOff());
            _cliMock.Verify(m => m.ExecuteCommand("pigs p 17 0"), Times.Once);
            _cliMock.Verify(m => m.ExecuteCommand("pigs p 22 0"), Times.Once);
        }

        [TestCase(0, "0")]
        [TestCase(1, "2")]
        [TestCase(10, "25")]
        [TestCase(20, "51")]
        [TestCase(30, "76")]
        [TestCase(40, "102")]
        [TestCase(50, "127")]
        [TestCase(100, "255")]
        public void AsAUser_IWantToBeAbleToSpecifyPctBrightnessValues(int pctValue, string value)
        {
            var controller = new LightController(_cliMock.Object, _scheduleManagerMock.Object);

            Assert.IsTrue(controller.SetLightValuePct(LightPin.WarmWhite, pctValue));

            _cliMock.Verify(m => m.ExecuteCommand($"pigs p 17 {value}"), Times.Once);
        }

        [Test]
        public void AsAUser_IWantToBeAbleToSleepTheLightsSoThatTheyTurnOffOverAPeriodOfTime()
        {
            var controller = new LightController(_cliMock.Object, _scheduleManagerMock.Object);
            controller.SetLightValue(LightPin.CoolWhite, 1);
            controller.SetLightValue(LightPin.WarmWhite, 1);

            controller.Sleep(30);
            _scheduleManagerMock.Raise(s => s.SleepEpoch += null, EventArgs.Empty);
            _scheduleManagerMock.Raise(s => s.SleepEpoch += null, EventArgs.Empty);

            _cliMock.Verify(m => m.ExecuteCommand("pigs p 22 0"), Times.Exactly(1));
            _cliMock.Verify(m => m.ExecuteCommand("pigs p 17 0"), Times.Exactly(1));

            _scheduleManagerMock.Verify(s => s.StopTimers());
        }

        [TestCase(30, 30)]
        public void AsAUser_IWantToBeAbleToSleepTheLightsAfterADelay(int sleepDuration, int sleepDelay)
        {
            var controller = new LightController(_cliMock.Object, _scheduleManagerMock.Object);
            controller.SetLightValue(LightPin.CoolWhite, 1);
            controller.SetLightValue(LightPin.WarmWhite, 1);

            controller.SleepWithDelay(sleepDuration, sleepDelay);

            _scheduleManagerMock.Verify(s => s.StartWithDelay(sleepDuration, sleepDelay));

            _scheduleManagerMock.Raise(s => s.SleepEpoch += null, EventArgs.Empty);
            _scheduleManagerMock.Raise(s => s.SleepEpoch += null, EventArgs.Empty);

            _cliMock.Verify(m => m.ExecuteCommand("pigs p 22 0"), Times.Exactly(1));
            _cliMock.Verify(m => m.ExecuteCommand("pigs p 17 0"), Times.Exactly(1));

            _scheduleManagerMock.Verify(s => s.StopTimers());
        }

        [Test]
        public void AsAUser_IWantToBeAbleToSetAnAlarmForTheLightsToTurnOnAtADefinedTime()
        {   
        }
    }
}
