using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.RegularExpressions;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("PageTable", Schema = "Purchase")]
    public class PageTable
    {
        public long Id { get; set; }
        public System.Guid GroupId { get; set; }
        public long? PageId { get; set; }
        public bool IsHeader { get; set; }
        public string Column0 { get; set; }
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
        public string Column6 { get; set; }
        public string Column7 { get; set; }
        public string Column8 { get; set; }
        public string Column9 { get; set; }
        public string Column10 { get; set; }
        public string Column11 { get; set; }
        public string Column12 { get; set; }
        public string Column13 { get; set; }
        public string Column14 { get; set; }
        public string Column15 { get; set; }
        public string Column16 { get; set; }
        public string Column17 { get; set; }
        public string Column18 { get; set; }
        public string Column19 { get; set; }
        public string Column20 { get; set; }
        public string Column21 { get; set; }
        public string Column22 { get; set; }
        public string Column23 { get; set; }
        public string Column24 { get; set; }
        public string Column25 { get; set; }
        public string Column26 { get; set; }

        public virtual Page Page { get; set; }

        public void SetColumn(int i, string value)
        {
            var column = String.Format("Column{0}", i);
            this.GetType().GetProperty(column).SetValue(this, value);
        }

       
    }
}