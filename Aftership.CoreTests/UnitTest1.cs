using AftershipAPI;
using AftershipAPI.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AftershipAPITests
{
    [TestClass]
    public class UnitTest1
    {
        ConnectionAPI connection;

        string[] couriersDetected = { "dpd", "fedex" };

        //post tracking number
        string trackingNumberPost = "05167019264110";
        string slugPost = "dpd";
        string orderIDPathPost = "www.whatever.com";
        string orderIDPost = "ID 1234";
        string customerNamePost = "Mr Smith";
        string titlePost = "this title";
        ISO3Country countryDestinationPost = ISO3Country.USA;
        string email1Post = "email@yourdomain.com";
        string email2Post = "another_email@yourdomain.com";
        string sms1Post = "+85292345678";
        string sms2Post = "+85292345679";
        string customProductNamePost = "iPhone Case";
        string customProductPricePost = "USD19.99";

        //tracking numbers to detect
        string trackingNumberToDetect = "09445246482536";
        string trackingNumberToDetectError = "asdq";

        //Tracking to Delete
        string trackingNumberDelete = "596454081704";
        string slugDelete = "fedex";


        //tracking to DeleteBad
        string trackingNumberDelete2 = "798865638020";

        static bool firstTime = true;

        Dictionary<string, string> firstCourier = new Dictionary<string, string>();
        Dictionary<string, string> firstCourierAccount = new Dictionary<string, string>();


        [TestInitialize()]
        public void SetUp()
        {
            string key = "77ce1c85-b047-486d-8ba4-06cd99d50672";
            connection = new ConnectionAPI(key);
        }

        [TestMethod]
        public void TestCreateTracking()
        {
            Tracking tracking1 = new Tracking(trackingNumberPost)
            {
                Slug = slugPost,
                OrderIDPath = orderIDPathPost,
                CustomerName = customerNamePost,
                OrderID = orderIDPost,
                Title = titlePost,
                DestinationCountryISO3 = countryDestinationPost
            };
            tracking1.AddEmails(email1Post);
            tracking1.AddEmails(email2Post);
            tracking1.AddCustomFields("product_name", customProductNamePost);
            tracking1.AddCustomFields("product_price", customProductPricePost);
            tracking1.AddSmses(sms1Post);
            tracking1.AddSmses(sms2Post);
            Tracking trackingPosted = connection.CreateTracking(tracking1);

            Assert.AreEqual(trackingNumberPost, trackingPosted.TrackingNumber, "#A01");
            Assert.AreEqual(slugPost, trackingPosted.Slug, "#A02");
            Assert.AreEqual(orderIDPathPost, trackingPosted.OrderIDPath, "#A03");
            Assert.AreEqual(orderIDPost, trackingPosted.OrderID, "#A04");
            Assert.AreEqual(countryDestinationPost,
                trackingPosted.DestinationCountryISO3, "#A05");

            Assert.IsTrue(trackingPosted.Emails.Contains(email1Post), "#A06");
            Assert.IsTrue(trackingPosted.Emails.Contains(email2Post), "#A07");
            Assert.AreEqual(2, trackingPosted.Emails.Count, "#A08");

            Assert.IsTrue(trackingPosted.Smses.Contains(sms1Post), "#A09");
            Assert.IsTrue(trackingPosted.Smses.Contains(sms2Post), "#A10");
            Assert.AreEqual(2, trackingPosted.Smses.Count, "#A11");

            Assert.AreEqual(customProductNamePost,
                trackingPosted.CustomFields["product_name"], "#A12");
            Assert.AreEqual(customProductPricePost,
                trackingPosted.CustomFields["product_price"], "#A13");
        }

        [TestMethod]
        public void TestCreateTrackingEmptySlug()
        {
            //test post only informing trackingNumber (the slug can be dpd and fedex)
            Tracking tracking2 = new Tracking(trackingNumberToDetect);
            Tracking trackingPosted2 = connection.CreateTracking(tracking2);
            Assert.AreEqual(trackingNumberToDetect, trackingPosted2.TrackingNumber, "#A14");
            Assert.AreEqual("dpd", trackingPosted2.Slug, "#A15");//the system assign dpd (it exist)
        }

        [TestMethod]
        public void TestGetTrackingByNumber()
        {
            string trackingNumber = "3799517046";
            string slug = "dhl";

            Tracking trackingGet1 = new Tracking(trackingNumber)
            {
                Slug = slug
            };

            Tracking tracking = connection.GetTrackingByNumber(trackingGet1);
            Assert.AreEqual(trackingNumber, tracking.TrackingNumber, "#A23");
            Assert.AreEqual(slug, tracking.Slug, "#A24");
            Assert.AreEqual(null, tracking.ShipmentType, "#A25");

            List<Checkpoint> checkpoints = tracking.Checkpoints;
            Checkpoint lastCheckpoint = checkpoints[checkpoints.Count - 1];
            Assert.IsTrue(checkpoints != null, "A25-1");
            Assert.IsTrue(checkpoints.Count > 1, "A25-2");

            Assert.IsTrue(!string.IsNullOrEmpty(lastCheckpoint.Message));
            Assert.IsTrue(!string.IsNullOrEmpty(lastCheckpoint.CountryName));
        }

        [TestMethod]
        public void TestGetTrackings()
        {
            //get the first 100 Trackings
            List<Tracking> listTrackings100 = connection.GetTrackings(1);
            // Assert.AreEqual(10, listTrackings100.Count);
            //at least we have 10 elements
            Assert.IsNotNull(listTrackings100[0].ToString());
            Assert.IsNotNull(listTrackings100[10].ToString());
        }

        [TestMethod]
        public void TestGetCouriers()
        {
            List<Courier> couriers = connection.GetCouriers();
            //total Couriers returned
            Assert.IsTrue(couriers.Count > 30);
            //check first courier

            Assert.IsTrue(!string.IsNullOrEmpty(couriers[0].Slug));
            Assert.IsTrue(!string.IsNullOrEmpty(couriers[0].Name));
            Assert.IsTrue(!string.IsNullOrEmpty(couriers[0].Phone));
            Assert.IsTrue(!string.IsNullOrEmpty(couriers[0].Other_name));
            Assert.IsTrue(!string.IsNullOrEmpty(couriers[0].Web_url));

            //try to acces with a bad API Key
            ConnectionAPI connectionBadKey = new ConnectionAPI("badKey");

            try
            {
                connectionBadKey.GetCouriers();
            }
            catch (Exception e)
            {
                Assert.AreEqual("{\"meta\":{\"code\":401,\"message\":\"Invalid API key.\",\"type\":\"Unauthorized\"},\"data\":{}}", e.Message);
            }
        }
    }
}
