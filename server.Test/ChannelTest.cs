using Moq;
using NUnit.Framework;
using server.Enums;
using server.Channel;
using server.Services;

namespace server.Test
{
    public class ChannelTest
    {

        [TestCase(0, "0")]
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
            
            var chnl = new LightChannel(mock.Object) 
            {
                Pin = LightPin.WarmWhite
            };

            Assert.IsTrue(chnl.SetChannelValuePct(pctValue));
            
            mock.Verify(m => m.ExecuteCommand($"pigs p 17 {value}"), Times.Once);
        }

        [Test]
        public void WhenChannelIncrementsValue_ThenTheExpectedCommandIsExecuted()
        {
            var mock = new Mock<ICLIService>();
            mock.Setup(m => m.ExecuteCommand(It.IsAny<string>())).Returns("");

            var chnl = new LightChannel(mock.Object)
            {
                Pin = LightPin.WarmWhite
            };

            Assert.IsTrue(chnl.IncrementBrightness());
            Assert.IsTrue(chnl.IncrementBrightness());
            Assert.IsTrue(chnl.IncrementBrightness());
            Assert.IsTrue(chnl.IncrementBrightness());
            Assert.IsTrue(chnl.IncrementBrightness());
            
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

            var chnl = new LightChannel(mock.Object)
            {
                Pin = LightPin.WarmWhite
            };

            chnl.SetChannelValue(255);

            Assert.IsTrue(chnl.DecrementBrightness());
            Assert.IsTrue(chnl.DecrementBrightness());
            Assert.IsTrue(chnl.DecrementBrightness());
            Assert.IsTrue(chnl.DecrementBrightness());
            Assert.IsTrue(chnl.DecrementBrightness());
            
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

            var chnl = new LightChannel(mock.Object)
            {
                Pin = LightPin.WarmWhite
            };

            Assert.IsFalse(chnl.SetChannelValue(-1));
            Assert.IsFalse(chnl.DecrementBrightness());

            Assert.AreEqual(0, chnl.CurrentValue);

            mock.Verify(m => m.ExecuteCommand(It.IsAny<string>()), Times.Never());
        }
        
        [Test]
        public void WhenChannelGoesAboveMaxValue_ThenItReturnsFalse()
        {
            var mock = new Mock<ICLIService>();
            mock.Setup(m => m.ExecuteCommand(It.IsAny<string>())).Returns("");

            var chnl = new LightChannel(mock.Object)
            {
                Pin = LightPin.WarmWhite
            };

            // Set it to be highest possible value
            Assert.IsTrue(chnl.SetChannelValue(255));
            mock.Invocations.Clear();

            Assert.IsFalse(chnl.SetChannelValue(256));
            Assert.IsFalse(chnl.IncrementBrightness());

            Assert.AreEqual(255, chnl.CurrentValue);
            
            mock.Verify(m => m.ExecuteCommand(It.IsAny<string>()), Times.Never());
        }

        [TestCase(30,7059)] // 30 minutes. 7059 ms between each change
        [TestCase(15, 3529)]
        [TestCase(45, 10588)]
        public void WhenChannelIsMaxValueAndGivenASleepDuration_ThenTheCorrectSleepIntervalIsReturned(int totalSleepDuration_Minutes, int expectedInterval_Seconds)
        {
            var mock = new Mock<ICLIService>();
            var chnl = new LightChannel(mock.Object)
            {
                Pin = LightPin.WarmWhite
            };

            chnl.SetChannelToMaxValue();

            Assert.AreEqual(expectedInterval_Seconds, chnl.GetIntervalToSleep(totalSleepDuration_Minutes));
        }

        [TestCase(200, 30, 9000)] // Currently at 200. 30 mins to turn off. 9000 ms interval
        [TestCase(100, 30, 18000)]
        [TestCase(50, 30, 36000)]
        public void WhenChannelIsAtAValueAndGivenASleepDuration_ThenTheCorrectSleepIntervalIsReturned(int currentValue, int totalSleepDuration_Minutes, int expectedInterval_Seconds)
        {
            var mock = new Mock<ICLIService>();
            var chnl = new LightChannel(mock.Object)
            {
                Pin = LightPin.WarmWhite
            };

            chnl.SetChannelValue(currentValue);

            Assert.AreEqual(expectedInterval_Seconds, chnl.GetIntervalToSleep(totalSleepDuration_Minutes));
        }
    }
}