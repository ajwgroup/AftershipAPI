using AftershipAPI;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AftershipAPI.Enums;

namespace AftershipAPITests
{
    [TestClass]
    public class ParametersTrackingTests
    {
        [TestMethod]
        public void GetLastCheckpoint_ProvideTracking_GetLastCheckpoint()
        {
            var tracking = new ParametersTracking();

            tracking.AddSlug("slug");
        }

        [TestMethod]
        public void GenerateQueryString_IncludeSlug_SlugIncludedInQueryString()
        {
            var tracking = new ParametersTracking();

            tracking.AddSlug("slug");

            var result = tracking.GenerateQueryString();

            result.Should().Be("page=1&limit=100&slug=slug");
        }

        [TestMethod]
        public void GenerateQueryString_IncludeAllParameters_QueryStringIncludesAllParameters()
        {
            var tracking = new ParametersTracking();

            tracking.AddSlug("slug");
            tracking.AddOrigin(ISO3Country.MCO);
            tracking.AddDestination(ISO3Country.GBR);
            tracking.AddTag(StatusTag.AttemptFail);
            tracking.AddField(FieldTracking.active);

            var result = tracking.GenerateQueryString();

            result.Should().Be("page=1&limit=100&slug=slug&origin=MCO&destination=GBR&tag=AttemptFail&fields=active");
        }

        [TestMethod]
        public void DeleteSlugs_AddAndRemoveSlug_SlugNotInQuery()
        {
            var tracking = new ParametersTracking();

            tracking.AddSlug("slug");
            tracking.DeleteSlugs();

            var result = tracking.GenerateQueryString();

            result.Should().Be("page=1&limit=100");
        }

        [TestMethod]
        public void DeleteDestination_RemoveDestination_DestinationNotInQuery()
        {
            var tracking = new ParametersTracking();

            tracking.AddDestination(ISO3Country.MCO);
            tracking.AddDestination(ISO3Country.GBR);
            tracking.DeleteDestination(ISO3Country.MCO);

            var result = tracking.GenerateQueryString();

            result.Should().Be("page=1&limit=100&destination=GBR");
        }

        [TestMethod]
        public void AddDestinations_DestinationsIsContainsABW()
        {
            var parameterTracking = new ParametersTracking();

            parameterTracking.AddDestination(ISO3Country.ABW);

            var result = parameterTracking.GenerateQueryString();

            result.Should().Contain("ABW");
        }

        [TestMethod]
        public void DeleteDestinations_DestinationsIsEmpty()
        {
            var parameterTracking = new ParametersTracking();

            parameterTracking.AddDestination(ISO3Country.ABW);

            parameterTracking.DeleteDestinations();

            var result = parameterTracking.GenerateQueryString();

            result.Should().NotContain("ABW");
        }

        [TestMethod]
        public void AddField_ContainsActiveField()
        {
            var parameterTracking = new ParametersTracking();

            parameterTracking.AddField(FieldTracking.active);

            var result = parameterTracking.GenerateQueryString();

            result.Should().Contain("active");
        }

        [TestMethod]
        public void DeleteFields_NoFields()
        {
            var parameterTracking = new ParametersTracking();

            parameterTracking.AddField(FieldTracking.active);

            parameterTracking.DeleteFields();

            var result = parameterTracking.GenerateQueryString();

            result.Should().NotContain("active");
        }

        [TestMethod]
        public void DeleteField_FieldAlreadyNotProvided_ResultShouldBeTheSame()
        {
            var parameterTracking = new ParametersTracking();

            parameterTracking.AddField(FieldTracking.active);

            var expected = parameterTracking.GenerateQueryString();

            parameterTracking.DeleteField(FieldTracking.checkpoints);

            var result = parameterTracking.GenerateQueryString();

            result.Should().Be(expected);
        }
    }
}
