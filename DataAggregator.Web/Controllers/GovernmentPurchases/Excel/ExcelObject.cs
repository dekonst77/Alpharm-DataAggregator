using System;
using DataAggregator.Domain.Model.GovernmentPurchases;

namespace DataAggregator.Web.GovernmentPurchasesExcel
{
    public class ExcelObject
    {
        public string Name { get; set; }

        public string Unit { get; set; }

        public string Amount { get; set; }

        public string Price { get; set; }

        public string Sum { get; set; }
        public string ReceiverId { get; set; }

        public string ReceiverRaw { get; set; }
        public string ReceiverRawA { get; set; }
        public string ReceiverRawB { get; set; }
        public PurchaseObjectReady GetPurchaseObjectReady()
        {
            var o = new PurchaseObjectReady
            {
                Sum = ConvertToDecimal(this.Sum),
                Price = ConvertToDecimal(this.Price),
                Unit = this.Unit,
                Name = this.Name
            };

            var amount = ConvertToDecimal(this.Amount);
            if (!string.IsNullOrEmpty(this.ReceiverId))
            {
                o.ReceiverId =Convert.ToInt64(this.ReceiverId);
            }
            o.ReceiverRaw = "";
            if (!string.IsNullOrEmpty(this.ReceiverRaw))
                o.ReceiverRaw = o.ReceiverRaw + this.ReceiverRaw+" ";

            if (!string.IsNullOrEmpty(this.ReceiverRawA))
                o.ReceiverRaw = o.ReceiverRaw + this.ReceiverRawA + " ";

            if (!string.IsNullOrEmpty(this.ReceiverRawB))
                o.ReceiverRaw = o.ReceiverRaw + this.ReceiverRawB + " ";

            o.ReceiverRaw = o.ReceiverRaw.Trim();
            if (!amount.HasValue)
                throw new  ApplicationException("Поле количество должно быть заполненно и должно быть числом");

            o.Amount = amount.Value;

            return o;
        }

        public ContractObjectReady GetContractObjectReady()
        {
            var o = new ContractObjectReady
            {
                Sum = ConvertToDecimal(this.Sum),
                Price = ConvertToDecimal(this.Price),
                Unit = this.Unit,
                Name = this.Name
            };

            var amount = ConvertToDecimal(this.Amount);

            if (!amount.HasValue)
                throw new ApplicationException("Поле количество должно быть заполненно и должно быть числом");

            o.Amount = amount.Value;

            return o;
        }

        private decimal? ConvertToDecimal(string value)
        {
            var clearValue = ClearString(value);
            decimal decimalValue;
            
            if(!Decimal.TryParse(clearValue, out decimalValue))
            {
                return null;
            }

            return decimalValue;
        }

        private string ClearString(string value)
        {
            switch (System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
            { 
                case ".":
                    return !string.IsNullOrEmpty(value) ? value.Replace(",", ".") : null;
                case ",":
                    return !string.IsNullOrEmpty(value) ? value.Replace(".", ",") : null;
            }
            return !string.IsNullOrEmpty(value) ? value.Replace(".", ",") : null;
        }

    }
}