using AftershipAPI.Enums;
using System.Collections.Generic;

namespace AftershipAPI
{
    public interface IConnectionAPI
    {
        Tracking CreateTracking(Tracking tracking);
        bool DeleteTracking(Tracking tracking);
        List<Courier> DetectCouriers(string trackingNumber);
        List<Courier> DetectCouriers(string trackingNumber, string trackingPostalCode, string trackingShipDate, string trackingAccountNumber, List<string> slugs);
        List<Courier> GetAllCouriers();
        List<Courier> GetCouriers();
        Checkpoint GetLastCheckpoint(Tracking tracking);
        Checkpoint GetLastCheckpoint(Tracking tracking, List<FieldCheckpoint> fields, string lang);
        Tracking GetTrackingByNumber(Tracking trackingGet);
        Tracking GetTrackingByNumber(Tracking trackingGet, List<FieldTracking> fields, string lang);
        List<Tracking> GetTrackings(int page);
        List<Tracking> GetTrackings(ParametersTracking parameters);
        List<Tracking> GetTrackingsNext(ParametersTracking parameters);
        List<string> ParseListFieldTracking(List<FieldTracking> list);
        Tracking PutTracking(Tracking tracking);
        Response Request(string method, string urlResource, string body);
        bool Retrack(Tracking tracking);
    }
}