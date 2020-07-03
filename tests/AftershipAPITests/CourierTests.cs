using AftershipAPI;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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

        [TestMethod]
        public void Courier()
        {
            JObject o = new JObject(
               new JProperty("slug", "slug"),
               new JProperty("name", "name"),
               new JProperty("phone", "phone"),
               new JProperty("other_name", "other_name"),
               new JProperty("web_url", "web_url"),
               new JProperty("required_fields", new List<string> { "required_fields" })
            );

            var result = o.ToObject<Courier>();

            var expected = new Courier("web_url", "slug", "name", "phone", "other_name");

            result.Should().BeEquivalentTo(expected);
        }
    }
}
