﻿using NUnit.Framework;
using server.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace server.Test
{
    class ExtensionTest
    {
        [TestCase(1, 60000)]
        [TestCase(10, 600000)]
        [TestCase(2, 120000)]
        [TestCase(30, 1800000)]
        [TestCase(60, 3600000)]
        public void WhenMinutesToMsExtensionCalled_GivenMinuteInput_ThenCorrectMsValueIsReturned(int min, int expected)
        {
            Assert.AreEqual(expected, min.MinutesToMS());
        }
    }
}
