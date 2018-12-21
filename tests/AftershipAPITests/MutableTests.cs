using AftershipAPI;
using AftershipAPI.Enums;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AftershipAPITests
{
   [TestClass]
   public class MutableTests
   {
       /// <summary>
       /// Tests that use up API Requests
       /// </summary>

       ConnectionAPI connection;

       Tracking NewTracking;
       Tracking NewTrackingWithSlug;
       Tracking TrackingToBeDeleted;
       Tracking TrackingToBeUpdated;

       [TestInitialize]
       public void SetUp()
       {
          var config = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .AddEnvironmentVariables()
           .Build();



          connection = new ConnectionAPI(config["AftershipApiKey"]);
       }

       private void CreateTrackingIfNotExist(string trackingNumber, string slug)
       {
           var checkTracking = new Tracking(trackingNumber) { Slug = slug };
           var tracking = connection.GetTrackingByNumber(checkTracking);

           if (tracking == null)
              connection.DeleteTracking(checkTracking);
       }

       private void DeleteTrackingIfExists(string trackingNumber, string slug)
       {
           var checkTracking = new Tracking(trackingNumber) { Slug = slug };

           try
           {
              var tracking = connection.GetTrackingByNumber(checkTracking);

              if (tracking != null)
                  connection.DeleteTracking(checkTracking);
           }
           catch { }
       }

       [TestMethod]
       public void CreateTracking_CreateTrackingWithoutSlug_CreatedSuccessfully()
       {
           const string trackingNumber = "1Z12345E0205271688";
           const string slug = "ups";

           DeleteTrackingIfExists(trackingNumber, slug);

           NewTracking = new Tracking(trackingNumber)
           {
              OrderIDPath = "OrderIDPathPost",
              CustomerName = "CustomerNamePost",
              OrderID = "OrderIDPost",
              Title = "TitlePost",
              DestinationCountryISO3 = ISO3Country.GBR
           };

           var result = connection.CreateTracking(NewTracking);

           result.TrackingNumber.Should().Be(NewTracking.TrackingNumber);
       }

       [TestMethod]
       public void CreateTracking_CreateTrackingWithSlug_CreatedSuccessfully()
       {
           const string trackingNumber = "990728071";
           const string slug = "ups";

           DeleteTrackingIfExists(trackingNumber, slug);

           NewTrackingWithSlug = new Tracking(trackingNumber)
           {
              Slug = slug,
              OrderIDPath = "OrderIDPathPost",
              CustomerName = "CustomerNamePost",
              OrderID = "OrderIDPost",
              Title = "TitlePost",
              DestinationCountryISO3 = ISO3Country.GBR
           };

           var result = connection.CreateTracking(NewTrackingWithSlug);

           result.TrackingNumber.Should().Be(NewTrackingWithSlug.TrackingNumber);
       }

       [TestMethod]
       public void DeleteTracking_TrackingIsDeleted()
       {
           const string trackingNumber = "1Z12345E0205271688";
           const string slug = "ups";

           CreateTrackingIfNotExist(trackingNumber, slug);
           TrackingToBeDeleted = new Tracking(trackingNumber) { Slug = slug };

           var result = connection.DeleteTracking(TrackingToBeDeleted);

           result.Should().BeTrue();
       }

       [TestMethod]
       public void PutTracking_ExistingTrackingIsSubmited_TrackingIsUpdated()
       {
           var now = DateTime.Now;

           TrackingToBeUpdated = new Tracking("ER751105042015062")
           {
              Slug = "ups",
              OrderIDPath = "OrderIDPathPost",
              CustomerName = "CustomerNamePost",
              OrderID = "OrderIDPost",
              Title = "TitlePost",
              UpdatedAt = now,
              DestinationCountryISO3 = ISO3Country.GBR
           };

           var result = connection.PutTracking(TrackingToBeUpdated);

           result.UpdatedAt.ToShortDateString().Should().Be(TrackingToBeUpdated.UpdatedAt.ToShortDateString());
       }
   }
}
