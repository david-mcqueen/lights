using Moq;
using NUnit.Framework;
using server.Enums;
using server.Services;

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
            controller.SetLightValue(LightPin.CoolWhite, 100);
            controller.SetLightValue(LightPin.WarmWhite, 100);
            
            mock.Invocations.Clear();
            controller.Sleep();
            
            
            mock.Verify(m => m.ExecuteCommand(It.IsAny<string>()), Times.Exactly(500));
        }
    }
}