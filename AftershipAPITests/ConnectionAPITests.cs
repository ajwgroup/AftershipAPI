using AftershipAPI;
using AftershipAPI.Enums;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;

namespace AftershipAPITests
{
    [TestClass]
    public class ConnectionAPITests
    {
        ConnectionAPI connection;

        [TestInitialize]
        public void Setup()
        {
            connection = new ConnectionAPI("0f5b6b47-dc92-49f3-9534-defbaaf55c1b");
        }

        [TestMethod]
        public void GetLastCheckpoint_ProvideTracking_GetLastCheckpoint()
        {
            var tracking = new Tracking("RU330890326NL") { Slug = "postnl-international" };

            var result = connection.GetLastCheckpoint(tracking);

            result.CheckpointTime.Should().Be("12/13/2018 15:20:00");
        }

        [TestMethod]
        public void GetLastCheckpoint_ProvideTrackingWithExtraParams_GetLastCheckpoint()
        {
            var fields = new List<FieldCheckpoint>() { FieldCheckpoint.created_at };
            var lang = "";

            var tracking = new Tracking("RU330890326NL") { Slug = "postnl-international" };

            var result = connection.GetLastCheckpoint(tracking, fields, lang);

            var expected = new DateTime(2018, 12, 13, 17, 35, 47);

            result.CreatedAt.Should().Be(expected);
        }

        [TestMethod]
        public void Retrack_TrackingIsActive_ThrowExceptionOnlyInactiveCanRetrack()
        {
            var tracking = new Tracking("990728071") { Slug = "ups" };

            Action result = () => connection.Retrack(tracking);

            result.Should().ThrowExactly<WebException>();
        }

        [TestMethod]
        public void GetTrackings_Page1_ShouldReturn4Trackings()
        {
            var result = connection.GetTrackings(1);

            result.Should().HaveCount(4);
        }

        [TestMethod]
        public void DetectCouriers_TrackingNumberPosted_NoCarrierDetected()
        {
            var result = connection.DetectCouriers("ER751105042015062");

            result.Should().BeEmpty();

        }

        [TestMethod]
        public void DetectCouriers_ExtraParameters_NoCarrierDetected()
        {
            var result = connection.DetectCouriers("ER751105042015062", "", "", "", new List<string>() { "ups" });

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetTrackingsNext_OnlyOnePageOfTrackings_ReturnsNull()
        {
            var parameterTracking = new ParametersTracking();

            parameterTracking.AddSlug("ups");

            var result = connection.GetTrackingsNext(parameterTracking);

            result.Should().BeEmpty();
        }
        
        [TestMethod]
        public void GetTrackingByNumber()
        {
            var tracking = new Tracking("RU330890326NL") { Slug = "postnl-international" };
            var fields = new List<FieldTracking>() { FieldTracking.created_at };

            var result = connection.GetTrackingByNumber(tracking, fields, "");

            var expected = new DateTime(2018, 12, 13, 14, 35, 35);

            result.CreatedAt.Should().Be(expected);
        }

        [TestMethod]
        public void GetCouriers_ListOfCouriersReturned_Only4Selected()
        {
            var couriers = connection.GetCouriers();

            couriers.Count.Should().Be(4);
        }


        [TestMethod]
        public void GetAllCouriers_ListOfCouriersReturned_MoreThan100()
        {
            var couriers = connection.GetAllCouriers();

            couriers.Count.Should().BeGreaterOrEqualTo(100);
        }

        [TestMethod]
        public void GetTrackingByNumber_ActiveTracking_ReturnsTrackingInformation()
        {
            var result = connection.GetTrackingByNumber(new Tracking("RU330890326NL") { Slug = "postnl-international" });

            result.Should().NotBeNull();
        }

        [TestMethod]
        public void GetTrackings_UseParametersTracking_TrackingIsReturned()
        {
            var parametersTracking = new ParametersTracking();
            parametersTracking.AddField(FieldTracking.tracking_number);

            List<Tracking> AllTrackings = connection.GetTrackings(parametersTracking);

            AllTrackings.Count.Should().BeGreaterOrEqualTo(2);
        }
    }
}
