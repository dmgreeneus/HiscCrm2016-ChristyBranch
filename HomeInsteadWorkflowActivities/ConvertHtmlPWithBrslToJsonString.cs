using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Text;
using Newtonsoft.Json;
using HtmlAgilityPack;


namespace HomeInsteadWorkflowActivities
{

    /// <summary>
    /// Converts String to Json String
    /// </summary>
    public class ConvertHtmlPWithBrsToJsonString : CodeActivity
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

            try {
                if (InputString.Get(executionContext) != null)
                {
                    JsonString.Set(executionContext, ParseHtmlPsToJsonString(tracingService, service, InputString.Get(executionContext)));
                }
            }
            catch
            {
            
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
        private String ParseHtmlPsToJsonString(ITracingService tracingService, IOrganizationService service, String input)
        {
            string output = null;
            StringBuilder jsonstring = new StringBuilder();
            jsonstring.Append("{");
            var document = new HtmlDocument();
            document.LoadHtml(input);
            HtmlNodeCollection ps = document.DocumentNode.SelectNodes("//p");
            if (ps != null && ps.Count > 0)
            {
                HtmlNode p = ps[0];
                string ptext = p.InnerText;
                string[] lines = ptext.Split('\r');
                foreach (string line in lines)
                {
                    string[] propertyvalue = line.Split(':');
                    if (propertyvalue.Length == 2)
                    {
                        if (jsonstring.Length > 1)
                        {
                            jsonstring.Append(",");
                        }
                        string property = propertyvalue[0].Replace(" ", string.Empty).Replace(":", string.Empty).Replace("&nbsp;", string.Empty).Replace("\r\n", string.Empty).Trim();
                        jsonstring.AppendFormat("'{0}':", property);
                        string value = propertyvalue[1].Replace(" ", string.Empty).Replace(":", string.Empty).Replace("&nbsp;", string.Empty).Replace("\r\n", string.Empty).Trim();
                        jsonstring.AppendFormat("'{0}'", value);
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
