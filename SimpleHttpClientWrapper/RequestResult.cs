using System.Collections.Generic;
using System.Net;

namespace SimpleHttpClientWrapper
{
    public class RequestResult
    {
        public bool Success
        {
            get => StatusCode == (int)HttpStatusCode.OK;
        }

        public int StatusCode { get; set; }

        public List<KeyValuePair<string, IEnumerable<string>>> Headers { get; set; }

        public string Data { get; set; }
    }

    public class RequestResult<T> where T : class
    {
        public bool Success
        {
            get => StatusCode == (int)HttpStatusCode.OK;
        }

        public int StatusCode { get; set; }

        public List<KeyValuePair<string, IEnumerable<string>>> Headers { get; set; }

        public T Data { get; set; }
    }
}
