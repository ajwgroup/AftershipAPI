using AftershipAPI;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AftershipAPITests
{
    [TestClass]
    public class DateMethodsTests
    {
        [TestMethod]
        public void ToString_ProvideDate_DateOutputInCorrectFormat()
        {
            var testDate = new DateTime(2018, 12, 30, 11, 59, 59);

            var result = DateMethods.ToString(testDate);

            result.Should().Be("2018-12-30T11:59:59+00:00");
        }
    }
}
