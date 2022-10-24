using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader.purchasefile
{
    [Table("FilePurchaseData", Schema = "purchasefile")]
    public class FilePurchaseData
    {
        public long Id { get; set; }
        public long FileAnalyzeTaskId { get; set; }
        public string Okpd2 { get; set; }
        public string IsVed { get; set; }
        public string IsVedManual { get; set; }
        public string INN { get; set; }
        public string PurchaseByTradeName { get; set; }
        public string TradeName { get; set; }
        public string NeedPacking { get; set; }
        public string JustificationNeedPacking { get; set; }
        public string FormDosageUnit { get; set; }
        public string Count { get; set; }
        public string Price { get; set; }
        public string Sum { get; set; }
        public string FormProduct { get; set; }
        public string Dosage { get; set; }
        public string Unit { get; set; }
        public string UnitCodeOkei { get; set; }
        public string UnitOkei { get; set; }
        public string CountPrimaryPacking { get; set; }
        public string ConsumerPackingCount { get; set; }
        public string BasicVariantDelivery { get; set; }



        public string ToName()
        { 
            StringBuilder builder = new StringBuilder();
            builder.Append(INN);
            builder.Append(" ");
            builder.Append(TradeName);
            builder.Append(" ");
            builder.Append(FormProduct);
            builder.Append(" ");
            builder.Append(Dosage);
            builder.Append(" ");
            builder.Append(UnitOkei);
            builder.Append(" ");
            builder.Append(CountPrimaryPacking);
            builder.Append(" ");
            builder.Append(ConsumerPackingCount);


            return Regex.Replace(builder.ToString(), @"\s+", " ").Trim().ToLower();
        }
       
    }

}
