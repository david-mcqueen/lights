using Moq;
using NUnit.Framework;
using server.Enums;
using server.Services;
using System;

namespace server.Test
{
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
            
            var controller = new LightController(mock.Object);
            
            Assert.IsTrue(controller.TurnOff());
            mock.Verify(m => m.ExecuteCommand("pigs p 17 0"), Times.Once);
            mock.Verify(m => m.ExecuteCommand("pigs p 22 0"), Times.Once);
        }

        [Test]
        public void WhenControllerSetsChannelValue_ThenTheExpectedCommandIsExecuted()
        {
            var mock = new Mock<ICLIService>();
            mock.Setup(m => m.ExecuteCommand(It.IsAny<string>())).Returns("");
            
            var controller = new LightController(mock.Object);
            
            Assert.IsTrue((controller.SetLightValue(LightPin.CoolWhite, 17)));
            
            mock.Verify(m => m.ExecuteCommand("pigs p 22 17"), Times.Once);
        }


        [Test]
        public void WhenControllerSleeps_ThenTheExpectedCommandIsExecuted()
        {
            var mock = new Mock<ICLIService>();
            mock.Setup(m => m.ExecuteCommand(It.IsAny<string>())).Returns("");
            
            var controller = new LightController(mock.Object);

            // Set the lights to be the nearest off value possible
            controller.SetLightValue(LightPin.CoolWhite, 1);
            controller.SetLightValue(LightPin.WarmWhite, 1);
            
            mock.Invocations.Clear();
            controller.Sleep((object sender, EventArgs e) =>
            {
                mock.Verify(m => m.ExecuteCommand("pigs p 17 0"), Times.Exactly(1));
                mock.Verify(m => m.ExecuteCommand("pigs p 22 0"), Times.Exactly(1));
            }, 1);

        }

        [TestCase(200, 200)]
        [TestCase(100, 100)]
        [TestCase(50, 50)]
        [TestCase(25, 25)]
        [TestCase(10, 10)]
        public void WhenControllerSleepsWithValue_ThenTheExpectedCommandIsExecuted(int startingValue, int executionAmount)
        {
            var mock = new Mock<ICLIService>();
            mock.Setup(m => m.ExecuteCommand(It.IsAny<string>())).Returns("");

            var controller = new LightController(mock.Object);

            // Set the lights to be the nearest off value possible
            controller.SetLightValue(LightPin.CoolWhite, startingValue);
            controller.SetLightValue(LightPin.WarmWhite, startingValue);

            mock.Invocations.Clear();
            controller.Sleep((object sender, EventArgs e) =>
            {
                mock.Verify(m => m.ExecuteCommand("pigs p 17 0"), Times.Exactly(executionAmount));
                mock.Verify(m => m.ExecuteCommand("pigs p 22 0"), Times.Exactly(1));
            }, 1);
        }
    }
}