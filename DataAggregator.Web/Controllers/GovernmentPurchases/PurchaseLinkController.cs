using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.GovernmentPurchases;
using DataAggregator.Domain.Model.GovernmentPurchases.Search;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ContractLink = DataAggregator.Domain.Model.GovernmentPurchases.Search.ContractLink;


namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    [Authorize(Roles = "GManager, GOperator")]
    public class PurchaseLinkController : BaseController
    {
        private GovernmentPurchasesContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GovernmentPurchasesContext(APP);
        }

        ~PurchaseLinkController()
        {
            _context.Dispose();
        }

        [HttpPost]
        public ActionResult GetLinks(string objectType)
        {
            _context.Database.ExecuteSqlCommand("exec [GovernmentPurchasesLoader].[search].[SyncEnd]");
            if (objectType.Equals("Contracts"))
                return GenerateServerAnswer("ok", _context.ContractLinkView.Take(50000).ToList());
            return GenerateServerAnswer("ok", _context.PurchaseLinkView.Take(50000).ToList());
        }

        [HttpPost]
        public ActionResult GetLawTypes()
        {
            return GenerateServerAnswer("ok", _context.LawType.OrderBy(l => l.Name).ToList());
        }
        
        [HttpPost]
        public ActionResult AddContractLink(string purchaseNumber, Byte lawTypeId, string reestrNumber)
        {
            if (_context.ContractLink.Any(c => c.PurchaseNumber == purchaseNumber && c.ReestrNumber == reestrNumber))
                return GenerateServerAnswer("error",
                    "Запись с номером закупки " + purchaseNumber + " и номером контракта " + reestrNumber +
                    " уже существует!");

            var newCL = new ContractLink();
            newCL.PurchaseNumber = purchaseNumber;
            newCL.LawTypeId = lawTypeId;
            newCL.ReestrNumber = reestrNumber;
            newCL.AddDate = DateTime.Now;
            newCL.UserGuid = new Guid(User.Identity.GetUserId());

            _context.ContractLink.Add(newCL);
            _context.SaveChanges();

            return GenerateServerAnswer("ok", _context.ContractLinkView.FirstOrDefault(c => c.Id == newCL.Id));
        }

        [HttpPost]
        public ActionResult AddPurchaseLink(string purchaseNumber, Byte lawTypeId, string purchaseUrl)
        {
            if (_context.PurchaseLink.Any(p => p.PurchaseNumber.Equals(purchaseNumber)) ||
                _context.SelectionPurchaseLink.Any(s => s.Number.Equals(purchaseNumber)))
                return GenerateServerAnswer("error", "Запись с номером закупки " + purchaseNumber + " уже существует!");


            var newPL = new PurchaseLink();
            newPL.PurchaseNumber = purchaseNumber;
            newPL.LawTypeId = lawTypeId;
            newPL.PurchaseUrl = purchaseUrl;
            newPL.AddDate = DateTime.Now;
            newPL.UserGuid = new Guid(User.Identity.GetUserId());

            _context.PurchaseLink.Add(newPL);
            _context.SaveChanges();

            return GenerateServerAnswer("ok", _context.PurchaseLinkView.FirstOrDefault(c => c.Id == newPL.Id));
        }

        [HttpPost]
        public ActionResult UploadFromExcel(string objectType, HttpPostedFileBase file)
        {
            string result="";
            if (objectType.Equals("Contracts"))
                result = UploadContractsFromExcel(file);
            else
                result = UploadPurchasesFromExcel(file);

            if (result.Equals("ok"))
                return GenerateServerAnswer("ok", null);
            return GenerateServerAnswer("error", result);
        }

        private string UploadContractsFromExcel(HttpPostedFileBase file)
        {
            var result = new StringBuilder();
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            using (var xlsx = new ExcelPackage(file.InputStream))
            {
                var sheet = xlsx.Workbook.Worksheets["contract"];

                if (
                    !(sheet.Cells[1, 1].Text.Equals("PurchaseNumber") && sheet.Cells[1, 2].Text.Equals("LawTypeName") &&
                      sheet.Cells[1, 3].Text.Equals("ReestrNumber")))
                    return "Некорректно заполнены заголовки таблицы. ";

                bool rowIsCorrect;
                var userGuid = new Guid(User.Identity.GetUserId());

                for (var i = 2; i <= sheet.Dimension.End.Row; i++)
                {
                    rowIsCorrect = true;

                    string rowReestrNumber = sheet.Cells[i, 3].GetValue<string>().Trim();
                    if (string.IsNullOrEmpty(rowReestrNumber))
                    {
                        result.Append("В строчке " + i + " не заполнен ReestrNumber. ");
                        rowIsCorrect = false;
                    }

                    string rowPurchaseNumber = sheet.Cells[i, 1].GetValue<string>();
                    if (rowPurchaseNumber != null)
                        rowPurchaseNumber = rowPurchaseNumber.Trim();
                    else
                        rowPurchaseNumber = rowReestrNumber + "C";


                    if (string.IsNullOrEmpty(rowPurchaseNumber))
                    {
                        result.Append("В строчке " + i + " не заполнен PurchaseNumber. ");
                        rowIsCorrect = false;
                    }

                    string rowLawTypeName = sheet.Cells[i, 2].GetValue<string>().Trim();
                    var rowLawType = new LawType();
                    if (string.IsNullOrEmpty(rowLawTypeName))
                    {
                        result.Append("В строчке " + i + " не заполнен LawTypeNam. ");
                        rowIsCorrect = false;
                    }
                    else
                    {
                        rowLawType = _context.LawType.SingleOrDefault(l => l.Name.Equals(rowLawTypeName));
                        if (rowLawType == null)
                        {
                            result.Append("В БД отсутствует Тип ФЗ, указанный в строчке " + i + ". ");
                            rowIsCorrect = false;
                        }
                    }

                    if (rowIsCorrect && 
                        !_context.ContractLink.Any(c => c.PurchaseNumber.Equals(rowPurchaseNumber) && c.ReestrNumber.Equals(rowReestrNumber))// &&
                        //!_context.SelectionContractLink.Any(s => s.PurchaseNumber.Equals(rowPurchaseNumber) && s.ReestrNumber.Equals(rowReestrNumber))
                        )
                        _context.ContractLink.Add(new ContractLink
                        {
                            PurchaseNumber = rowPurchaseNumber,
                            LawTypeId = rowLawType.Id,
                            ReestrNumber = rowReestrNumber,
                            AddDate = DateTime.Now,
                            UserGuid = userGuid
                        });

                    _context.SaveChanges();
                }
            }

            return result.Length == 0 ? "ok" : result.ToString();
        }

        private string UploadPurchasesFromExcel(HttpPostedFileBase file)
        {
            var result = new StringBuilder();
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            using (var xlsx = new ExcelPackage(file.InputStream))
            {
                var sheet = xlsx.Workbook.Worksheets[0];

                if (
                    !(sheet.Cells[1, 1].Text.Equals("PurchaseNumber") && sheet.Cells[1, 2].Text.Equals("LawTypeName") &&
                      sheet.Cells[1, 3].Text.Equals("PurchaseUrl")))
                    return "Некорректно заполнены заголовки таблицы. ";

                bool rowIsCorrect;
                var userGuid = new Guid(User.Identity.GetUserId());

                for (var i = 2; i <= sheet.Dimension.End.Row; i++)
                {
                    rowIsCorrect = true;

                    var rowPurchaseNumber = sheet.Cells[i, 1].GetValue<string>().Trim();
                    if (string.IsNullOrEmpty(rowPurchaseNumber))
                    {
                        result.Append("В строчке " + i + " не заполнен PurchaseNumber. ");
                        rowIsCorrect = false;
                    }

                    var rowLawTypeName = sheet.Cells[i, 2].GetValue<string>().Trim();
                    var rowLawType = new LawType();
                    if (string.IsNullOrEmpty(rowLawTypeName))
                    {
                        result.Append("В строчке " + i + " не заполнен LawTypeName. ");
                        rowIsCorrect = false;
                    }
                    else
                    {
                        rowLawType = _context.LawType.SingleOrDefault(l => l.Name.Equals(rowLawTypeName));
                        if (rowLawType == null)
                        {
                            result.Append("В БД отсутствует Тип ФЗ, указанный в строчке " + i + ". ");
                            rowIsCorrect = false;
                        }
                    }

                    var rowPurchaseUrl = sheet.Cells[i, 3].GetValue<string>().Trim();
                    if (string.IsNullOrEmpty(rowPurchaseUrl))
                    {
                        result.Append("В строчке " + i + " не заполнен PurchaseUrl. ");
                        rowIsCorrect = false;
                    }
                    else if (!rowPurchaseUrl.StartsWith("https://zakupki.gov.ru/"))
                    {
                        result.Append("В строчке " + i + " PurchaseUrl не содержит адрес сайта госзакупок (https://zakupki.gov.ru/). ");
                        rowIsCorrect = false;
                    }

                    if (rowIsCorrect && !_context.PurchaseLink.Any(p => p.PurchaseNumber.Equals(rowPurchaseNumber)) //&&
                        //!_context.SelectionPurchaseLink.Any(s => s.Number.Equals(rowPurchaseNumber))
                        )
                        _context.PurchaseLink.Add(new PurchaseLink
                        {
                            PurchaseNumber = rowPurchaseNumber,
                            LawTypeId = rowLawType.Id,
                            PurchaseUrl = rowPurchaseUrl,
                            AddDate = DateTime.Now,
                            UserGuid = userGuid
                        });

                    _context.SaveChanges();
                }
            }

            return result.Length == 0 ? "ok" : result.ToString();
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