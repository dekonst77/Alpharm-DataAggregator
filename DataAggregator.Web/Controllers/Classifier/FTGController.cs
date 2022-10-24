using DataAggregator.Core.Models.Classifier;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Classifier
{

    [Authorize(Roles = "SBoss")]
    public class FTGController : BaseController
    {

        private DrugClassifierContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new DrugClassifierContext(APP);
        }

        //Загрузка все фтг
        [HttpPost]
        public ActionResult Load(ClassifierEditorModelJson model)
        {
            var ftg = _context.FTG.ToList();

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = ftg
            };

            return jsonNetResult;
        }

        //Изменение ФТГ
        [HttpPost]
        public ActionResult Change(FTG value)
        {
            dynamic result = new ExpandoObject();

            //Проверяем что ФТГ уже заведен
            if (value.Id == 0)
            {

                result.Message = "Такое ФТГ еще не существует";
                result.Success = false;
                result.Ftg = null;

                return new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = result
                };
            }

            //Проверяем что такой ФТГ нет
            if (_context.FTG.Any(f => string.Equals(f.Value, value.Value) && f.Id != value.Id))
            {
                result.Message = "Такое значение уже существует в справочнике ФТГ";
                result.Success = false;
                result.Ftg = null;

                return new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = result
                };
    
            }

            var ftg = _context.FTG.Single(f => f.Id == value.Id);
            ftg.Value = value.Value;

            _context.SaveChanges();

            result.Message = string.Empty;
            result.Success = true;
            result.Ftg = ftg;

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }

        //Добавление нового ФТГ
        [HttpPost]
        public ActionResult Add(string value)
        {

            var ftg = new FTG() { Value = value };
            dynamic result = new ExpandoObject();


            if (string.IsNullOrEmpty(value))
            {
                result.Message = "Не задано название ФТГ";
                result.Success = false;
                result.Ftg = ftg;


                return new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = result
                };
            }


            if (_context.FTG.Any(f => string.Equals(f.Value, value)))
            {
                result.Message = "Такое значение уже существует в справочнике ФТГ";
                result.Success = false;
                result.Ftg = null;

                return new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = result
                };
            }


            _context.FTG.Add(ftg);
            _context.SaveChanges();

            result.Message = string.Empty;
            result.Success = true;
            result.Ftg = ftg;




            return new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = result
                };

        }


        //Изменение ФТГ
        [HttpPost]
        public ActionResult Delete(FTG value)
        {
            dynamic result = new ExpandoObject();

            //Проверяем что ФТГ уже заведен
            if (value.Id == 0)
            {

                result.Message = "Такое ФТГ еще не существует";
                result.Success = false;
                result.Ftg = null;

                return new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = result
                };
            }
         
                try
                {
                    var ftg = _context.FTG.Single(f => f.Id == value.Id);
                    _context.FTG.Remove(ftg);
                    _context.SaveChanges();
                    result.Message = string.Empty;
                    result.Success = true;
                    result.Ftg = null;

                }
                catch (Exception e)
                {
                    result.Message = "Данный ФТГ нельзя удалить, так как он используется" + e.Message;
                    result.Success = false;
                    result.Ftg = null;
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