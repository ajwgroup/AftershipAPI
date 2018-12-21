using AftershipAPI.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AftershipAPI
{
    public class Tracking
    {
        ///Tracking ID in the Afthership system 
        public string Id { get; set; }

        ///Tracking number of a shipment. Duplicate tracking numbers, or tracking number with invalid tracking
        ///number format will not be accepted. 
        public string TrackingNumber { get; set; }

        ///Unique code of each courier. If you do not specify a slug, Aftership will automatically detect
        ///the courier based on the tracking number format and your selected couriers
        public string Slug { get; set; }

        /// Email address(es) to receive email notifications. Use comma for multiple emails. 
        public List<string> Emails { get; set; } = new List<string>();

        /// Phone number(s) to receive sms notifications. Use comma for multiple emails.
        ///Enter + area code before phone number. 
        public List<string> Smses { get; set; } = new List<string>();

        /// Title of the tracking. Default value as trackingNumber 
        public string Title { get; set; }

        /// Customer name of the tracking. 
        public string CustomerName { get; set; }

        /// ISO Alpha-3(three letters)to specify the destination of the shipment.
        /// If you use postal service to send international shipments, AfterShip will automatically
        /// get tracking results at destination courier as well (e.g. USPS for USA). 
        public ISO3Country DestinationCountryISO3 { get; set; }

        ///  Origin country of the tracking. ISO Alpha-3 
        public ISO3Country OriginCountryISO3 { get; set; }

        /// Text field for order ID 
        public string OrderID { get; set; }

        /// Text field for order path 
        public string OrderIDPath { get; set; }

        /// Custom fields that accept any TEXT STRING
        public Dictionary<string, string> CustomFields { get; set; } = new Dictionary<string, string>();

        /// fields informed by Aftership API

        ///  Date and time of the tracking created. 
        public DateTime CreatedAt { get; set; }

        /// Date and time of the tracking last updated. 
        public DateTime UpdatedAt { get; set; }

        /// Whether or not AfterShip will continue tracking the shipments.
        ///Value is `false` when status is `Delivered` or `Expired`. 
        public bool Active { get; set; }

        /// Expected delivery date (if any). 
        public string ExpectedDelivery { get; set; }

        ///  Number	Number of packages under the tracking. 
        public int ShipmentPackageCount { get; set; }

        ///  Number of attempts AfterShip tracks at courier's system. 
        public int TrackedCount { get; set; }

        /// Shipment type provided by carrier (if any). 
        public string ShipmentType { get; set; }

        /// Signed by information for delivered shipment (if any). 
        public string SignedBy { get; set; }

        ///  Source of how this tracking is added.  
        public string Source { get; set; }

        /// Current status of tracking. 
        public StatusTag Tag { get; set; }

        /// Array of Hash describes the checkpoint information. 
        public List<Checkpoint> Checkpoints { get; set; } = new List<Checkpoint>();

        ///Tracking Account number tracking_account_number
        public string TrackingAccountNumber { get; set; }

        ///Tracking postal code tracking_postal_code
        public string TrackingPostalCode { get; set; }

        ///Tracking ship date tracking_ship_date
        public string TrackingShipDate { get; set; }

        public void AddEmails(string emails) => Emails.Add(emails);

        public void DeleteEmail(string email) => Emails.Remove(email);

        public void AddSmses(string smses) => Smses.Add(smses);

        public void DeleteSmes(string smses) => Smses.Remove(smses);

        public void AddCustomFields(string field, string value) => CustomFields.Add(field, value);

        public void DeleteCustomFields(string field) => CustomFields.Remove(field);

        public Tracking(string trackingNumber)
        {
            TrackingNumber = trackingNumber;
            Title = trackingNumber;
        }

        public Tracking(JObject trackingJSON)
        {
            Id = trackingJSON["id"] == null ? null : (string)trackingJSON["id"];

            //fields that can be updated by the user
            TrackingNumber = trackingJSON["tracking_number"] == null ? null : (string)trackingJSON["tracking_number"];
            Slug = trackingJSON["slug"] == null ? null : (string)trackingJSON["slug"];
            Title = trackingJSON["title"] == null ? null : (string)trackingJSON["title"];
            CustomerName = trackingJSON["customer_name"] == null ? null : (string)trackingJSON["customer_name"];
            string destination_country_iso3 = (string)trackingJSON["destination_country_iso3"];

            if (destination_country_iso3 != null && destination_country_iso3 != string.Empty)
                DestinationCountryISO3 = (ISO3Country)Enum.Parse(typeof(ISO3Country), destination_country_iso3);

            OrderID = trackingJSON["order_id"] == null ? null : (string)trackingJSON["order_id"];

            OrderIDPath = trackingJSON["order_id_path"] == null ? null : (string)trackingJSON["order_id_path"];

            TrackingAccountNumber = trackingJSON["tracking_account_number"] == null ? null : (string)trackingJSON["tracking_account_number"];
            TrackingPostalCode = trackingJSON["tracking_postal_code"] == null ? null :
                (string)trackingJSON["tracking_postal_code"];
            TrackingShipDate = trackingJSON["tracking_ship_date"] == null ? null :
                (string)trackingJSON["tracking_ship_date"];

            JArray smsesArray = trackingJSON["smses"] == null ? null : (JArray)trackingJSON["smses"];
            if (smsesArray != null && smsesArray.Count != 0)
                Smses.AddRange(smsesArray.Select(smses => (string)smses));

            JArray emailsArray = trackingJSON["emails"] == null ? null : (JArray)trackingJSON["emails"];
            if (emailsArray != null && emailsArray.Count != 0)
                Emails.AddRange(emailsArray.Select(email => (string)email));

            JObject customFieldsJSON = trackingJSON["custom_fields"] == null || !trackingJSON["custom_fields"].HasValues ? null :
                (JObject)trackingJSON["custom_fields"];

            if (customFieldsJSON != null)
            {
                List<JProperty> keys = customFieldsJSON.Properties().ToList();
                keys.ForEach(item => CustomFields.Add(item.Name, (string)customFieldsJSON[item.Name]));
            }

            //fields that can't be updated by the user, only retrieve
            CreatedAt = trackingJSON["created_at"] == null ? DateTime.MinValue : (DateTime)trackingJSON["created_at"];
            UpdatedAt = trackingJSON["updated_at"] == null ? DateTime.MinValue : (DateTime)trackingJSON["updated_at"];
            ExpectedDelivery = trackingJSON["expected_delivery"] == null ? null : (string)trackingJSON["expected_delivery"];
            Active = trackingJSON["active"] == null ? false : (bool)trackingJSON["active"];
            string origin_country_iso3 = (string)trackingJSON["origin_country_iso3"];

            if (!string.IsNullOrEmpty(origin_country_iso3))
                OriginCountryISO3 = (ISO3Country)Enum.Parse(typeof(ISO3Country), origin_country_iso3);

            ShipmentPackageCount = trackingJSON["shipment_package_count"] == null ? 0 :
                (int)trackingJSON["shipment_package_count"];

            ShipmentType = trackingJSON["shipment_type"] == null ? null : (string)trackingJSON["shipment_type"];
            SignedBy = trackingJSON["singned_by"] == null ? null : (string)trackingJSON["signed_by"];
            Source = trackingJSON["source"] == null ? null : (string)trackingJSON["source"];
            Tag = (string)trackingJSON["tag"] == null ? 0 :
                (StatusTag)Enum.Parse(typeof(StatusTag), (string)trackingJSON["tag"]);

            TrackedCount = trackingJSON["tracked_count"] == null ? 0 : (int)trackingJSON["tracked_count"];

            // checkpoints
            JArray checkpointsArray = trackingJSON["checkpoints"] == null ? null : (JArray)trackingJSON["checkpoints"];
            if (checkpointsArray != null && checkpointsArray.Count != 0)
                Checkpoints = checkpointsArray.Select(checkpoint => new Checkpoint((JObject)checkpoint)).ToList();
        }

        public string GetJSONPost()
        {
            var trackingJSON = new JObject
            {
                { "tracking_number", new JValue(TrackingNumber) }
            };

            if (Slug != null) trackingJSON.Add("slug", new JValue(Slug));
            if (Title != null) trackingJSON.Add("title", new JValue(Title));
            if (Emails.Any()) trackingJSON["emails"] = new JArray(Emails);
            if (Smses.Any()) trackingJSON["smses"] = new JArray(Smses);
            if (CustomerName != null) trackingJSON.Add("customer_name", new JValue(CustomerName));
            if (DestinationCountryISO3 != 0) trackingJSON.Add("destination_country_iso3", new JValue(DestinationCountryISO3.ToString()));
            if (OrderID != null) trackingJSON.Add("order_id", new JValue(OrderID));
            if (OrderIDPath != null) trackingJSON.Add("order_id_path", new JValue(OrderIDPath));
            if (TrackingAccountNumber != null) trackingJSON.Add("tracking_account_number", new JValue(TrackingAccountNumber));
            if (TrackingPostalCode != null) trackingJSON.Add("tracking_postal_code", new JValue(TrackingPostalCode));
            if (TrackingShipDate != null) trackingJSON.Add("tracking_ship_date", new JValue(TrackingShipDate));
            if (CustomFields.Any()) trackingJSON["custom_fields"] = JObject.FromObject(CustomFields);

            var globalJSON = new JObject
            {
                ["tracking"] = trackingJSON
            };

            return globalJSON.ToString();
        }

        public string GeneratePutJSON()
        {
            var trackingJSON = new JObject();
            if (Title != null) trackingJSON.Add("title", new JValue(Title));
            if (Emails.Any()) trackingJSON["emails"] = new JArray(Emails);
            if (Smses.Any()) trackingJSON["smses"] = new JArray(Smses);
            if (CustomerName != null) trackingJSON.Add("customer_name", new JValue(CustomerName));
            if (OrderID != null) trackingJSON.Add("order_id", new JValue(OrderID));
            if (OrderIDPath != null) trackingJSON.Add("order_id_path", new JValue(OrderIDPath));
            if (CustomFields.Any()) trackingJSON["custom_fields"] = JObject.FromObject(CustomFields);

            var globalJSON = new JObject
            {
                ["tracking"] = trackingJSON
            };

            return globalJSON.ToString();
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
