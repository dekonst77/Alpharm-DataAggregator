using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("OrganizationLoad", Schema = "Purchase")]
    public class OrganizationLoad
    {
        public long Id { get; set; }
        public long GZId { get; set; }
        public Byte LawTypeId { get; set; }
        public string URL { get; set; }
        public int AnalyzeId { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
    }
}