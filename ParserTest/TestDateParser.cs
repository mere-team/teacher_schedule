using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schedule.Parsers;

namespace ParserTest
{
    [TestClass]
    public class TestDateParser
    {
        [TestMethod]
        public void TestMethod1()
        {
            DateTime lastUpdate = DateParser.GetLastUpdateStudentSchedule();

            Assert.AreEqual(25, lastUpdate.Day);
            Assert.AreEqual(2, lastUpdate.Month);
            Assert.AreEqual(2016, lastUpdate.Year);
        }
    }
}
