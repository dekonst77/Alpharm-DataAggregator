using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DrugClassifier.SearchTerms;
using DataAggregator.Web.Models.Systematization;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace DataAggregator.Web.Controllers.Systematization
{
    public class SearchTermsController : BaseController
    {
        private DrugClassifierContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new DrugClassifierContext(APP);
        }

        ~SearchTermsController()
        {
            _context.Dispose();
        }

         [HttpPost]
         public JsonResult SetSynonym(SynonymJson synonymJson)
         {

             var userGuid = new Guid(User.Identity.GetUserId());

             SynonymJson synonym = synonymJson;

             LogSynonym log = new LogSynonym()
             {
                 DrugClearId = synonym.DrugClearId,
                 RecordId = synonym.OriginalId,
                 TableName = synonym.SynTableName,
                 UserId = userGuid
             };

             _context.LogSynonym.Add(log);
             
             dynamic synomymEntity = null;

             switch (synonym.SynTableName)
             {
                 case "SynINNGroup":
                     synomymEntity = _context.getSyn<IEnumerable<SynINNGroup>, SynINNGroup>(_context.SynINNGroup, synonym.Value, synonym.OriginalId);
                     break;
                 case "SynFormProduct":
                     synomymEntity = _context.getSyn<IEnumerable<SynFormProduct>, SynFormProduct>(_context.SynFormProduct, synonym.Value, synonym.OriginalId);
                     break;
                 case "SynTradeName":
                     synomymEntity = _context.getSyn<IEnumerable<SynTradeName>, SynTradeName>(_context.SynTradeName, synonym.Value, synonym.OriginalId);
                     break;
                 case "SynDosageGroup":
                     synomymEntity = _context.getSyn<IEnumerable<SynDosageGroup>, SynDosageGroup>(_context.SynDosageGroup, synonym.Value, synonym.OriginalId);
                     break;
             }

             if (synomymEntity != null) 
                 synomymEntity.Count++;
             else
                 throw new ApplicationException(string.Format("{0} - неизвестная таблица синонимов", synonym.SynTableName));

             _context.SaveChanges();

             return Json(true);
         }


    }

    //public class SearchSyn<TL, T>
    //    where TL : IEnumerable<AbstractSynonym>
    //                                where T : AbstractSynonym, new()
    //{
    //    public T getSyn(TL list, string value, long OriginalId)
    //    {
    //        var result = list.FirstOrDefault(l => l.OriginalId == OriginalId && l.Value.Equals(value));

    //        if (result != null)
    //            return (T)result;

    //        return new T()
    //        {
    //            Value = value,
    //            OriginalId = OriginalId,
    //            Count = 1
    //        };
    //    }
    //}
}