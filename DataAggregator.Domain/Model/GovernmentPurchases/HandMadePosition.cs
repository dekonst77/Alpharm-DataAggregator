using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;


namespace DataAggregator.Domain.Model.GovernmentPurchases
{

    [Table("Covid19_Vaccinate", Schema = "dbo")]
    public class Covid19_Vaccinate
    {
        [Key]
        public long Id { get; set; }
        public long Region_Id { get; set; }
        public int Category_Id { get; set; }
        public int Nature_Id { get; set; }
        public DateTime Period { get; set; }
       // public long Classifer_Id { get; set; }
        public long? InputFirst { get; set; }
        public long? InputSecond { get; set; }
        public long? InputRevaccinated { get; set; }
        public long? InputСhildren { get; set; }
        public long? First { get; set; }
        public long? Second { get; set; }
        public long? Revaccinated { get; set; }
        public long? Сhildren { get; set; }
        public long Recipient_Id { get; set; }
        public long Customer_Id { get; set; }
        //public long? Doses { get; set; }
        public Guid? UserLastUpdate { get; set; }
        public DateTime UserLastUpdateDate { get; set; }
        public string Link { get; set; }
        public string Comments { get; set; }
    }

    [Table("Covid19_Product", Schema = "dbo")]
    public class Covid19_Product
    {
        [Key]
        public long Classifer_Id { get; set; }
        public string Drug { get; set; }
        public string DrugShort { get; set; }
        public int Package { get; set; }
        public Guid? UserLastUpdate { get; set; }
        public DateTime? UserLastUpdateDate { get; set; }
    }
    [Table("Covid19_Product_History", Schema = "dbo")]
    public class Covid19_Product_History
    {
        [Key]
        public long Row { get; set; }
        public long Classifer_Id { get; set; }
        public string Drug { get; set; }
        public string DrugShort { get; set; }
        public int Package { get; set; }
        public Guid? UserLastUpdate { get; set; }
        public DateTime? UserLastUpdateDate { get; set; }
    }
    [Table("Covid19_Vaccinate_Product", Schema = "dbo")]
    public class Covid19_Vaccinate_Product
    {
        [Key]
        public long Id { get; set; }   
        public long Vaccinate_Id { get; set; }
        public long Classifer_Id { get; set; }
        public decimal? Dosage { get; set; }
        public string Link { get; set; }
        public Guid? UserLastUpdate { get; set; }
        public DateTime? UserLastUpdateDate { get; set; }


    }
 
    [Table("Covid19_Product_Price", Schema = "dbo")]
    public class Covid19_Product_Price
    {
        [Key]
        public long Id { get; set; }
        public long Classifer_Id { get; set; }
        public DateTime DateBegin { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateEnd { get; set; }
        public decimal? Price { get; set; }
        public Guid? UserLastUpdate { get; set; }
        public DateTime? UserLastUpdateDate { get; set; }     
        public virtual Covid19_Product Covid19_Product { get; set; }
    }
    [Table("Covid19_Product_Price_History", Schema = "dbo")]
    public class Covid19_Product_Price_History
    {
        [Key]
        public long Row { get; set; }
        public long Id { get; set; }
        public long Classifer_Id { get; set; }
        public DateTime DateBegin { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd}")]

        public DateTime DateEnd { get; set; }
        public decimal? Price { get; set; }
        public Guid? UserLastUpdate { get; set; }
        public DateTime? UserLastUpdateDate { get; set; }
    }
    [Table("Covid19_Region", Schema = "dbo")]
    public class Covid19_Region
    {
        [Key]
        public long Region_Id { get; set; }
        public string FederationSubject { get; set; }    
    }
    public class Covid19_Vaccinate_View
    {
        [Key]
        public long Id { get; set; }
        public long Region_Id { get; set; }   
        public DateTime Period { get; set; }
        public long Classifer_Id { get; set; }
        public long? InputFirst { get; set; }
        public long? InputSecond { get; set; }
        public long? InputRevaccinated { get; set; }
        public long? InputСhildren { get; set; }
        public long? First { get; set; }
        public long? Second { get; set; }
        public long? Revaccinated { get; set; }
        public long? Сhildren { get; set; }
        public long? VaccinatedPeople { get; set; }
        public long? NumDoses { get; set; }
        public long? TotalDoses { get; set; }
        public long? FirstByLastPeriod { get; set; }
        public long? RevaccinatedDoses { get; set; }
        public long? СhildrenDoses { get; set; }
        public Guid? UserLastUpdate { get; set; }
        public DateTime? UserLastUpdateDate { get; set; }
        public string Link { get; set; }
        public string Comments { get; set; }
      
    }
    public class Covid19_Column_View
    {
        [Key]
        public long Id { get; set; }
        public string ColName { get; set; }
        public string ColText { get; set; }
        public string ColFormula { get; set; }
        public Boolean IsEditable { get; set; }
        public Boolean IsTotal { get; set; }
        public int Sort { get; set; }
        public Boolean IsNumForm { get; set; }

    }




}
