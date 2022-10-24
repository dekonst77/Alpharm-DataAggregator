using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Domain.Model.DrugClassifier.Log;
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
    public class VEDController : BaseController
    {

        private DrugClassifierContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new DrugClassifierContext(APP);
        }

        //Загрузка TradeName
        [HttpPost]
        public ActionResult LoadTradeName()
        {
            //Список ЖНВЛП
            var vedList = _context.VEDClassificationByTN.Include("TradeName").Include("FormProduct").ToList();

            //Получаем список стобцов
            var vedGroup = vedList.GroupBy(v => new { v.TradeNameId, v.TradeName, v.FormProductId, v.FormProduct }).ToList();

            //Получаем список колонок
            var columns = _context.VEDPeriod.ToList();


            List<ExpandoObject> rows = new List<ExpandoObject>();

            //Добавляем записи ЖНВЛП
            foreach (var ved in vedGroup)
            {
                dynamic row = new ExpandoObject();

                row.TradeNameId = ved.Key.TradeNameId;
                row.TradeName = ved.Key.TradeName.Value;
                row.FormProductId = ved.Key.FormProductId;
                row.FormProduct = ved.Key.FormProduct.Value;

                var dictionary = (IDictionary<string, object>)row;

                foreach (var col in columns)
                {
                    var inPeriod = ved.Any(v => v.VEDPeriodId == col.Id);
                    dictionary.Add("Y" + col.Id, inPeriod);
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


        [HttpPost]
        public ActionResult ChangeChecked(VEDChecked value)
        {
            var result = new Result();

            //Если такая запись существует - удаляем её

            var vedchecked = _context.VEDChecked.SingleOrDefault(v =>
                        v.INNGroupId == value.INNGroupId &&
                        v.FormProductId == value.FormProductId);


            var userGuid = new Guid(User.Identity.GetUserId());

            var log = new VEDCheckedChange()
            {

                INNGroupId = value.INNGroupId,
                FormProductId = value.FormProductId,
                UserId = userGuid
            };



            //Открываем транзакцию
            using (var transaction = _context.Database.BeginTransaction())
            {

                if (vedchecked != null)
                {
                    log.ActionTypeId = 7; //Удаление
                    _context.VEDChecked.Remove(vedchecked);
                }
                else
                {
                    log.ActionTypeId = 1; //Добавление
                    _context.VEDChecked.Add(value);
                }

                _context.VEDCheckedChange.Add(log);

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
            var notvedList = _context.NoVedView.Include("InnGroup").Include("FormProduct").ToList();

            //Список ЖНВЛП
            var vedList = _context.VedView.Include("InnGroup").Include("FormProduct").ToList();


            //Получаем список стобцов
            var vedGroup = vedList.GroupBy(v => new { v.INNGroupId, v.InnGroup, v.FormProductId, v.FormProduct, v.Checked }).ToList();

            //Получаем список колонок
            var columns = _context.VEDPeriod.ToList();


            List<ExpandoObject> rows = new List<ExpandoObject>();

            //Добавляем записи ЖНВЛП
            foreach (var ved in vedGroup)
            {
                dynamic row = new ExpandoObject();

                row.Checked = ved.Key.Checked;
                row.INNGroupId = ved.Key.INNGroupId;
                row.INNGroup = ved.Key.InnGroup.Description;
                row.FormProductId = ved.Key.FormProductId;
                row.FormProduct = ved.Key.FormProduct.Value;

                var dictionary = (IDictionary<string, object>)row;

                foreach (var col in columns)
                {
                    var inPeriod = ved.Any(v => v.VEDPeriodId == col.Id);
                    dictionary.Add("Y" + col.Id, inPeriod);
                }

                rows.Add(row);
            }
            //Добавляем остальные записи
            foreach (var notved in notvedList)
            {
                dynamic row = new ExpandoObject();
                row.Checked = notved.Checked;
                row.INNGroupId = notved.INNGroupId;
                row.FormProductId = notved.FormProductId;
                row.INNGroup = notved.InnGroup.Description;
                row.FormProduct = notved.FormProduct.Value;
                row.DateAdd = notved.DateAdd;

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



        //Изменение TradeName
        [HttpPost]
        public ActionResult ChangeTradeName(VEDClassificationByTN value)
        {
            var result = new Result();

            //Если такая запись существует - удаляем её

            var ved = _context.VEDClassificationByTN.SingleOrDefault(v =>
                        v.FormProductId == value.FormProductId &&
                        v.TradeNameId == value.TradeNameId &&
                        v.VEDPeriodId == value.VEDPeriodId);

            var userGuid = new Guid(User.Identity.GetUserId());

            var log = new VEDChange
            {

                TradeNameId = value.TradeNameId,
                FormProductId = value.FormProductId,
                VEDPeriodId = value.VEDPeriodId,
                UserId = userGuid
            };

            //Открываем транзакцию
            using (var transaction = _context.Database.BeginTransaction())
            {

                if (ved != null)
                {
                    log.ActionTypeId = 7;//Удаление
                    _context.VEDClassificationByTN.Remove(ved);
                }
                else
                {
                    log.ActionTypeId = 1;//Добавление
                    _context.VEDClassificationByTN.Add(value);
                }

                _context.VedChange.Add(log);

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


        //Изменение МНН
        [HttpPost]
        public ActionResult Change(VEDClassification value)
        {
            var result = new Result();

            //Если такая запись существует - удаляем её

            var ved = _context.VEDClassification.SingleOrDefault(v =>
                        v.FormProductId == value.FormProductId &&
                        v.INNGroupId == value.INNGroupId &&
                        v.VEDPeriodId == value.VEDPeriodId);


            var userGuid = new Guid(User.Identity.GetUserId());

            var log = new VEDChange
            {

                INNGroupId = value.INNGroupId,
                FormProductId = value.FormProductId,
                VEDPeriodId = value.VEDPeriodId,
                UserId = userGuid
            };

            //Открываем транзакцию
            using (var transaction = _context.Database.BeginTransaction())
            {
                if (ved != null)
                {
                    log.ActionTypeId = 7; //Удаление
                    _context.VEDClassification.Remove(ved);
                }
                else
                {
                    log.ActionTypeId = 1; //Добавление
                    _context.VEDClassification.Add(value);
                }

                _context.VedChange.Add(log);


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

                if (_context.VEDClassification.Any(v => v.VEDPeriodId == model.PeriodIdTo) ||
                    _context.VEDClassificationByTN.Any(v => v.VEDPeriodId == model.PeriodIdTo))
                {
                    result.Message = "Целевой период уже имеет записи о ЖНВЛП, сначала сделайте его пустым";
                    result.Success = false;
                    result.Data = null;
                }
                else
                {
                    var userId = new Guid(User.Identity.GetUserId());

                    _context.CopyPeriod(periodIdFrom: model.PeriodIdFrom, periodIdTo:model.PeriodIdTo, userId:userId);

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
                var data = _context.VEDPeriod.ToList();

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
        public ActionResult PeriodRemove(VEDPeriod period)
        {
            var result = new Result();
            try
            {
                if (_context.VEDClassification.Any(v => v.VEDPeriodId == period.Id))
                    throw new ApplicationException("У данного периода уже заданы ЖНВЛП");


                var periodLoad = _context.VEDPeriod.Single(v => v.Id == period.Id);


                var userGuid = new Guid(User.Identity.GetUserId());

                var log = new VEDPeriodChange
                {
                    ActionTypeId = 7, //Удаление
                    MonthStart = null,
                    YearStart = null,
                    MonthEnd = null,
                    YearEnd = null,
                    Name = null,
                    UserId = userGuid,
                    VedPeriodId = periodLoad.Id
                };

                //Открываем транзакцию
                using (var transaction = _context.Database.BeginTransaction())
                {
                    _context.VedPeriodChange.Add(log);
                    _context.VEDPeriod.Remove(periodLoad);
                    _context.SaveChanges();
                    transaction.Commit();
                }

                var data = _context.VEDPeriod.ToList();

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
        public ActionResult PeriodAdd(VEDPeriod period)
        {
            var result = new Result();

            try
            {
                if (_context.VEDPeriod.Any(v => v.Name == period.Name))
                    throw new ApplicationException("Период с таким именем уже существует");

                VEDPeriod addPeriod = new VEDPeriod
                {
                    Name = period.Name,
                    YearStart = period.YearStart,
                    MonthStart = period.MonthStart,
                    YearEnd = period.YearEnd,
                    MonthEnd = period.MonthEnd
                };


                var userGuid = new Guid(User.Identity.GetUserId());

                var log = new VEDPeriodChange
                {
                    ActionTypeId = 1, //Добавление
                    MonthStart = addPeriod.MonthStart,
                    YearStart = addPeriod.YearStart,
                    MonthEnd = addPeriod.MonthEnd,
                    YearEnd = addPeriod.YearEnd,
                    Name = addPeriod.Name,
                    UserId = userGuid,
                    VedPeriodId = addPeriod.Id
                };

                //Открываем транзакцию
                using (var transaction = _context.Database.BeginTransaction())
                {
                    _context.VEDPeriod.Add(addPeriod);
                    _context.VedPeriodChange.Add(log);
                    _context.SaveChanges();
                    transaction.Commit();
                }

                var data = _context.VEDPeriod.ToList();
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
                var count1 = _context.VEDClassification.Count(v => v.VEDPeriodId == periodId);
                var count2 = _context.VEDClassificationByTN.Count(v => v.VEDPeriodId == periodId);

                result.Message = string.Empty;
                result.Success = true;
                result.Data = count1 + count2;

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
        public ActionResult PeriodChange(VEDPeriod period)
        {
            var result = new Result();

            try
            {
                if (_context.VEDPeriod.Any(v => v.Name == period.Name && v.Id != period.Id))
                    throw new ApplicationException("Период с таким именем уже существует");

                var p = _context.VEDPeriod.Single(v => v.Id == period.Id);
                p.Name = period.Name;
                p.YearStart = period.YearStart;
                p.MonthStart = period.MonthStart;
                p.YearEnd = period.YearEnd;
                p.MonthEnd = period.MonthEnd;

                var userGuid = new Guid(User.Identity.GetUserId());

                var log = new VEDPeriodChange()
                {
                    ActionTypeId = 2,
                    YearStart = p.YearStart,
                    MonthStart = p.MonthStart,
                    YearEnd = p.YearEnd,
                    MonthEnd = p.MonthEnd,
                    Name = p.Name,
                    VedPeriodId = p.Id,
                    UserId = userGuid
                };

                //Открываем транзакцию
                using (var transaction = _context.Database.BeginTransaction())
                {
                    _context.VedPeriodChange.Add(log);
                    _context.SaveChanges();
                    transaction.Commit();
                }

                var data = _context.VEDPeriod.ToList();

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