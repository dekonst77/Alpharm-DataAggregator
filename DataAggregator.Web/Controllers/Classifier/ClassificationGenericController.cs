using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Web.Models.Classifier;
using Kendo.Mvc.Extensions;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace DataAggregator.Web.Controllers.Classifier
{

    [Authorize(Roles = "GManager")]
    public class ClassificationGenericController : BaseController
    {
        private DrugClassifierContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new DrugClassifierContext(APP);
        }

        ~ClassificationGenericController()
        {
            _context.Dispose();
        }

        /// <summary>
        /// Получить данные
        /// </summary>
        [HttpPost]
        public ActionResult LoadData()
        {
            
            try
            {
                //var data = _context.ClassificationGeneric.Select(c => new 
                //    {
                //        InnGroup = c.INNGroup.Description,
                //        User = c.User.FullName,
                //        OwnerTradeMark = c.OwnerTradeMark.Value,
                //        TradeName = c.TradeName.Value
                //    }).ToList();
                List<ClassificationGenericModel> models = _context.ClassificationGeneric.Select(c => new ClassificationGenericModel()
                {
                    TradeName =  c.TradeName.Value,
                    OwnerTradeMark = c.OwnerTradeMark.Value,
                    InnGroup = c.INNGroup.Description,
                    Id = c.Id,
                    User = c.User != null ? c.User.FullName:string.Empty,
                    DateEdit = c.DateEdit,
                    Generic = c.Generic != null ? c.Generic.Value:String.Empty
                }).ToList();
                    
                //var models = data.Select(c => ClassificationGenericModel.Create(c)).ToList();
                return new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = models
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
          
           
        }
        
        /// <summary>
        /// Загрузка справочника 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> LoadGeneric()
        {
            List<Generic> result = await _context.Generic.ToListAsync();

            var jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };
            return jsonNetResult;

        }
        
        //Устанавливает выбранным элементам класс
        [HttpPost]
        public ActionResult SetGeneric(List<long> ids, long genericId)
        {
            var userGuid = new Guid(User.Identity.GetUserId());

            var user = _context.User.FirstOrDefault(u => u.Id == userGuid.ToString());

            if (user == null)
                throw new ApplicationException("user not found");

            DateTime dateUpdate = DateTime.Now;

            using (var context = new DrugClassifierContext(APP))
            {
                var editList = context.ClassificationGeneric.Where(g => ids.Contains(g.Id)).ToList();
                editList.ForEach(e =>
                {
                    e.GenericId = genericId;
                    e.UserId = userGuid.ToString();
                    e.DateEdit = dateUpdate;
                });
                context.SaveChanges();
            }

            var jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new { UserFullName = user.FullName, Success = true, DateUpdate = dateUpdate }
            };

            return jsonNetResult;

        }
    }

}