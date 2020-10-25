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
        public void WhenChannelIsSetWithAPctValue_ThenTheExpectedCommandIsExecuted(int pctValue, string value)
        {
            var mock = new Mock<ICLIService>();
            mock.Setup(m => m.ExecuteCommand(It.IsAny<string>())).Returns("");
            
            var controller = new Channel(LightPin.WarmWhite, mock.Object);

            Assert.IsTrue(controller.SetChannelValuePct(pctValue));
            
            mock.Verify(m => m.ExecuteCommand($"pigs p 17 {value}"), Times.Once);
        }

        [Test]
        public void WhenChannelIncrementsValue_ThenTheExpectedCommandIsExecuted()
        {
            var mock = new Mock<ICLIService>();
            mock.Setup(m => m.ExecuteCommand(It.IsAny<string>())).Returns("");
            
            var controller = new Channel(LightPin.WarmWhite, mock.Object);

            Assert.IsTrue(controller.IncrementBrightness());
            Assert.IsTrue(controller.IncrementBrightness());
            Assert.IsTrue(controller.IncrementBrightness());
            Assert.IsTrue(controller.IncrementBrightness());
            Assert.IsTrue(controller.IncrementBrightness());
            
            mock.Verify(m => m.ExecuteCommand($"pigs p 17 1"), Times.Once);
            mock.Verify(m => m.ExecuteCommand($"pigs p 17 2"), Times.Once);
            mock.Verify(m => m.ExecuteCommand($"pigs p 17 3"), Times.Once);
            mock.Verify(m => m.ExecuteCommand($"pigs p 17 4"), Times.Once);
            mock.Verify(m => m.ExecuteCommand($"pigs p 17 5"), Times.Once);
        }
        
        [Test]
        public void WhenChannelDecrementsValue_ThenTheExpectedCommandIsExecuted()
        {
            var mock = new Mock<ICLIService>();
            mock.Setup(m => m.ExecuteCommand(It.IsAny<string>())).Returns("");
            
            var controller = new Channel(LightPin.WarmWhite, mock.Object);

            controller.SetChannelValue(255);

            Assert.IsTrue(controller.DecrementBrightness());
            Assert.IsTrue(controller.DecrementBrightness());
            Assert.IsTrue(controller.DecrementBrightness());
            Assert.IsTrue(controller.DecrementBrightness());
            Assert.IsTrue(controller.DecrementBrightness());
            
            mock.Verify(m => m.ExecuteCommand($"pigs p 17 254"), Times.Once);
            mock.Verify(m => m.ExecuteCommand($"pigs p 17 253"), Times.Once);
            mock.Verify(m => m.ExecuteCommand($"pigs p 17 252"), Times.Once);
            mock.Verify(m => m.ExecuteCommand($"pigs p 17 251"), Times.Once);
            mock.Verify(m => m.ExecuteCommand($"pigs p 17 250"), Times.Once);
        }

        [Test]
        public void WhenChannelGoesBelow0_ThenItReturnsFalse()
        {
            var mock = new Mock<ICLIService>();
            mock.Setup(m => m.ExecuteCommand(It.IsAny<string>())).Returns("");
            
            var controller = new Channel(LightPin.WarmWhite, mock.Object);
            
            Assert.IsFalse(controller.SetChannelValue(-1));
            Assert.IsFalse(controller.DecrementBrightness());
            
            mock.Verify(m => m.ExecuteCommand(It.IsAny<string>()), Times.Never());
        }
        
        [Test]
        public void WhenChannelGoesAboveMaxValue_ThenItReturnsFalse()
        {
            var mock = new Mock<ICLIService>();
            mock.Setup(m => m.ExecuteCommand(It.IsAny<string>())).Returns("");
            
            var controller = new Channel(LightPin.WarmWhite, mock.Object);
            
            // Set it to be highest possible value
            Assert.IsTrue(controller.SetChannelValue(255));
            mock.Invocations.Clear();

            Assert.IsFalse(controller.SetChannelValue(256));
            Assert.IsFalse(controller.IncrementBrightness());
            
            mock.Verify(m => m.ExecuteCommand(It.IsAny<string>()), Times.Never());
        }
    }
}