using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Text;
using Newtonsoft.Json;
using HtmlAgilityPack;
using System.Linq;


namespace HomeInsteadWorkflowActivities
{

    /// <summary>
    /// Converts String to Json String
    /// </summary>
    public class ConvertHtmlTdsToJsonString : CodeActivity
    {

        /// <summary>
        /// String to Parse and Convert
        /// </summary>
        [Input("InputString")]
        public InArgument<String> InputString { get; set; }




        [Output("JsonString")]
        public OutArgument<String> JsonString { get; set; }


        /// <summary>
        /// Execute Method.
        /// </summary>
        /// <param name="context">CodeActivityContext object.</param>
        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();

            // Create the context
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                if (InputString.Get(executionContext) != null)
                {
                    JsonString.Set(executionContext, ParseHtmlTdsToJsonString(tracingService, service, InputString.Get(executionContext)));
                }
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message;
                throw new InvalidOperationException(exceptionMessage, ex);
            }

        }



        /// <summary>
        /// Clones a record and returns the newly created record information as EntityReference.
        /// </summary>
        /// <param name="tracingService">ITracingService object.</param>
        /// <param name="service">IOrganizationService object.</param>
        /// <param name="entityReference">EntityReference object of record to be cloned.</param>
        /// <param name="parameters">Any extra parameters to be added to the new record to be created.</param>
        /// <returns>Newly created record information as an entityreference object.</returns>
        private String ParseHtmlTdsToJsonString(ITracingService tracingService, IOrganizationService service, String input)
        {
            string output = null;
            StringBuilder jsonstring = new StringBuilder();
            jsonstring.Append("{");
            var document = new HtmlDocument();
            var isTD = true;
            document.LoadHtml(input);
            var metaTags = document.DocumentNode.SelectNodes("//meta");
            if (metaTags != null)
            {
                foreach (var tag in metaTags)
                {
                    if (tag.Attributes["name"] != null && tag.Attributes["content"] != null)
                    {
                        if (tag.Attributes["name"].Value == "Generator")

                        {
                            isTD = false;
                        }
                    }
                }
            }
            bool isPreviousProperty = false;
            string property = null;
            string value = null;
            if (isTD)
            {
                HtmlNodeCollection tds = document.DocumentNode.SelectNodes("//td");
                if (tds != null && tds.Count > 0)
                {
                    foreach (HtmlNode td in tds)
                    {
                        string tdtext = td.InnerText;
                        if ((tdtext.Contains(":") && !tdtext.Contains("Last Modified") && !tdtext.Contains(":/") ) || (tdtext.Contains("-") && !tdtext.Any(c => char.IsDigit(c))) || (tdtext.Contains("=") && !tdtext.Contains("?")))//TODO:  IDENTIFY PROPERTY VS VALUE BETTER
                        {
                            if (jsonstring.Length > 1)
                            {
                                jsonstring.Append(",");
                            }
                            isPreviousProperty = true;
                            property = td.InnerText.Replace(" ", string.Empty).Replace(":", string.Empty).Replace("&nbsp;", string.Empty).Replace("/", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Replace("-", string.Empty).Replace("\r\n", string.Empty).Replace("?",string.Empty).Replace("EMail","Email").Replace("#",string.Empty).Replace(",",string.Empty).Replace(".",string.Empty).Trim();
                            jsonstring.AppendFormat("'{0}':", property);
                        }
                        else if (isPreviousProperty)
                        {
                            isPreviousProperty = false;
                            value = td.InnerText.Replace("&nbsp;", string.Empty).Replace("\r\n", string.Empty).Replace("'",string.Empty).Trim();
                            jsonstring.AppendFormat("'{0}'", value);
                        }
                    }
                }
            }
            else
            {
                HtmlNodeCollection brs = document.DocumentNode.SelectNodes("//body");
                if (brs != null && brs.Count > 0)
                {
                    HtmlNode br = brs[0];
                    string brtext = br.InnerText;
                    string[] lines = brtext.Split('\r');
                    foreach (string line in lines)
                    {
                        string[] propertyvalue = line.Split(':');
                        if (propertyvalue.Length == 2)
                        {
                            if (jsonstring.Length > 1)
                            {
                                jsonstring.Append(",");
                            }
                            //property = propertyvalue[0].Replace(" ", string.Empty).Replace(":", string.Empty).Replace("&nbsp;", string.Empty).Replace("\r\n", string.Empty).Trim();
                            property = propertyvalue[0].Replace(" ", string.Empty).Replace(":", string.Empty).Replace("&nbsp;", string.Empty).Replace("?", string.Empty).Replace("/", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Replace("-", string.Empty).Replace("\r\n", string.Empty).Trim();
                            jsonstring.AppendFormat("'{0}':", property);
                            value = propertyvalue[1].Replace(":", string.Empty).Replace("&nbsp;", string.Empty).Replace("\r\n", string.Empty).Trim();
                            jsonstring.AppendFormat("'{0}'", value);
                        }
                    }
                }


            }


            jsonstring.Append("}");
            var deserialized = JsonConvert.DeserializeObject(jsonstring.ToString());
            output = JsonConvert.SerializeObject(deserialized);
            if (output == "{}") { output = null; }
            return output;

        }



    }

}
