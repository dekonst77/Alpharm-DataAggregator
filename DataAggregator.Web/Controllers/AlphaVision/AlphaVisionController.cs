
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Alphavision;
using DataAggregator.Domain.Model.Alphavision.User;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.LPU
{
    public class AlphaVisionController : BaseController
    {
        private AlphaVisionContext _context;

        public const string baseUrlAPI = "https://alph-r01-s-ap04/api/";
        public const string adminLogin = "admin@alpharm.ru";
        public const string adminPassword = "rLmRUjrUpw7nl46hwTmw";

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new AlphaVisionContext(APP);
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
        }

        ~AlphaVisionController()
        {
            _context.Dispose();
        }

        [HttpGet]
        [Authorize(Roles = "AV_UserAdmin")]
        public async Task<ActionResult> Users()
        {
            try
            {
                    var userList = await (from user in _context.AspNetUsers
                                      
                                     select new
                                     {
                                         user.Id,
                                         user.SupplierId,
                                         SupplierName = user.Supplier.Name,
                                         user.UserName,
                                         user.Email,
                                         user.Post,
                                         PostName = user.Post.Name,
                                         RoleNames = (from userRole in user.UserRoles //[AspNetUserRoles]
                                                      join role in _context.AspNetRoles //[AspNetRoles]
                                                        on userRole.RoleId equals role.Id
                                                      select role.Name).ToList()
                                     }).ToListAsync();

                return ReturnData(userList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "AV_UserAdmin")]
        public async Task<ActionResult> CreateUser(RegisterUserModel user)
        {
            try
            {
                var authResponse = await Auth(adminLogin, adminPassword);
                if (authResponse != null && authResponse.status == "Success")
                {
                    var tokenData = (TokenData)JsonSerializer.Deserialize<TokenData>(authResponse.data);

                    return null;// ReturnData(users);
                }
                else
                {
                    return BadRequest("Не удалось прочитать данные для аутентификации");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<AuthenticationResponse> Auth(string login, string password)
        {
            
            var requestObject = new AuthenticationRequest() { Login = login, Password = password };
            string requestBodyString = JsonSerializer.Serialize(requestObject);
            var request = new HttpRequestMessage
            {
                Content = new StringContent(requestBodyString, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrlAPI + "auth")
            };
            //request.Content.Headers.Add("Content-Type", "application/json");
            //request.Properties.Add("login", login);
            //request.Properties.Add("password", password);

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            AuthenticationResponse authResponse = null;

            try { 
                var responseStream = await response.Content.ReadAsStreamAsync();
                authResponse = await JsonSerializer.DeserializeAsync<AuthenticationResponse>(responseStream);
                return authResponse;
            }
            catch
            {
                return authResponse;
            }

        }
    }
}