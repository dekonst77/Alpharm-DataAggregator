using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.LPU.Alphavision;
using DataAggregator.Domain.Model.LPU.Alphavision.User;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.LPU
{
    public class AlphaVisionController : BaseController
    {
        private AlphaVisionContext _context;

        private static readonly HttpClient httpClient;
        public const string baseUrlAPI = "https://alph-r01-s-ap04/api/";
        public const string adminLogin = "admin@alpharm.ru";
        public const string adminPassword = "rLmRUjrUpw7nl46hwTmw";

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new AlphaVisionContext(APP);
        }

        ~AlphaVisionController()
        {
            _context.Dispose();
        }

        [HttpGet]
        [Authorize(Roles = "AV_UserAdmin")]
        public ActionResult Users()
        {
            try
            {
                var users = _context.AspNetUsers.ToList();
                return ReturnData(users);
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
                if (authResponse != null)
                {
                    var users = _context.AspNetUsers.ToList();
                    return ReturnData(users);
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
            var request = new HttpRequestMessage(HttpMethod.Post, baseUrlAPI + "auth");
            request.Headers.Add("Content-Type", "application/json");
            request.Properties.Add("login", login);
            request.Properties.Add("password", password);

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