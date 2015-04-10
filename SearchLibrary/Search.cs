using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOCSEARCHOUTSERVER.SearchLibrary
{
    public class Search
    {
        public Search(string _ID)
        {
            ID = _ID;
            docs = new List<string>();
        }
        public IList<string> docs { get; set; }
        public string ID { get; set; }
    }
}