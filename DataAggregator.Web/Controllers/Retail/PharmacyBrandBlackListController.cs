using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Retail;
using DataAggregator.Domain.Model.Retail.View;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss")]
    public class PharmacyBrandBlackListController : BaseController
    {
        private RetailContext _retailContext;
        private DrugClassifierContext _classifierContext;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _retailContext = new RetailContext();
            _classifierContext = new DrugClassifierContext(APP);
        }

        ~PharmacyBrandBlackListController()
        {
            _retailContext.Dispose();
            _classifierContext.Dispose();
        }

        [HttpGet]
        public ActionResult ExportToExcel()
        {
            var stream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            using (var xlsx = new ExcelPackage(stream))
            {
                xlsx.Workbook.Worksheets.Add("BlackList");
                var sheet = xlsx.Workbook.Worksheets[1];
                sheet.Cells[1, 1].LoadFromDataTable(_retailContext.GetTargetPharmacyBrandBlackListTable(), true);
                xlsx.Save();
            }

            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "blacklist.xlsx");
        }

        [HttpPost]
        public ActionResult UploadFromExcel(HttpPostedFileBase file)
        {
            var fileContents = new List<TargetPharmacyBrandBlackListView>();
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            using (var xlsx = new ExcelPackage(file.InputStream))
            {
                var sheet = xlsx.Workbook.Worksheets[1];

                if (!(sheet.Cells[1, 1].Text.Equals("TargetPharmacyId") && sheet.Cells[1, 2].Text.Equals("BrandId") && sheet.Cells[1, 3].Text.Equals("Brand")))
                {
                    throw new Exception("Некорректное содержимое файла");
                }

                for (var i = 2; i <= sheet.Dimension.End.Row; i++)
                {
                    fileContents.Add(new TargetPharmacyBrandBlackListView
                    {
                        TargetPharmacyId = sheet.Cells[i, 1].GetValue<long>(),
                        BrandId = sheet.Cells[i, 2].GetValue<long?>(),
                        Brand = sheet.Cells[i, 3].GetValue<string>()
                    });
                }
            }

            foreach (var fileRow in fileContents)
            {
                if (fileRow.BrandId == null)
                {
                    if (string.IsNullOrEmpty(fileRow.Brand))
                    {
                        throw new Exception("В одной из строчек не заполнен бренд");
                    }

                    var brand = _classifierContext.Brand.FirstOrDefault(b => b.Value.Equals(fileRow.Brand));

                    if (brand == null)
                    {
                        throw new Exception("Бренд не найден в классификаторе");
                    }

                    fileRow.BrandId = brand.Id;
                }
                else
                {
                    if (!_classifierContext.Brand.Any(b => b.Id == fileRow.BrandId))
                    {
                        throw new Exception("Бренд не найден в классификаторе");
                    }
                }

                if (!_retailContext.TargetPharmacyBrandBlackList.Any(bl => bl.TargetPharmacyId == fileRow.TargetPharmacyId && bl.BrandId == fileRow.BrandId))
                {
                    _retailContext.TargetPharmacyBrandBlackList.Add(new TargetPharmacyBrandBlackList
                    {
                        TargetPharmacyId = fileRow.TargetPharmacyId,
                        BrandId = (long) fileRow.BrandId
                    });
                }
            }

            _retailContext.SaveChanges();
            return null;
        }

        [HttpPost]
        public ActionResult GetBlackList()
        {
            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = _retailContext.TargetPharmacyBrandBlackListView
            };
        }

        [HttpPost]
        public ActionResult DeletePositions(List<TargetPharmacyBrandBlackList> positionsToDelete)
        {
            positionsToDelete.ForEach(p => _retailContext.TargetPharmacyBrandBlackList.Remove(_retailContext.TargetPharmacyBrandBlackList.First(b => b.TargetPharmacyId == p.TargetPharmacyId && b.BrandId == p.BrandId)));
           _retailContext.SaveChanges();
            return null;
        }
    }
}