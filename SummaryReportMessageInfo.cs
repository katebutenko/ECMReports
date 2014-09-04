using Sitecore.Modules.EmailCampaign.Core;
using Sitecore.Modules.EmailCampaign.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Support.ECMReports
{
    public class SummaryReportMessageInfo: IMessageBase
    {
        // Fields
        private string id;

        public string ID
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value.TrimStart(new char[] { '{' }).TrimEnd(new char[] { '}' });
            }
        }

        public string Name { get; set; }

        public int Sent { get; set; }
        public int Delivered { get; set; }
        public double Opens { get; set; }
        public double Clicks { get; set; }
        
        //public string State { get; set; }
        public DateTime Date { get; set; }
 

        
    }

}
