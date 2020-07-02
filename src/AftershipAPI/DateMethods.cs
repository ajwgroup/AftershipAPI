using System;
// changes to support Russian culture, check this http://stackoverflow.com/questions/2193012/string-was-not-recognized-as-a-valid-datetime-format-dd-mm-yyyy

namespace AftershipAPI
{
    public static class DateMethods
    {
        private const string ISO8601Short = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";

        // since we pass it to UniversalTime we can add the +00:00 manually
        public static string ToString(DateTime date) => date.ToString(ISO8601Short);

        public static string ToISO8601Short(this DateTime date) => date.ToString(ISO8601Short);
    }
}