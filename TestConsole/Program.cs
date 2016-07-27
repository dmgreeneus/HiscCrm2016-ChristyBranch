using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using HtmlAgilityPack;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //TEXT
            string input = null;
            input += "Franchise:      Home Instead Senior Care\r";
            input += "-------------------------------------------------------------------------\r";
            input += "First Name:     Isaac\r";
            input += "Last Name:      Olawuyi\r";
            input += "Email:  isaaccare@yahoo.com\r";
            input += "Address:        7217 Lighthouse Rd\r";
            input += "City:   Arlington\r";
            input += "State:  TX\r";
            input += "Zip Code:       76002\r";
            input += "Country:        US=\r";
            input += "Day Phone:      9039440888\r";
            input += "Available Capital:      $100,000 - $250,000\r";
            input += "Time Frame:     Within 90 days\r";
            input += "Day Phone:      9039440888\r";

            Console.WriteLine("STRING");
            //string output = ParseTextToJson(input);
            //Console.WriteLine(output);

            ////HTML TDs
            //string fileTD = @"C:\Users\dgreene\Documents\BizDev\Home Instead\Workflow\HiscCrm2016-ChristyBranch\TestConsole\HTMLPage1.html";
            //var documentTD = new HtmlDocument();
            //documentTD.Load(fileTD);
            //string inputhtmltd = documentTD.DocumentNode.InnerHtml;

            //Console.WriteLine("HTML TD");
            //string outputhtmltd = ParseHtmlTdsToJson(inputhtmltd);
            //Console.WriteLine(outputhtmltd);

            ////HTML Ps
            //string fileP = @"C:\Users\dgreene\Documents\BizDev\Home Instead\Workflow\HiscCrm2016-ChristyBranch\TestConsole\HTMLPage2.html";
            //var documentP = new HtmlDocument();
            //documentP.Load(fileP);
            //string inputhtmlp = documentP.DocumentNode.InnerHtml;

            //Console.WriteLine("HTML P");
            //string outputhtmlp = ParseHtmlPsToJson(inputhtmlp);
            //Console.WriteLine(outputhtmlp);

            ////HTML BRs
            //string fileBR = @"C:\Users\dgreene\Documents\BizDev\Home Instead\Workflow\HiscCrm2016-ChristyBranch\TestConsole\HTMLPage3.html";
            //var documentBR = new HtmlDocument();
            //documentBR.Load(fileBR);
            //string inputhtmlbr = documentBR.DocumentNode.InnerHtml;

            //Console.WriteLine("HTML BR");
            //string outputhtmlbr = ParseHtmlPWithBRsToJson(inputhtmlbr);
            //Console.WriteLine(outputhtmlbr);

            //HTML BODY BR's
            String fileBody = @"C:\Users\dgreene\Documents\BizDev\Home Instead\Workflow\HiscCrm2016-ChristyBranch\TestConsole\HTMLPage4.html";
            var documentBody = new HtmlDocument();
            documentBody.Load(fileBody);
            string inputhtmlbody = documentBody.DocumentNode.InnerHtml;

            Console.WriteLine("HTML Body");
            string outputhtmlbody = ParseHtmlBrsBodytoJson(inputhtmlbody);
            Console.WriteLine(outputhtmlbody);

            Console.ReadLine();
        }

        private static string ParseHtmlBrsBodytoJson(string input)
        {
            string output = null;
            var isTD = false;
            StringBuilder jsonstring = new StringBuilder();
            jsonstring.Append("{");
            string property = null;
            string value = null;
            if (!String.IsNullOrEmpty(input))
            {
                var document = new HtmlDocument();
                document.LoadHtml(input);
                var metaTags = document.DocumentNode.SelectNodes("//meta");
                
                if (metaTags != null)
                {
                    foreach (var tag in metaTags)
                    {
                        if (tag.Attributes["name"] != null && tag.Attributes["content"] != null)
                        {
                            if (tag.Attributes["name"].Value != "Generator")

                            {
                                isTD = true;
                               
                               
                            }
                        }
                    }
                }
                else
                {
                    isTD = false;
                    
                }
                if(!isTD)
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
                                property = propertyvalue[0].Replace(" ", string.Empty).Replace(":", string.Empty).Replace("&nbsp;", string.Empty).Replace("\r\n", string.Empty).Trim();
                                jsonstring.AppendFormat("'{0}':", property);
                                value = propertyvalue[1].Replace(" ", string.Empty).Replace(":", string.Empty).Replace("&nbsp;", string.Empty).Replace("\r\n", string.Empty).Trim();
                                jsonstring.AppendFormat("'{0}'", value);
                            }
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
        private static string ParseTextToJson(string input)
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
                var deserialized = JsonConvert.DeserializeObject(jsonstring.ToString());
                output = JsonConvert.SerializeObject(deserialized);
            }
            return output;
        }

        private static string ParseHtmlTdsToJson(string input)
        {
            string output = null;
            StringBuilder jsonstring = new StringBuilder();
            jsonstring.Append("{");
            var document = new HtmlDocument();
            document.LoadHtml(input);
            HtmlNodeCollection tds = document.DocumentNode.SelectNodes("//td");
            bool isPreviousProperty = false;
            string property = null;
            string value = null;
            if (tds != null && tds.Count > 0)
            {
                foreach (HtmlNode td in tds)
                {
                    string tdtext = td.InnerText;
                    if ((tdtext.Contains(":") && !tdtext.Contains("Last Modified") && !tdtext.Contains(":/") && !tdtext.Any(c => char.IsDigit(c))) ||(tdtext.Contains("-") && !tdtext.Any(c => char.IsDigit(c))) || (tdtext.Contains("=") && !tdtext.Contains("?")))//TODO:  IDENTIFY PROPERTY VS VALUE BETTER
                    {
                        if (jsonstring.Length > 1)
                        {
                            jsonstring.Append(",");
                        }
                        isPreviousProperty = true;
                        property = td.InnerText.Replace(" ", string.Empty).Replace(":", string.Empty).Replace("&nbsp;", string.Empty).Replace("\r\n", string.Empty).Trim();
                        jsonstring.AppendFormat("'{0}':", property);
                    }
                    else if (isPreviousProperty)
                    {
                        isPreviousProperty = false;
                        value = td.InnerText.Replace("&nbsp;", string.Empty).Replace("\r\n", string.Empty).Trim();
                        jsonstring.AppendFormat("'{0}'", value);
                    }
                }
            }
            jsonstring.Append("}");
            var deserialized = JsonConvert.DeserializeObject(jsonstring.ToString());
            output = JsonConvert.SerializeObject(deserialized);
            return output;
        }

        private static string ParseHtmlPsToJson(string input)
        {
            string output = null;
            StringBuilder jsonstring = new StringBuilder();
            jsonstring.Append("{");
            var document = new HtmlDocument();
            document.LoadHtml(input);
            HtmlNodeCollection ps = document.DocumentNode.SelectNodes("//p");
            if (ps != null && ps.Count > 0)
            {
                foreach (HtmlNode p in ps)
                {
                    string ptext = p.InnerText;
                    string[] propertyvalue = ptext.Split(':');
                    if (propertyvalue.Length == 2)
                    {
                        if (jsonstring.Length > 1)
                        {
                            jsonstring.Append(",");
                        }
                        string property = propertyvalue[0].Replace(" ", string.Empty).Replace(":", string.Empty).Replace("&nbsp;", string.Empty).Replace("\r\n", string.Empty).Trim();
                        jsonstring.AppendFormat("'{0}':", property);
                        string value = propertyvalue[1].Replace(":", string.Empty).Replace("&nbsp;", string.Empty).Replace("\r\n", string.Empty).Trim();
                        jsonstring.AppendFormat("'{0}'", value);
                    }
                }
            }
            jsonstring.Append("}");
            var deserialized = JsonConvert.DeserializeObject(jsonstring.ToString());
            output = JsonConvert.SerializeObject(deserialized);
            return output;
        }

        private static string ParseHtmlPWithBRsToJson(string input)
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
            return output;
        }
    }
}
