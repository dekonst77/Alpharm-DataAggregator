using DataAggregator.Domain.DAL;
using DataAggregator.Web.Models.GovernmentPurchases.CalculatedDataEditor;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    [Authorize(Roles = "GManager")]
    public class CalculatedDataEditorController : BaseController
    {
        private GovernmentPurchasesContext _context;
        private DrugClassifierContext _drugClassifierContext;
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GovernmentPurchasesContext(APP);
            _drugClassifierContext = new DrugClassifierContext(APP);
        }

        ~CalculatedDataEditorController()
        {
            _context.Dispose();
            _drugClassifierContext.Dispose();
        }

        [HttpPost]
        public ActionResult GetData(string request)
        {
            var calculatedData = new List<Dictionary<string, object>>();
            request = request.Replace(" ", ",");
            var rawIds = request.Split(',');

            var purchasesToGet = new HashSet<string>();
            foreach (var rawId in rawIds)
                purchasesToGet.Add(rawId.Trim());

            var result = _context.CalculatedDataView.Where(cd => purchasesToGet.Contains(cd.PurchaseNumber)).ToList();

            

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.Take(10000).OrderBy(cd => cd.PurchaseNumber).ToList()
            };
        }

        [HttpPost]
        public ActionResult GetDataByFilter(Models.GovernmentPurchases.CalculatedDataEditor.Filter filter)
        {
            _context.Database.CommandTimeout = 0;
            var result = _context.CalculatedDataView.Select(c => c);

            if (!string.IsNullOrEmpty(filter.PurchaseId))
            {
                var LPurchaseId = GetListLong(filter.PurchaseId);
                result = result.Where(r => LPurchaseId.Contains(r.PurchaseId));
            }

            if (!string.IsNullOrEmpty(filter.DrugId))
            {
                var LDrugId = GetListLong(filter.DrugId);
                result = result.Where(r => LDrugId.Contains(r.DrugId));
            }

            if (!string.IsNullOrEmpty(filter.ClassifierId))
            {
                var LClassifierId = GetListLong(filter.ClassifierId);
                result = result.Where(r => LClassifierId.Contains(r.ClassifierId));
            }

            if (!string.IsNullOrEmpty(filter.OwnerTradeMark))
                result = result.Where(r => r.OwnerTradeMark.Contains(filter.OwnerTradeMark));

            if (!string.IsNullOrEmpty(filter.OwnerTradeMarkId))
            {
                var LOwnerId = GetListLong(filter.OwnerTradeMarkId);
                result = result.Where(r => LOwnerId.Contains(r.OwnerTradeMarkId));
            }

            if (!string.IsNullOrEmpty(filter.Packer))
                result = result.Where(r => r.Packer.Contains(filter.Packer));

            if (!string.IsNullOrEmpty(filter.PackerId))
            {
                var LPackerID = GetListLong(filter.PackerId);
                result = result.Where(r => LPackerID.Contains(r.PackerId));
            }

            if (!string.IsNullOrEmpty(filter.PurchaseNumber))
                result = result.Where(r => r.PurchaseNumber.Contains(filter.PurchaseNumber));

            if (!string.IsNullOrEmpty(filter.INNGroup))
                result = result.Where(r => r.InnGroup.Contains(filter.INNGroup));

            if (filter.PurchaseDateBeginStartValue.Year > 1)
                result = result.Where(r => r.PurchaseDateBegin >= filter.PurchaseDateBeginStartValue);

            if (filter.PurchaseDateBeginEndValue.Year > 1)
            {
                filter.PurchaseDateBeginEndValue = filter.PurchaseDateBeginEndValue.AddDays(1);
                result = result.Where(r => r.PurchaseDateBegin < filter.PurchaseDateBeginEndValue);
            }

            if (!string.IsNullOrEmpty(filter.DrugTradeName))
                result = result.Where(r => r.DrugTradeName.Contains(filter.DrugTradeName));

            if (filter.SelectedNatureIds != null)
                result = result.Where(r => r.NatureId!=null && 
                                           filter.SelectedNatureIds.ToList().Contains((Byte)r.NatureId));

            if (filter.SelectedCategoryIds != null)
                result = result.Where(r => r.CategoryId != null &&
                                           filter.SelectedCategoryIds.ToList().Contains((Byte)r.CategoryId));

            if (!string.IsNullOrEmpty(filter.DrugDescription))
                result = result.Where(r => r.DrugDescription.Contains(filter.DrugDescription));

            if (!string.IsNullOrEmpty(filter.ObjectReadyName))
                result = result.Where(r => r.ObjectReadyName.Contains(filter.ObjectReadyName));

            if (!string.IsNullOrEmpty(filter.GoodsCategoryName))
                result = result.Where(r => r.GoodsCategoryName.Contains(filter.GoodsCategoryName));

            if (filter.SelectedFederalDistrictNames != null)
                result = result.Where(r => !string.IsNullOrEmpty(r.RegionFederalDistrict) &&
                                           filter.SelectedFederalDistrictNames.ToList().Contains(r.RegionFederalDistrict));

            if (filter.SelectedFederationSubjectNames != null)
                result = result.Where(r => !string.IsNullOrEmpty(r.RegionFederationSubject) &&
                                           filter.SelectedFederationSubjectNames.ToList().Contains(r.RegionFederationSubject));

            if (filter.VNC)
            {
                result = result.Where(w => w.VNC == true);
            }
            if (filter.IncludeContracts)
            {
                if (filter.IncludePurchases) //и контракты, и закупки
                {
                }
                else //только контракты
                {
                    result = result.Where(r => r.ObjectType.Equals("Contract"));
                }
            }
            else
            {
                if (filter.IncludePurchases) //только закупки
                    result = result.Where(r => r.ObjectType.Equals("Purchase"));
                else //ничего
                    result = result.Where(r => false);
            }

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result.Take(10000).ToList()
            };
        }
        
        public ActionResult GetManufacturerList()
        {
            var result =
                _drugClassifierContext.Manufacturer.Select(m => new {Id = m.Id, Value = m.Value}).ToList();

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };
        }

        public ActionResult GetCategoryList()
        {
            var result = _context.Category.OrderBy(c=>c.Name).Select(c => new { Id = c.Id, displayValue = c.Name}).ToList();

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };
        }

        public ActionResult GetNatureList()
        {
            var result = _context.Nature.OrderBy(c => c.Name).Select(c => new { Id = c.Id, displayValue = c.Name }).ToList();

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };
        }
        [HttpPost]
        public ActionResult RecalculateObjectsEI(long[] lotIds)
        {
            DataTable lotIdsDt = new DataTable();
            lotIdsDt.Columns.Add(new DataColumn("LotId", typeof(long)));
            foreach (var lotId in lotIds)
                if (lotIdsDt.AsEnumerable().All(row => row.Field<long>("LotId") != lotId))
                {
                    DataRow dr = lotIdsDt.NewRow();
                    dr["LotId"] = lotId;
                    lotIdsDt.Rows.Add(dr);
                }

            _context.AutoCorrectAmount(lotIdsDt, new Guid(User.Identity.GetUserId()));
            return null;
        }

        [HttpPost]
        public ActionResult RecalculateObjects(long[] lotIds)
        {
            DataTable lotIdsDt = new DataTable();
            lotIdsDt.Columns.Add(new DataColumn("LotId", typeof(long)));
            foreach (var lotId in lotIds)
                if (lotIdsDt.AsEnumerable().All(row => row.Field<long>("LotId") != lotId))
                {
                    DataRow dr = lotIdsDt.NewRow();
                    dr["LotId"] = lotId;
                    lotIdsDt.Rows.Add(dr);
                }

            _context.CopyToCalculatedPurchaseObjectPrepareThenExecute(lotIdsDt);
            _context.CopyToCalculatedContractObjectPrepareThenExecute(lotIdsDt);

            return null;
        }


        [HttpPost]
        public ActionResult ChangeObjectReadyName(string objectType, long readyId, string objectReadyName, string ObjectReadyUnit, bool setCorrectedToNull)
        {
            dynamic readyObject; //PurchaseObjectReady or ContractObjectReady
            dynamic subItem; //Lot or Contract

            switch (objectType)
            {
                case "Purchase":
                    readyObject = _context.PurchaseObjectReady.Single(por => por.Id == readyId);
                    subItem = readyObject.Lot;
                    break;
                case "Contract":
                    readyObject = _context.ContractObjectReady.Single(por => por.Id == readyId);
                    subItem = readyObject.Contract;
                    break;
                default:
                    throw new ApplicationException("Incorrect object type. Correct types: Purchase, Contract.");
            }

            readyObject.Name = objectReadyName;
            readyObject.Unit = ObjectReadyUnit;

            subItem.LastChangedObjectsDate = DateTime.Now;
            subItem.LastChangedObjectsUserId = new Guid(User.Identity.GetUserId());

            //чтобы строка ушла на обработку
            readyObject.DrugRawId = null;
            readyObject.DrugClassifierId = null;
            readyObject.ClassifierId = null;

            if (setCorrectedToNull)
            {
                readyObject.AmountCorrected = null;
                readyObject.PriceCorrected = null;
                readyObject.SumCorrected = null;
                subItem.LastChangedObjectsCorrectedDate = null;
                subItem.LastChangedObjectsCorrectedUserId = null;
            }

            _context.SaveChanges();
            return null;
        }

        [HttpPost]
        public ActionResult SaveCalculatedObject(string objectType,long PurchaseId, long readyId, string amount, string price, string sum,bool VNC,bool UseContractData)
        {
            var c = new CultureInfo("ru");
            c.NumberFormat.NumberDecimalSeparator = ",";
            var Purchase = _context.Purchase.Single(p => p.Id == PurchaseId);
            dynamic readyObject;

            switch (objectType)
            {
                case "Purchase":
                    readyObject = _context.PurchaseObjectReady.Single(por => por.Id == readyId);
                    readyObject.Lot.LastChangedObjectsCorrectedDate = DateTime.Now;
                    readyObject.Lot.LastChangedObjectsCorrectedUserId = new Guid(User.Identity.GetUserId());
                    break;
                case "Contract":
                    readyObject = _context.ContractObjectReady.Single(por => por.Id == readyId);
                    readyObject.Contract.LastChangedObjectsCorrectedDate = DateTime.Now;
                    readyObject.Contract.LastChangedObjectsCorrectedUserId = new Guid(User.Identity.GetUserId());
                    break;
                default:
                    throw new ApplicationException("Incorrect object type. Correct types: Purchase, Contract.");
            }

            decimal? dAmount = null;
            if (!string.IsNullOrEmpty(amount))
            {
                var clearAmount = amount.Trim().Replace(".", ",");
                dAmount = decimal.Parse(clearAmount, c);
            }

            decimal? dPrice = null;
            if (!string.IsNullOrEmpty(price))
            {
                var clearPrice = price.Trim().Replace(".", ",");
                dPrice = decimal.Parse(clearPrice, c);
            }

            decimal? dSum = null;
            if (!string.IsNullOrEmpty(sum))
            {
                var clearSum = sum.Trim().Replace(".", ",");
                dSum = decimal.Parse(clearSum, c);
            }

            readyObject.AmountCorrected = dAmount;
            readyObject.PriceCorrected = dPrice;
            readyObject.SumCorrected = dSum;
            readyObject.VNC = VNC;
            Purchase.UseContractData = UseContractData;
            Purchase.LastChangedUserId = new Guid(User.Identity.GetUserId());
            _context.SaveChanges();
            return null;
        }

        [HttpPost]
        public ActionResult GetRegionNames(int level)
        {
            var result =
                _context.RegionName.Where(rn => rn.Level == level)
                    .OrderBy(rn => rn.Name)
                    .Select(rn => new {Id = rn.Id, displayValue = rn.Name})
                    .ToList();

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };
        }

        public ActionResult Multiple(List<ObjectIdentifier> idsToMultiple, string multiplier)
        {
            var purchaseObjectIds = idsToMultiple.Where(d => d.ObjectType.Equals("Purchase")).Select(d => d.ObjectId).ToList();
            var contractObjectIds = idsToMultiple.Where(d => d.ObjectType.Equals("Contract")).Select(d => d.ObjectId).ToList();

            var c = new CultureInfo("ru");
            c.NumberFormat.NumberDecimalSeparator = ",";

            if (purchaseObjectIds.Any())
            {
                var objectsToEdit = _context.PurchaseObjectReady.Where(or => purchaseObjectIds.Contains(or.Id)).ToList();

                foreach (var objectReady in objectsToEdit)
                {
                    objectReady.AmountCorrected = objectReady.Amount * decimal.Parse(multiplier.Replace(".", ","), c);
                    objectReady.Lot.LastChangedObjectsCorrectedDate = DateTime.Now;
                    objectReady.Lot.LastChangedObjectsCorrectedUserId = new Guid(User.Identity.GetUserId());
                }
            }

            if (contractObjectIds.Any())
            {
                var objectsToEdit = _context.ContractObjectReady.Where(or => contractObjectIds.Contains(or.Id)).ToList();

                foreach (var objectReady in objectsToEdit)
                {
                    objectReady.AmountCorrected = objectReady.Amount * decimal.Parse(multiplier.Replace(".", ","), c);
                    objectReady.Contract.LastChangedObjectsCorrectedDate = DateTime.Now;
                    objectReady.Contract.LastChangedObjectsCorrectedUserId = new Guid(User.Identity.GetUserId());
                }
            }

            _context.SaveChanges();
            return null;
        }
        public ActionResult ClearObjectReadyCorrected(List<ExtendObjectIdentifier> idsToDivide)
        {
            //ObjectReadyAmountCorrected
            // ObjectReadyPriceCorrected
            // ObjectReadySumCorrected
            var purchaseObjectIds = idsToDivide.Where(d => d.ObjectType.Equals("Purchase") && !string.IsNullOrEmpty(d.DiviedTo)).Select(d => d.ObjectId).ToList();
            var contractObjectIds = idsToDivide.Where(d => d.ObjectType.Equals("Contract") && !string.IsNullOrEmpty(d.DiviedTo)).Select(d => d.ObjectId).ToList();  

            if (purchaseObjectIds.Any())
            {
                var objectsToEdit = _context.PurchaseObjectReady.Where(or => purchaseObjectIds.Contains(or.Id)).ToList();
                foreach (var objectReady in objectsToEdit)
                {
                    objectReady.AmountCorrected = null;
                    objectReady.PriceCorrected = null;
                    objectReady.SumCorrected = null;
                    objectReady.Lot.LastChangedObjectsCorrectedDate = DateTime.Now;
                    objectReady.Lot.LastChangedObjectsCorrectedUserId= new Guid(User.Identity.GetUserId());
                }
            }

            if (contractObjectIds.Any())
            {
                var objectsToEdit = _context.ContractObjectReady.Where(or => contractObjectIds.Contains(or.Id)).ToList();

                foreach (var objectReady in objectsToEdit)
                {
                    objectReady.AmountCorrected = null;
                    objectReady.PriceCorrected = null;
                    objectReady.SumCorrected = null;
                    //objectReady.Lot.LastChangedObjectsCorrectedDate = DateTime.Now;
                }
            }

            _context.SaveChanges();
            return null;
        }

        public ActionResult DividePackaging(List<ExtendObjectIdentifier> idsToDivide)
        {
            var purchaseObjectIds = idsToDivide.Where(d => d.ObjectType.Equals("Purchase") && !string.IsNullOrEmpty(d.DiviedTo)).Select(d => d.ObjectId).ToList();
            var contractObjectIds = idsToDivide.Where(d => d.ObjectType.Equals("Contract") && !string.IsNullOrEmpty(d.DiviedTo)).Select(d => d.ObjectId).ToList();

            var c = new CultureInfo("ru");
            c.NumberFormat.NumberDecimalSeparator = ",";


            if (purchaseObjectIds.Any())
            {
                var objectsToEdit = _context.PurchaseObjectReady.Where(or => purchaseObjectIds.Contains(or.Id)).ToList();




                foreach (var objectReady in objectsToEdit)
                {

                    var divider = idsToDivide.Single(i => i.ObjectId == objectReady.Id && i.ObjectType.Equals("Purchase")).DiviedTo;
                    
                    objectReady.AmountCorrected = objectReady.Amount / decimal.Parse(divider.Replace(".", ","), c);
                    objectReady.Lot.LastChangedObjectsCorrectedDate = DateTime.Now;
                    objectReady.Lot.LastChangedObjectsCorrectedUserId = new Guid(User.Identity.GetUserId());
                }
            }

            if (contractObjectIds.Any())
            {
                var objectsToEdit = _context.ContractObjectReady.Where(or => contractObjectIds.Contains(or.Id)).ToList();

                foreach (var objectReady in objectsToEdit)
                {

                    var divider = idsToDivide.Single(i => i.ObjectId == objectReady.Id && i.ObjectType.Equals("Contract")).DiviedTo;

                    objectReady.AmountCorrected = objectReady.Amount / decimal.Parse(divider.Replace(".", ","), c);
                    objectReady.Contract.LastChangedObjectsCorrectedDate = DateTime.Now;
                    objectReady.Contract.LastChangedObjectsCorrectedUserId = new Guid(User.Identity.GetUserId());
                }
            }

            _context.SaveChanges();
            return null;
        }


        public ActionResult clearAvgPrice(int ClassifierId)
        {
            System.Data.SqlClient.SqlParameter pDrugId = new System.Data.SqlClient.SqlParameter("@ClassifierId", ClassifierId);
            _context.Database.ExecuteSqlCommand("dbo.clearAvgPrice @ClassifierId", pDrugId);
            return null;
        }
            public ActionResult Divide(List<ObjectIdentifier> idsToDivide, string divider)
        {
            var purchaseObjectIds = idsToDivide.Where(d => d.ObjectType.Equals("Purchase")).Select(d => d.ObjectId).ToList();
            var contractObjectIds = idsToDivide.Where(d => d.ObjectType.Equals("Contract")).Select(d => d.ObjectId).ToList();

            var c = new CultureInfo("ru");
            c.NumberFormat.NumberDecimalSeparator = ",";

            if (purchaseObjectIds.Any())
            {
                var objectsToEdit = _context.PurchaseObjectReady.Where(or => purchaseObjectIds.Contains(or.Id)).ToList();

                foreach (var objectReady in objectsToEdit)
                {
                    objectReady.AmountCorrected = objectReady.Amount / decimal.Parse(divider.Replace(".", ","), c);
                    objectReady.Lot.LastChangedObjectsCorrectedDate = DateTime.Now;
                    objectReady.Lot.LastChangedObjectsCorrectedUserId = new Guid(User.Identity.GetUserId());
                }
            }

            if (contractObjectIds.Any())
            {
                var objectsToEdit = _context.ContractObjectReady.Where(or => contractObjectIds.Contains(or.Id)).ToList();

                foreach (var objectReady in objectsToEdit)
                {
                    objectReady.AmountCorrected = objectReady.Amount / decimal.Parse(divider.Replace(".", ","), c);
                    objectReady.Contract.LastChangedObjectsCorrectedDate = DateTime.Now;
                    objectReady.Contract.LastChangedObjectsCorrectedUserId = new Guid(User.Identity.GetUserId());
                }
            }

            _context.SaveChanges();
            return null;
        }
    }
}