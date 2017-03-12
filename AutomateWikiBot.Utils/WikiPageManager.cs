using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace AutomateWikiBot.Utils
{
    public class WikiPageManager
    {

        public string TenantURL { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }

        public string AccessToken { get; set; }

        public WikiPageManager(string tenantURL, string clientID, string clientSecret )
        {
            this.TenantURL = tenantURL;
            this.ClientID = clientID;
            this.ClientSecret = clientSecret;    
        } 

        public void ProcessChanges(string webID, string listID, string siteUrl)
        {
            ClientContext cc = null;
            try
            {
                #region Setup SharePoint Online ClientContext
                //DO NOT USE THIS IN PRODUCTION
                //Try to implement proper authentication based on Azure AD app instead!!! 
                string url = String.Format("https://{0}{1}", TenantURL, siteUrl);
                string initString = "YOUR PASSWORD";
                // Instantiate the secure string.
                SecureString testString = new SecureString();
                // Use the AppendChar method to add each char value to the secure string.
                foreach (char ch in initString)
                    testString.AppendChar(ch);

                cc = new ClientContext(url);
                cc.AuthenticationMode = ClientAuthenticationMode.Default;
                cc.Credentials = new SharePointOnlineCredentials("YOUR USER", testString);
                #endregion

                #region Grab the wikipage PublishingPageContent for the page the web hook was triggered
                Web web = cc.Site.OpenWebById(new Guid(webID));
                cc.Load(web);
                List ls = web.Lists.GetById(new Guid(listID));
                cc.Load(ls);                
                ChangeQuery query = new ChangeQuery(false, false);
                query.Item = true;                
                query.Update = true;
                ChangeCollection changes = ls.GetChanges(query);
                cc.Load(changes);
                cc.ExecuteQuery();

                Console.WriteLine("Starting to process changes for List {0} in Web {1} under site {2}", ls.Title, web.Title, siteUrl);

                Dictionary<String, String> qna = new Dictionary<string, string>();

                foreach (Change change in changes)
                {
                    if(change is Microsoft.SharePoint.Client.ChangeItem)
                    {
                        ChangeItem ci = change as ChangeItem;
                        var changeType = ci.ChangeType.ToString();
                        var itemId = ci.ItemId.ToString();
                        Console.WriteLine("Going to process changes for {0}: {1}", itemId, changeType);
                        Dictionary<string, string> res=  this.GetWikiPageContententForKB(ci.ItemId,new Guid(listID), siteUrl);
                        foreach (string key in res.Keys)
                        {
                            if(!qna.ContainsKey(key))
                            {
                                qna.Add(key, res[key]);
                            }
                        }
                        
                    }
                }

                KnowledgeBaseManager kb = new KnowledgeBaseManager("https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/knowledgebases/", "CLIENT SECRETE", "KNOWLEDGEBASE ID");
                kb.AddQnAToKB(qna);
                kb.PublishKB();

                #endregion

            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (cc != null)
                {
                    cc.Dispose();
                }

            }
        }
        public Dictionary<String, String> GetWikiPageContententForKB(int itemID, Guid listID, string siteUrl)
        {
            Dictionary<String, String> qna = new Dictionary<string, string>();
            ClientContext cc = null;
            try
            {
                #region Setup SharePoint Online ClientContext              
                //DO NOT USE THIS IN PRODUCTION
                //Try to implement proper authentication based on Azure AD app instead!!! 
                string url = String.Format("https://{0}{1}", TenantURL, siteUrl);            
                string initString = "PASSWORD";
                // Instantiate the secure string.
                SecureString testString = new SecureString();
                // Use the AppendChar method to add each char value to the secure string.
                foreach (char ch in initString)
                    testString.AppendChar(ch);

                cc = new ClientContext(url);
                cc.AuthenticationMode = ClientAuthenticationMode.Default;
                cc.Credentials = new SharePointOnlineCredentials("USER", testString);

                #endregion

                #region Grab the wikipage PublishingPageContent for the page the web hook was triggered
                List ls = cc.Web.Lists.GetById(listID);
                ListItem item = ls.GetItemById(itemID);
                cc.Load(item);
                cc.ExecuteQuery();

                String wikicontent = item["PublishingPageContent"].ToString();

                var html = new HtmlDocument();
                html.LoadHtml(wikicontent);                
                string header = "";                                

                foreach (HtmlNode node in html.DocumentNode.ChildNodes)
                {
                    if (node.Name.StartsWith("h"))
                    {
                        header = node.InnerText;
                    }
                    else if (node.Name.StartsWith("p"))
                    {
                        if (!qna.ContainsKey(header))
                        {
                            qna.Add(header, node.InnerText);
                        }
                        else
                        {
                            var existingValue = qna[header];
                            qna.Remove(header);
                            qna.Add(header, existingValue + " " + node.InnerText);
                        }
                    }
                }


                
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (cc != null)
                {
                    cc.Dispose();
                }
                #endregion

            }            
            return qna;
        }
    }
}
