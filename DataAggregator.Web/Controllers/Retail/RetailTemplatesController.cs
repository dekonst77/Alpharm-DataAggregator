using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Retail;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;


namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss")]
    public class RetailTemplatesController : BaseController
    {
        private readonly RetailContext _context;

        public RetailTemplatesController()
        {
            _context = new RetailContext(APP);
        }

        ~RetailTemplatesController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult GetAllSources()
        {
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = _context.Source.ToList()
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult GetTemplates(long id)
        {
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = _context.Template.Where(t => t.SourceId == id && t.IsActual).ToList()
            };

            return jsonNetResult;
        }



        [HttpPost]
        public ActionResult GetTemplate(long id)
        {
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = _context.TemplateField.Where(t => t.TemplateId == id && t.ParentId == null).ToList()
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult GetTemplateFieldName()
        {
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = _context.TemplateFieldName.ToList()
            };

            return jsonNetResult;
        }


        [HttpPost]
        public ActionResult RenameSource(long id, string value)
        {
            var source = _context.Source.FirstOrDefault(s => s.Id == id);

            if (source == null)
                throw new ApplicationException("source not found");

            if (source.FileInfo != null && source.FileInfo.Count > 0)
                throw new ApplicationException("This Source already using");

            source.Name = value;

            _context.SaveChanges();

            return Json(true);
        }

        [HttpPost]
        public ActionResult AddSource()
        {
            var source = new Source()
            {
                Name = "Новый источник"
            };

            _context.Source.Add(source);

            _context.SaveChanges();

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = source
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult RemoveSource(long id)
        {
            var source = _context.Source.FirstOrDefault(s => s.Id == id);

            if (source == null)
                throw new ApplicationException("Source not found");

            if (source.FileInfo != null && source.FileInfo.Count > 0)
                throw new ApplicationException("This Source already using");

            var templates = _context.Template.Where(t => t.SourceId == id).ToList();

            for (var i = 0; i < templates.Count; i++)
            {
                RemoveTemplate(templates[i]);
            }
            
            _context.Source.Remove(source);

            _context.SaveChanges();

            return Json(true);
        }

        [HttpPost]
        public ActionResult RenameTemplate(long id, string value)
        {
            var template = _context.Template.FirstOrDefault(s => s.Id == id);

            if (template == null)
                throw new ApplicationException("source not found");

            template.Name = value;

            _context.SaveChanges();

            return Json(true);
        }

        [HttpPost]
        public ActionResult AddTemplate(long sourceId)
        {
            var template = new Template()
            {
                Name = "Новый шаблон",
                SourceId = sourceId,
                IsActual = true
            };

            _context.Template.Add(template);

            _context.SaveChanges();

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = template
            };

            return jsonNetResult;
        }

        private void RemoveTemplate(Template template)
        {
            var filesLoadedUsingThisTemplate = _context.Database.SqlQuery<int>(
                "select count(*) from FileData where TemplateId = @templateId", new SqlParameter("@templateId", template.Id))
                .Single();
            if (filesLoadedUsingThisTemplate > 0)
            {
                template.IsActual = false;
            }
            else
            {
                var fields = _context.TemplateField.Where(f => f.TemplateId == template.Id);

                _context.TemplateField.RemoveRange(fields);

                _context.Template.Remove(template);
            }


        }


        [HttpPost]
        public ActionResult RemoveTemplate(long id)
        {
            var template = _context.Template.FirstOrDefault(t => t.Id == id);

            if (template == null)
                throw new ApplicationException("source not found");

            RemoveTemplate(template);
            _context.SaveChanges();
            return Json(true);

        }

        [HttpPost]
        public ActionResult SaveTemplate(List<TemplateField> templateFieldsJson, long templateId)
        {

            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {

                    var temlpate = _context.Template.FirstOrDefault(t => t.Id == templateId);

                    if(temlpate == null)
                        throw new ApplicationException("template not found");

                    var fields = _context.TemplateField.Where(f => f.TemplateId == templateId);

                    _context.TemplateField.RemoveRange(fields);

                    removeId(templateFieldsJson);

                    _context.TemplateField.AddRange(templateFieldsJson);

                    var maxNumberHeaderRows = 0;

                    foreach (var templateField in temlpate.TemplateField.ToList())
                    {
                        var levelsCount = templateField.GetLevelsCount();

                        if (levelsCount > maxNumberHeaderRows)
                        {
                            maxNumberHeaderRows = levelsCount;
                        }
                    }

                    temlpate.NumberHeaderRows = maxNumberHeaderRows;

                    _context.SaveChanges();

                    dbContextTransaction.Commit();
                }
                catch //(Exception e)
                {
                    dbContextTransaction.Rollback();
                }
            }

           
            return Json(true);



        }

        private void removeId(List<TemplateField> templateFieldsJson)
        {
           

            foreach (var templateField in templateFieldsJson)
            {
                templateField.Id = 0;
                templateField.ParentId = null;

                if (templateField.Childs != null)
                {
                    templateField.FieldName = null;
                    removeId(templateField.Childs.ToList());
                }
            }
           
        }
    }
}