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
        public ActionResult SaveSources(ICollection<Source> array)
        {
            try
            {
                foreach (var item in array)
                {
                    var source = _context.Source.FirstOrDefault(x => x.Id == item.Id);

                    if (source == null)
                        throw new Exception("Source not found");

                    source.IsPutEcomData = item.IsPutEcomData;
                    source.Priority = item.Priority;
                }
                _context.SaveChanges();

                return Json(true);
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
        public ActionResult AddSource(string name)
        {
            try
            {
                if (_context.Source.Any(x => x.Name == name))
                    throw new Exception("Источник с таким наименованием уже существует");

                var source = new Source()
                {
                    Name = name,
                    IsPutEcomData = false
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
        public ActionResult RemoveSource(long id)
        {
            try
            {
                var source = _context.Source.FirstOrDefault(s => s.Id == id);

                if (source == null)
                    throw new Exception("Source not found");

                var templates = _context.Template.Where(t => t.SourceId == id).ToList();

                for (var i = 0; i < templates.Count; i++)
                {
                    RemoveTemplate(templates[i]);
                }

                _context.Source.Remove(source);
                _context.SaveChanges();

                return Json(true);
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
        public ActionResult RenameTemplate(long id, string value)
        {
            try
            {
                var template = _context.Template.FirstOrDefault(s => s.Id == id);

                if (template == null)
                    throw new Exception("source not found");

                template.Name = value;
                _context.SaveChanges();

                return Json(true);
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
            try
            {
                var template = _context.Template.FirstOrDefault(t => t.Id == id);

                if (template == null)
                    throw new Exception("source not found");

                RemoveTemplate(template);
                _context.SaveChanges();
                return Json(true);
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
        public ActionResult SaveTemplate(List<TemplateField> templateFieldsJson, long templateId)
        {

            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var temlpate = _context.Template.FirstOrDefault(t => t.Id == templateId);

                    if (temlpate == null)
                        throw new Exception("template not found");

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

                    return Json(true);
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    string msg = e.Message;
                    while (e.InnerException != null)
                    {
                        e = e.InnerException;
                        msg += e.Message;
                    }
                    return BadRequest(msg);

                }
            }
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