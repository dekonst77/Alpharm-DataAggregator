using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.View;
using DataAggregator.Domain.Model.GovernmentPurchases;
using DataAggregator.Domain.Model.GovernmentPurchases.View;
using DataAggregator.Web.Models.GovernmentPurchases.Fulfilment;
using DataAggregator.Web.Models.GovernmentPurchases.Suppliers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier;
using System.Data.SqlClient;
using System.Data;

namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    [Authorize(Roles = "GManager ")]
    public class FulfilmentController : BaseController
    {
        private GovernmentPurchasesContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GovernmentPurchasesContext(APP);
        }

        ~FulfilmentController()
        {
            _context.Dispose();
        }
        /// <summary>
        /// считать с БД и сформировать данные в формате вывода на форму для json
        /// </summary>
        /// <param name="ReestrNumber"></param>
        /// <returns></returns>
        private IQueryable<FulfilmentJson> GetFulfilmentList(string ReestrNumber)
        {
            var query = from v in _context.ContractAndStageObjectGrls_View
                        join p in _context.ContractAndFulfilmentObjectReplace.Where(cc => cc.Status == 1) on v.contractQuantityId equals p.contractQuantityId into gj
                        from x in gj.DefaultIfEmpty()
                        join u in _context.User on x.UserGuid equals u.Id into uj
                        from us in uj.DefaultIfEmpty()
                        select new FulfilmentJson()
                        {
                            Type = (v.Ind == 0) ? "Контракт" : "Исполнение",
                            Id = v.Id,
                            Number = v.Number,
                            ContractId = v.ContractId,
                            ReestrNumber = v.ReestrNumber,
                            ContractSum = v.ContractSum,
                            ActuallyPaid = v.ActuallyPaid,
                            SumIsp = v.SumIsp,
                            ObjectId = v.ObjectId,
                            Name = v.Name,
                            Unit = v.Unit,
                            Amount = v.Amount,
                            Price = v.Price,
                            Sum = v.Sum,
                            ClassifierId = x == null ? v.ClassifierId : (x.ClassifierId == null ? v.ClassifierId : x.ClassifierId),
                            INNGroup = v.INNGroup,
                            TradeName = v.TradeName,
                            DrugDescription = v.DrugDescription,
                            Corporation = v.Corporation,
                            Packer = v.Packer,
                            ObjectCalculatedAmount = x == null ? v.ObjectCalculatedAmount : (x.ObjectCalculatedAmount == null ? v.ObjectCalculatedAmount : x.ObjectCalculatedAmount),
                            ObjectCalculatedPrice = x == null ? v.ObjectCalculatedPrice : (x.ObjectCalculatedPrice == null ? v.ObjectCalculatedPrice : x.ObjectCalculatedPrice),
                            ObjectCalculatedSum = v.ObjectCalculatedSum,
                            Seria = v.Seria,
                            INNGroupIsp = v.INNGroupIsp,
                            TradeNameIsp = v.TradeNameIsp,
                            DrugDescriptionIsp = v.DrugDescriptionIsp,
                            ProvisorAction = v.ProvisorAction,
                            contractQuantityId = v.contractQuantityId,

                            uClassifierId = x == null ? null : x.ClassifierId,
                            uObjectCalculatedAmount = x == null ? null : x.ObjectCalculatedAmount,
                            uObjectCalculatedPrice = x == null ? null : x.ObjectCalculatedPrice,
                            uUserGuid = x == null ? String.Empty : x.UserGuid,
                            uEditDate = x == null ? (DateTime?)null : (DateTime?)x.EditDate,
                            uStatus = x == null ? (int?)null : (int?)x.Status,
                            UserName = x == null ? String.Empty : us.FullNameWithoutPatronymic,

                            oClassifierId = v.ClassifierId,
                            oObjectCalculatedAmount = v.ObjectCalculatedAmount,
                            oObjectCalculatedPrice = v.ObjectCalculatedPrice
                        };

            return query
                .Where(cc => (cc.ReestrNumber == ReestrNumber));
        }

        /// <summary>
        /// Загрузить данные для привязки
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadFulfilmentList(string ReestrNumber)
        {
            JsonNetResult jsonNetResult;

            if (String.IsNullOrWhiteSpace(ReestrNumber))
                return new JsonNetResult();

            var result = GetFulfilmentList(ReestrNumber).ToList();

            jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }
        /// <summary>
        /// сохранение измененных данных
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(ICollection<FulfilmentJson> array)
        {
            try
            {
                List<ContractAndFulfilmentObjectReplace> records = new List<ContractAndFulfilmentObjectReplace>();
                List<long> lId = new List<long>();

                foreach (var item in array)
                {
                    ContractAndFulfilmentObjectReplace updRec;
                    int flg = 0;
                    try
                    {
                        updRec = _context.ContractAndFulfilmentObjectReplace.Where(cc => cc.contractQuantityId == item.contractQuantityId && cc.Status == 1).First();
                    } catch (Exception ex) {
                        updRec = new ContractAndFulfilmentObjectReplace() { contractQuantityId = 0 };
                        _context.Entry<ContractAndFulfilmentObjectReplace>(updRec).State = System.Data.Entity.EntityState.Added;
                    }
                    lId.Add(item.contractQuantityId.Value);

                    if (item.ClassifierId.HasValue && item.oClassifierId != item.ClassifierId && updRec.contractQuantityId == 0)
                    {
                        updRec.ClassifierId = item.ClassifierId.Value;
                        flg |= 1;
                    } else if (updRec.contractQuantityId == item.contractQuantityId.Value && item.oClassifierId == item.ClassifierId)
                    {
                        updRec.ClassifierId = null;
                    } else if (updRec.contractQuantityId == item.contractQuantityId.Value)
                    {
                        updRec.ClassifierId = item.ClassifierId.Value;
                        flg |= 1;
                    }

                    if (item.ObjectCalculatedAmount.HasValue && item.oObjectCalculatedAmount != item.ObjectCalculatedAmount && updRec.contractQuantityId == 0)
                    {
                        updRec.ObjectCalculatedAmount = item.ObjectCalculatedAmount.Value;
                        flg |= 2;
                    } else if (updRec.contractQuantityId == item.contractQuantityId.Value && item.oObjectCalculatedAmount == item.ObjectCalculatedAmount)
                    {
                        updRec.ObjectCalculatedAmount = null;
                    }
                    else if (updRec.contractQuantityId == item.contractQuantityId.Value)
                    {
                        updRec.ObjectCalculatedAmount = item.ObjectCalculatedAmount.Value;
                        flg |= 2;
                    }

                    if (item.ObjectCalculatedPrice.HasValue && item.oObjectCalculatedPrice != item.ObjectCalculatedPrice && updRec.contractQuantityId == 0)
                    {
                        updRec.ObjectCalculatedPrice = item.ObjectCalculatedPrice.Value;
                        flg |= 4;
                    }
                    else if (updRec.contractQuantityId == item.contractQuantityId.Value && item.oObjectCalculatedPrice == item.ObjectCalculatedPrice)
                    {
                        updRec.ObjectCalculatedPrice = null;
                    }
                    else if (updRec.contractQuantityId == item.contractQuantityId.Value)
                    {
                        updRec.ObjectCalculatedPrice = item.ObjectCalculatedPrice.Value;
                        flg |= 4;
                    }

                    if (flg != 0)
                    {
                        updRec.contractQuantityId = item.contractQuantityId.Value;
                        updRec.Status = 1;
                        updRec.EditDate = DateTime.Now;
                        updRec.UserGuid = item.uUserGuid;
                        _context.SaveChanges();
                    } else if (updRec.contractQuantityId != 0 && _context.Entry<ContractAndFulfilmentObjectReplace>(updRec).State == System.Data.Entity.EntityState.Modified)
                    {
                        _context.Entry<ContractAndFulfilmentObjectReplace>(updRec).State = System.Data.Entity.EntityState.Deleted;
                        _context.SaveChanges();
                    }
                }

                string ReestrNumber = "";
                if (array.Count > 0) { ReestrNumber = array.First().ReestrNumber; }

                var que = GetFulfilmentList(ReestrNumber);

                var result = que.Where(cc => lId.Contains(cc.contractQuantityId.Value)).ToList();

                ViewData["FulfilmentRecord"] = result;

                var Data = new JsonResultData() { Data = ViewData, status = "ок", Success = true };

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = Data
                };
                return jsonNetResult;
            }
            catch (Exception e)
            {
                string msg = e.Message;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    msg += e.Message;
                }
                return BadRequest(msg);
            }
        }

        [HttpPost]
        public ActionResult CheсkClassifierId(string ClassifierId)
        {
            string ret = "";
            string error = "0";

            try
            {
                long cId = long.Parse(ClassifierId);                
                var query = @"SELECT EV.TradeName FROM [DrugClassifier].[Classifier].[ExternalView_FULL] AS EV WHERE EV.ClassifierId = @ClassifierId";

                var result = _context.Database.SqlQuery<string>(
                        query
                        , new SqlParameter { ParameterName = "@ClassifierId", SqlDbType = SqlDbType.BigInt, Value = cId }
                        ).ToList();
                if ( result == null || result.Count == 0 || result[0] == null) 
                { ret = "Нет "+ ClassifierId +" в классификаторе!"; error = "1"; }
                else 
                { ret = "ТН: "+result[0]; }
            }
            catch (Exception e)
            {
                ret = "Ошибка доступа";
                error = "2";
            }
            ViewData["Ret"] = ret;

            var Data = new JsonResultData() { Data = ViewData, status = error, Success = true };

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = Data
            };
            return jsonNetResult;
        }

    }
}