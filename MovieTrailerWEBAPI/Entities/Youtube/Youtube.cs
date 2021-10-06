using MovieTrailerWEBAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TutorialWebApi.Entities
{
    public class Youtube
    {
        public Youtube()
        {
            id = new id();
            snippet = new snippet();
        }
        public id id { get; set; }
        public snippet snippet { get; set; }
    }
}
