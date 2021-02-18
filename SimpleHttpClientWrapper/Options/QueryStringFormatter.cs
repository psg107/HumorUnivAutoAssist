using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SimpleHttpClientWrapper;

namespace SimpleHttpClientWrapper
{
    public class QueryStringFormatter : MediaTypeFormatter
    {
        public override bool CanReadType(Type type)
        {
            return true;
            //throw new NotImplementedException();
        }

        public override bool CanWriteType(Type type)
        {
            return true;
            //throw new NotImplementedException();
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                var queries = value.GetType().GetProperties()
                    .Where(x => x.GetValue(value, null) != null)
                    .Select(x =>
                    {
                        var displayAttribute = x.GetCustomAttributes(false).FirstOrDefault(y => y.GetType() == typeof(KeyNameAttribute)) as KeyNameAttribute;

                        var k = displayAttribute?.Name ?? x.Name;
                        var v = HttpUtility.UrlEncode(x.GetValue(value, null).ToString());

                        return $"{k}={v}";
                    });
                var queryString = string.Join("&", queries);
                var byteQuery = Encoding.UTF8.GetBytes(queryString);
                writeStream.Write(byteQuery, 0, byteQuery.Length);
            });
        }
    }
}
