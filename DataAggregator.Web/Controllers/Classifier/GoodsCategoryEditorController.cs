using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace DataAggregator.Web.Controllers.Classifier
{
    [Authorize(Roles = "SBoss")]
    public class GoodsCategoryEditorController : BaseController
    {
        [HttpPost]
        public ActionResult GetSections()
        {
            using (var context = new DrugClassifierContext(APP))
            {
                try
                {
                    return ReturnData(context.GoodsSection.ToList());
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }

        [HttpPost]
        public ActionResult RenameSection(long id, string value)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                try
                {
                    if (context.GoodsSection.Any(s => s.Id != id && s.Name.Equals(value)))
                    {
                        throw new ApplicationException("Такой раздел уже существует!");
                    }

                    GoodsSection section = context.GoodsSection.FirstOrDefault(s => s.Id == id);

                    if (section == null)
                        throw new ApplicationException("Раздел не найден!");

                    section.Name = value;
                    context.SaveChanges();

                    return ReturnData(null);
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }

        [HttpPost]
        public ActionResult AddSection()
        {
            using (var context = new DrugClassifierContext(APP))
            {
                try
                {
                    var section = new GoodsSection
                    {
                        Name = "Новый раздел"
                    };

                    context.GoodsSection.Add(section);
                    context.SaveChanges();

                    return ReturnData(section);
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }

        [HttpPost]
        public ActionResult RemoveSection(long id)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                try
                {
                    GoodsSection section = context.GoodsSection.FirstOrDefault(s => s.Id == id);

                    if (section == null)
                        throw new ApplicationException("Раздел не найден!");

                    if (section.GoodsCategory.Count > 0)
                        throw new ApplicationException("Удаление невозможно! Этот раздел уже используется в БД!");

                    context.GoodsSection.Remove(section);
                    context.SaveChanges();

                    return ReturnData(null);
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }
        
        [HttpPost]
        public ActionResult GetCategories(long id)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                try
                {
                    return
                        ReturnData(
                            context.GoodsCategory.Where(t => t.GoodsSectionId == id)
                                .Include(t => t.GoodsSection) //чтобы EF не ругался
                                .ToList());
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }

        [HttpPost]
        public ActionResult RenameCategory(long id, string value)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                try
                {
                    if (context.GoodsCategory.Any(s => s.Id!= id && s.Name.Equals(value)))
                    {
                        throw new ApplicationException("Такая категория уже существует!");
                    }

                    GoodsCategory category = context.GoodsCategory.FirstOrDefault(s => s.Id == id);

                    if (category == null)
                        throw new ApplicationException("Категория не найдена!");

                    category.Name = value;
                    context.SaveChanges();

                    return ReturnData(null);
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }

        [HttpPost]
        public ActionResult AddCategory(long sectionId)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                try
                {
                    var category = new GoodsCategory
                    {
                        Name = "Новая категория",
                        GoodsSectionId = sectionId
                    };

                    context.GoodsCategory.Add(category);
                    context.SaveChanges();

                    return ReturnData(category);
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }

        [HttpPost]
        public ActionResult RemoveCategory(long id)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                try
                {
                    GoodsCategory category = context.GoodsCategory.FirstOrDefault(s => s.Id == id);

                    if (category == null)
                        throw new ApplicationException("Категория не найдена в БД!");

                    if (category.GoodsClear.Count > 0 || category.GoodsCategoryKeywords.Count > 0)
                        throw new ApplicationException("Удаление невозможно! Эта категория уже используется в БД!");

                    context.GoodsCategory.Remove(category);
                    context.SaveChanges();

                    return ReturnData(null);
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }

        [HttpPost]
        public ActionResult ChangeSection(long categoryId, long newSectionId)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                try
                {
                    GoodsCategory category = context.GoodsCategory.FirstOrDefault(s => s.Id == categoryId);

                    if (category == null)
                        throw new ApplicationException("Категория не найдена в БД!");

                    category.GoodsSectionId = newSectionId;
                    context.SaveChanges();

                    return ReturnData(null);
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }

        [HttpPost]
        public ActionResult GetKeywords(long id)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                try
                {
                    return ReturnData(context.GoodsCategoryKeyword.Where(t => t.GoodsCategoryId == id).ToList());
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }

        [HttpPost]
        public ActionResult RenameKeyword(long id, string value)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                try
                {
                    GoodsCategoryKeyword keyword = context.GoodsCategoryKeyword.FirstOrDefault(s => s.Id == id);

                    if (
                        context.GoodsCategoryKeyword.Any(
                            s => s.Id != id && s.GoodsCategoryId == keyword.GoodsCategoryId && s.Name.Equals(value)))
                    {
                        throw new ApplicationException("Такое ключевое слово уже существует!");
                    }

                    if (keyword == null)
                        throw new ApplicationException("Ключевое слово не найдено в БД!");

                    keyword.Name = value;
                    context.SaveChanges();

                    return ReturnData(null);
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }

        [HttpPost]
        public ActionResult AddKeyword(long categoryId)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                try
                {
                    var keyword = new GoodsCategoryKeyword
                    {
                        Name = "Новое ключевое слово",
                        GoodsCategoryId = categoryId
                    };

                    context.GoodsCategoryKeyword.Add(keyword);
                    context.SaveChanges();

                    return ReturnData(keyword);
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }

        [HttpPost]
        public ActionResult RemoveKeyword(long id)
        {
            using (var context = new DrugClassifierContext(APP))
            {
                try
                {
                    GoodsCategoryKeyword keyword = context.GoodsCategoryKeyword.FirstOrDefault(t => t.Id == id);

                    if (keyword == null)
                        throw new ApplicationException("Ключевое слово не найдено в БД!");

                    context.GoodsCategoryKeyword.Remove(keyword);
                    context.SaveChanges();

                    return ReturnData(null);
                }
                catch (ApplicationException e)
                {
                    return BadRequest(e.Message);
                }
            }
        }

        private ActionResult ReturnError(ApplicationException e)
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