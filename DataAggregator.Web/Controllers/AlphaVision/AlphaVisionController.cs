
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Alphavision;
using DataAggregator.Domain.Model.Alphavision.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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

                var userList = await GetUsersList();
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
                var tokendata = await AuthRequest(adminLogin, adminPassword);
                var data = await CreateUserRequest(user, tokendata.access_token);

                var createduser = await GetUsersList(user.Email);

                return ReturnData(createduser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<dynamic> GetUsersList(string email = "") {
            var userList = await(from user in _context.AspNetUsers
                                 where email == "" || user.Email == email
                                 select new
                                 {
                                     user.Id,
                                     user.SupplierId,
                                     SupplierName = user.Supplier.Name,
                                     user.UserName,
                                     user.Email,
                                     user.Name,
                                     user.Surname,
                                     user.Patronymic,
                                     user.Post,
                                     PostName = user.Post.Name,
                                     user.ApiEnabled,
                                     user.CreatedDate,
                                     RoleNames = (from userRole in user.UserRoles //[AspNetUserRoles]
                                                  join role in _context.AspNetRoles //[AspNetRoles]
                                                    on userRole.RoleId equals role.Id
                                                  select role.Name).ToList()
                                 }).ToListAsync();
            return userList;
        }

        private async Task<TokenData> AuthRequest(string login, string password)
        {
            var authResponse = await CreateRequest("auth", new AuthenticationRequest() { Login = login, Password = password });

            if (authResponse != null && authResponse.status == ResponseStatus.Success)
            {
                return (TokenData)JsonSerializer.Deserialize<TokenData>(authResponse.data);
            }
            else
            {
                if (authResponse != null)
                    throw new Exception(authResponse.message);
                else
                    throw new Exception("Не удалось прочитать данные для аутентификации");
            }
        }

        private async Task<AuthenticationResponse> CreateUserRequest(RegisterUserModel user, string token)
        {
            var authResponse = await CreateRequest("user/create", user, token);

            if (authResponse != null && authResponse.status == ResponseStatus.Success)
            {
                return authResponse;
            }
            else
            {
                if (authResponse != null)
                    throw new Exception(authResponse.message);
                else
                    throw new Exception("Не удалось создать пользователя");
            }
        }

        private async Task<AuthenticationResponse> CreateRequest<T>(string endpoint, T obj, string token = "")
        {
            System.Reflection.PropertyInfo[] Props = typeof(T).GetProperties();
        
            string requestBodyString = JsonSerializer.Serialize(obj);
            var request = new HttpRequestMessage
            {
                Content = new StringContent(requestBodyString, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrlAPI + endpoint)
            };
            if (endpoint != "auth")
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            AuthenticationResponse authResponse = null;

            try
            {
                var client = new HttpClient();

                var response = await client.SendAsync(request);
                //response.EnsureSuccessStatusCode();
           
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