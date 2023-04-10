using DataAggregator.Core.Models.GovernmentPurchases.GovernmentPurchases;
using DataAggregator.Domain.BulkInsert;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.GovernmentPurchases;
using DataAggregator.Domain.Model.GovernmentPurchases.View;
using DataAggregator.Web.Models.Common;
using DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//using DataAggregator.Web.GovernmentPurchasesExcel;
using DataAggregator.Core.XLS;

namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    [Authorize(Roles = "GManager, GOperator, GOperatorRemote")]
    public class GovernmentPurchasesController : BaseController
    {      

        //protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        //{
        //    base.Initialize(requestContext);
            
        //}


        /// <summary>
        /// Список пользователей
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetUsers()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = _context.User.ToList()
            };

            return jsonNetResult;
        }

        /// <summary>
        /// Фильтр региона закупок
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetRegionFilter()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new RegionFilterJson
                {
                    City = _context.Region.GroupBy(r => r.City).Select(g => g.Key).ToList(),
                    District = _context.Region.GroupBy(r => r.District).Select(g => g.Key).ToList(),
                    FederalDistrict = _context.Region.GroupBy(r => r.FederalDistrict).Select(g => g.Key).ToList(),
                    FederationSubject = _context.Region.GroupBy(r => r.FederationSubject).Select(g => g.Key).ToList()
                }
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult DeletePurchase(long id)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var userGuid = new Guid(User.Identity.GetUserId());
            _context.DeletePurchase(id, userGuid);
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = null
            };

            return jsonNetResult;
        }

        /// <summary>
        /// Получить новые Purchase для обработки
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPurchases()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var userGuid = new Guid(User.Identity.GetUserId());

            // у пользователя не должно быть закупок в работе
            var userDrugsCount = _context.PurchaseInWork.Count(p => p.UserId == userGuid);
            if (userDrugsCount != 0)
            {
                throw new ApplicationException("user data not empty");
            }

            _context.GetPurchases(5, userGuid);

            return LoadPurchases();
        }


        /// <summary>
        /// Забрать закупки с открытыми РОП
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "GSupplierResult")]
        public ActionResult GetPurchasesSupplierResult()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var userGuid = new Guid(User.Identity.GetUserId());

            // у пользователя не должно быть закупок в работе
            var userDrugsCount = _context.PurchaseInWork.Count(p => p.UserId == userGuid);
            if (userDrugsCount != 0)
            {
                throw new ApplicationException("user data not empty");
            }

            _context.GetPurchasesSupplierResult(5, userGuid);

            return LoadPurchases();
        }

        /// <summary>
        /// Получить новые Purchase с контрактами для обработки
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPurchasesWithContracts(Byte KK)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var userGuid = new Guid(User.Identity.GetUserId());

            // у пользователя не должно быть закупок в работе
            var userDrugsCount = _context.PurchaseInWork.Count(p => p.UserId == userGuid);
            if (userDrugsCount != 0)
            {
                throw new ApplicationException("user data not empty");
            }

            _context.GetPurchasesWithContracts(5, userGuid, KK);

            return LoadPurchases();
        }

        /// <summary>
        /// Получить отложенные Purchase для обработки
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetIsLaterPurchases()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var userGuid = new Guid(User.Identity.GetUserId());

            // у пользователя не должно быть закупок в работе
            var userDrugsCount = _context.PurchaseInWork.Count(p => p.UserId == userGuid);
            if (userDrugsCount != 0)
            {
                throw new ApplicationException("user data not empty");
            }

            var sql = @"	select top(5) p.Id
	                        from dbo.Purchase as p
	                        inner join dbo.StatusHistory as sh on sh.PurchaseId = p.Id
	                        where sh.StatusId = 100 and sh.IsActual = 1 and (p.AssignedToUserId = '" + userGuid + @"' or p.AssignedToUserId is null) and p.PurchaseClassId = 6
	                        order by p.AssignedToUserId desc, p.HigherPriority desc, p.DateBegin desc";

            _context.GetPurchasesByFilter(sql, userGuid);

            return LoadPurchases();
        }



        /// <summary>
        /// Получить Purchase по фильтру
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPurchasesByFilter(PurchasesFilterJson filter)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var userGuid = new Guid(User.Identity.GetUserId());

            // у пользователя не должно быть закупок в работе
            var userDrugsCount = _context.PurchaseInWork.Count(p => p.UserId == userGuid);
            if (userDrugsCount != 0)
            {
                throw new ApplicationException("user data not empty");
            }

            var sql = filter.GetFilter(5000, userGuid);

            _context.GetPurchasesByFilter(sql, userGuid);
            return LoadPurchases();
        }
        [HttpPost]
        public ActionResult CreateNewPurchase()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var userGuid = new Guid(User.Identity.GetUserId());

            // у пользователя не должно быть закупок в работе
            var userDrugsCount = _context.PurchaseInWork.Count(w => w.UserId == userGuid);
            if (userDrugsCount != 0)
            {
                throw new ApplicationException("user data not empty");
            }
            string Number = Guid.NewGuid().ToString();
            Purchase p = new Purchase()
            {
                Number = Number,
                LawTypeId = 1,
                MethodId = 23,
                SiteName = "-",
                SiteURL = "-",
                Name = "",
                SourceId=0,
                StageId = 1,
                DateBegin = DateTime.Now,
                DateEnd = DateTime.Now,
                URL = "-",
                DateCreate = DateTime.Now,
                HigherPriority = false,
                PurchaseClassId = 1,
                PurchaseClassUserId = userGuid,
                UseContractData = false
            };
            _context.Purchase.Add(p) ;
            _context.SaveChanges();
            Lot L = new Lot() { PurchaseId = p.Id, Number = 1, Sum = 0 };
            _context.Lot.Add(L);
            _context.SaveChanges();
            _context.SupplierResult.Add(new SupplierResult() { LotStatusId = 1, LotId = L.Id });
            _context.SaveChanges();
            _context.Database.ExecuteSqlCommand(@"INSERT INTO [GovernmentPurchases].dbo.StatusHistory
			(PurchaseId, StatusID, IsActual)
			values( "+Convert.ToString(p.Id)+", 100,1)");

            _context.GetPurchasesByFilter("select "+Convert.ToString(p.Id), userGuid);
            return LoadPurchases();
        }

        /// <summary>
        /// Вернуть Purchase
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SetPurchases()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var userGuid = new Guid(User.Identity.GetUserId());

            _context.SetPurchases(userGuid);

            return Json(true);
        }

        /// <summary>
        /// Загрузить обрабатываемые Purchases (например после забора данных пользователем или при открытии приложения, если в работе уже есть данные)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadPurchases()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<PurchaseLightJson>();

            var userGuid = new Guid(User.Identity.GetUserId());

            var purchasesIdList = _context.PurchaseInWork.Where(p => p.UserId == userGuid).Select(p => p.Id);
            var purchases = _context.Purchase.Where(p => purchasesIdList.Contains(p.Id)).ToList();

            foreach (var purchase in purchases)
            {
                result.Add(new PurchaseLightJson(purchase));
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.ToList()
            };

            return jsonNetResult;
        }

        /// <summary>
        /// Загрузить конкретную закупку (для обновления данных)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadPurchase(long id)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var purchase = _context.Purchase.Single(p => p.Id == id);

            var result = new PurchaseJson(_context, purchase);

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }
        [HttpPost]
        public ActionResult LoadPlanG(long purchaseId)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var planG = _context.PlanG_View.Where(p => p.PurchaseId == purchaseId);


            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = planG.ToList()
            };

            return jsonNetResult;
        }
        /// <summary>
        /// Загрузить лоты закупки
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadLots(long purchaseId)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<LotJson>();
            var lots = _context.Lot.Where(l => l.PurchaseId == purchaseId).ToList();

            foreach (var lot in lots)
            {
                result.Add(new LotJson(_context, lot));
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        /// <summary>
        /// Загрузить конкретный лот (для обновления данных)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadLot(long id)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var lot = _context.Lot.Single(l => l.Id == id);

            var result = new LotJson(_context, lot);

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }

        /// <summary>
        /// Загрузить контракты лота
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadContracts(long lotId, string ReestrNumber)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<ContractJson>();

            var contracts = _context.Contract.Where(c => (lotId > 0 && c.LotId == lotId) || (ReestrNumber != "" && c.ReestrNumber == ReestrNumber)).ToList();

            foreach (var contract in contracts)
            {
                result.Add(new ContractJson(_context, contract));
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        /// <summary>
        /// Загрузить результат определения поставщика
        /// </summary>
        /// <param name="lotId">Идентификатор лота</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "GSupplierResult")]
        public ActionResult LoadSupplierResult(long lotId)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var supplierResult = _context.SupplierResult.SingleOrDefault(sr => sr.LotId == lotId);


            User user = null;

            if (supplierResult.LastChangedUserId != null)
                user = _context.User.Single(u => u.Id == supplierResult.LastChangedUserId.ToString());

            SupplierResultModel result = new SupplierResultModel(supplierResult, user);


            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;



        }

        /// <summary>
        /// Сохранить изменния результата определения поставщика
        /// </summary>
        /// <param name="supplierResult"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "GSupplierResult")]
        public ActionResult SaveSupplierResult(SupplierResultModel supplierResult)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var supr = _context.SupplierResult.Single(sr => sr.Id == supplierResult.Id);

            supr.LastChangedUserId = new Guid(User.Identity.GetUserId());
            supr.LastChangedDate = DateTime.Now;
            supr.ProtocolDate = supplierResult.ProtocolDate;
            if (supr.ProtocolDate.HasValue)
            {
                supr.ProtocolDate = supr.ProtocolDate.Value.ToUniversalTime().Date;
            }
            supr.ProtocolNumber = supplierResult.ProtocolNumber;
            supr.ForCheck = supplierResult.ForCheck;


            var purchase = supr.Lot.Purchase;
            if (supplierResult.Stage != null)
                purchase.StageId = supplierResult.Stage.Id;

            if (supplierResult.LotStatus.Id != null)
                supr.LotStatusId = supplierResult.LotStatus.Id.Value;


            foreach (var supplier in supplierResult.SupplierList)
            {
                if (!supplier.Number.HasValue &&
                    supplier.Supplier == null &&
                    !supplier.Sum.HasValue)
                {
                    //Удален

                    if (supplier.Id > 0)
                    {
                        var s = supr.SupplierList.First(sl => sl.Id == supplier.Id);
                        _context.SupplierList.Remove(s);
                    }
                }
                else
                {



                    if (supplier.Id > 0)
                    {
                        var s = supr.SupplierList.First(sl => sl.Id == supplier.Id);
                        s.Number = supplier.Number;
                        s.Sum = supplier.Sum;

                        if (s.SupplierRaw != null && s.SupplierRaw.SupplierId != supplier.Supplier.Id ||
                            s.SupplierRaw == null)
                        {
                            if (supplier.Supplier != null && supplier.Supplier.Id > 0)
                            {
                                s.SupplierId = supplier.Supplier.Id;
                            }
                            else
                            {
                                s.SupplierId = null;
                            }
                        }
                    }
                    else
                    {
                        //новый
                        var s = new SupplierList
                        {
                            Number = supplier.Number,
                            Sum = supplier.Sum
                        };

                        if (supplier.Supplier != null)
                        {
                            s.SupplierId = supplier.Supplier.Id;
                        }
                        else
                        {
                            s.SupplierId = null;
                        }

                        s.SupplierResult = supr;

                        _context.SupplierList.Add(s);

                    }
                }
            }

            _context.SaveChanges();


            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = true
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult ChangeLotStatus(long supplierResultId, long lotStatusId)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            Stage result = null;

            SupplierResult sr = _context.SupplierResult.Single(s => s.Id == supplierResultId);


            //Определяем количество лотов

            var lots = sr.Lot.Purchase.Lot;

            if (lots.Count == 1)
            {
                //Однолотовая

                //Статус лота ставится "Завершен"/"Завершен 1 участник", то "Закупка завершена"
                if (lotStatusId == 2 || lotStatusId == 3)
                {
                    result = _context.Stage.Single(s => s.Id == 3);
                }

                //Статус лота ставится "Не состоялся", то "Определение поставщика завершено"
                if (lotStatusId == 4)
                {
                    result = _context.Stage.Single(s => s.Id == 8);
                }

                //Статус лота ставится "Отменен", то "Определение поставщика отменено"
                if (lotStatusId == 5)
                {
                    result = _context.Stage.Single(s => s.Id == 9);
                }
            }
            else
            {
                //Многолотовая

                //Если у закупки нету РОП со статусом Отменен и Статус лота ставится "Завершен"/"Завершен 1 участник"
                if (!lots.Any(l => l.SupplierResult.Any(s => s.LotStatusId == 5)) && (lotStatusId == 2 || lotStatusId == 3))
                {
                    //То    "Определение поставщика завершено"
                    result = _context.Stage.Single(s => s.Id == 8);
                }
            }

            long? id = result != null ? result.Id : (long?)null;

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = id
            };

            return jsonNetResult;
        }

        /// <summary>
        /// Загрузить конкретный контракт (для обновления данных)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadContract(long id)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var contract = _context.Contract.Single(c => c.Id == id);
            var contractPaymentStage = _context.ContractPaymentStage.Where(w=>w.ContractId== id);
            var contractPaymentStage_new = _context.Contract_check_ContractPaymentStage.Where(w => w.ContractId == id);
            var paymentType = _context.PaymentType.ToList();

            var result = new ContractJson(_context, contract);
            ViewBag.Contract = result;
            ViewBag.ContractPaymentStage = contractPaymentStage;
            ViewBag.ContractPaymentStage_new = contractPaymentStage_new;
            ViewBag.PaymentType = paymentType;
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ViewBag
            };

            return jsonNetResult;
        }

        /// <summary>
        /// Загрузить объекты закупки
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadObjects(long lotId)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<ObjectJson>();

            var purchaseObjects = _context.PurchaseObject.Where(o => o.LotId == lotId).ToList();

            foreach (var purchaseObject in purchaseObjects)
            {
                result.Add(new ObjectJson(purchaseObject));
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        /// <summary>
        /// Загрузить объекты контракта
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadContractObjects(long contractId)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<ContractObjectJson>();

            var contractObjects = _context.ContractObject.Where(o => o.ContractId == contractId).ToList();

            foreach (var contractObject in contractObjects)
            {
                result.Add(new ContractObjectJson(contractObject));
            }

            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }

        [HttpPost]
        public ActionResult GetSupplierByFilter(SupplierFilterModel filter)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<SupplierModel>();

            if (filter.NotSet())
            {
                return Json(null);
            }

            var suppliers = (IQueryable<Supplier>)_context.Supplier;

            if (!string.IsNullOrEmpty(filter.Name))
            {
                suppliers = suppliers.Where(o => o.Name.Contains(filter.Name));
            }

            if (!string.IsNullOrEmpty(filter.INN))
            {
                suppliers = suppliers.Where(o => o.INN.Contains(filter.INN));
            }


            if (!string.IsNullOrEmpty(filter.KPP))
            {
                suppliers = suppliers.Where(o => o.Name.Contains(filter.KPP));
            }

            if (!string.IsNullOrEmpty(filter.LocationAddress))
            {
                suppliers = suppliers.Where(o => o.LocationAddress.Contains(filter.LocationAddress));
            }

            if (filter.Id.HasValue)
            {
                suppliers = suppliers.Where(o => o.Id == filter.Id.Value);
            }

            suppliers = suppliers.Take(5000);

            if (suppliers.Any())
            {
                result = suppliers.ToList().Select(o => new SupplierModel(o)).ToList();
            }


            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }


        /// <summary>
        /// Поиск организации по фильтру
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetOrganizationByFilter(OrganizationFilterJson filter)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<OrganizationJson>();

            if (filter.NotSet())
            {
                return Json(null);
            }

            var organizations = (IQueryable<Organization>)_context.Organization.Where(w=>w.ActualId==null);

            if (!string.IsNullOrEmpty(filter.INN))
            {
                organizations = organizations.Where(o => o.INN.StartsWith(filter.INN));
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                organizations = organizations.Where(o => o.FullName.Contains(filter.Name));
            }


            if (filter.Is_Customer)
            {
                organizations = organizations.Where(o => o.Is_Customer==true);
            }


            if (filter.Is_Recipient)
            {
                organizations = organizations.Where(o => o.Is_Recipient==true);
            }

            if (!string.IsNullOrEmpty(filter.Address))
            {
                organizations = organizations.Where(o => o.LocationAddress.Contains(filter.Address));
            }

            organizations = organizations.Take(5000);

            if (organizations.Any())
            {
                result = organizations.ToList().Select(o => new OrganizationJson(_context,o)).ToList();
            }


            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult LoadObjectsReady(long lotId)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<PurchaseObjectReadyJson>();

            var purchaseObjects = _context.PurchaseObjectReady.Where(o => o.LotId == lotId).ToList();

            foreach (var purchaseObject in purchaseObjects)
            {
                result.Add(new PurchaseObjectReadyJson(purchaseObject));
            }

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };
        }

        [HttpPost]
        public ActionResult LoadContractObjectsReady(long contractId)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<ContractObjectReadyJson>();

            var contractObjects = _context.ContractObjectReady.Where(o => o.ContractId == contractId).ToList();

            var contractObjects_History = _context.ContractObjectReady_History.Where(o => o.ContractId == contractId).OrderByDescending(o=>o.DT).OrderBy(o2=>o2.Name).ToList();
            var contractstageObjects = _context.contract_stage_Objects_View.Where(o => o.ContractId == contractId).OrderBy(o2 => o2.contractQuantityId).ToList();
            foreach (var contractObject in contractObjects)
            {
                result.Add(new ContractObjectReadyJson(contractObject));
            }
            ViewData["contractObjects"] = result;
            ViewData["contractObjects_History"] = contractObjects_History;
            ViewData["contractstageObjects"] = contractstageObjects;
            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ViewData
            };
        }

        class DecimalConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(decimal) || objectType == typeof(decimal?));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JToken token = JToken.Load(reader);
                if (token.Type == JTokenType.Float || token.Type == JTokenType.Integer)
                {
                    return token.ToObject<decimal>();
                }
                if (token.Type == JTokenType.String)
                {
                    // customize this to suit your needs
                    return Decimal.Parse(token.ToString(),
                           System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
                }
                if (token.Type == JTokenType.Null && objectType == typeof(decimal?))
                {
                    return null;
                }
                throw new JsonSerializationException("Unexpected token type: " +
                                                      token.Type.ToString());
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Сохранить информацию о закупке (не сохраняя информацию о вложенных лотах и объектах)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SavePurchaseInfo(PurchaseJson purchaseJson)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            //PurchaseJson purchaseJson = JsonConvert.DeserializeObject<PurchaseJson>(purchaseJsonString, new DecimalConverter());

            if (purchaseJson == null)
                throw new ApplicationException("purchaseJson required argument");

            var purchase = _context.Purchase.FirstOrDefault(p => p.Id == purchaseJson.Id);

            if (purchase == null)
                throw new ApplicationException("Purchase not found");
            //Для варианта когда закупка создаётся человеком
            purchase.Number = purchaseJson.Number;
            purchase.SiteName = purchaseJson.SiteName;
            purchase.SiteURL = purchaseJson.SiteURL;
            purchase.URL = purchaseJson.Url;
            purchase.SourceId = (Byte)purchaseJson.Source.Id;
            purchase.LawTypeId = (Byte)purchaseJson.LawType.Id;
            purchase.MethodId = (Byte)purchaseJson.Method.Id;


            purchase.ConclusionReason = purchaseJson.ConclusionReason;

            purchase.CustomerId = purchaseJson.Customer.Id;

            purchase.StageId = purchaseJson.Stage.Id;
            purchase.CategoryId = purchaseJson.Category.Id;
            purchase.NatureId = purchaseJson.Nature.Id;
            purchase.Nature_L2Id = purchaseJson.Nature_L2.Id;

            purchase.Name = purchaseJson.Name;
            purchase.DateBegin = purchaseJson.DateBegin;
            purchase.DateEndFirstParts = purchaseJson.DateEndFirstParts;
            purchase.DateEnd = purchaseJson.DateEnd;

            purchase.Comment = purchaseJson.Comment;

            if (purchaseJson.PurchaseClass.Id != null)
            {
                purchase.PurchaseClassId = (Byte)purchaseJson.PurchaseClass.Id;
            }

            if (purchaseJson.Payment != null)
            {
                foreach (var paymentJson in purchaseJson.Payment)
                {
                    var payment = purchase.Payment.FirstOrDefault(p => p.Id == paymentJson.Id);

                    if (payment == null)
                        throw new ApplicationException("Нарушение целостности данных в Payment");

                    payment.KBK = paymentJson.KBK;
                    payment.KOSGU = paymentJson.KOSGU;
                    payment.PaymentTypeId = paymentJson.PaymentType.Id;
                    payment.Sum = paymentJson.Sum;
                    payment.PaymentYearId = paymentJson.PaymentYear.Id;
                }
            }

            if (purchaseJson.DeliveryTimeInfo != null)
            {
                foreach (var deliveryInfoJson in purchaseJson.DeliveryTimeInfo)
                {
                    if (!deliveryInfoJson.DeliveryTimePeriod.Id.HasValue)
                        throw new ApplicationException("Нарушение целостности данных в DeliveryTimeInfo");

                    DeliveryTimeInfo deliverInfo = null;

                    if (deliveryInfoJson.Id.HasValue)
                    {
                        deliverInfo =
                            purchase.DeliveryTimeInfo.FirstOrDefault(
                                d => d.Id == deliveryInfoJson.Id && d.PurchaseId == purchaseJson.Id);
                        if (deliverInfo == null)
                            throw new ApplicationException("Нарушение целостности данных в DeliveryTimeInfo");
                    }
                    else
                    {
                        deliverInfo = new DeliveryTimeInfo
                        {
                            PurchaseId = purchaseJson.Id
                        };
                        _context.DeliveryTimeInfo.Add(deliverInfo);
                    }

                    deliverInfo.Count = deliveryInfoJson.Count;
                    deliverInfo.DateStart = deliveryInfoJson.DateStart.ToUniversalTime();
                    deliverInfo.DateEnd = deliveryInfoJson.DateEnd.ToUniversalTime();
                    deliverInfo.DeliveryTimePeriodId = deliveryInfoJson.DeliveryTimePeriod.Id.Value;
                    deliverInfo.Count = deliveryInfoJson.Count;
                }

                var deliveryTimeInfoForDeleteId =
                    purchase.DeliveryTimeInfo.Select(d => d.Id)
                        .Except(purchaseJson.DeliveryTimeInfo.Where(d => d.Id.HasValue).Select(d => d.Id.Value))
                        .ToList();

                var deliveryTimeInfoForDelete = _context.DeliveryTimeInfo.Where(d => deliveryTimeInfoForDeleteId.Contains(d.Id));

                _context.DeliveryTimeInfo.RemoveRange(deliveryTimeInfoForDelete);
            }
            else
            {
                var deliveryTimeInfoForDelete = _context.DeliveryTimeInfo.Where(d => d.PurchaseId == purchase.Id);
                _context.DeliveryTimeInfo.RemoveRange(deliveryTimeInfoForDelete);
            }

            if (purchaseJson.PurchaseNatureMixed != null)
            {
                var listMixed = purchaseJson.PurchaseNatureMixed.GetPurchaseNatureMixed();

                //Удаляем старые
                _context.PurchaseNatureMixed.RemoveRange(purchase.PurchaseNatureMixed.ToList());

                //Добавляем новые
                listMixed.ForEach(l =>
                {
                    l.Purchase = purchase;
                    _context.PurchaseNatureMixed.Add(l);
                });

            }

            var userGuid = new Guid(User.Identity.GetUserId());
            purchase.LastChangedUserId = userGuid;
            purchase.LastChangedDate = DateTime.Now;

            _context.SaveChanges();

            return Json(true);
        }

        /// <summary>
        /// Сохранить информацию о лоте (не сохраняя информацию о вложенных объектах)
        /// </summary>
        /// <param name="lotJson"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveLotInfo(LotJson lotJson)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            if (lotJson == null)
                throw new ApplicationException("lotJson required argument");

            var lot = _context.Lot.FirstOrDefault(l => l.Id == lotJson.Id);

            if (lot == null)
                throw new ApplicationException("lot not found");

            lot.Sum = lotJson.Sum;

            var oldLotFundings = lot.LotFunding.ToList();
            _context.LotFunding.RemoveRange(oldLotFundings);

            foreach (var f in lotJson.FundingList)
            {
                foreach (var c in f.CheckedList)
                {
                    if (c.Checked)
                    {
                        _context.LotFunding.Add(new LotFunding()
                        {
                            FundingId = f.Id,
                            LotId = lot.Id
                        });
                    }
                }
            }

            var userGuid = new Guid(User.Identity.GetUserId());
            lot.LastChangedUserId = userGuid;
            lot.LastChangedDate = DateTime.Now;

            _context.SaveChanges();

            return Json(true);
        }



        [HttpPost]
        public ActionResult AddContractInfo(ContractJson contractJson, long lotId)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            if (contractJson == null)
                throw new ApplicationException("contractJson required argument");

            //проверяем на конфликт для индекса IX_Contract(пара ReestrNumber и LotId)
            if (_context.Contract.Any(c => c.ReestrNumber.Equals(contractJson.ReestrNumber) && c.LotId == lotId))
                return Json(false);

            Contract contract = new Contract
            {
                LotId = lotId,
                ReestrNumber = contractJson.ReestrNumber,
                ContractNumber = contractJson.ContractNumber,
                Url = contractJson.Url,
                ContractStatusId = contractJson.ContractStatus.Id,
                ReceiverId = contractJson.ReceiverId
            };
            if (contractJson.DateBegin != null)
                contract.DateBegin = contractJson.DateBegin.Value.ToUniversalTime().Date;
            if (contractJson.DateEnd != null)
                contract.DateEnd = contractJson.DateEnd.Value.ToUniversalTime().Date;
            if (contractJson.ConclusionDate != null)
                contract.ConclusionDate = contractJson.ConclusionDate.Value.ToUniversalTime().Date;
            contract.Sum = contractJson.Sum;
            contract.ActuallyPaid = contractJson.ActuallyPaid;
            contract.CurrencyId = 4;
            if (contractJson.SupplierRaw != null)
                contract.SupplierRawId = contractJson.SupplierRaw.Id;

            if (contractJson.SupplierRaw == null || contractJson.Supplier.Id != contractJson.SupplierRaw.Supplier.Id)
                contract.SupplierId = contractJson.Supplier.Id;

            _context.Contract.Add(contract);

            var lot = _context.Lot.Single(l => l.Id == lotId);

            var obj = _context.PurchaseObject.Where(o => o.LotId == lot.Id).ToList();


            foreach (var purchasesObject in obj)
            {
                ContractObject o = new ContractObject
                {
                    Amount = purchasesObject.Amount,
                    Contract = contract,
                    Name = purchasesObject.Name,
                    OKPD = purchasesObject.OKPD,
                    Price = purchasesObject.Price,
                    Sum = purchasesObject.Sum,
                    Unit = purchasesObject.Unit
                };

                _context.ContractObject.Add(o);
            }

            var objReady = _context.PurchaseObjectReady.Where(o => o.LotId == lot.Id).ToList();

            foreach (var purchasesObjectReady in objReady)
            {
                ContractObjectReady o = new ContractObjectReady
                {
                    Amount = purchasesObjectReady.Amount,
                    Contract = contract,
                    Name = purchasesObjectReady.Name,
                    OKPD = purchasesObjectReady.OKPD,
                    Price = purchasesObjectReady.Price,
                    Sum = purchasesObjectReady.Sum,
                    Unit = purchasesObjectReady.Unit
                };

                _context.ContractObjectReady.Add(o);
            }

            _context.SaveChanges();

            return Json(true);

        }

        /// <summary>
        /// Сохранить информацию о контракте (не сохраняя информацию о вложенных объектах)
        /// </summary>
        /// <param name="contractJson"></param>
        /// <param name="useContractData"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveContractInfo(ContractJson contractJson, bool useContractData)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            if (contractJson == null)
                throw new ApplicationException("contractJson required argument");

            var contract = _context.Contract.FirstOrDefault(c => c.Id == contractJson.Id);

            if (contract == null)
                throw new ApplicationException("contract not found");

            var purchase = contract.Lot.Purchase;

            contract.ReceiverId = contractJson.ReceiverId;
            contract.Sum = contractJson.Sum;
            contract.ActuallyPaid = contractJson.ActuallyPaid;
            contract.ContractStatusId = contractJson.ContractStatus.Id;
            contract.ReestrNumber = contractJson.ReestrNumber.Trim();
            contract.KK = contractJson.KK;

            contract.ConclusionDate = contractJson.ConclusionDate;
            contract.DateBegin = contractJson.DateBegin;
            contract.DateEnd = contractJson.DateEnd;

            purchase.UseContractData = useContractData;

            var userGuid = new Guid(User.Identity.GetUserId());
            contract.LastChangedUserId = userGuid;
            contract.LastChangedDate = DateTime.Now;

            _context.SaveChanges();

            return Json(true);
        }

        /// <summary>
        /// Список всех статусов лота для РОП
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetLotStatusList()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<DictionaryElementJson>();

            var statusList = _context.LotStatus.ToList();

            foreach (var status in statusList)
            {
                result.Add(new DictionaryElementJson() { Id = status.Id, Name = status.Name });
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.ToList()
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult GetStageList()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<DictionaryElementJson>();

            var stageList = _context.Stage.ToList();

            foreach (var stage in stageList)
            {
                result.Add(new DictionaryElementJson() { Id = stage.Id, Name = stage.Name });
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.ToList()
            };

            return jsonNetResult;
        }
        [HttpPost]
        public ActionResult GetSourceList()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<DictionaryElementJson>();

            var itemList = _context.Source.ToList();

            foreach (var item in itemList)
            {
                result.Add(new DictionaryElementJson() { Id = item.Id, Name = item.Name });
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.ToList()
            };
            return jsonNetResult;
        }
        [HttpPost]
        public ActionResult GetMethodList()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<DictionaryElementJson>();

            var itemList = _context.Method.ToList();

            foreach (var item in itemList)
            {
                result.Add(new DictionaryElementJson() { Id = item.Id, Name = item.Name });
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.ToList()
            };
            return jsonNetResult;
        }
        [HttpPost]
        public ActionResult GetLawTypeList()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<DictionaryElementJson>();

            var itemList = _context.LawType.ToList();

            foreach (var item in itemList)
            {
                result.Add(new DictionaryElementJson() { Id = item.Id, Name = item.Name });
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.ToList()
            };
            return jsonNetResult;
        }
        [HttpPost]
        public ActionResult LoadSPR()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            ViewData["ContractKK"] = _context.ContractKK.OrderBy(o => o.Id).ToList();
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ViewData
            };
            return jsonNetResult;
        }
        /// <summary>
        /// Список всех категорий закупки
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCategoryList()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<DictionaryElementJson>();

            var categoryList = _context.Category.ToList();

            foreach (var category in categoryList)
            {
                result.Add(new DictionaryElementJson() { Id = category.Id, Name = category.Name });
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.ToList()
            };

            return jsonNetResult;
        }




        /// <summary>
        /// Список статусов контракта
        /// </summary>
        [HttpPost]
        public ActionResult GetContractStatusList()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<DictionaryElementJson>();

            var contractStatusList = _context.ContractStatus.ToList();

            foreach (var contractStatus in contractStatusList)
            {
                result.Add(new DictionaryElementJson() { Id = contractStatus.Id, Name = contractStatus.Name });
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.ToList()
            };

            return jsonNetResult;
        }

        /// <summary>
        /// Список всех характеров закупки
        /// </summary>
        /// <returns></returns>
        public ActionResult GetNatureList()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<NatureJson>();

            var natureList = _context.Nature.ToList();

            foreach (var nature in natureList)
            {
                result.Add(new NatureJson() { Id = nature.Id, CategoryId = nature.CategoryId, Name = nature.Name });
            }

            result.Add(new NatureJson() { Id = 0, CategoryId = 0, Name = "Не задан" });

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.ToList()
            };

            return jsonNetResult;
        }
        public ActionResult GetNatureList_L2()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<Nature_L2Json>();
            var natureList = _context.Nature_L2.OrderBy(o1=>o1.Nature_L1.Name).OrderBy(o2=>o2.Name).ToList();
            foreach (var nature in natureList)
            {
                result.Add(new Nature_L2Json(nature));
            }

            result.Add(new Nature_L2Json() { Id = 0, Name = "[Не задан]", Nature_L1Id = 0 });

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.ToList()
            };

            return jsonNetResult;
        }

        /// <summary>
        /// Список источников финансирования
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetFundingList()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<DictionaryElementJson>();

            var fundingList = _context.Funding.ToList();

            foreach (var funding in fundingList)
            {
                result.Add(new DictionaryElementJson() { Id = funding.Id, Name = funding.Name });
            }

            result.Add(new DictionaryElementJson() { Id = 0, Name = "Не задан" });

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.ToList()
            };

            return jsonNetResult;
        }

        /// <summary>
        /// Список разделов закупки в зависимости от роли пользователя.
        /// Для всех, кроме сотрудиков в офисе: текущий раздел, "ЛС", "На проверку"
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetRoleDependentPurchaseClassList(Byte selectedPurchaseClassId)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<DictionaryElementJson>();

            List<PurchaseClass> purchaseClassList = new List<PurchaseClass>();

            if (!User.IsInRole("GManager") && !User.IsInRole("GOperator"))
            {
                purchaseClassList = _context.PurchaseClass.Where(pc => pc.Id == selectedPurchaseClassId ||
                                                                       pc.Id == 2 || //ЛС
                                                                       pc.Id == 4 //На проверку
                                                                ).OrderBy(p => p.Id).ToList();
            }
            else
            {
                purchaseClassList = _context.PurchaseClass.OrderBy(p => p.Id).ToList();
            }

            foreach (var purchaseClass in purchaseClassList)
            {
                result.Add(new DictionaryElementJson() { Id = purchaseClass.Id, Name = purchaseClass.Name });
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.ToList()
            };

            return jsonNetResult;
        }

        /// <summary>
        /// Список разделов закупки
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPurchaseClassList()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<DictionaryElementJson>();

            var purchaseClassList = _context.PurchaseClass.OrderBy(p => p.Id).ToList();

            foreach (var purchaseClass in purchaseClassList)
            {
                result.Add(new DictionaryElementJson() { Id = purchaseClass.Id, Name = purchaseClass.Name });
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.ToList()
            };

            return jsonNetResult;
        }

        /// <summary>
        /// Список всех периодов поставки
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDeliveryTimePeriodList()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<DictionaryElementJson>();

            var periodList = _context.DeliveryTimePeriod.ToList();

            foreach (var period in periodList)
            {
                result.Add(new DictionaryElementJson() { Id = period.Id, Name = period.Name });
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.ToList()
            };

            return jsonNetResult;
        }

        /// <summary>
        /// Список всех годов финансирования
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetPaymentYearList()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<DictionaryElementJson>();

            var paymentYearList = _context.PaymentYear.Where(py => py.Id >= 2012 && py.Id <= 2034).ToList();

            foreach (var paymentYear in paymentYearList)
            {
                result.Add(new DictionaryElementJson() { Id = paymentYear.Id, Name = paymentYear.Name });
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.ToList()
            };

            return jsonNetResult;
        }
        [HttpPost]
        public ActionResult GetPaymentTypeList()
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var result = new List<DictionaryElementJson>();

            var l = _context.PaymentType.ToList();

            foreach (var e in l)
            {
                result.Add(new DictionaryElementJson() { Id = e.Id, Name = e.Name });
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.ToList()
            };

            return jsonNetResult;
        }
        [HttpPost]
        public JsonResult UploadPurchaseTemplate(IEnumerable<HttpPostedFileBase> uploads)
        {
            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            using (var excel = new PurchaseExcel())
            {
                var file = uploads.FirstOrDefault();
                if (file != null)
                {
                    var objects = excel.GetPurchaseObjectReady(file.InputStream).ToList();

                    var jsonResult = Json(objects, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;

                    return jsonResult;
                }
                else
                {
                    return Json(new EmptyResult(), JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public FileResult DownloadPurchaseTemplate(IEnumerable<PurchaseObjectReadyJson> objects)
        {
            using (var excel = new PurchaseExcel())
            {
                var bytes = excel.GetExcel(objects);
                return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Шаблон");
            }
        }

        [HttpPost]
        public JsonResult UploadContractTemplate(IEnumerable<HttpPostedFileBase> uploads)
        {
            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            using (var excel = new ContractExcel())
            {
                var file = uploads.FirstOrDefault();
                if (file != null)
                {
                    var objects = excel.GetContractObjectReady(file.InputStream).ToList();

                    var jsonResult = Json(objects, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
                else
                {
                    return Json(new EmptyResult(), JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public FileResult DownloadContractTemplate(IEnumerable<ContractObjectReadyJson> objects)
        {
            using (var excel = new ContractExcel()) 
            { 
                var bytes = excel.GetExcel(objects);
                return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Шаблон");
            }
        }

        /// <summary>
        /// Сохранить информацию об объектах закупки
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="lotId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SavePurchaseObjectsReady(IEnumerable<PurchaseObjectReadyJson> objects, long lotId)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var existlot = _context.Lot.FirstOrDefault(l => l.Id == lotId);

            if (existlot == null)
            {
                throw new ApplicationException("lot not found");
            }

            //Записываем пользователя кто изменил
            var userGuid = new Guid(User.Identity.GetUserId());
            var guid = Guid.NewGuid();

            //Добавляем новые объекты
            if (objects != null)
            {
                //Отключаем отслеживание изменний
                _context.Database.CommandTimeout = 0;

                var fileData = new List<PurchaseObjectReadyBulkInsert>();

                foreach (var objectReady in objects)
                {
                    var purchaseObjectReady = new PurchaseObjectReadyBulkInsert
                    {
                        LotId = lotId,
                        GroupId = guid,
                        UserId = userGuid
                    };
                    purchaseObjectReady.PorId = objectReady.Id;
                    objectReady.CopyTo(purchaseObjectReady);
                    purchaseObjectReady.ReceiverId = objectReady.ReceiverId;
                    purchaseObjectReady.ReceiverRaw = objectReady.ReceiverRaw;
                    fileData.Add(purchaseObjectReady);
                }

                _context.BulkInsert(fileData);
            }

            _context.TransferPurchaseObjectReadyFromBulkInsert(guid, userGuid, lotId);
            /*http://s-dev1:8080/redmine/issues/3860
Если пользователь заполнил объекты закупки, а объекты контракта при этом отсутствуют, то автоматически объекты закупки должны копироваться в объекты контракта.
Пользователь, изменивший объекты ТЗ контракта - Robot. Дату обновления ТЗ контракта обновить.
Изменения производить при сохранении объектов ТЗ.
 */

            if (objects != null && existlot.Purchase.Number.EndsWith("C"))
            {
                var contract = _context.Contract.FirstOrDefault(c => c.LotId == existlot.Id);
                if (contract != null)
                {
                    var COR_count = _context.ContractObjectReady.Where(w => w.ContractId == contract.Id).Count();
                    if (COR_count == 0)
                    {
                        //http://s-dev1:8080/redmine/issues/7085
                        var _context_robot = new GovernmentPurchasesContext(APP_Robot);
                        foreach (var OC in objects)
                        {
                            _context_robot.ContractObjectReady.Add(new ContractObjectReady()
                            {
                                Amount = OC.Amount,
                                Id = 0,
                                Name = OC.Name,
                                Price = OC.Price,
                                Sum = OC.Sum,
                                Unit = OC.Unit,
                                ContractId = contract.Id,
                                OKPD = "",
                                VNC = false,
                                Manufacturer = ""
                            });
                        }
                        _context_robot.SaveChanges();

                        contract.LastChangedObjectsUserId = new Guid("f65022f4-03e5-47c5-bf05-e3c964e3b9dc");
                        contract.LastChangedObjectsDate = DateTime.Now;
                        _context.SaveChanges();
                        return Json(2);
                    }
                }
                
            }
            return Json(1);
        }

        /// <summary>
        /// Сохранить информацию об объектах контракта
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="contractId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveContractObjectsReady(IEnumerable<ContractObjectReadyJson> objects, long contractId)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var contract = _context.Contract.FirstOrDefault(c => c.Id == contractId);
           

            if (contract == null)
            {
                throw new ApplicationException("contract not found");
            }
            var oldObjectsReady = _context.ContractObjectReady.Where(o => o.ContractId == contractId);
            _context.ContractObjectReady.RemoveRange(oldObjectsReady);

            if (objects != null)
            {
                foreach (var objectReady in objects)
                {
                    var contractObjectReady = new ContractObjectReady
                    {
                        ContractId = contractId
                    };
                    objectReady.CopyTo(contractObjectReady);
                    _context.ContractObjectReady.Add(contractObjectReady);
                }
            }

            var userGuid = new Guid(User.Identity.GetUserId());
            contract.LastChangedObjectsUserId = userGuid;
            contract.LastChangedObjectsDate = DateTime.Now;

            _context.SaveChanges();
            /*http://s-dev1:8080/redmine/issues/3860
Если пользователь заполнил объекты контракта, а объекты закупки при этом отсутствуют, то автоматически объекты контракта должны копироваться в объекты закупки.
Получатель проставить равным Заказчику.
Пользователь, изменивший объекты ТЗ закупки - Robot. Дату обновления ТЗ закупки обновить.
Изменения производить при сохранении объектов контракта.
             */


            if (objects != null && contract.Lot.Purchase.Number.EndsWith("C"))
            {
                var POR_count = _context.PurchaseObjectReady.Where(w => w.LotId == contract.LotId).Count();
                if (POR_count == 0)
                {
                    //http://s-dev1:8080/redmine/issues/7085
                    var _context_robot = new GovernmentPurchasesContext(APP_Robot);
                    long? ReceiverId = contract.Lot.Purchase.CustomerId;
                    //http://s-dev1:8080/redmine/issues/7085
                    foreach (var OC in objects)
                    {
                        _context_robot.PurchaseObjectReady.Add(new PurchaseObjectReady()
                        {
                            Amount = OC.Amount,
                            Id = 0,
                            Name = OC.Name,
                            Price = OC.Price,
                            ReceiverId = ReceiverId,
                            ReceiverRaw=null,
                            Sum = OC.Sum,
                            Unit = OC.Unit,
                            LotId = contract.LotId,
                            OKPD = "",
                            VNC = false
                        });
                    }
                    _context_robot.SaveChanges();
                    contract.Lot.LastChangedObjectsUserId= new Guid("f65022f4-03e5-47c5-bf05-e3c964e3b9dc");
                    contract.Lot.LastChangedObjectsDate = DateTime.Now;
                    _context.SaveChanges();
                    return Json(2);
                }
                
            }


            return Json(1);
        }



        [HttpPost]
        public ActionResult GenerateContract(long lotId)
        {
            GovernmentPurchasesContext _context = new GovernmentPurchasesContext(APP);
            var lot = _context.Lot.First(p => p.Id == lotId);

            var purchase = _context.Purchase.Single(p => p.Id == lot.PurchaseId);

            var supplierResult = lot.SupplierResult.FirstOrDefault();

            // со статусом лота Завершен и Завершен/один участник (LotStatus = 2, LotStatus = 3) 
            bool permissionCreate = supplierResult.LotStatusId == 2 || supplierResult.LotStatusId == 3;

            if (!permissionCreate)
                throw new ApplicationException("Статус лота должен быть \"Завершён\" или \"Завершён/один участник\"!");

            SupplierList supplierWinner = null;

            if (supplierResult != null)
            {
                var supplierList = _context.SupplierList.Where(s => s.SupplierResultId == supplierResult.Id);

                supplierWinner = supplierList.OrderBy(s => s.Number).FirstOrDefault();
            }

            Contract contract = new Contract
            {
                ReestrNumber = purchase.Number + "Z",
                Url = purchase.URL,
                ContractStatusId = 2
            };
            contract.ContractStatus = _context.ContractStatus.Single(cs => cs.Id == contract.ContractStatusId);
            contract.ReceiverId = purchase.CustomerId;
            contract.Receiver = purchase.Customer;
            contract.ConclusionDate = purchase.DateEnd.AddDays(10);
            contract.ContractNumber = purchase.Number + "Z";
            contract.Sum = lot.Sum;
            contract.CurrencyId = 4;
            contract.DateBegin = contract.ConclusionDate;
            contract.DateEnd = contract.ConclusionDate;

            if (supplierWinner != null)
            {
                contract.SupplierRawId = supplierWinner.SupplierRawId;
                contract.SupplierRaw = supplierWinner.SupplierRaw;

                //if (supplierWiner.SupplierRaw != null && supplierWiner.SupplierRaw.Supplier != null)
                //{
                //    contract.SupplierId = supplierWiner.SupplierRaw.Supplier.Id;
                //    contract.Supplier = supplierWiner.SupplierRaw.Supplier;
                //}
            }

            contract.ActuallyPaid = lot.Sum;

            var result = new ContractJson(_context, contract);

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;

        }
    }
}