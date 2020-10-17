using Moq;
using NUnit.Framework;
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
            controller.TurnOff();
            
            mock.Verify(m => m.ExecuteCommand("pigs p 17 0"), Times.Once);
            mock.Verify(m => m.ExecuteCommand("pigs p 22 0"), Times.Once);
        }
    }
}