using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TestCRMApiCode
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        string hardcodedUsername = "svc-dev-mldyn@MicrolandDynamics.onmicrosoft.com";
        string hardcodedPassword = "Microland@123";

        //string clientId = "04f79a58-2b65-4390-ab51-6c97328abc5f";
        //string clientId = "77560240-4a65-4d7c-8c65-540c285fcaf1";
        string clientId = "605a2058-8fda-4d4b-9353-7142ab85dd0a";

        //string clientSecret = "rI1ir6g6fB7Qqxub.8F9h2t8.y.uU-mvj_";
        //string clientSecret = "7S-L3VCA~BOInnd~Nc9Q_egu8W~Lo3D84r";
        //string clientSecret = "IP-jO~68Yv-DOYTNpI5y_r6g9~7~l2hM43";
        string clientSecret = "6M9.mdfp73nl.W4X-Y.ZpwHxfTj4ypqMu-";

        //string authority = "https://login.microsoftonline.com/971d5af1-c43d-4b58-882f-fa3271fc7f0a";
        //string authority = "https://login.microsoftonline.com/971d5af1-c43d-4b58-882f-fa3271fc7f0a";
        string authority = "https://login.microsoftonline.com/971d5af1-c43d-4b58-882f-fa3271fc7f0a";

        string resourceUrl = "https://mldev05.crm11.dynamics.com/"; // Org URL  


        protected void Page_Load(object sender, EventArgs e)
        {

            var _response = CrmRequest(
               HttpMethod.Get, resourceUrl + "api/data/v9.1/accounts?$select=accountid,accountnumber")
               .Result.Content.ReadAsStringAsync();

            if (_response is null)
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

            string accessToken;


            //try
            //{
            //    System.Diagnostics.Debugger.Break();
            //    UserPasswordCredential credential = new UserPasswordCredential(hardcodedUsername, hardcodedPassword);
            //    AuthenticationContext authenticationContext = new AuthenticationContext(authority);
            //    var authResult = await authenticationContext.AcquireTokenAsync(resourceUrl, clientId, credential);


            //    accessToken = authResult.AccessToken;
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}




            try
            {
                ClientCredential credentials = new ClientCredential(clientId, clientSecret);
                AuthenticationContext authenticationContext = new AuthenticationContext(authority);
                AuthenticationResult result = await authenticationContext.AcquireTokenAsync(resourceUrl, credentials).ConfigureAwait(false);
                accessToken = result.AccessToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }





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
            

            ///2.0 Leads

            ////CRMLeadModel leadModel = new CRMLeadModel()
            ////{
            ////    firstname = "Omar",
            ////    lastname = "Sheikh",
            ////    middlename = "Hassan"
            ////};
            ////var leadUrl = $"https://mldev05.crm8.dynamics.com/api/data/v9.0/leads";

            ////var json = JsonConvert.SerializeObject(leadModel);
            ////HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            ////var client = new HttpClient();
            ////client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            ////var resMsg =   await client.PostAsync(leadUrl, content);
            ////return resMsg;

            ///3.0 Accounts
            var AccountsUrl = $"https://mldev05.crm8.dynamics.com/api/data/v9.0/accounts";

            CRMAccountModel AccountModel = new CRMAccountModel()
            {
                name = "Sample Account Omar",
                creditonhold = false,
                address1_latitude = "47.639583",
                description = "This is the description of the sample account",
                revenue = 5000,
                accountcategorycode = 1
            };

            var json = JsonConvert.SerializeObject(AccountModel);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            //var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var resMsg = await client.PostAsync(AccountsUrl, content);
            return resMsg;

        }
    }

    /// <summary>
    /// Lead data model used to posted to CRM web api
    /// </summary>
    public class CRMLeadModel
    {
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string lastname { get; set; }
        public string telephone1 { get; set; }
    }

    public class CRMAccountModel
    {
        public string name { get; set; }
        public bool creditonhold { get; set; }
        public string address1_latitude { get; set; }
        public string description { get; set; }
        public int revenue { get; set; }
        public int accountcategorycode { get; set; }
    }
}
