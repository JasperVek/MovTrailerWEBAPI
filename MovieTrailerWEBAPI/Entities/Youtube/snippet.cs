using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTrailerWEBAPI.Entities
{
    public class snippet
    {
        public DateTime publishedAt { get; set; }
        public string title { get; set; }
        public string channelTitle { get; set; }
    }
}
