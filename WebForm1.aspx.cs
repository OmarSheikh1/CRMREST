using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TestCRMApiCode
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        string clientId = "04f79a58-2b65-4390-ab51-6c97328abc5f";
        string clientSecret = "rI1ir6g6fB7Qqxub.8F9h2t8.y.uU-mvj_";
        protected void Page_Load(object sender, EventArgs e)
        {

            var accounts = CrmRequest(
               HttpMethod.Get,
               "https://mldev05.crm8.dynamics.com/api/data/v9.1/accounts?$select=accountid,accountnumber")
               .Result.Content.ReadAsStringAsync();

            if (accounts is null)
            {
            }
        }

        //public async Task<string> AccessTokenGenerator()
        //{
        //    try
        //    {
        //        string authority = "https://login.microsoftonline.com/971d5af1-c43d-4b58-882f-fa3271fc7f0a";
        //        string resourceUrl = "https://mldev05.crm8.dynamics.com/"; // Org URL  



        //        ClientCredential credentials = new ClientCredential(clientId, clientSecret);
        //        AuthenticationContext authenticationContext = new AuthenticationContext(authority);
        //        AuthenticationResult result = await authenticationContext.AcquireTokenAsync(resourceUrl, credentials).ConfigureAwait(false);

        //        //ClientCredential credentials = new ClientCredential(clientId, clientSecret);
        //        //AuthenticationContext authContext = new AuthenticationContext(authority);
        //        //var result = await authContext.AcquireTokenAsync(resourceUrl, credentials);
        //        return result.AccessToken;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }


        //}
        public async Task<HttpResponseMessage> CrmRequest(HttpMethod httpMethod, string requestUri, string body = null)
        {
            //var accessToken = await AccessTokenGenerator();

            string authority = "https://login.microsoftonline.com/971d5af1-c43d-4b58-882f-fa3271fc7f0a";
            string resourceUrl = "https://mldev05.crm8.dynamics.com/"; // Org URL  



            ClientCredential credentials = new ClientCredential(clientId, clientSecret);
            AuthenticationContext authenticationContext = new AuthenticationContext(authority);
            AuthenticationResult result = await authenticationContext.AcquireTokenAsync(resourceUrl, credentials).ConfigureAwait(false);

            var accessToken = result.AccessToken;

            var client = new HttpClient();
            var msg = new HttpRequestMessage(httpMethod, requestUri);
            msg.Headers.Add("OData-MaxVersion", "4.0");
            msg.Headers.Add("OData-Version", "4.0");
            msg.Headers.Add("Prefer", "odata.include-annotations=\"*\"");
            msg.Headers.Add("Accept", "application/json");
            //msg.Headers.Add("Content-Type", "application/json; charset=utf-8");

            // Passing AccessToken in Authentication header  
            msg.Headers.Add("Authorization", $"Bearer {accessToken}");

            //if (body != null)
            //    msg.Content = new StringContent(body, UnicodeEncoding.UTF8, "application/json");

            return await client.SendAsync(msg);
        }
    }
}