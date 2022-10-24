using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace DataAggregator.Domain.Model.DataReport
{
    [Table("Worker", Schema = "dbo")]
    public class Worker
    {
        [Key]
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
    [Table("Companies", Schema = "dbo")]
    public class Companies
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }
    }
    [Table("Rep_Type", Schema = "dbo")]
    public class Rep_Type
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }
    }
    [Table("Rep_Param", Schema = "dbo")]
    public class Rep_Param
    {
        [Key]
        public int Id { get; set; }
        public int Rep_TypeId { get; set; }
        public int WorkerId { get; set; }
        public string Name { get; set; }
        public string Period { get; set; }
        public bool IsActive { get; set; }

        public string Param_word { get; set; }
        public string Param_INN { get; set; }
        public string Param_ATCEphmra { get; set; }
        public string Param_Region_Customer { get; set; }
        public string Param_Region_Receiver { get; set; }
        public string Param_Customer_INN { get; set; }
        public string Param_TN { get; set; }
        public DateTime LastSend { get; set; }
        public DateTime Create { get; set; }

        public void IsNull()
        {
            if (Name == null) Name = "";
            if (Param_word == null) Param_word = "";
            if (Param_INN == null) Param_INN = "";
            if (Param_ATCEphmra == null) Param_ATCEphmra = "";
            if (Param_Region_Customer == null) Param_Region_Customer = "";
            if (Param_Region_Receiver == null) Param_Region_Receiver = "";
            if (Param_Customer_INN == null) Param_Customer_INN = "";
            if (Param_TN == null) Param_TN = "";
            if (Period == null) Period = "";
        }
    }


    [Table("WebAggReports", Schema = "report")]
    public class WebAggReports
    {
        [Key]
        public int Id { get; set; }
        public string Server { get; set; }
        public string Query { get; set; }
        public string Area { get; set; }
        public string Name { get; set; }
        public string Roles { get; set; } 
        public string Filters { get; set; }
    }
}
