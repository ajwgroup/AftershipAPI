using AftershipAPI;
using AftershipAPI.Enums;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AftershipAPITests
{
    [TestClass]
    public class Tests
    {
        ConnectionAPI connection;

        [TestInitialize]
        public void SetUp()
        {
            connection = new ConnectionAPI(tokenAfthership: "0f5b6b47-dc92-49f3-9534-defbaaf55c1b");     
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
