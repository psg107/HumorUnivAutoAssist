using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HumorUnivAutoAssist.Models
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
