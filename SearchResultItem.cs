using Sitecore.ContentSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Support.ECMReports
{
    public class SearchResultItem : Sitecore.ContentSearch.SearchTypes.SearchResultItem
    {
        [IndexField("_displayname")]
        public string DisplayName { get; set; }

        [IndexField("state")]
        public string State { get; set; }
 
    }
}
