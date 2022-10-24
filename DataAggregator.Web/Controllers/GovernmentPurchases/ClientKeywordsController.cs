using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.GovernmentPurchases.Keywords;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    [Authorize(Roles = "GManager")]
    public class ClientKeywordsController : BaseController
    {
        private GovernmentPurchasesContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GovernmentPurchasesContext(APP);
        }

        ~ClientKeywordsController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult GetClientsList()
        {
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = _context.Client.OrderBy(c => c.Name).ToList()
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult GetClientKeywords(long clientId, bool onlyActive)
        {
            var result = _context.ClientKeywordView.Where(k => k.ClientId == clientId);
            
            if (onlyActive)
            {
                result = result.Where(k => k.DateEnd == null);
            }

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.OrderBy(k => k.KeywordText).ToList()
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult LoadKeywords()
        {
            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = _context.Keyword.Take(50000).ToList()
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult ClientAdd(string name)
        {
            try
            {
                if (String.IsNullOrEmpty(name))
                {
                    return GenerateServerAnswer("error", "Наименование не может быть пустым!");
                }
                else
                {
                    if (_context.Client.Count(c => c.Name.Equals(name)) > 0)
                    {
                        return GenerateServerAnswer("error", "Клиент с таким наименованием уже существует!");
                    }

                    var newClient = new Client();
                    newClient.Name = name;

                    _context.Client.Add(newClient);
                    _context.SaveChanges();

                    return GenerateServerAnswer("ok", null);
                }
            }
            catch (Exception e)
            {
                return GenerateServerAnswer("error", "Ошибка при сохранении! " + e.Message);
            }
        }

        [HttpPost]
        public ActionResult ClientEdit(string name, long clientId)
        {
            try
            {
                if (String.IsNullOrEmpty(name))
                {
                    return GenerateServerAnswer("error", "Наименование не может быть пустым!");
                }
                else
                {
                    if (_context.Client.Count(c => c.Name.Equals(name)) > 0)
                    {
                        return GenerateServerAnswer("error", "Клиент с таким наименованием уже существует!");
                    }

                    var dbClient = _context.Client.Single(c => c.Id == clientId);
                    if (dbClient == null)
                    {
                        return GenerateServerAnswer("error", "Клиент не найден! Возможно, его уже удалили из БД. Перезагрузите страницу.");
                    }
                    dbClient.Name = name;

                    _context.SaveChanges();
                    return GenerateServerAnswer("ok", null);
                }
            }
            catch (Exception e)
            {
                return GenerateServerAnswer("error", "Ошибка при сохранении! " + e.Message);
            }
        }

        [HttpPost]
        public ActionResult ClientDelete(long clientId)
        {
            try
            {
                var dbClient = _context.Client.Single(c => c.Id == clientId);
                if (dbClient == null)
                {
                    return GenerateServerAnswer("error", "Клиент не найден! Возможно, его уже удалили из БД. Перезагрузите страницу.");
                }

                var dbClientKeyword = _context.ClientKeyword.Where(ck => ck.ClientId == dbClient.Id);

                if (_context.ClientKeyword.Count(ck => ck.ClientId == dbClient.Id) == 0)
                {
                    _context.Client.Remove(dbClient);
                }
                else
                {
                    return GenerateServerAnswer("error", "Невозможно удалить клиента! Для него указаны ключевые слова.");
                }

                _context.SaveChanges();
                return GenerateServerAnswer("ok", null);
            }
            catch (Exception e)
            {
                return GenerateServerAnswer("error", "Ошибка при удалении! " + e.Message);
            }
        }

        [HttpPost]
        public ActionResult KeywordAdd(string keywordText, long clientId)
        {
            try
            {
                var dbKeyword = _context.Keyword.SingleOrDefault(k => k.Text.Equals(keywordText));

                if (dbKeyword == null)
                {
                    dbKeyword = new Keyword();
                    dbKeyword.Text = keywordText;
                    _context.Keyword.Add(dbKeyword);
                    _context.SaveChanges();
                }

                if (_context.ClientKeyword.Count(ck => ck.ClientId == clientId &&
                                                       ck.KeywordId == dbKeyword.Id &&
                                                       ck.DateEnd == null) != 0)
                {
                    return GenerateServerAnswer("error", "Такое значение уже есть в БД!");
                }

                var newClientKeyword = new ClientKeyword();
                newClientKeyword.ClientId = clientId;
                newClientKeyword.KeywordId = dbKeyword.Id;
                newClientKeyword.DateStart = DateTime.Now;
                newClientKeyword.StartUserId = new Guid(User.Identity.GetUserId());

                _context.ClientKeyword.Add(newClientKeyword);
                _context.SaveChanges();

                return GenerateServerAnswer("ok", _context.ClientKeywordView.Single(ckv => ckv.Id == newClientKeyword.Id));
            }
            catch (Exception e)
            {
                return GenerateServerAnswer("error", "Ошибка при сохранении! " + e.Message);
            }
        }
        
        [HttpPost]
        public ActionResult KeywordsDelete(List<long> keywordsIdList)
        {
            try
            {
                foreach (var keyWordId in keywordsIdList)
                {
                    var dbClientKeyword = _context.ClientKeyword.SingleOrDefault(ck => ck.Id == keyWordId);

                    if (dbClientKeyword == null)
                    {
                        return GenerateServerAnswer("error", "Запись Id=" + keyWordId + " не найдена в таблице ClientKeyword!");
                    }

                    if (dbClientKeyword.DateEnd == null)
                    {
                        dbClientKeyword.DateEnd = DateTime.Now;
                        dbClientKeyword.EndUserId = new Guid(User.Identity.GetUserId());
                    }
                }
                _context.SaveChanges();

                var changeList = new List<ClientKeywordView>();
                foreach (var keywordId in keywordsIdList)
                {
                    changeList.Add(_context.ClientKeywordView.Single(p => p.Id == keywordId));
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