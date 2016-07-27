using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Text;
//using Newtonsoft.Json;



namespace HomeInsteadWorkflowActivities
{

    /// <summary>
    /// Converts String to Json String
    /// </summary>
    public class ConvertTextToJsonString : CodeActivity
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

            if (InputString.Get(executionContext) != null)
            {
                JsonString.Set(executionContext, ParseStringToJsonString(tracingService, service, InputString.Get(executionContext)));
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
        private String ParseStringToJsonString(ITracingService tracingService, IOrganizationService service, String input)
        {
            string output = null;
            StringBuilder jsonstring = new StringBuilder();
            if (!String.IsNullOrEmpty(input))
            {
                string[] lines = input.Split('\r');
                jsonstring.Append("{");
                foreach (string line in lines)
                {
                    string[] propertyvalue = line.Split(':');
                    if (propertyvalue.Length == 2)
                    {
                        if (jsonstring.Length > 1)
                        {
                            jsonstring.Append(",");
                        }
                        string property = propertyvalue[0].Trim().Replace(" ", string.Empty);
                        jsonstring.AppendFormat("'{0}':", property);
                        string value = propertyvalue[1].Trim();
                        jsonstring.AppendFormat("'{0}'", value);
                    }
                }
                jsonstring.Append("}");
                //TODO:  Uncomment once get newtonjson.dll installed in GAC
                //var deserialized = JsonConvert.DeserializeObject(jsonstring.ToString());
                //output = JsonConvert.SerializeObject(deserialized);
                output = jsonstring.ToString();    //TODO:  Remove once get newtonjson.dll installed in GAC
                if (output == "{}") { output = null; }
            }
            return output;
        }



    }

}
