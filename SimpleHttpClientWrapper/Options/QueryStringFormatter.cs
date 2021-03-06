﻿using SimpleHttpClientWrapper.Helpers;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;

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
                var queryString = QueryStringHelper.ObjectToQueryString(value);
                var byteQuery = Encoding.UTF8.GetBytes(queryString);
                writeStream.Write(byteQuery, 0, byteQuery.Length);
            });
        }
    }
}
