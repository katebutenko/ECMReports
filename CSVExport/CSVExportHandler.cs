using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Sitecore.Diagnostics;
using Sitecore.Data;
using Sitecore.Modules.EmailCampaign.Speak.Web.Core;
using Sitecore.Modules.EmailCampaign;
using Sitecore.Modules.EmailCampaign.Messages;
using Sitecore.Modules.EmailCampaign.Core;

namespace Sitecore.Support.ECMReports.CSVExport
{
    public class CSVExportHandler : IHttpHandler
    {   
        public void ProcessRequest(HttpContext httpContext)
        {
            Assert.ArgumentNotNull(httpContext, "httpContext");
            string action = httpContext.Request.Params["action"];

            if (action == "SummaryReport")
            {

                SummaryReport emailInfoRep = new SummaryReport(httpContext.Request.Params["managerroot"]);
                IEnumerable<SummaryReportMessageInfo> dataItems = emailInfoRep.GetRecentlyDispatched("anything","Updated DESC",0,50);

                string detailListId = "{2F6E3CBB-0B13-4254-9018-0423D7D4D5DB}"; // an ID of the item to format csv

                string export = CsvExport.ExportDetailsListToCsv(dataItems, detailListId);

                string filename = "SummaryReport_Latest_50_Emails_" + DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
                try
                {
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ContentType = "text/csv";
                    HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".csv");
                    HttpContext.Current.Response.Write(export);
                }
                catch (Exception exception)
                {
                    Log.Error(exception.Message, exception, this);
                }
                return;
            }
                                       
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
 

 

    }
}
