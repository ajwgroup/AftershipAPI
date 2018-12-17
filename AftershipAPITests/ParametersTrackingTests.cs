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
    }
}
