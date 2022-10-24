using DataAggregator.Core.Filter;
using DataAggregator.Core.GoodsClassifier;
using DataAggregator.Core.Models.Classifier;
using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.Classifier;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.Mvc;
using DataAggregator.Web.Models.GoodsClassifier;

namespace DataAggregator.Web.Controllers.Classifier
{

    [Authorize(Roles = "SBoss")]
    public class GoodsClassifierEditorController : BaseController
    {
        private DrugClassifierContext _context;

        private static readonly object LockObject = new object();

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new DrugClassifierContext(APP);
        }

        ~GoodsClassifierEditorController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult GetGoodsCategoryList()
        {
            try
            {
                var result = _context.GoodsCategory.OrderBy(c => c.GoodsSection.Name).ThenBy(c => c.Name);

                return ReturnData(result.ToList());
            }
            catch (Exception e)
            {
                return ReturnError(e);
            }
        }

        [HttpPost]
        public ActionResult GetGoodsBrandList()
        {
            try
            {
                var result = _context.Brand.Where(b => b.UseGoodsClassifier).OrderBy(b => b.Value).ToList();

                return ReturnData(result);
            }
            catch (Exception e)
            {
                return ReturnError(e);
            }
        }

        [HttpPost]
        public ActionResult GetGoodsBrand(string goodsTradeNameValue, string ownerTradeMarkValue)
        {
            try
            {
                var result =
                    _context.GoodsBrandClassification.FirstOrDefault(
                        g => g.GoodsTradeName.Value.Equals(goodsTradeNameValue) && g.OwnerTradeMark.Value.Equals(ownerTradeMarkValue));

                return ReturnData(result != null ? result.GoodsBrand : null);
            }
            catch (Exception e)
            {
                return ReturnError(e);
            }
        }

        [HttpPost]
        public ActionResult GetParameterGroupsList(long? goodsCategoryId, long? goodsId, long? ownerTrademarkId, long? packerId)
        {
            try
            {
                if (goodsCategoryId != null && goodsCategoryId > 0)
                {
                    using (var context = new DrugClassifierContext(APP))
                    {
                        //получаем список ParameterGroup, связанных с выбранной категорией
                        var parameterGroupList =
                            context.ParameterGroup.Where(g => g.GoodsCategoryId == goodsCategoryId).ToList();

                        //если в режиме редактирования, то получаем GoodsProductionInfo выбранного препарата
                        var goodsProductionInfo =
                            context.GoodsProductionInfo.FirstOrDefault(
                                g =>
                                    g.GoodsId == goodsId && g.OwnerTradeMarkId == ownerTrademarkId &&
                                    g.PackerId == packerId);

                        var goodsProductionInfoId = goodsProductionInfo != null ? (long?)goodsProductionInfo.Id : null;

                        var result = new List<ParameterGroupJson>();

                        foreach (var parameterGroup in parameterGroupList)
                        {
                            var parameterGroupJson = new ParameterGroupJson();
                            parameterGroupJson.Id = parameterGroup.Id;
                            parameterGroupJson.Name = parameterGroup.Name;
                            parameterGroupJson.ParametersList = GeParametersList(parameterGroup.Id);

                            foreach (var parameter in parameterGroupJson.ParametersList)
                            {
                                var gpip =
                                    context.GoodsProductionInfoParameter.SingleOrDefault(
                                        g =>
                                            g.GoodsProductionInfoId == goodsProductionInfoId &&
                                            g.ParameterId == parameter.Id);
                                if (gpip != null)
                                {
                                    parameterGroupJson.SelectedParameterValue = parameter.Value;
                                    break;
                                }
                            }

                            result.Add(parameterGroupJson);
                        }

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new List<ParameterGroupJson>(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private List<ParameterJson> GeParametersList(long parameterGroupId)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                var result = new List<ParameterJson>();
                var parametersWithNoParent =
                    context.Parameter.Where(p => p.ParameterGroupId == parameterGroupId && p.ParentId == null).ToList();

                foreach (var parameter in parametersWithNoParent)
                {
                    if (!context.Parameter.Any(p => p.ParentId == parameter.Id))
                    {
                        var parameterJson = new ParameterJson(parameter);
                        result.Add(parameterJson);
                    }
                }

                var parametersWithParent =
                    context.Parameter.Where(p => p.ParameterGroupId == parameterGroupId && p.ParentId != null).ToList();

                foreach (var parameter in parametersWithParent)
                {
                    var parameterJson = new ParameterJson(parameter);
                    result.Add(parameterJson);
                }

                return result.OrderBy(p => p.Value).ToList();
            }
        }

        [HttpPost]
        public ActionResult LoadClassifier(long goodsId, long ownerTradeMarkId, long packerId)
        {
            var good = _context.Goods.FirstOrDefault(g => g.Id == goodsId);

            if (good == null)
            {
                throw new ApplicationException("Товар с id " + goodsId + " не обнаружен!");
            }

            var ownerTradeMark = _context.Manufacturer.FirstOrDefault(t => t.Id == ownerTradeMarkId);

            if (ownerTradeMark == null)
                throw new ApplicationException("Правообладатель с id " + ownerTradeMarkId + " не обнаружен!");

            var packer = _context.Manufacturer.FirstOrDefault(p => p.Id == packerId);

            if (packer == null)
                throw new ApplicationException("Упаковщик с id " + packerId + " не обнаружен!");

            var productionInfo =
                _context.GoodsProductionInfo.First(
                    p => p.GoodsId == goodsId && p.OwnerTradeMarkId == ownerTradeMarkId && p.PackerId == packerId);

            var classifier = _context.ClassifierInfo.FirstOrDefault(t => t.GoodsProductionInfoId == productionInfo.Id);
            if (classifier == null)
                throw new ApplicationException("Классификатор для доп. материала с id " + productionInfo.Id + " не обнаружен!");

            var goodsBrandClassification =
                _context.GoodsBrandClassification.FirstOrDefault(
                    gbc => gbc.GoodsTradeNameId == good.GoodsTradeNameId && gbc.OwnerTradeMarkId == ownerTradeMarkId);

            var model = new GoodsClassifierEditorModelJson
            {
                GoodsId = goodsId,
                GoodKey = goodsId.ToString(),
                GoodsDescription = good.GoodsDescription,
                GoodsTradeName = new DictionaryJson { Id = good.GoodsTradeNameId, Value = good.GoodsTradeName.Value },
                GoodsBrand =
                    goodsBrandClassification != null
                        ? new DictionaryJson
                        {
                            Id = goodsBrandClassification.BrandId,
                            Value = goodsBrandClassification.GoodsBrand.Value
                        }
                        : null,
                OwnerTradeMark =
                    new DictionaryJson()
                    {
                        Id = ownerTradeMark.Id,
                        Value = ownerTradeMark.Value
                    },
                Packer = new DictionaryJson() { Id = packer.Id, Value = packer.Value },
                PackerId = packer.Id,
                OwnerTradeMarkId = ownerTradeMark.Id,
                ProductionInfoId = productionInfo.Id,
                GoodsCategory = new GoodsCategoryJson()
                {
                    Id = productionInfo.GoodsCategoryId ?? 0,
                    Name = productionInfo.GoodsCategoryId != null ? productionInfo.GoodsCategory.Name : ""
                },
                Used = productionInfo.Used,
                Comment = productionInfo.Comment,
                ToRetail = classifier.ToRetail.GetValueOrDefault()
            };

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = model
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult GetClassifierEditorView(GoodsClassifierFilter filter)
        {
            try
            {
                var result = _context.GoodsSystematizationView.Select(cv => cv);

                if (!String.IsNullOrEmpty(filter.GoodsTradeName))
                {
                    if (filter.GoodsTradeNameId != null)
                    {
                        result = result.Where(sv => sv.GoodsTradeNameId == filter.GoodsTradeNameId);
                    }
                    else
                    {
                        var searchMask = filter.GoodsTradeName.Replace('*', '%');
                        var idList =
                            _context.GoodsTradeName.Where(x => SqlFunctions.PatIndex(searchMask, x.Value) > 0).Select(x => x.Id).ToList();
                        result = result.Where(sv => (idList.Contains(sv.GoodsTradeNameId)));
                    }
                }

                if (!String.IsNullOrEmpty(filter.OwnerTradeMark))
                {
                    if (filter.OwnerTradeMarkId != null)
                    {
                        result = result.Where(sv => sv.OwnerTradeMarkId == filter.OwnerTradeMarkId);
                    }
                    else
                    {
                        var searchMask = filter.OwnerTradeMark.Replace('*', '%');
                        var idList =
                            _context.Manufacturer.Where(x => SqlFunctions.PatIndex(searchMask, x.Value) > 0).Select(x => x.Id).ToList();
                        result = result.Where(sv => (idList.Contains(sv.OwnerTradeMarkId)));
                    }
                }

                if (filter.OwnerTradeMarkId > 0)
                {
                    var idList =
                        _context.Manufacturer.Where(x => x.Id == filter.OwnerTradeMarkId).Select(x => x.Id).ToList();

                    result = result.Where(sv => (idList.Contains(sv.OwnerTradeMarkId)));
                }

                if (!String.IsNullOrEmpty(filter.Packer))
                {
                    if (filter.PackerId != null)
                    {
                        result = result.Where(sv => sv.PackerId == filter.PackerId);
                    }
                    else
                    {
                        var searchMask = filter.Packer.Replace('*', '%');
                        var idList =
                            _context.Manufacturer.Where(x => SqlFunctions.PatIndex(searchMask, x.Value) > 0).Select(x => x.Id).ToList();
                        result = result.Where(sv => (idList.Contains(sv.PackerId)));
                    }
                }

                if (filter.PackerId > 0)
                {
                    var idList =
                        _context.Manufacturer.Where(x => x.Id == filter.PackerId).Select(x => x.Id).ToList();

                    result = result.Where(sv => (idList.Contains(sv.PackerId)));
                }

                if (!String.IsNullOrEmpty(filter.GoodsDescription))
                {
                    var searchMask = filter.GoodsDescription.Replace('*', '%');

                    result = result.Where(sv => SqlFunctions.PatIndex(searchMask, sv.GoodsDescription) > 0);
                }

                if (filter.GoodsCategory != null && filter.GoodsCategory.Id != 0)
                {
                    result = result.Where(sv => sv.GoodsCategoryId == filter.GoodsCategory.Id);
                }

                if (filter.GoodsId != null)
                {
                    result = result.Where(sv => sv.GoodsId == (long)filter.GoodsId);
                }
                if (filter.ClassifierId > 0)
                {
                    result = result.Where(w => w.ClassifierId == (long)filter.ClassifierId);
                }

                return ReturnData(result.ToList());
            }
            catch (Exception e)
            {
                return ReturnError(e);
            }
        }

        [HttpPost]
        public ActionResult CheckClassifier(GoodsClassifierEditorModelJson model)
        {
            try
            {
                lock (LockObject)
                {

                    var userGuid = new Guid(User.Identity.GetUserId());
                    GoodsClassifierEditor editor = new GoodsClassifierEditor(_context, userGuid);
                    var data = editor.CheckClassifier(model);

                    return ReturnData(data);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        [HttpPost]
        public ActionResult AddClassifier(GoodsClassifierEditorModelJson model, bool tryMode)
        {

            try
            {
                lock (LockObject)
                {

                    var userGuid = new Guid(User.Identity.GetUserId());
                    GoodsClassifierEditor editor = new GoodsClassifierEditor(_context, userGuid);
                    var data = editor.AddClassifier(model, tryMode);
                    return ReturnData(data);
                }
            }
            catch (Exception e)
            {
                return ReturnError(e);
            }
        }

        [HttpPost]
        public ActionResult GetChanges(GoodsClassifierEditorModelJson model)
        {
            try
            {
                lock (LockObject)
                {

                    var userGuid = new Guid(User.Identity.GetUserId());
                    GoodsClassifierEditor editor = new GoodsClassifierEditor(_context, userGuid);
                    var data = editor.GetChanges(model);

                    return ReturnData(data);
                }
            }
            catch (Exception e)
            {
                return ReturnError(e);
            }
        }

        [HttpPost]
        public ActionResult CheckRecreate(GoodsClassifierEditorModelJson model)
        {
            try
            {
                lock (LockObject)
                {

                    var userGuid = new Guid(User.Identity.GetUserId());
                    GoodsClassifierEditor editor = new GoodsClassifierEditor(_context, userGuid);

                    var data = editor.CheckRecreate(model);

                    return ReturnData(data);

                }
            }
            catch (Exception e)
            {
                return ReturnError(e);
            }
        }

        [HttpPost]
        public ActionResult ChangeClassifier(GoodsClassifierEditorModelJson model)
        {
            try
            {
                lock (LockObject)
                {
                    var userGuid = new Guid(User.Identity.GetUserId());
                    GoodsClassifierEditor editor = new GoodsClassifierEditor(_context, userGuid);
                    var data = editor.ChangeClassifier(model, true);
                    return ReturnData(data);
                }
            }
            catch (ApplicationException e)
            {
                return ReturnError(e);
            }
        }

        [HttpPost]
        public ActionResult ReCreateClassifier(GoodsClassifierEditorModelJson model)
        {
            try
            {
                lock (LockObject)
                {
                    var userGuid = new Guid(User.Identity.GetUserId());
                    GoodsClassifierEditor editor = new GoodsClassifierEditor(_context, userGuid);

                    var data = editor.ReCreateClassifier(model, true);

                    return ReturnData(data);
                }
            }
            catch (ApplicationException e)
            {
                return ReturnError(e);
            }
        }

        [HttpPost]
        public ActionResult MergeClassifier(GoodsClassifierEditorModelJson model)
        {
            try
            {
                lock (LockObject)
                {
                    var userGuid = new Guid(User.Identity.GetUserId());
                    GoodsClassifierEditor editor = new GoodsClassifierEditor(_context, userGuid);

                    var data = editor.MergeClassifier(model, true);

                    return ReturnData(data);
                }
            }
            catch (ApplicationException e)
            {
                return ReturnError(e);
            }
        }

        ///public ActionResult GetKey(long fieldId)
        ///{
        ///    var field = _context.Manufacturer.SingleOrDefault(m => m.Id == fieldId);
        ///    return ReturnData(field != null ? field.Id : null);
        ///}

        private ActionResult ReturnError(Exception e)
        {
            ResponseModel result = new ResponseModel();

            result.Success = false;
            result.Data = null;
            result.ErrorMessage = e.Message;

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            LogError(e);

            return jsonNetResult;
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

    }
}