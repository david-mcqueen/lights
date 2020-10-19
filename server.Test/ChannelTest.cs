using Moq;
using NUnit.Framework;
using server.Enums;
using server.Models;
using server.Services;

namespace server.Test
{
    public class ChannelTest
    {

        [TestCase(1, "2")]
        [TestCase(10, "25")]
        [TestCase(20, "51")]
        [TestCase(30, "76")]
        [TestCase(40, "102")]
        [TestCase(50, "127")]
        [TestCase(100, "255")]
        public void ChannelCanCalculateValueFromPct(int pctValue, string value)
        {
            var mock = new Mock<ICLIService>();
            mock.Setup(m => m.ExecuteCommand(It.IsAny<string>())).Returns("");
            
            var controller = new Channel(LightPin.WarmWhite, mock.Object);

            controller.SetChannelValuePct(pctValue);
            
            mock.Verify(m => m.ExecuteCommand($"pigs p 17 {value}"), Times.Once);
        }
    }
}