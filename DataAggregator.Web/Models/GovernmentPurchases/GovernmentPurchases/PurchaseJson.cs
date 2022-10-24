using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.GovernmentPurchases;
using DataAggregator.Domain.Model.GovernmentPurchases.View;
using DataAggregator.Domain.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Web.Security;
using DataAggregator.Web.Models.Common;

namespace DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases
{
    public class PurchaseJson
    {
        public PurchaseJson()
        {
        }

        public PurchaseJson(GovernmentPurchasesContext context, Purchase purchase)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (purchase == null)
            {
                throw new ArgumentNullException("purchase");
            }

            Id = purchase.Id;
            Number = purchase.Number;

            Customer = purchase.Customer == null
                ? new OrganizationJson() { Id = null}
                : new OrganizationJson(context,purchase.Customer);

            LawType = purchase.LawType == null
                ? new DictionaryElementJson() { Id = null, Name = null }
                : new DictionaryElementJson() { Id = purchase.LawType.Id, Name = purchase.LawType.Name };

            Method = purchase.Method == null
                ? new DictionaryElementJson() { Id = null, Name = null }
                : new DictionaryElementJson() { Id = purchase.Method.Id, Name = purchase.Method.Name };

            Source = purchase.Source == null
                ? new DictionaryElementJsonByte() { Id = null, Name = null }
                : new DictionaryElementJsonByte() { Id = purchase.Source.Id, Name = purchase.Source.Name };

            SiteName = purchase.SiteName;
            SiteURL = purchase.SiteURL;
            Name = purchase.Name;

            Stage = purchase.Stage == null
                ? new DictionaryElementJsonByte() { Id = null, Name = null }
                : new DictionaryElementJsonByte() { Id = purchase.Stage.Id, Name = purchase.Stage.Name };

            StandartContractNumber = purchase.StandartContractNumber;
            DateBegin = purchase.DateBegin;
            DateEnd = purchase.DateEnd;
            DateEndFirstParts = purchase.DateEndFirstParts;
            Url = purchase.URL;
            ConclusionReason = purchase.ConclusionReason;

            DeliveryTime = purchase.DeliveryTime;
            WhoIsPurchasing = purchase.WhoIsPurchasing;
            PriceJustification = purchase.PriceJustification;

            Category = purchase.Category == null
                ? new DictionaryElementJsonByte() { Id = null, Name = null }
                : new DictionaryElementJsonByte() { Id = purchase.Category.Id, Name = purchase.Category.Name };

            Nature = purchase.Nature == null
                ? new NatureJson() { Id = null, CategoryId = null, Name = null }
                : new NatureJson() { Id = purchase.Nature.Id, CategoryId = purchase.Nature.CategoryId, Name = purchase.Nature.Name };

            Nature_L2 = purchase.Nature_L2Id == null
    ? new Nature_L2Json() { Id = null, Nature_L1Id = null, Name = null }
    : new Nature_L2Json() { Id = purchase.Nature_L2.Id, Nature_L1Id = purchase.Nature_L2.Nature_L1Id, Name = purchase.Nature_L2.Name };

            DeliveryTimeInfo = purchase.DeliveryTimeInfo.Select(i => new DeliveryTimeInfoJson(i)).ToList();

            Payment = purchase.Payment.Select(p => new PaymentJson(p)).ToList();

            PurchaseClass = new DictionaryElementJsonByte() { Id = purchase.PurchaseClass.Id, Name = purchase.PurchaseClass.Name };

            LastChangedUser = purchase.LastChangedUserId == null ? String.Empty : string.Format("{0}, {1:dd.MM.yyyy}", context.User.Single(u => u.Id == purchase.LastChangedUserId.ToString()).FullNameWithoutPatronymic, purchase.LastChangedDate);

            PurchaseNatureMixed = new PurchaseNatureMixedModel(purchase.PurchaseNatureMixed.ToList());

            ContractDataWasUsed = context.ShipmentInfo.Any(si => si.PurchaseId == purchase.Id && si.FromContract);

            UseContractData = purchase.UseContractData;

            Comment = purchase.Comment;
            
        }

        public long Id { get; set; }
        public DictionaryElementJsonByte Source { get; set; }

        public string Number { get; set; }

        public OrganizationJson Customer { get; set; }

        public DictionaryElementJson LawType { get; set; }

        public DictionaryElementJson Method { get; set; }

        public string SiteName { get; set; }

        public string SiteURL { get; set; }
        public string ConclusionReason { get; set; }

        public string Name { get; set; }

        public DictionaryElementJsonByte Stage { get; set; }

        public string StandartContractNumber { get; set; }

       // [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime DateBegin { get; set; }

       // [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime DateEnd { get; set; }

      //  [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? DateEndFirstParts { get; set; }

        public string Url { get; set; }

        public DictionaryElementJson Type { get; set; }

        public string DeliveryTime { get; set; }

        public string WhoIsPurchasing { get; set; }

        public string PriceJustification { get; set; }

        public DictionaryElementJsonByte Category { get; set; }

        public NatureJson Nature { get; set; }

        public Nature_L2Json Nature_L2 { get; set; }

        public List<DeliveryTimeInfoJson> DeliveryTimeInfo { get; set; }

        public List<PaymentJson> Payment { get; set; }

        public DictionaryElementJsonByte PurchaseClass { get; set; }

        public string LastChangedUser { get; set; }

        public PurchaseNatureMixedModel PurchaseNatureMixed { get; set; }

        /// <summary>
        /// Признак того что при последнем расчете ослов были использованы контрактные данные
        /// </summary>
        public bool ContractDataWasUsed { get; set; }

        /// <summary>
        /// Принудительное использование контрактных данных в расчете ослов
        /// </summary>
        public bool UseContractData { get; set; }

        public string Comment { get; set; }
       
    }
}