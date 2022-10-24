using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.FederalBenefit;
using DataAggregator.Domain.Model.DrugClassifier.Log.FederalBenefit;
using DataAggregator.Web.Models.Classifier;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Classifier
{
    [Authorize(Roles = "SBoss")]
    public class FederalBenefitController : BaseController
    {

        private DrugClassifierContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new DrugClassifierContext(APP);
        }

        [HttpPost]
        public ActionResult ChangeChecked(FederalBenefitChecked value)
        {
            var result = new Result();

            //Если такая запись существует - удаляем её

            var FederalBenefitchecked = _context.FederalBenefitChecked.SingleOrDefault(v =>
                        v.INNGroupId == value.INNGroupId &&
                        v.FormProductId == value.FormProductId);


            var userGuid = new Guid(User.Identity.GetUserId());

            var log = new FederalBenefitCheckedChange()
            {

                INNGroupId = value.INNGroupId,
                FormProductId = value.FormProductId,
                UserId = userGuid
            };



            //Открываем транзакцию
            using (var transaction = _context.Database.BeginTransaction())
            {

                if (FederalBenefitchecked != null)
                {
                    log.ActionTypeId = 7; //Удаление
                    _context.FederalBenefitChecked.Remove(FederalBenefitchecked);
                }
                else
                {
                    log.ActionTypeId = 1; //Добавление
                    _context.FederalBenefitChecked.Add(value);
                }

                _context.FederalBenefitCheckedChange.Add(log);

                _context.SaveChanges();

                transaction.Commit();
            }

            result.Message = string.Empty;
            result.Success = true;
            result.Data = null;

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }
        

        //Загрузка
        [HttpPost]
        public ActionResult Load()
        {
            //Список не ЖНВЛП
            var notFederalBenefitList = _context.NoFederalBenefitView.Include("InnGroup").Include("FormProduct").ToList();

            //Список ЖНВЛП
            var FederalBenefitList = _context.FederalBenefitView.Include("InnGroup").Include("FormProduct").ToList();


            //Получаем список стобцов
            var FederalBenefitGroup = FederalBenefitList.GroupBy(v => new { v.INNGroupId, v.InnGroup, v.FormProductId, v.FormProduct, v.Checked }).ToList();

            //Получаем список колонок
            var columns = _context.FederalBenefitPeriod.ToList();


            List<ExpandoObject> rows = new List<ExpandoObject>();

            //Добавляем записи ЖНВЛП
            foreach (var FederalBenefit in FederalBenefitGroup)
            {
                dynamic row = new ExpandoObject();

                row.Checked = FederalBenefit.Key.Checked;
                row.INNGroupId = FederalBenefit.Key.INNGroupId;
                row.INNGroup = FederalBenefit.Key.InnGroup.Description;
                row.FormProductId = FederalBenefit.Key.FormProductId;
                row.FormProduct = FederalBenefit.Key.FormProduct.Value;

                var dictionary = (IDictionary<string, object>)row;

                foreach (var col in columns)
                {
                    var inPeriod = FederalBenefit.Any(v => v.FederalBenefitPeriodId == col.Id);
                    dictionary.Add("Y" + col.Id, inPeriod);
                }

                rows.Add(row);
            }
            //Добавляем остальные записи
            foreach (var notFederalBenefit in notFederalBenefitList)
            {
                dynamic row = new ExpandoObject();
                row.Checked = notFederalBenefit.Checked;
                row.INNGroupId = notFederalBenefit.INNGroupId;
                row.FormProductId = notFederalBenefit.FormProductId;
                row.INNGroup = notFederalBenefit.InnGroup.Description;
                row.FormProduct = notFederalBenefit.FormProduct.Value;
                row.DateAdd = notFederalBenefit.DateAdd;

                var dictionary = (IDictionary<string, object>)row;

                foreach (var col in columns)
                {
                    dictionary.Add("Y" + col.Id, false);
                }

                rows.Add(row);
            }

            dynamic result = new ExpandoObject();
            result.Rows = rows;
            result.Columns = columns;

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }
     
        //Изменение МНН
        [HttpPost]
        public ActionResult Change(FederalBenefitClassification value)
        {
            var result = new Result();

            //Если такая запись существует - удаляем её

            var FederalBenefit = _context.FederalBenefitClassification.SingleOrDefault(v =>
                        v.FormProductId == value.FormProductId &&
                        v.INNGroupId == value.INNGroupId &&
                        v.FederalBenefitPeriodId == value.FederalBenefitPeriodId);


            var userGuid = new Guid(User.Identity.GetUserId());

            var log = new FederalBenefitChange
            {

                INNGroupId = value.INNGroupId,
                FormProductId = value.FormProductId,
                FederalBenefitPeriodId = value.FederalBenefitPeriodId,
                UserId = userGuid
            };

            //Открываем транзакцию
            using (var transaction = _context.Database.BeginTransaction())
            {
                if (FederalBenefit != null)
                {
                    log.ActionTypeId = 7; //Удаление
                    _context.FederalBenefitClassification.Remove(FederalBenefit);
                }
                else
                {
                    log.ActionTypeId = 1; //Добавление
                    _context.FederalBenefitClassification.Add(value);
                }

                _context.FederalBenefitChange.Add(log);


                _context.SaveChanges();

                transaction.Commit();
            }

            result.Message = string.Empty;
            result.Success = true;
            result.Data = null;

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }


        [HttpPost]
        public ActionResult CopyPeriod(CopyPeriodModel model)
        {
            var result = new Result();

            try
            {

                if (_context.FederalBenefitClassification.Any(v => v.FederalBenefitPeriodId == model.PeriodIdTo))
                {
                    result.Message = "Целевой период уже имеет записи о Федеральной льготе, сначала сделайте его пустым";
                    result.Success = false;
                    result.Data = null;
                }
                else
                {
                    var userId = new Guid(User.Identity.GetUserId());

                    _context.FederalBenefitCopyPeriod(periodIdFrom: model.PeriodIdFrom, periodIdTo: model.PeriodIdTo, userId: userId);

                    result.Message = string.Empty;
                    result.Success = true;
                    result.Data = null;
                }
                
            }
            catch (Exception e)
            {
                LogError(e);
                result.Message = e.Message;
                result.Success = false;
                result.Data = null;
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }


        [HttpPost]
        public ActionResult PeriodLoad()
        {
            var result = new Result();
            try
            {
                var data = _context.FederalBenefitPeriod.ToList();

                result.Message = string.Empty;
                result.Success = true;
                result.Data = data;
            }
            catch (Exception e)
            {
                LogError(e);
                result.Message = e.Message;
                result.Success = false;
                result.Data = null;
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult PeriodRemove(FederalBenefitPeriod period)
        {
            var result = new Result();
            try
            {
                if (_context.FederalBenefitClassification.Any(v => v.FederalBenefitPeriodId == period.Id))
                    throw new ApplicationException("У данного периода уже заданы ФЛ");


                var periodLoad = _context.FederalBenefitPeriod.Single(v => v.Id == period.Id);


                var userGuid = new Guid(User.Identity.GetUserId());

                var log = new FederalBenefitPeriodChange
                {
                    ActionTypeId = 7, //Удаление
                    MonthStart = null,
                    YearStart = null,
                    MonthEnd = null,
                    YearEnd = null,
                    Name = null,
                    UserId = userGuid,
                    FederalBenefitPeriodId = periodLoad.Id
                };

                //Открываем транзакцию
                using (var transaction = _context.Database.BeginTransaction())
                {
                    _context.FederalBenefitPeriodChange.Add(log);
                    _context.FederalBenefitPeriod.Remove(periodLoad);
                    _context.SaveChanges();
                    transaction.Commit();
                }

                var data = _context.FederalBenefitPeriod.ToList();

                result.Message = string.Empty;
                result.Success = true;
                result.Data = data;
            }
            catch (Exception e)
            {
                LogError(e);
                result.Message = e.Message;
                result.Success = false;
                result.Data = null;
            }



            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult PeriodAdd(FederalBenefitPeriod period)
        {
            var result = new Result();

            try
            {
                if (_context.FederalBenefitPeriod.Any(v => v.Name == period.Name))
                    throw new ApplicationException("Период с таким именем уже существует");

                FederalBenefitPeriod addPeriod = new FederalBenefitPeriod
                {
                    Name = period.Name,
                    YearStart = period.YearStart,
                    MonthStart = period.MonthStart,
                    YearEnd = period.YearEnd,
                    MonthEnd = period.MonthEnd
                };


                var userGuid = new Guid(User.Identity.GetUserId());

                var log = new FederalBenefitPeriodChange
                {
                    ActionTypeId = 1, //Добавление
                    MonthStart = addPeriod.MonthStart,
                    YearStart = addPeriod.YearStart,
                    MonthEnd = addPeriod.MonthEnd,
                    YearEnd = addPeriod.YearEnd,
                    Name = addPeriod.Name,
                    UserId = userGuid,
                    FederalBenefitPeriodId = addPeriod.Id
                };

                //Открываем транзакцию
                using (var transaction = _context.Database.BeginTransaction())
                {
                    _context.FederalBenefitPeriod.Add(addPeriod);
                    _context.FederalBenefitPeriodChange.Add(log);
                    _context.SaveChanges();
                    transaction.Commit();
                }

                var data = _context.FederalBenefitPeriod.ToList();
                result.Message = string.Empty;
                result.Success = true;
                result.Data = data;
            }
            catch (Exception e)
            {
                LogError(e);
                result.Message = e.Message;
                result.Success = false;
                result.Data = null;
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult GetCount(long periodId)
        {
            var result = new Result();

            try
            {
                var count = _context.FederalBenefitClassification.Count(v => v.FederalBenefitPeriodId == periodId);

                result.Message = string.Empty;
                result.Success = true;
                result.Data = count;

            }
            catch (Exception e)
            {
                LogError(e);
                result.Message = e.Message;
                result.Success = false;
                result.Data = null;
            }
            
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }


        [HttpPost]
        public ActionResult PeriodChange(FederalBenefitPeriod period)
        {
            var result = new Result();

            try
            {
                if (_context.FederalBenefitPeriod.Any(v => v.Name == period.Name && v.Id != period.Id))
                    throw new ApplicationException("Период с таким именем уже существует");

                var p = _context.FederalBenefitPeriod.Single(v => v.Id == period.Id);
                p.Name = period.Name;
                p.YearStart = period.YearStart;
                p.MonthStart = period.MonthStart;
                p.YearEnd = period.YearEnd;
                p.MonthEnd = period.MonthEnd;

                var userGuid = new Guid(User.Identity.GetUserId());

                var log = new FederalBenefitPeriodChange()
                {
                    ActionTypeId = 2,
                    YearStart = p.YearStart,
                    MonthStart = p.MonthStart,
                    YearEnd = p.YearEnd,
                    MonthEnd = p.MonthEnd,
                    Name = p.Name,
                    FederalBenefitPeriodId = p.Id,
                    UserId = userGuid
                };

                //Открываем транзакцию
                using (var transaction = _context.Database.BeginTransaction())
                {
                    _context.FederalBenefitPeriodChange.Add(log);
                    _context.SaveChanges();
                    transaction.Commit();
                }

                var data = _context.FederalBenefitPeriod.ToList();

                result.Message = string.Empty;
                result.Success = true;
                result.Data = data;
            }
            catch (Exception e)
            {

                LogError(e);
                result.Message = e.Message;
                result.Success = false;
                result.Data = null;
            }



            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }



    }
}