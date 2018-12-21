using AftershipAPI;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AftershipAPITests
{
    [TestClass]
    public class CourierTests
    {
        [TestMethod]
        public void CourierToString_ReturnCourierOutputString()
        {
            var courier = new Courier("web", "slug", "name", "phone", "other_name");

            var result = courier.ToString();

            result.Should().Be("Courier{slug='slug', name='name', phone='phone', other_name='other_name', web_url='web'}");
        }
    }
}
