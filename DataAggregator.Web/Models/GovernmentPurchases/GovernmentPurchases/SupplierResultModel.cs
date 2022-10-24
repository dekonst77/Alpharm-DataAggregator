using System;
using System.Collections.Generic;
using System.Linq;
using DataAggregator.Domain.Model.GovernmentPurchases;
using DataAggregator.Domain.Model.GovernmentPurchases.View;
using DataAggregator.Web.Models.Common;

namespace DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases
{
    /// <summary>
    /// Результат определения 
    /// </summary>
    public class SupplierResultModel
    {
        public long Id { get; set; }
        public DictionaryElementJson LotStatus { get; set; }
        public string ProtocolNumber { get; set; }
        public DateTime? ProtocolDate { get; set; }
        public decimal? Sum { get; set; }
        public SupplierModel Winner { get; set; }
        public List<SupplierListModel> SupplierList { get; set; }
        public bool ForCheck { get; set; }
        public string LastChangedUser { get; set; }
        public DateTime? DateBegin { get; set; }
        public DictionaryElementJsonByte Stage { get; set; }

        public SupplierResultModel()
        {
            
        }

        public SupplierResultModel(SupplierResult model, User LastChangedUser)
        {

            this.Id = model.Id;
            this.LastChangedUser = LastChangedUser == null ? String.Empty : string.Format("{0}, {1:dd.MM.yyyy}", LastChangedUser.FullNameWithoutPatronymic, model.LastChangedDate);
            this.LotStatus = new DictionaryElementJson() { Id = model.LotStatus.Id, Name = model.LotStatus.Name };
            this.ProtocolNumber = model.ProtocolNumber;
            this.ProtocolDate = model.ProtocolDate;
            this.Sum = model.Sum;
            this.ForCheck = model.ForCheck;
            this.DateBegin = model.Lot.Purchase.DateBegin;
            

            if (model.SupplierRaw != null)
            {
                Winner = new SupplierModel();
                Winner.Id = model.SupplierRaw.Id;
                Winner.Name = model.SupplierRaw.Name;
                Winner.INN = model.SupplierRaw.INN;
            }
           
            SupplierList = new List<SupplierListModel>();

            if (model.SupplierList != null && model.SupplierList.Count > 0)
            {

                foreach (var supplierList in model.SupplierList.ToList())
                {
                    var supplierModel = new SupplierListModel
                    {
                        Id = supplierList.Id,
                        Sum = supplierList.Sum,
                        SupplierRaw = new SupplierModel(),
                        Number = supplierList.Number
                    };

                    if (supplierList.SupplierRaw != null)
                    {
                        supplierModel.SupplierRaw.Id = supplierList.SupplierRaw.Id;
                        supplierModel.SupplierRaw.Name = supplierList.SupplierRaw.Name;
                        supplierModel.SupplierRaw.INN = supplierList.SupplierRaw.INN;
                    }

                    supplierModel.Supplier = new SupplierModel();

                    //Если вручную задано справочное значение
                    if (supplierList.Supplier != null)
                    {
                        supplierModel.Supplier.Id = supplierList.Supplier.Id;
                        supplierModel.Supplier.Name = supplierList.Supplier.Name;
                        supplierModel.Supplier.INN = supplierList.Supplier.INN;
                    }
                    //Если не задано, то подтягиваем то, что берется из справочника
                    else if (supplierList.SupplierRaw != null && supplierList.SupplierRaw.Supplier != null)
                    {
                        supplierModel.Supplier.Id = supplierList.SupplierRaw.Supplier.Id;
                        supplierModel.Supplier.Name = supplierList.SupplierRaw.Supplier.Name;
                        supplierModel.Supplier.INN = supplierList.SupplierRaw.Supplier.INN;
                    }

                    SupplierList.Add(supplierModel);
                }

                if (SupplierList.Count == 1)
                {
                    SupplierList.Add(new SupplierListModel() {});
                }

            }
            else
            {
                SupplierList.Add(new SupplierListModel() {  });
                SupplierList.Add(new SupplierListModel() {  });
            }
        }
    }

    /// <summary>
    /// Список получателей
    /// </summary>
    public class SupplierListModel
    {
        public long Id { get; set; }
        public SupplierModel SupplierRaw { get; set; }
        public SupplierModel Supplier { get; set; }
        public decimal? Sum { get; set; }
        public int? Number { get; set; }
    }

    public class SupplierModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string INN { get; set; }
        public string LocationAddress { get; set; }

        public SupplierModel()
        {
            Id = 0;
            Name = String.Empty;
            INN =String.Empty;
            LocationAddress = String.Empty;
        }

        public SupplierModel(Supplier supplier)
        {
            this.Id = supplier.Id;
            this.Name = supplier.Name;
            this.Id = supplier.Id;
            this.INN = supplier.INN;
            this.LocationAddress = supplier.LocationAddress;
        }
    }
}