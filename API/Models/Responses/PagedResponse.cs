using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Responses
{
    public class PagedResponse<T>
    {
        public PagedResponse(IEnumerable<T> data)
        {
            Data = data;
        }

        public Dictionary<string, string> Meta { get; set; } = new Dictionary<string, string>();
        public IEnumerable<T> Data { get; set; }
        public Dictionary<string, string> Links { get; set; } = new Dictionary<string, string>();
    }
}