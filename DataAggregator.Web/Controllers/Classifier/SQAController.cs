using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Web.Models.Classifier;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.View;

namespace DataAggregator.Web.Controllers.Classifier
{

    [Authorize(Roles = "SBoss")]
    public class SQAController : BaseController
    {
        private DrugClassifierContext _context;
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new DrugClassifierContext(APP);
        }

        [HttpPost]
        public ActionResult Load()
        {

            _context.ClassifierUpdateSQA();
            var atc = _context.SQAView.ToList();


            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = atc
            };
        }

        [HttpPost]
        public ActionResult Change(int Id, bool value)
        {

            dynamic result = new ExpandoObject();
            result.Success = true;

            try
            {
                SQA sqa = _context.SQA.FirstOrDefault(s => s.Id == Id);

                if (sqa == null)
                    throw new ApplicationException("");

                sqa.IsSQA = value;
                _context.SaveChanges();

                result.Success = true;
            }
            catch (Exception e)
            {
                LogError(e);
                result.Message = e.Message;
                result.Success = false;
            }

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };
        }
        
       
        //Удаляем 
        [HttpPost]
        public ActionResult Delete(AtcGroupModel value)
        {
            dynamic result = new ExpandoObject();

            try
            {
             

                _context.SaveChanges();
                result.Success = true;
            }
            catch (Exception e)
            {
                LogError(e);
                result.Message = "Нельзя удалить выбранный элемент";
                result.Success = false;
            }

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };
        }

     
    }
}