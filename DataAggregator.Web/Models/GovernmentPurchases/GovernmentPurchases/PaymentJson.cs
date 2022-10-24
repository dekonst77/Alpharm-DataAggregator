using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAggregator.Domain.Model.GovernmentPurchases;
using DataAggregator.Web.Models.Common;

namespace DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases
{
    public class PaymentJson
    {
        public PaymentJson()
        {

        }

        public PaymentJson(Payment payment)
        {
            if (payment == null)
            {
                throw new ArgumentNullException("payment");
            }

            Id = payment.Id;
            KBK = payment.KBK;
            KOSGU = payment.KOSGU;
            Sum = payment.Sum;
            PaymentType = payment.PaymentType == null
                ? new DictionaryElementJson() { Id = null, Name = null }
                : new DictionaryElementJson() { Id = payment.PaymentType.Id, Name = payment.PaymentType.Name };


            PaymentYear = payment.PaymentYear == null
                ? new DictionaryElementJson() { Id = null, Name = null }
                : new DictionaryElementJson() { Id = payment.PaymentYear.Id, Name = payment.PaymentYear.Name };
        }

        public long Id { get; set; }

        public string KBK { get; set; }

        public string KOSGU { get; set; }
        public decimal? Sum { get; set; }


        public DictionaryElementJson PaymentYear { get; set; }
        public DictionaryElementJson PaymentType { get; set; }
    }
}