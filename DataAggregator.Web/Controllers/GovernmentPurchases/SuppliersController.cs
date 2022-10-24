using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.GovernmentPurchases;
using DataAggregator.Domain.Model.GovernmentPurchases.View;
using DataAggregator.Web.Models.GovernmentPurchases.Suppliers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.GovernmentPurchases
{
    [Authorize(Roles = "GManager ")]
    public class SuppliersController : BaseController
    {
        private GovernmentPurchasesContext _context;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _context = new GovernmentPurchasesContext(APP);
        }

        ~SuppliersController()
        {
            _context.Dispose();
        }

        /// <summary>
        /// Загрузить данные для привязки
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadSupplierRawList(SupplierRawFilterJson supplierRawFilterJson)
        {
            string readyString = " 1=1";
            if (supplierRawFilterJson.Ready != supplierRawFilterJson.NotReady)
                readyString = supplierRawFilterJson.Ready ? " sr.SupplierId is not null" : " sr.SupplierId is null";

            string sql = " select top 10000 sr.*" +
                         " from dbo.SupplierRawBinding as sr" +
                         " where " + readyString;

            int id;
            if (!string.IsNullOrEmpty(supplierRawFilterJson.Id) && int.TryParse(supplierRawFilterJson.Id, out id))
                sql += " and sr.Id = " + supplierRawFilterJson.Id;

            if (!string.IsNullOrEmpty(supplierRawFilterJson.Name))
                sql += " and sr.Name like '%" + supplierRawFilterJson.Name + "%'";

            if (!string.IsNullOrEmpty(supplierRawFilterJson.Address))
                sql += " and sr.Address like '%" + supplierRawFilterJson.Address + "%'";

            if (!string.IsNullOrEmpty(supplierRawFilterJson.Phone))
                sql += " and sr.Phone like '%" + supplierRawFilterJson.Phone + "%'";

            if (!string.IsNullOrEmpty(supplierRawFilterJson.INN))
                sql += " and sr.INN like '%" + supplierRawFilterJson.INN + "%'";

            if (!string.IsNullOrEmpty(supplierRawFilterJson.KPP))
                sql += " and sr.KPP like '%" + supplierRawFilterJson.KPP + "%'";

            if (!string.IsNullOrEmpty(supplierRawFilterJson.SupplierId))
                sql += " and sr.SupplierId = " + supplierRawFilterJson.SupplierId;

            if (!string.IsNullOrEmpty(supplierRawFilterJson.SupplierName))
                sql += " and sr.SupplierName like '%" + supplierRawFilterJson.SupplierName + "%'";

            var supplierRaw = _context.Database.SqlQuery<SupplierRawBinding>(sql).ToList();


            dynamic result = new ExpandoObject();

            result.Data = supplierRaw.Select(sr => new SupplierRawBindingJson(_context, sr)).ToList();
            result.Count = _context.SupplierRawBinding.Count(srb => srb.SupplierId == null);

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }

        /// <summary>
        /// Загрузить справочник
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadSupplierList(SupplierFilterJson supplierFilterJson)
        {
            string sql = "select top 10000 s.* from dbo.Supplier as s where 1=1";

            int id;
            if (!string.IsNullOrEmpty(supplierFilterJson.Id) && int.TryParse(supplierFilterJson.Id, out id))
                sql += " and s.Id = " + supplierFilterJson.Id;

            if (!string.IsNullOrEmpty(supplierFilterJson.Name))
                sql += " and s.Name like '%" + supplierFilterJson.Name + "%'";

            if (!string.IsNullOrEmpty(supplierFilterJson.INN))
                sql += " and s.INN like '%" + supplierFilterJson.INN + "%'";

            if (!string.IsNullOrEmpty(supplierFilterJson.KPP))
                sql += " and s.KPP like '%" + supplierFilterJson.KPP + "%'";

            if (!string.IsNullOrEmpty(supplierFilterJson.LocationAddress))
                sql += " and s.LocationAddress like '%" + supplierFilterJson.LocationAddress + "%'";

            if (!string.IsNullOrEmpty(supplierFilterJson.ContactMail))
                sql += " and s.ContactMail like '%" + supplierFilterJson.ContactMail + "%'";

            if (!string.IsNullOrEmpty(supplierFilterJson.PhoneNumber))
                sql += " and s.PhoneNumber like '%" + supplierFilterJson.PhoneNumber + "%'";

            var supplier = _context.Database.SqlQuery<Supplier>(sql).ToList();

            var result = supplier.Select(s => new SupplierJson(_context, s)).ToList();

            JsonNetResult jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = result
            };

            return jsonNetResult;
        }

        [HttpPost]
        public ActionResult BindSupplier(List<long> supplierRawListId, long supplierId)
        {
            try
            {
                var supplierRawList = _context.SupplierRaw.Where(sr => supplierRawListId.Contains(sr.Id)).ToList();
                var supplier = _context.Supplier.Single(s => s.Id == supplierId);

                if (supplierRawList.Count() != supplierRawListId.Count())
                    throw new ApplicationException("supplierRaw is not found");

                if (supplier == null)
                    throw new ApplicationException("supplier is not found");

                foreach (var supplierRaw in supplierRawList)
                    supplierRaw.SupplierId = supplierId;

                _context.SaveChanges();
            }
            catch (Exception)
            {
                return Json(false);
            }

            return Json(true);
        }

        [HttpPost]
        public ActionResult AddSupplier(SupplierJson supplier)
        {
            try
            {
                var newSupplier = supplier.ToDomain();
                _context.Supplier.Add(newSupplier);
                _context.SaveChanges();
                return Json(newSupplier.Id);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        [HttpPost]
        public ActionResult EditSupplier(SupplierJson supplier)
        {
            try
            {
                var supplierForEdit = _context.Supplier.Single(s => s.Id == supplier.Id);

                if (supplierForEdit == null)
                    throw new ApplicationException("supplier not found");

                supplierForEdit.Name = supplier.Name;
                supplierForEdit.INN = supplier.INN;
                supplierForEdit.KPP = supplier.KPP;
                supplierForEdit.LocationAddress = supplier.LocationAddress;
                supplierForEdit.ContactMail = supplier.ContactMail;
                supplierForEdit.PhoneNumber = supplier.PhoneNumber;

                _context.SaveChanges();
            }
            catch (Exception)
            {
                return Json(false);
            }

            return Json(true);
        }

        [HttpPost]
        public ActionResult DeleteSupplier(List<long> id)
        {
            try
            {
                var supplierForDelete = _context.Supplier.Where(s => id.Contains(s.Id)).ToList();

                var linkCount = _context.SupplierRaw.Count(sr => sr.SupplierId != null && id.Contains((long)sr.SupplierId));

                if (linkCount > 0)
                    throw new ApplicationException("Удаление не возможно. У данного элемента справочника есть привязанные позиции");

                _context.Supplier.RemoveRange(supplierForDelete);

                _context.SaveChanges();

                return Json(true);
            }
            catch (ApplicationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public ActionResult MergeSupplier(List<long> id, long resultSupplierId)
        {
            try
            {

                _context.MergeSuppliers(id, resultSupplierId);
            }
            catch (Exception)
            {
                return Json(false);
            }

            return Json(true);
        }
    }
}