using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.Classifier;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;


namespace DataAggregator.Web.Controllers.Classifier
{
    [Authorize(Roles = "SBoss")]
    public class DataChangeExcelController : BaseController
    {
        [HttpPost]
        public FileResult ATCWhoLinkMKB_Update_Get()
        {
            var _context = new DrugClassifierContext(APP);

            _context.Database.CommandTimeout = 0;
            var Mkblink = _context.ATCWhoLinkMKBView.Select(s => new { s.ATCWho_Value, s.ATCWho_Description,s.mkb_code,s.mkb_name }).OrderBy(o => o.ATCWho_Value).ThenBy(o2=>o2.mkb_code).ToList();
            var Mkb = _context.MKB.Select(s => new { s.mkb_code, s.mkb_name }).OrderBy(o => o.mkb_code).ToList();


            Excel.Excel excel = new Excel.Excel();
            excel.Create();

            excel.InsertDataTable("ATC-mkb", 1, 1, Mkblink, true, true, null);
            excel.InsertDataTable("Mkb", 1, 1, Mkb, true, true, null);

            byte[] bb = excel.SaveAsByte();
            return File(bb, "application/vnd.ms-excel", "ATCWhoLinkMKB_Update.xlsx");
        }

        [HttpPost]
        public ActionResult ATCWhoLinkMKB_Update_Save(IEnumerable<System.Web.HttpPostedFileBase> uploads)
        {
            if (uploads == null || !uploads.Any())
                throw new ApplicationException("uploads not set");

            var file = uploads.First();
            string filename = @"\\s-sql1\Upload\ATCWhoLinkMKB_Update.xlsx";

            if (System.IO.File.Exists(filename))
                System.IO.File.Delete(filename);
            file.SaveAs(filename);

            var _context = new DrugClassifierContext(APP);
            _context.Database.ExecuteSqlCommand("exec [Classifier].[ATCWhoLinkMKB_Update]");

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = new JsonResultData() { Data = null, count = 0, status = "ок", Success = true }
            };
            return jsonNetResult;
        }
    }
}