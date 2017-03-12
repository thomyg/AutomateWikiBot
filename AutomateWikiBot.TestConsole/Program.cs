using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomateWikiBot.Utils;


namespace AutomateWikiBot.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Hello to the AutomateWikiBot test console ...");

            WikiPageManager wp = new WikiPageManager("tenantURL","clientID","clientSecrete");

            wp.ProcessChanges("webID", "listID", "siteUrl");

                        
            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
            
            
        }
    }
}
