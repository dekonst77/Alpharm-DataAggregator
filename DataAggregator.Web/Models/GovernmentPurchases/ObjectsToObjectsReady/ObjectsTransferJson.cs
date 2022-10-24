using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAggregator.Domain.Model.GovernmentPurchases;

namespace DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases
{
    public class ObjectsTransferJson
    {
        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public bool TransferObjects { get; set; }

        public bool TransferContracts { get; set; }
    }
}