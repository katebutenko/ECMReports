using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Diagnostics;
using Sitecore.Web.UI.WebControls;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Text;
using Sitecore.Links;
using System.Web.UI;
using Sitecore.Modules.EmailCampaign.Speak.Web.Core;
using Sitecore;
using System.Web;
using Sitecore.Shell.Framework.Commands;
using System.Collections.Specialized;
using Sitecore.SecurityModel;
using Sitecore.Web;
using Sitecore.Modules.EmailCampaign;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.XamlSharp.Continuations;
using Sitecore.Modules.EmailCampaign.Core;
using System.ComponentModel;
using Sitecore.ComponentModel;
using Sitecore.Configuration;
using Sitecore.Web.UI.WebControls.Extensions;

namespace Sitecore.Support.ECMReports.CSVExport
{
    public class SummaryReportCSVExportCommand: Command, ISupportsContinuation
    {
        public override void Execute(CommandContext context)
        {
            //ClientPipelineArgs args = new ClientPipelineArgs();
            //Util.StartClientPipeline(this, "Run", args);

            string rootID = UIFactory.Instance.GetSpeakContext().ManagerRoot.InnerItem.ID.ToString();

            Sitecore.Web.UI.Sheer.SheerResponse
       .Eval("window.top.location.href='" + "/sitecore modules/shell/EmailCampaign/Handlers/SitecoreSupportCsvExportHandler.ashx?action=SummaryReport&managerroot=" + rootID + "';");

        }

        private bool CreateList(string numberOfMessages)
        {
            //SheerResponse.
            string rootID = UIFactory.Instance.GetSpeakContext().ManagerRoot.InnerItem.ID.ToString();

            Sitecore.Web.UI.Sheer.SheerResponse
       .Eval("window.top.location.href='" + "/sitecore modules/shell/EmailCampaign/Handlers/SitecoreSupportCsvExportHandler.ashx?action=SummaryReport&managerroot=" + rootID+"';");

            //Sitecore.Web.WebUtil.Redirect("/sitecore modules/shell/EmailCampaign/Handlers/SitecoreSupportCsvExportHandler.ashx?action=SummaryReport&managerroot="+rootID);

            return true;
        }

            

            private Page GetCurrentPage()
            {
                return (HttpContext.Current.Handler as Page);
            }

            protected void Run(ClientPipelineArgs args)
            {
                if (!args.IsPostBack)
                {
                    SheerResponse.CheckModified(true);
                    args.WaitForPostBack();
                }
                else if ((args.Result != "cancel") && (args.HasResult && (args.Result != "cancel")))
                {
                    if ((args.Result == "yes") || (args.Result == "no"))
                    {
                        if (args.Result == "yes")
                        {
                            this.SaveChanges();
                        }
                        SheerResponse.Input("Enter the number of latest emails to export: ", "50", "^[1-9][0-9]", String.Format("'{0}' is not a valid number.", new object[] { "$Input" }), 100);
                        args.WaitForPostBack();
                    }
                    else if (this.CreateList(args.Result))
                    {
                        //SheerResponse.Alert("Finished", new string[0]);
                        //NotificationManager.Instance.Notify("RefreshRecipientLists", new EventArgs());
                    }
                    else
                    {
                        //SheerResponse.Alert(EcmTexts.Localize("The '{0}' list already exists, please choose another name.", new object[] { args.Result }), new string[0]);
                    }
                }
            }

            private void SaveChanges()
            {
                Page currentPage = this.GetCurrentPage();
                if (currentPage != null)
                {
                    IEnumerable<IChangeTracking> enumerable = currentPage.Controls.Flatten<IChangeTracking>();
                    if (Enumerable.All<IValidateChangeTracking>(currentPage.Controls.Flatten<IValidateChangeTracking>(), (Func<IValidateChangeTracking, bool>)(control => control.Validate(null))))
                    {
                        foreach (IChangeTracking tracking in enumerable)
                        {
                            if (tracking.IsChanged)
                            {
                                tracking.AcceptChanges();
                            }
                        }
                    }
                }
            }
        

 

    }
}
