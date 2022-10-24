using DataAggregator.Core.Classifier;
using DataAggregator.Core.Filter;
using DataAggregator.Core.Models.Classifier;
using DataAggregator.Domain.DAL;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers
{

    [Authorize(Roles = "SBoss")]
    public class ManufacturerController : BaseController
    {
        private DrugClassifierContext _context;
        private static readonly object LockObject = new object();
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new DrugClassifierContext(APP);
        }

        ~ManufacturerController()
        {
            _context.Dispose();
        }
        public ActionResult Edit(long? id)
        {
            return View();
        }
        public ActionResult Index(long? id)
        {
            return View();
        }
        private class ResponseModel
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public object Data { get; set; }
        }
        private static new ActionResult ReturnData(object data)
        {
            ResponseModel result = new ResponseModel();

            result.Success = true;
            result.Data = data;
            result.ErrorMessage = null;

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult GetClassifierEditorView(string filter)
        {
            try
            {
                var result = _context.Manufacturer.Select(cv => cv);

                if (!String.IsNullOrEmpty(filter))
                {
                    long id = 0;
                    if (long.TryParse(filter, out id) == true)
                    {
                        result = result.Where(sv => sv.Id == id);
                    }
                    else
                    {
                        result = result.Where(sv => sv.Value.Contains(filter) || sv.Corporation.Value.Contains(filter) || sv.Country.Value.Contains(filter));
                    }
                }
                else
                {
                    result = result.Where(sv => sv.CountryId==null || sv.CorporationId==null);
                }

                return ReturnData(result.OrderBy(o=>o.Value).ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        public ActionResult LoadCountryList(long? currentCountryId)
        {
            var CountryMatchList =
                _context.Country.Where(Country => Country.Id==currentCountryId)
                    .OrderBy(Country => Country.Value)
                    .Select(Country => new { Id = Country.Id, DisplayValue = Country.Value, Value = Country.Value })
                    .ToList();

            var CountryDontMatchList =
                _context.Country
                    .OrderBy(Country => Country.Value)
                    .Select(Country => new { Id = Country.Id, DisplayValue = Country.Value, Value = Country.Value })
                    .ToList();

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new
                {
                    CountryList = CountryMatchList.Concat(CountryDontMatchList)
                }
            };

            return jsonNetResult;
        }
        [HttpPost]
        public ActionResult LoadClassifier(long selectedID)
        {
            try
            {
                var result = _context.Manufacturer.FirstOrDefault(d => d.Id == selectedID);

                if (result == null)
                {
                    throw new ApplicationException("Производитель не найден");
                }

                var model = new ManufacturerJson
                {                    
                    CorporationId = result.CorporationId,
                    CountryId = result.CountryId,
                    filter = "",
                    Id = result.Id,
                    Value = result.Value,
                    Value_eng = result.Value_eng
                };


                if (model.CorporationId.HasValue)
                {
                    model.Corporation_Value = result.Corporation.Value;
                    model.Corporation_Value_eng = result.Corporation.Value_eng;
                }
                else
                {
                    model.Corporation_Value = "";
                    model.Corporation_Value_eng = "";
                }

                if (model.CountryId.HasValue)
                    model.Country = new DictionaryJson() { Id = result.Country.Id, Value = result.Country.Value };
                else
                    model.Country = new DictionaryJson() { Id = 0, Value = "" };
               // model.Restore();

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = model
                };

                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult Save(ManufacturerJson model, bool isNew,bool isMerge)
        {
            try
            {
                Domain.Model.DrugClassifier.Classifier.Manufacturer MNF = new Domain.Model.DrugClassifier.Classifier.Manufacturer();
                if (string.IsNullOrEmpty(model.Value) || string.IsNullOrEmpty(model.Corporation_Value) || model.Country.Id == 0)
                {
                    return BadRequest("Заполните все поля получше");
                }

                if (model != null)
                {
                    model.Value = ClearString(model.Value);
                    model.Corporation_Value = ClearString(model.Corporation_Value);
                    model.Value_eng = ClearString(model.Value_eng);
                    model.Corporation_Value_eng = ClearString(model.Corporation_Value_eng);
                }

                if (isNew ==true)
                    model.Id = 0;

                //Поиск корпорации
                var corp = _context.Corporation.FirstOrDefault(c => c.Value == model.Corporation_Value);
                if (corp != null)//нашли корпорацию
                {
                    model.CorporationId = corp.Id;
                    corp.Value = model.Corporation_Value;
                    corp.Value_eng = model.Corporation_Value_eng;
                }
                else
                {
                    var newcorp = _context.Corporation.Add(new Domain.Model.DrugClassifier.Classifier.Corporation() { Value = model.Corporation_Value, Value_eng = model.Corporation_Value_eng });
                    _context.SaveChanges();
                    model.CorporationId = newcorp.Id;
                }
                // }

                //проверка на дубль
                var dubl = _context.Manufacturer.FirstOrDefault(mnf => mnf.Value == model.Value && mnf.Id != model.Id);
                if (dubl != null)
                {
                    if (isMerge == true)
                    {
                        return BadRequest("Нет алгоритма");
                        //MergeManufacturer(model.Id, dubl.Id);
                       /* try //ИЗ продуктов скопировал просто так
                        {
                            lock (LockObject)
                            {
                                var userGuid = new Guid(User.Identity.GetUserId());
                                ClassifierEditor editor = new ClassifierEditor(_context, userGuid);

                                var data = editor.MergeClassifier(model, true);

                                return ReturnData(data);
                            }
                        }
                        catch (ApplicationException e)
                        {
                            return BadRequest(e.Message);
                        }*/
                    }
                    else
                    {
                        if (model.Id > 0)
                        {
                            return BadRequest("Внимание это дубль! код дубля = " + dubl.Id.ToString());
                        }
                        else
                        {
                            return BadRequest("Такой производитель уже есть! код = " + dubl.Id.ToString());
                        }
                    }
                }

                if (model.Id > 0)
                {
                    MNF = _context.Manufacturer.FirstOrDefault(mnf => mnf.Id == model.Id);
                }
                MNF.CorporationId = model.CorporationId;
                MNF.CountryId = model.Country.Id;
                MNF.Value = model.Value;
                MNF.Value_eng = model.Value_eng;
                if (model.Id <= 0)
                {
                    _context.Manufacturer.Add(MNF);
                }
                _context.SaveChanges();

                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = MNF.Id
                };

                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public ActionResult Delete(ManufacturerJson model)
        {
            try
            {//14497
                if (model.Id > 0)
                {
                    var mnf = _context.Manufacturer.Where(w => w.Id == model.Id).First();
                    if (_context.ProductionInfo.Where(w => w.OwnerTradeMarkId == mnf.Id || w.PackerId == mnf.Id).Count() > 0)
                    {
                        throw new ApplicationException("Есть связки в продуктах");
                    }
                    if (_context.GoodsProductionInfo.Where(w => w.OwnerTradeMarkId == mnf.Id || w.PackerId == mnf.Id).Count() > 0)
                    {
                        throw new ApplicationException("Есть связки в доп");
                    }
                    if (_context.DrugClassification.Where(w => w.OwnerTradeMarkId == mnf.Id).Count() > 0)
                    {
                        throw new ApplicationException("Есть связки в продуктах");
                    }
                    if (_context.GoodsBrandClassification.Where(w => w.OwnerTradeMarkId == mnf.Id).Count() > 0)
                    {
                        throw new ApplicationException("Есть связки в брендах доп");
                    }
                    if (_context.RegistrationCertificates.Where(w => w.OwnerRegistrationCertificateId == mnf.Id).Count() > 0)
                    {
                        throw new ApplicationException("Есть связки в сертификатах");
                    }
                    _context.Manufacturer.Remove(mnf);
                    _context.SaveChanges();
                }
                JsonNetResult jsonNetResult = new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = "ok"
                };

                return jsonNetResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        private string ClearString(string value)
        {



            if (string.IsNullOrEmpty(value))
                return value;

            //Замена не разрывных пробелов на обычные
            value = Regex.Replace(value, @"\u00A0", " ");
            //Удаление двойных пробелов
            while (value.Contains("  "))
            {
                value = value.Replace("  ", " ").Trim();
            }
            if (string.Equals(value, "~"))
                return null;



            return value.Trim();
        }

    }
}