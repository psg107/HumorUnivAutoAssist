using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SimpleHttpClientWrapper.Helpers
{
    public static class QueryStringHelper
    {
        public static string ObjectToQueryString(object value)
        {
            var queries = value.GetType().GetProperties()
                    .Select(x =>
                    {
                        var displayAttribute = x.GetCustomAttributes(false).FirstOrDefault(y => y.GetType() == typeof(KeyNameAttribute)) as KeyNameAttribute;

                        var rawV = x.GetValue(value, null);

                        var k = displayAttribute?.Name ?? x.Name;
                        var v = rawV == null ? "null" : HttpUtility.UrlEncode(rawV.ToString());

                        return $"{k}={v}";
                    });
            var queryString = string.Join("&", queries);

            return queryString;
        }
    }
}
