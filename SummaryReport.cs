using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.EmailCampaign;
using Sitecore.Modules.EmailCampaign.Core;
using Sitecore.Modules.EmailCampaign.Core.Analytics;
using Sitecore.Modules.EmailCampaign.Messages;
using Sitecore.Modules.EmailCampaign.Speak.Web.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.StringExtensions;
using Sitecore.Data;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;

namespace Sitecore.Support.ECMReports
{
    public class SummaryReport
    {
        // Fields
        private readonly AnalyticsFactory analyticsFactory = AnalyticsFactory.Instance;
        
        private readonly ID rootID;

        // Methods
        public SummaryReport()
        {
            if (ID.IsNullOrEmpty(rootID))
            {
                rootID = UIFactory.Instance.GetSpeakContext().ManagerRoot.InnerItem.ID;
            }
        }      
        public SummaryReport(string managerRootID)
        {
            if (!String.IsNullOrEmpty(managerRootID))
            {
                rootID = ID.Parse(managerRootID);
            }            
        }      


        private SummaryReportMessageInfo GetSummaryInfo(MessageItem message, PlanData planData)
        {
            MessageStateInfo messageStateInfo = this.GetMessageStateInfo(message);
            SummaryReportMessageInfo info = new SummaryReportMessageInfo
            {
                ID = messageStateInfo.ID,
                Name = messageStateInfo.Name,
                Sent = messageStateInfo.Total, //returns GetAutomationStatesCount
                Delivered = messageStateInfo.Sent,   //return (this.Processed - this.Failed); Processed means all that are not in RecipientsQueued or Send In Progress state
                Date = messageStateInfo.Updated,
                             
            };
            //int emailCount = -1;
            if (planData != null)
            {
                PlanStatistics planStatistics = this.analyticsFactory.GetPlanStatistics(planData);
                info.Opens = planStatistics.GetOpened();
                info.Clicks = planStatistics.GetClicked();
                //emailCount = planStatistics.GetActual(); //  return (((this.GetTotal() - this.Data.InvalidAddress) - this.Data.MailBounced) - this.Data.Unsent);

            }
           
            return info;
        }

        protected virtual MessageStateInfo GetMessageStateInfo(MessageItem message)
        {
            return new MessageStateInfo(message);
        }

        public MessageItem GetMessage(SearchResultItem i)
        {
            return Factory.GetMessage(i.ItemId);
        }

        public List<SummaryReportMessageInfo> GetRecentlyDispatched(string query, string sortArgument, int startIndex, int pageSize)
        {
            Assert.ArgumentNotNullOrEmpty(query, "query");
            Assert.ArgumentNotNullOrEmpty(sortArgument, "sortArgument");

            List<MessageItem> messages = (from m in Enumerable.Select<SearchResultItem, MessageItem>(this.GetSortedItems(query, sortArgument).Skip<SearchResultItem>(startIndex).Take<SearchResultItem>(pageSize), 
                                              this.GetMessage)
                                          where m != null
                                          select m).ToList<MessageItem>();
            if (messages.Count == 0)
            {
                return new List<SummaryReportMessageInfo>();
            }            
            List<PlanData> planDataList = this.analyticsFactory.GetAnalyticsDataGateway().GetPlanData(messages);
            return (from m in messages select this.GetSummaryInfo(m, Enumerable.FirstOrDefault<PlanData>(planDataList, (Func<PlanData, bool>)(item => (item.PlanId == m.PlanId.ToGuid()))))).ToList<SummaryReportMessageInfo>();
        }

        protected virtual IEnumerable<SearchResultItem> GetSortedItems(string query, string sortArgument)
        {                                             
            Assert.ArgumentNotNullOrEmpty(query, "query");
            Assert.ArgumentNotNullOrEmpty(sortArgument, "sortArgument");

            Func<SearchResultItem, object> func;
            if (sortArgument.StartsWith("Name "))
            {
                func = i => i.DisplayName;
            }
            else
            {
                func = i => i.Updated;
            }
            
            List<SearchResultItem> items;
            using (var searchContext = ContentSearchManager.GetIndex("sitecore_master_index").CreateSearchContext())
            {
                IQueryable<SearchResultItem> queryable = searchContext.GetQueryable<SearchResultItem>();
                
                IOrderedEnumerable<SearchResultItem> results;                
                
                if (!sortArgument.EndsWith(" ASC", StringComparison.OrdinalIgnoreCase))
                {
                    results = queryable.Where(x => x.Paths.Contains(rootID) && ((x["state"] == "Sent") || (x["state"] == "Sending"))).OrderByDescending(func);
                }
                else
                {
                    results = queryable.Where(x => x.Paths.Contains(rootID) && ((x["state"] == "Sent") || (x["state"] == "Sending"))).OrderBy(func);
                }
                
                items = results.ToList();                
            }
            return items;                       
        }

    }
}
