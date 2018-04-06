using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace TudolinkWeb.Utils
{
    public class SendCloudApi
    {
        public static bool Send(string mail, string subject, string html)
        {
            String url = "http://sendcloud.sohu.com/webapi/mail.send.json";


            AppConfig config = new AppConfig();
            var api_user = config.AppConfigGet("SendCloudApi_User");
            var api_key = config.AppConfigGet("SendCloudApi_Key");
    
            HttpClient client = null;
            HttpResponseMessage response = null;

            try
            {
                client = new HttpClient();

                List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();

                paramList.Add(new KeyValuePair<string, string>("api_user", api_user));
                paramList.Add(new KeyValuePair<string, string>("api_key", api_key));
                paramList.Add(new KeyValuePair<string, string>("from", "service@tudolink.org"));
                paramList.Add(new KeyValuePair<string, string>("fromname", "ToduLink"));
                paramList.Add(new KeyValuePair<string, string>("to", mail));
                paramList.Add(new KeyValuePair<string, string>("subject", subject));
                paramList.Add(new KeyValuePair<string, string>("html", html));

                response = client.PostAsync(url, new FormUrlEncodedContent(paramList)).Result;
                String result = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine("result:{0}", result);
                return result.Contains("success");
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            finally
            {
                if (null != client)
                {
                    client.Dispose();
                }
            }
            return false;
        }

        static void Main1(string[] args)
        {
            String tos = "to1@sendcloud.org;to2@sendcloud.org";
            //Send(tos);
            Console.ReadKey();
        }

    }


}