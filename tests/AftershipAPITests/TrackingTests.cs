using AftershipAPI;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            var jsonString = tracking.GeneratePutJSON();

            var result = JObject.Parse(jsonString);

            result["tracking"].Values().Should().Contain("title", "emails", "smses", "customer_name", "order_id", "order_id_path");
        }

        [TestMethod]
        public void GetJSONPost()
        {
            var tracking = new Tracking("RU330890326NL") { Slug = "postnl-international" };

            var jsonString = tracking.GetJSONPost();

            //result.Should().Be("{\r\n  \"tracking\": {\r\n    \"tracking_number\": \"RU330890326NL\",\r\n    \"slug\": \"postnl-international\",\r\n    \"title\": \"RU330890326NL\"\r\n  }\r\n}");
            var json = JObject.Parse(jsonString);

            var result = json["tracking"].Values();

            result.Should().Contain("RU330890326NL", "postnl-international");
        }

        [TestMethod]
        public void ToStringOutput()
        {
            var tracking = new Tracking("RU330890326NL") { Slug = "postnl-international" };

            var result = tracking.ToString();

            result.Should().Be("_id: \n_trackingNumber: RU330890326NL\n_slug: postnl-international");
        }

        //Borked
        [TestMethod]
        public void GetJSONPost_AddFieldsToTracking_ReturnsStringWithUpdatedTracking()
        {
            var tracking = new Tracking("RU330890326NL") { Slug = "postnl-international" };
            tracking.AddEmails("email");
            tracking.AddSmses("sms");
            tracking.AddCustomFields("custom", "field");


            var jsonString = tracking.GetJSONPost();

            var json = JObject.Parse(jsonString);

            var result = json["tracking"].Values();

            result.Should().Contain("RU330890326NL", "postnl-international", "email");

            //jsonString.Should().Be("{\r\n  \"tracking\": {\r\n    \"tracking_number\": \"RU330890326NL\",\r\n    \"slug\": \"postnl-international\",\r\n    \"title\": \"RU330890326NL\",\r\n    \"emails\": [\r\n      \"email\"\r\n    ],\r\n    \"smses\": [\r\n      \"sms\"\r\n    ],\r\n    \"custom_fields\": {\r\n      \"custom\": \"field\"\r\n    }\r\n  }\r\n}");
        }
    }
}
