using AftershipAPI;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                Active = true,
                Title = "title",
                Emails = new List<string>() { "test@emxample.com" },
                Smses = new List<string>() { "sms" },
                CustomerName = "customer",
                OrderID = "orderID",
                OrderIDPath = "idpath"
            };

            var result = tracking.GeneratePutJSON();

            result.Should().Be("{\r\n  \"tracking\": {\r\n    \"title\": \"title\",\r\n    \"emails\": [\r\n      \"test@emxample.com\"\r\n    ],\r\n    \"smses\": [\r\n      \"sms\"\r\n    ],\r\n    \"customer_name\": \"customer\",\r\n    \"order_id\": \"orderID\",\r\n    \"order_id_path\": \"idpath\"\r\n  }\r\n}");
        }

        [TestMethod]
        public void GetJSONPost()
        {
            var tracking = new Tracking("RU330890326NL") { Slug = "postnl-international" };

            var result = tracking.GetJSONPost();

            result.Should().Be("{\r\n  \"tracking\": {\r\n    \"tracking_number\": \"RU330890326NL\",\r\n    \"slug\": \"postnl-international\",\r\n    \"title\": \"RU330890326NL\"\r\n  }\r\n}");
        }


        [TestMethod]
        public void ToStringOutput()
        {
            var tracking = new Tracking("RU330890326NL") { Slug = "postnl-international" };

            var result = tracking.ToString();

            result.Should().Be("_id: \n_trackingNumber: RU330890326NL\n_slug: postnl-international");
        }

        [TestMethod]
        public void HetJSONPost_AddFieldsToTracking_ReturnsStringWithUpdatedTracking()
        {
            var tracking = new Tracking("RU330890326NL") { Slug = "postnl-international" };
            tracking.AddEmails("email");
            tracking.AddSmses("sms");
            tracking.AddCustomFields("custom", "field");


            var result = tracking.GetJSONPost();

            result.Should().Be("{\r\n  \"tracking\": {\r\n    \"tracking_number\": \"RU330890326NL\",\r\n    \"slug\": \"postnl-international\",\r\n    \"title\": \"RU330890326NL\",\r\n    \"emails\": [\r\n      \"email\"\r\n    ],\r\n    \"smses\": [\r\n      \"sms\"\r\n    ],\r\n    \"custom_fields\": {\r\n      \"custom\": \"field\"\r\n    }\r\n  }\r\n}");
        }
    }
}
