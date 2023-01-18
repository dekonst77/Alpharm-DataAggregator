using System;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.GovernmentPurchases;
using DataAggregator.Domain.Utils;
using DataAggregator.Web.Models.Common;
using Newtonsoft.Json;

namespace DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases
{
    public class ContractJson
    {
        public ContractJson()
        {

        }

        public ContractJson(GovernmentPurchasesContext context, Contract contract)
        {
            if (contract == null)
            {
                throw new ArgumentNullException("contract");
            }

            Id = contract.Id;
            ReestrNumber = contract.ReestrNumber;
            Sum = contract.Sum;
            Url = contract.Url;
            ReceiverId = contract.ReceiverId;
            Receiver = contract.ReceiverId == null ? string.Empty : contract.Receiver.ShortName;

            if (ReceiverId > 0 && contract.Receiver.ActualId > 0)
            {
                var ro = context.Organization.Where(w => w.Id == contract.Receiver.ActualId).Single();
                Receiver = ro.ShortName;
                ReceiverId = contract.Receiver.ActualId;
            }

            LastChangedUser = contract.LastChangedUserId == null
                ? string.Empty
                : string.Format("{0}, {1:dd.MM.yyyy}",
                    context.User.Single(u => u.Id == contract.LastChangedUserId.ToString()).FullNameWithoutPatronymic,
                    contract.LastChangedDate);

            LastChangedObjectsUser = contract.LastChangedObjectsUserId == null
                ? string.Empty
                : string.Format("{0}, {1:dd.MM.yyyy}",
                    context.User.Single(u => u.Id == contract.LastChangedObjectsUserId.ToString())
                        .FullNameWithoutPatronymic, contract.LastChangedObjectsDate);

            LastChangedUser_UserName= contract.LastChangedObjectsUserId == null
                ? string.Empty
                : string.Format("{0}",
                    context.User.Single(u => u.Id == contract.LastChangedObjectsUserId.ToString())
                        .UserName);

            LastChangedObjectsUser_UserName = contract.LastChangedObjectsUserId == null
                ? string.Empty
                : string.Format("{0}",
                    context.User.Single(u => u.Id == contract.LastChangedObjectsUserId.ToString())
                        .UserName);

            KK = contract.KK;

            IsReady = context.ContractObjectReady.Any(cor => cor.ContractId == contract.Id);


            //новые поля

            ContractStatus = contract.ContractStatus == null
                ? new DictionaryElementJson() {Id = null, Name = null}
                : new DictionaryElementJson() {Id = contract.ContractStatus.Id, Name = contract.ContractStatus.Name};
            //ContractStatusId = (long)contract.ContractStatusId;

            ConclusionDate = contract.ConclusionDate;

            DateBegin = contract.DateBegin;

            DateEnd = contract.DateEnd;

            StatusDate = contract.StatusDate;

            ActuallyPaid = contract.ActuallyPaid;

            ContractNumber = contract.ContractNumber;

            SupplierRaw = contract.SupplierRaw;
            change_objects = contract.change_objects;
            change_reason = contract.change_reason;

            Supplier = new SupplierModel();

            //Если вручную задано справочное значение
            if (contract.Supplier != null)
            {
                Supplier = new SupplierModel(contract.Supplier);
            }
            else
            {
                if (SupplierRaw != null && SupplierRaw.Supplier != null)
                {
                    Supplier = new SupplierModel(SupplierRaw.Supplier);
                }
            }
        }

        public DateTime? ConclusionDate { get; set; }

        public DateTime? DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        public DateTime? StatusDate { get; set; }

        public long Id { get; set; }
        //public long ContractStatusId { get; set; }

        public string ReestrNumber { get; set; }

        public decimal? Sum { get; set; }

        public decimal? ActuallyPaid { get; set; }

        public string Url { get; set; }

        public long? ReceiverId { get; set; }

        public string Receiver { get; set; }

        public string LastChangedUser { get; set; }

        public string LastChangedUser_UserName { get; set; }
        public string LastChangedObjectsUser_UserName { get; set; }

        public string LastChangedObjectsUser { get; set; }

        public Byte KK { get; set; }

        public bool IsReady { get; set; }

        public DictionaryElementJson ContractStatus { get; set; }

        public string ContractNumber { get; set; }

        public string change_objects { get; set; }
        public string change_reason { get; set; }

        public SupplierRaw SupplierRaw { get; set; }

        public SupplierModel Supplier { get; set; }
    }
}