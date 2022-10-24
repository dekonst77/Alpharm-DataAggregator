using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Security.Claims;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using Microsoft.Owin.Security;

using Newtonsoft.Json;

using DataAggregator.Web.App_Start;
using DataAggregator.Web.Models;
using DataAggregator.Web.Models.Common;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.GovernmentPurchases;
using System.Data;

namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    [Authorize(Roles = "GManager")]
    public class CheckContractController : BaseController
    {
        private GovernmentPurchasesContext _context;
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GovernmentPurchasesContext(APP);
        }

        ~CheckContractController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult Load_Contract_check_view()
        {
            //_context.Set_CONTEXT_INFO(User.Identity.Name);
            _context.Database.CommandTimeout = 0;
            var data = _context.Contract_check_view.ToList();



            var result = new Dictionary<string, object>
            {
                { "Data", data },
                { "Status", "ok" }
            };

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };
            return jsonNetResult;
        }
        [HttpPost]
        public ActionResult Contract_check_Set_check(System.Collections.Generic.List<string> model,string type, bool isSet)
        {
            try
            {
                _context.Database.CommandTimeout = 0;
                //_context.Set_CONTEXT_INFO(User.Identity.Name);
                DataTable ReestrNumberIDs = new DataTable();
                ReestrNumberIDs.Columns.Add(new DataColumn("Id", typeof(string)));
                foreach (string ReestrNumber in model)
                {
                    DataRow dr = ReestrNumberIDs.NewRow();
                    dr["Id"] = ReestrNumber;
                    ReestrNumberIDs.Rows.Add(dr);
                }
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 3600 * 3;

                    command.Connection = (SqlConnection)_context.Database.Connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@ReestrNumberList", ReestrNumberIDs);
                    command.Parameters.AddWithValue("@isSet", isSet);
                    command.Parameters.AddWithValue("@user", User.Identity.Name);

                    command.CommandText = "dbo.[" + type + "]";
                    _context.Database.Connection.Open();
                    command.ExecuteNonQuery();
                }
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = "ок"
                };

                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Load_ContractPaymentStage()
        {
            //_context.Set_CONTEXT_INFO(User.Identity.Name);
            _context.Database.CommandTimeout = 0;
            var data = _context.Contract_check_ContractPaymentStage_view.ToList();
            var Funding = _context.Funding.Select(s => new sprItem() { Id = s.Id, Name = s.Name }).ToList();
            var Nature = _context.Nature.Select(s=>new sprItem() { Id=s.Id,Name=s.Name}).ToList();

           // var data1 = data.Where(w => w.PurchaseNumber == "0119200000119012238").ToList();

            var result = new Dictionary<string, object>
            {
                { "Data", data },
                { "Funding", Funding },
                { "Nature", Nature },
                { "Status", "ok" }
            };

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };
            return jsonNetResult;
        }
        [HttpPost]
        public ActionResult Contract_check_ContractPaymentStage_KBK_Set(System.Collections.Generic.List<long> model, bool isSet)
        {
            try
            {
                //_context.Set_CONTEXT_INFO(User.Identity.Name);
                _context.Database.CommandTimeout = 0;
                foreach (long ContractId in model)
                {
                    SqlParameter param_ContractId = new SqlParameter("@ContractId", ContractId);
                    SqlParameter param_isSet = new SqlParameter("@isSet", isSet);
                    SqlParameter param_user = new SqlParameter("@user", User.Identity.Name);
                    _context.Database.ExecuteSqlCommand("dbo.[Contract_check_ContractPaymentStage_KBK_Set] @ContractId, @isSet, @user", param_ContractId, param_isSet, param_user);
                }

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = "ок"
                };

                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult SetNature(System.Collections.Generic.List<long> model, Byte NatureId)
        {
            try
            {
                var nature = _context.Nature.Where(w => w.Id == NatureId).Single();
                foreach (long PurchaseId in model)
                {
                    var p = _context.Purchase.Where(w => w.Id == PurchaseId).Single();
                    p.NatureId = nature.Id;
                    p.CategoryId = nature.CategoryId;
                 }
                _context.SaveChanges();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = "ок"
                };
                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult SetFunding(System.Collections.Generic.List<long> model, Byte FundingId)
        {
            try
            {
                var funding = _context.Funding.Where(w => w.Id == FundingId).Single();
                foreach (long LotId in model)
                {
                    var lf = _context.LotFunding.Where(w => w.LotId == LotId);
                    _context.LotFunding.RemoveRange(lf);
                    _context.LotFunding.Add(new LotFunding() { LotId = LotId, FundingId = funding.Id });
                }
                _context.SaveChanges();
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = "ок"
                };
                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class sprItem
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
    }
}