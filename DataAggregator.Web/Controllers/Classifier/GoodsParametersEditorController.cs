using DataAggregator.Domain.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier;
using DataAggregator.Web.Models.GoodsClassifier;
using Newtonsoft.Json;

namespace DataAggregator.Web.Controllers.Classifier
{
    [Authorize(Roles = "SBoss")]
    public class GoodsParametersEditorController : BaseController
    {
        [HttpPost]
        public ActionResult GetGoodsCategoryList()
        {
            using (var context = new DrugClassifierContext(APP))
            {
                try
                {
                    return ReturnData(context.GoodsCategory.Include(t => t.GoodsSection)
                        .OrderBy(c => c.GoodsSection.Name).ThenBy(c => c.Name).ToList());
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }

        [HttpPost]
        public ActionResult GetParameterGroups(long? goodsCategoryId)
        {
            if (goodsCategoryId != null)
            {
                using (var context = new DrugClassifierContext(APP))
                {
                    try
                    {
                        var parameterGroupList =
                           context.ParameterGroup.Where(g => g.GoodsCategoryId == goodsCategoryId)
                               .OrderBy(p => p.Name)
                               .ToList();


                        return ReturnData(parameterGroupList);
                    }
                    catch (ApplicationException e)
                    {
                        return BadRequest(e.Message);
                    }
                }
            }

            return Json(new List<ParameterJson>(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetParameters(long? parentId, int level)
        {
            if (parentId != null)
            {
                using (var context = new DrugClassifierContext(APP))
                {
                    try
                    {
                        switch (level)
                        {
                            case 1:
                                return ReturnData(context.Parameter.Where(p => p.ParameterGroupId == parentId && p.ParentId == null).ToList());
                                //break;
                            case 2:
                                return ReturnData(context.Parameter.Where(p => p.ParentId == parentId).ToList());
                               // break;
                        }
                    }
                    catch (ApplicationException e)
                    {
                        return BadRequest(e.Message);
                    }
                }
            }

            return Json(new List<ParameterJson>(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RenameParameterGroup(long id, string newValue)
        {
            if (id > 0 && !String.IsNullOrEmpty(newValue))
            {
                using (var context = new DrugClassifierContext(APP))
                {
                    try
                    {
                        var parameterGroupDb = context.ParameterGroup.FirstOrDefault(pg => pg.Id == id);

                        if (parameterGroupDb == null)
                        {
                            throw new ApplicationException("В БД не найден ParameterGroup с Id = " + id);
                        }

                        parameterGroupDb.Name = newValue;
                        context.SaveChanges();
                    }
                    catch (ApplicationException e)
                    {
                        return BadRequest(e.Message);
                    }
                }
            }

            return ReturnData(null);
        }

        [HttpPost]
        public ActionResult RenameParameter(long id, string newValue)
        {
            if (id > 0 && !String.IsNullOrEmpty(newValue))
            {
                using (var context = new DrugClassifierContext(APP))
                {
                    try
                    {
                        var parameterDb = context.Parameter.FirstOrDefault(p => p.Id == id);

                        if (parameterDb == null)
                        {
                            throw new ApplicationException("В БД не найден Parameter с Id = " + id);
                        }

                        parameterDb.Value = newValue;
                        context.SaveChanges();
                    }
                    catch (ApplicationException e)
                    {
                        return BadRequest(e.Message);
                    }
                }
            }

            return ReturnData(null);
        }

        [HttpPost]
        public ActionResult AddParameterGroup(string newValue, long goodsCategoryId)
        {
            if (goodsCategoryId > 0 && !String.IsNullOrEmpty(newValue))
            {
                using (var context = new DrugClassifierContext(APP))
                {
                    try
                    {
                        var parameterGroup = context.ParameterGroup.FirstOrDefault(pg => pg.Name.Equals(newValue));

                        if (parameterGroup != null)
                        {
                            return BadRequest("Значение \"" + newValue + "\"уже существует!");
                        }

                        parameterGroup = new ParameterGroup() { Name = newValue, GoodsCategoryId = goodsCategoryId };
                        context.ParameterGroup.Add(parameterGroup);
                        context.SaveChanges();

                        return ReturnData(parameterGroup);
                    }
                    catch (ApplicationException e)
                    {
                        return BadRequest(e.Message);
                    }
                }
            }

            return BadRequest("AddParameterGroup: неверные входные параметры");
        }

        [HttpPost]
        public ActionResult AddParameter(string newValue, long parameterGroupId, long? parentId)
        {
            if (!String.IsNullOrEmpty(newValue) && parameterGroupId > 0 && (parentId == null || parentId > 0))
            {
                using (var context = new DrugClassifierContext(APP))
                {
                    try
                    {
                        if (
                            context.Parameter.Any(
                                p =>
                                    p.ParameterGroupId == parameterGroupId && p.ParentId == parentId &&
                                    p.Value.Equals(newValue)))
                        {
                            return BadRequest("Значение \"" + newValue + "\"уже существует в БД!");
                        }

                        var parameter = new Parameter()
                        {
                            ParameterGroupId = parameterGroupId,
                            ParentId = parentId,
                            Value = newValue
                        };

                        context.Parameter.Add(parameter);
                        context.SaveChanges();

                        return ReturnData(parameter);
                    }
                    catch (ApplicationException e)
                    {
                        return BadRequest(e.Message);
                    }
                }
            }

            return BadRequest("AddParameter: неверные входные параметры");
        }

        [HttpPost]
        public ActionResult RemoveParameterGroup(long parameterGroupId)
        {


            using (var context = new DrugClassifierContext(APP))
            {

                //Открываем транзакцию
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var parameterGroupDb = context.ParameterGroup.FirstOrDefault(pg => pg.Id == parameterGroupId);

                        if (parameterGroupDb == null)
                        {
                            return BadRequest("В БД не найден ParameterGroup с Id = " + parameterGroupId);
                        }

                        var childParameters =
                            context.Parameter.Where(p => p.ParameterGroupId == parameterGroupId && p.ParentId == null)
                                .ToList();

                        foreach (var childParameter in childParameters)
                        {
                            RemoveParameter_InContext(childParameter.Id, context);
                        }


                        parameterGroupDb = context.ParameterGroup.First(pg => pg.Id == parameterGroupId);
                        context.ParameterGroup.Remove(parameterGroupDb);
                        context.SaveChanges();


                        transaction.Commit();
                    }
                    catch (ApplicationException e)
                    {

                        transaction.Rollback();
                        return BadRequest(e.Message);
                    }
                }
            }
            return null;
        }


        [HttpPost]
        public ActionResult RemoveParameter(long parameterId)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                //Открываем транзакцию
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        RemoveParameter_InContext(parameterId, context);
                        transaction.Commit();
                    }
                    catch (ApplicationException e)
                    {
                        transaction.Rollback();
                        return BadRequest(e.Message);
                    }
                }
            }
            return null;
        }

        //отдельный приватный метод чтобы можно было использовать тот же контекст при удалении ParameterGroup
        private void RemoveParameter_InContext(long parameterId, DrugClassifierContext context)
        {
            var parameterDb = context.Parameter.FirstOrDefault(p => p.Id == parameterId);

            if (parameterDb == null)
            {
                throw new Exception("В БД не найден Parameter с Id = " + parameterId);
            }

            var childParameters = context.Parameter.Where(p => p.ParentId == parameterId).ToList();

            foreach (var childParameter in childParameters)
            {
                RemoveParameter_InContext(childParameter.Id, context);
            }

            var gpipList =
                context.GoodsProductionInfoParameter.Where(g => g.ParameterId == parameterId).ToList();
            foreach (var gpip in gpipList)
            {
                context.GoodsProductionInfoParameter.Remove(gpip);
            }
            context.SaveChanges();

            context.Parameter.Remove(parameterDb);
            context.SaveChanges();
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