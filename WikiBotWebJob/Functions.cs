using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using AutomateWikiBot.Utils;

namespace WikiBotWebJob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("YOUR QUEUE REFERENCE")] NotificationModel notification, TextWriter log)
        {
            log.WriteLine(String.Format("Processing subscription {0} for site {1}", notification.SubscriptionId, notification.SiteUrl));
            Console.WriteLine(String.Format("Processing subscription {0} for site {1}", notification.SubscriptionId, notification.SiteUrl));
            WikiPageManager wp = new WikiPageManager("tenantURL","CLIENT ID" , "CLIENT SECRET");
            wp.ProcessChanges(notification.WebId, notification.Resource, notification.SiteUrl);
                
        }
    }
}
