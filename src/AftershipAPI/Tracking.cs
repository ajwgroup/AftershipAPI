using AftershipAPI.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AftershipAPI
{

    public class Tracking
    {
        ///Tracking ID in the Afthership system 
        [JsonProperty("id")]
        public string Id { get; set; }

        ///Tracking number of a shipment. Duplicate tracking numbers, or tracking number with invalid tracking
        ///number format will not be accepted. 
        [JsonProperty("tracking_number")]
        public string TrackingNumber { get; set; }

        ///Unique code of each courier. If you do not specify a slug, Aftership will automatically detect
        ///the courier based on the tracking number format and your selected couriers
        [JsonProperty("slug")]
        public string Slug { get; set; }

        /// Email address(es) to receive email notifications. Use comma for multiple emails. 
        [JsonProperty("emails")]
        public List<string> Emails { get; set; } = new List<string>();

        /// Phone number(s) to receive sms notifications. Use comma for multiple emails.
        ///Enter + area code before phone number. 
        [JsonProperty("smses")]
        public List<string> Smses { get; set; } = new List<string>();

        /// Title of the tracking. Default value as trackingNumber 
        [JsonProperty("title")]
        public string Title { get; set; }

        /// Customer name of the tracking. 
        [JsonProperty("customer_name")]
        public string CustomerName { get; set; }

        /// ISO Alpha-3(three letters)to specify the destination of the shipment.
        /// If you use postal service to send international shipments, AfterShip will automatically
        /// get tracking results at destination courier as well (e.g. USPS for USA). 
        [JsonProperty("destination_country_iso3")]
        public ISO3Country? DestinationCountryISO3 { get; set; }

        ///  Origin country of the tracking. ISO Alpha-3 
        [JsonProperty("origin_country_iso3")]
        public ISO3Country? OriginCountryISO3 { get; set; }

        /// Text field for order ID 
        [JsonProperty("order_id")]
        public string OrderID { get; set; }

        /// Text field for order path 
        [JsonProperty("order_id_path")]
        public string OrderIDPath { get; set; }

        /// Custom fields that accept any TEXT STRING
        [JsonProperty("custom_fields")]
        public Dictionary<string, string> CustomFields { get; set; } = new Dictionary<string, string>();

        /// fields informed by Aftership API

        ///  Date and time of the tracking created. 
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        /// Date and time of the tracking last updated. 
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        /// Whether or not AfterShip will continue tracking the shipments.
        ///Value is `false` when status is `Delivered` or `Expired`. 
        [JsonProperty("active")]
        public bool Active { get; set; }

        /// Expected delivery date (if any). 
        [JsonProperty("expected_delivery")]
        public string ExpectedDelivery { get; set; }

        ///  Number	Number of packages under the tracking. 
        [JsonProperty("shipment_package_count")]
        public int ShipmentPackageCount { get; set; }

        ///  Number of attempts AfterShip tracks at courier's system. 
        [JsonProperty("tracked_count")]
        public int TrackedCount { get; set; }

        /// Shipment type provided by carrier (if any). 
        [JsonProperty("shipment_type")]
        public string ShipmentType { get; set; }

        /// Signed by information for delivered shipment (if any). 
        [JsonProperty("signed_by")]
        public string SignedBy { get; set; }

        ///  Source of how this tracking is added.  
        [JsonProperty("source")]
        public string Source { get; set; }

        /// Current status of tracking. 
        [JsonProperty("tag")]
        public StatusTag Tag { get; set; }

        /// Array of Hash describes the checkpoint information. 
        [JsonProperty("checkpoints")]
        public List<Checkpoint> Checkpoints { get; set; } = new List<Checkpoint>();

        ///Tracking Account number tracking_account_number
        [JsonProperty("tracking_account_number")]
        public string TrackingAccountNumber { get; set; }

        ///Tracking postal code tracking_postal_code
        [JsonProperty("tracking_postal_code")]
        public string TrackingPostalCode { get; set; }

        ///Tracking ship date tracking_ship_date
        [JsonProperty("tracking_shipment_date")]
        public string TrackingShipDate { get; set; }

        public void AddEmails(string emails) => Emails.Add(emails);

        public void DeleteEmail(string email) => Emails.Remove(email);

        public void AddSmses(string smses) => Smses.Add(smses);

        public void DeleteSmes(string smses) => Smses.Remove(smses);

        public void AddCustomFields(string field, string value) => CustomFields.Add(field, value);

        public void DeleteCustomFields(string field) => CustomFields.Remove(field);

        public Tracking(){}

        public Tracking(string trackingNumber)
        {
            TrackingNumber = trackingNumber;
            Title = trackingNumber;
        }

        public string GetQueryRequiredFields()
        {
            bool containsInfo = false;
            var qs = new Querystring();
            if (TrackingAccountNumber != null)
            {
                containsInfo = true;
                qs.Add("tracking_account_number", TrackingAccountNumber);
            }
            if (TrackingPostalCode != null)
            {
                qs.Add("tracking_postal_code", TrackingPostalCode);
                containsInfo = true;
            }
            if (TrackingShipDate != null)
            {
                qs.Add("tracking_ship_date", TrackingShipDate);
                containsInfo = true;
            }
            if (containsInfo)
            {
                return qs.ToString();
            }
            return "";
        }

        public override string ToString() => $"_id: {Id}\n_trackingNumber: {TrackingNumber}\n_slug: {Slug}";
    }
}
