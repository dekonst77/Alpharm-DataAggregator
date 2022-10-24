using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Changes;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.View;
using DataAggregator.Web.Models.Classifier;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Classifier
{
    [Authorize(Roles = "SBoss")]
    public class DataTransferController : BaseController
    {
        private DrugClassifierContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new DrugClassifierContext(APP);
        }

        [HttpPost]
        public ActionResult GetLeftClassifier(DataTransferClassifierFilter filter)
        {
            var classifier = GetClassifier(filter, false);

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = classifier
            };
        }

        [HttpPost]
        public ActionResult GetRightClassifier(DataTransferClassifierFilter filter)
        {
            var classifier = GetClassifier(filter, true);

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = classifier
            };
        }

        [HttpPost]
        public ActionResult GetTransferedData()
        {
            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = _context.TransferedDataView.ToList()
            };
        }

        private List<DataTransferClassifierView> GetClassifier(DataTransferClassifierFilter filter, bool withHistory)
        {
            var classifier = _context.DataTransferClassifierView.Where(c => (filter.DrugId == null || c.DrugId == filter.DrugId) &&
                                                                            (filter.PackerId == null || c.PackerId == filter.PackerId) &&
                                                                            (filter.OwnerTradeMarkId == null || c.OwnerTradeMarkId == filter.OwnerTradeMarkId) &&
                                                                            (string.IsNullOrEmpty(filter.TradeName) || c.TradeName.ToLower().Contains(filter.TradeName)) &&
                                                                            (string.IsNullOrEmpty(filter.Packer) || c.Packer.ToLower().Contains(filter.Packer)) &&
                                                                            (string.IsNullOrEmpty(filter.OwnerTradeMark) || c.OwnerTradeMark.ToLower().Contains(filter.OwnerTradeMark)));
            if (!withHistory)
            {
                classifier = classifier.Where(c => !c.IsHistorical);
            }

            return classifier.ToList();
        }

        [HttpPost]
        public ActionResult Transfer(List<long> classifierIdsFrom, long classifierIdTo, DataTransferOptions options)
        {
            Dictionary<long, long> transferList = new Dictionary<long, long>();

            /*
             * Если переносим не по всем параметрам, а частично, проверим, что все такие связки есть в классификаторе
             * Сохраним их ClassifierId для дальнейшей проверки по временным пересечениям
             * Если переносим полностью, просто добавим ClassifierId в список дальнейшей проверки
             */
            if (!(options.TransferDrugId && options.TransferPackerId && options.TransferOwnerTradeMarkId))
            {
                var transferTo =
                    _context.DataTransferClassifierView.FirstOrDefault(c => c.ClassifierId == classifierIdTo);

                var checkList =
                    _context.DataTransferClassifierView.Where(c => classifierIdsFrom.Contains(c.ClassifierId))
                                                       .Select(c => new DrugOwnerPackerEntity
                                                       {
                                                           OldClassifierId = c.ClassifierId,
                                                           DrugId = options.TransferDrugId ? transferTo.DrugId : c.DrugId,
                                                           PackerId = options.TransferPackerId ? transferTo.PackerId : c.PackerId,
                                                           OwnerTradeMarkId = options.TransferOwnerTradeMarkId ? transferTo.OwnerTradeMarkId : c.OwnerTradeMarkId
                                                       }).ToList();

                foreach (var drug in checkList)
                {
                    var classifierEntity =
                        _context.DataTransferClassifierView.FirstOrDefault(c => c.DrugId == drug.DrugId &&
                                                                                c.PackerId == drug.PackerId &&
                                                                                c.OwnerTradeMarkId == drug.OwnerTradeMarkId);

                    if (classifierEntity != null)
                    {
                        transferList.Add(drug.OldClassifierId, classifierEntity.ClassifierId);
                    }
                    else
                    {
                        Response.StatusCode = (int) HttpStatusCode.BadRequest;
                        return Json("Нет необходимой связки в классификаторе");
                    }
                }
            }
            else
            {
                foreach (var classifierId in classifierIdsFrom)
                {
                    transferList.Add(classifierId, classifierIdTo);
                }
            }


            /*
             * Получим списки пересечений по датам по данным, из которых переносим (недопустимые ситуации)
             * Если хоть в одном списке есть элемент - продолжение невозможно
             */
            var fromInFrom = _context.ClassifierTransfer.Count(ct => classifierIdsFrom.Contains(ct.ClassifierIdFrom) &&
               DbFunctions.CreateDateTime(ct.YearStart ?? DateTime.MinValue.Year, ct.MonthStart ?? DateTime.MinValue.Month, 1, 0, 0, 0) <= DbFunctions.CreateDateTime(options.YearTo ?? DateTime.MaxValue.Year, options.MonthTo ?? DateTime.MaxValue.Month, 1, 0, 0, 0) &&
               DbFunctions.CreateDateTime(ct.YearEnd ?? DateTime.MaxValue.Year, ct.MonthEnd ?? DateTime.MaxValue.Month, 1, 0, 0, 0) >= DbFunctions.CreateDateTime(options.YearFrom ?? DateTime.MinValue.Year, options.MonthFrom ?? DateTime.MinValue.Month, 1, 0, 0, 0));

            if (fromInFrom > 0)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Пересечение временных интервалов");
            }

            var fromInTo = _context.ClassifierTransfer.Count(ct => classifierIdsFrom.Contains(ct.ClassifierIdTo) &&
               DbFunctions.CreateDateTime(ct.YearStart ?? DateTime.MinValue.Year, ct.MonthStart ?? DateTime.MinValue.Month, 1, 0, 0, 0) <= DbFunctions.CreateDateTime(options.YearTo ?? DateTime.MaxValue.Year, options.MonthTo ?? DateTime.MaxValue.Month, 1, 0, 0, 0) &&
               DbFunctions.CreateDateTime(ct.YearEnd ?? DateTime.MaxValue.Year, ct.MonthEnd ?? DateTime.MaxValue.Month, 1, 0, 0, 0) >= DbFunctions.CreateDateTime(options.YearFrom ?? DateTime.MinValue.Year, options.MonthFrom ?? DateTime.MinValue.Month, 1, 0, 0, 0));

            if (fromInTo > 0)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Пересечение временных интервалов");
            }

            var toList = transferList.Select(l => l.Value);
            var toInFrom = _context.ClassifierTransfer.Count(ct => toList.Contains(ct.ClassifierIdFrom) &&
               DbFunctions.CreateDateTime(ct.YearStart ?? DateTime.MinValue.Year, ct.MonthStart ?? DateTime.MinValue.Month, 1, 0, 0, 0) <= DbFunctions.CreateDateTime(options.YearTo ?? DateTime.MaxValue.Year, options.MonthTo ?? DateTime.MaxValue.Month, 1, 0, 0, 0) &&
               DbFunctions.CreateDateTime(ct.YearEnd ?? DateTime.MaxValue.Year, ct.MonthEnd ?? DateTime.MaxValue.Month, 1, 0, 0, 0) >= DbFunctions.CreateDateTime(options.YearFrom ?? DateTime.MinValue.Year, options.MonthFrom ?? DateTime.MinValue.Month, 1, 0, 0, 0));

            if (toInFrom > 0)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Пересечение временных интервалов");
            }


            /*
             * А теперь все сохраним, если до этого нигде не вылетело
             */
            foreach (var transfer in transferList)
            {
                _context.ClassifierTransfer.Add(new ClassifierTransfer
                {
                    ClassifierIdFrom = transfer.Key,
                    ClassifierIdTo = transfer.Value,
                    YearStart = options.YearFrom,
                    MonthStart = options.MonthFrom,
                    YearEnd = options.YearTo,
                    MonthEnd = options.MonthTo,
                    Date = DateTime.Now,
                    UserId = new Guid(User.Identity.GetUserId())
                });
            }

            _context.SaveChanges();

            return null;
        }

        public ActionResult Edit(List<long> transfersToEdit, DataTransferOptions options)
        {
            var transfers = _context.ClassifierTransfer.Where(ct => transfersToEdit.Contains(ct.Id)).ToList();

            var ignoreList = transfers.Select(t => t.Id);
            var fromList = transfers.Select(t => t.ClassifierIdFrom);
            var toList = transfers.Select(t => t.ClassifierIdTo);

            /*
             * Получим списки пересечений по датам по данным, из которых переносим (недопустимые ситуации)
             * Если хоть в одном списке есть элемент - продолжение невозможно
             * Так же игнорим уже существующие записи
             */
            var fromInFrom = _context.ClassifierTransfer.Count(ct => fromList.Contains(ct.ClassifierIdFrom) &&
               !ignoreList.Contains(ct.Id) &&
               DbFunctions.CreateDateTime(ct.YearStart ?? DateTime.MinValue.Year, ct.MonthStart ?? DateTime.MinValue.Month, 1, 0, 0, 0) <= DbFunctions.CreateDateTime(options.YearTo ?? DateTime.MaxValue.Year, options.MonthTo ?? DateTime.MaxValue.Month, 1, 0, 0, 0) &&
               DbFunctions.CreateDateTime(ct.YearEnd ?? DateTime.MaxValue.Year, ct.MonthEnd ?? DateTime.MaxValue.Month, 1, 0, 0, 0) >= DbFunctions.CreateDateTime(options.YearFrom ?? DateTime.MinValue.Year, options.MonthFrom ?? DateTime.MinValue.Month, 1, 0, 0, 0));

            if (fromInFrom > 0)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Пересечение временных интервалов");
            }

            var fromInTo = _context.ClassifierTransfer.Count(ct => fromList.Contains(ct.ClassifierIdTo) &&
               !ignoreList.Contains(ct.Id) &&
               DbFunctions.CreateDateTime(ct.YearStart ?? DateTime.MinValue.Year, ct.MonthStart ?? DateTime.MinValue.Month, 1, 0, 0, 0) <= DbFunctions.CreateDateTime(options.YearTo ?? DateTime.MaxValue.Year, options.MonthTo ?? DateTime.MaxValue.Month, 1, 0, 0, 0) &&
               DbFunctions.CreateDateTime(ct.YearEnd ?? DateTime.MaxValue.Year, ct.MonthEnd ?? DateTime.MaxValue.Month, 1, 0, 0, 0) >= DbFunctions.CreateDateTime(options.YearFrom ?? DateTime.MinValue.Year, options.MonthFrom ?? DateTime.MinValue.Month, 1, 0, 0, 0));

            if (fromInTo > 0)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Пересечение временных интервалов");
            }

            var toInFrom = _context.ClassifierTransfer.Count(ct => toList.Contains(ct.ClassifierIdFrom) &&
               !ignoreList.Contains(ct.Id) &&
               DbFunctions.CreateDateTime(ct.YearStart ?? DateTime.MinValue.Year, ct.MonthStart ?? DateTime.MinValue.Month, 1, 0, 0, 0) <= DbFunctions.CreateDateTime(options.YearTo ?? DateTime.MaxValue.Year, options.MonthTo ?? DateTime.MaxValue.Month, 1, 0, 0, 0) &&
               DbFunctions.CreateDateTime(ct.YearEnd ?? DateTime.MaxValue.Year, ct.MonthEnd ?? DateTime.MaxValue.Month, 1, 0, 0, 0) >= DbFunctions.CreateDateTime(options.YearFrom ?? DateTime.MinValue.Year, options.MonthFrom ?? DateTime.MinValue.Month, 1, 0, 0, 0));

            if (toInFrom > 0)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Пересечение временных интервалов");
            }

            foreach (var transfer in transfers)
            {
                transfer.YearStart = options.YearFrom;
                transfer.MonthStart = options.MonthFrom;
                transfer.YearEnd = options.YearTo;
                transfer.MonthEnd = options.MonthTo;
            }

            _context.SaveChanges();

            return null;
        }

        [HttpPost]
        public ActionResult DeleteTransfers(List<long> transfersToDelete)
        {
            _context.ClassifierTransfer.RemoveRange(_context.ClassifierTransfer.Where(ct => transfersToDelete.Contains(ct.Id)));
            _context.SaveChanges();
            return null;
        }
    }
}