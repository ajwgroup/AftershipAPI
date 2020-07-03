using AftershipAPI;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace AftershipAPITests
{

    [TestClass]
    public class TrackingTests
    {
        [TestMethod]
        public void GeneratePutJSON()
        {
            var tracking = new Tracking("RU330890326NL")
            {
                Slug = "postnl-international",
                Title = "title",
                Emails = new List<string>() { "test@emxample.com" },
                Smses = new List<string>() { "sms" },
                CustomerName = "customer",
                OrderID = "orderID",
                OrderIDPath = "idpath"
            };

            var jsonString = JsonConvert.SerializeObject(tracking);

            var result = JObject.Parse(jsonString);

            result.Values().Should().Contain("title", "emails", "smses", "customer_name", "order_id", "order_id_path");
        }

        [TestMethod]
        public void GetJSONPost()
        {
            var tracking = new Tracking("RU330890326NL") { Slug = "postnl-international" };

            var jsonString = JsonConvert.SerializeObject(tracking);

            var json = JObject.Parse(jsonString);

            var result = json.Values();

            result.Should().Contain("RU330890326NL", "postnl-international");
        }

        [TestMethod]
        public void ToStringOutput()
        {
            var tracking = new Tracking("RU330890326NL") { Slug = "postnl-international" };

            var result = tracking.ToString();

            result.Should().Be("_id: \n_trackingNumber: RU330890326NL\n_slug: postnl-international");
        }

        [TestMethod]
        public void GetJSONPost_AddFieldsToTracking_ReturnsStringWithUpdatedTracking()
        {
            var tracking = new Tracking("RU330890326NL") { Slug = "postnl-international" };
            tracking.AddEmails("email");
            tracking.AddSmses("sms");
            tracking.AddCustomFields("custom", "field");


            var jsonString = JsonConvert.SerializeObject(tracking);

            var json = JObject.Parse(jsonString);

            var result = json.Values();

            result.Should().Contain("RU330890326NL", "postnl-international", "email");
        }
    }
}
