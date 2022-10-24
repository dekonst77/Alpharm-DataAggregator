using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.GovernmentPurchases.Search;
using DataAggregator.Web.Models.GovernmentPurchases.DistributionKeyWords;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    [Authorize(Roles = "GManager")]
    public class DistributionKeyWordsController : BaseController
    {
        private GovernmentPurchasesContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GovernmentPurchasesContext(APP);
        }

        ~DistributionKeyWordsController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult GetKeyWordsList(bool onlyActive)
        {
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = onlyActive ? _context.PurchaseClassAutoListView.Where(p => p.DateEnd == null).Take(50000).ToList() : 
                                    _context.PurchaseClassAutoListView.Take(50000).ToList()
            };

            return jsonNetResult;
        }
        
        [HttpPost]
        public ActionResult GetlistTypesList()
        {
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = _context.ListType.OrderBy(l => l.Value).Select(l => l.Value).ToList()
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult AddKeyWord(PurchaseClassAutoListJson keyWord, List<PurchaseClassAutoListView> changeList, bool recheck)
        {
            try
            {
                var dbPCALs = _context.PurchaseClassAutoList.Where(p => p.Value.Equals(keyWord.Value) && 
                                                              p.DateEnd == null);
                if (dbPCALs.Count() != 0)
                {
                    return GenerateServerAnswer("error", "Такое значение уже есть в списке \"" + dbPCALs.ToList()[0].ListType.Value + "\"!");
                }

                var newPCAL = new PurchaseClassAutoList();
                newPCAL.Value = keyWord.Value;
                newPCAL.ListType = _context.ListType.Single(l => l.Value.Equals(keyWord.ListTypeValue));
                newPCAL.DateStart = DateTime.Now;
                newPCAL.StartUserId = new Guid(User.Identity.GetUserId());
                newPCAL.Recheck = recheck;
            
                _context.PurchaseClassAutoList.Add(newPCAL);
                _context.SaveChanges();

                if (changeList == null)
                {
                    changeList = new List<PurchaseClassAutoListView>();
                }

                changeList.Add(_context.PurchaseClassAutoListView.Single(p => p.Id == newPCAL.Id));

                return GenerateServerAnswer("ok", changeList);
            }
            catch (Exception e)
            {
                return GenerateServerAnswer("error", "Ошибка при сохранении! " + e.Message);
            }
        }

        [HttpPost]
        public ActionResult EditKeyWord(PurchaseClassAutoListJson keyWord, bool recheck)
        {
            try
            {
                var dbPCALs = _context.PurchaseClassAutoList.Where(p => p.Value.Equals(keyWord.Value) &&
                                                                   p.DateEnd == null);

                if (dbPCALs.Count() != 0)
                {
                    var firstDbWord = dbPCALs.ToList()[0];

                    if (firstDbWord.ListType.Value == keyWord.ListTypeValue)
                    {
                        return GenerateServerAnswer("error", "Такая запись уже существует!");
                    }
                    else
                    {
                        if (firstDbWord.Id != keyWord.Id) //если Id равны, то это у записи изменили тип списка
                        {
                            GenerateServerAnswer("error", "Такое значение уже есть в списке \"" + dbPCALs.ToList()[0].ListType.Value + "\"!");    
                        }
                    }
                }

                //список аннулированных и новых записей
                var changeList = new List<PurchaseClassAutoListView>();

                //аннулируем старую запись и создаем новую
                var dbPCAL = _context.PurchaseClassAutoList.SingleOrDefault(p => p.Id == keyWord.Id);
                dbPCAL.DateEnd = DateTime.Now;
                dbPCAL.EndUserId = new Guid(User.Identity.GetUserId());

                _context.SaveChanges();

                changeList.Add(_context.PurchaseClassAutoListView.Single(p => p.Id == keyWord.Id));

                var newPCAL = new PurchaseClassAutoListJson();
                newPCAL.Value = keyWord.Value;
                newPCAL.ListTypeValue = keyWord.ListTypeValue;
                return AddKeyWord(newPCAL, changeList, recheck);
            }
            catch (Exception e)
            {
                return GenerateServerAnswer("error", "Ошибка при редактировании! " + e.Message);
            }
        }

        [HttpPost]
        public ActionResult DeleteKeyWords(List<PurchaseClassAutoListJson> keyWordsList)
        {
            try
            {
                foreach (var keyWord in keyWordsList)
                {
                    var dbPCAL = _context.PurchaseClassAutoList.SingleOrDefault(p => p.Id == keyWord.Id);

                    if (dbPCAL == null)
                    {
                        return GenerateServerAnswer("error", "Запись " + keyWord.Value + "(" + keyWord.ListTypeValue + ") не найдена в БД!");
                    }

                    if (dbPCAL.DateEnd == null)
                    {
                        dbPCAL.DateEnd = DateTime.Now;
                        dbPCAL.EndUserId = new Guid(User.Identity.GetUserId());
                    }
                }
                _context.SaveChanges();

                var changeList = new List<PurchaseClassAutoListView>();
                foreach (var keyWord in keyWordsList)
                {
                    changeList.Add(_context.PurchaseClassAutoListView.Single(p => p.Id == keyWord.Id));
                }

                return GenerateServerAnswer("ok", changeList);
            }
            catch (Exception e)
            {
                return GenerateServerAnswer("error", "Ошибка при редактировании! " + e.Message);
            }
        }

        private JsonNetResult GenerateServerAnswer(string status, object data)
        {
            var result = new Dictionary<string, object>();
            result.Add("Data", data);
            result.Add("Status", status);

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }
    }
}