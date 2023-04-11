using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Collections.Generic;

namespace DataAggregator.Domain.Model.Distr
{
    [Table("Supplier", Schema = "Dic")]
    public class Supplier
    {
        [Key]
        public long Id { get; set; }
        public int DistributorId { get; set; }
    }

    [Table("Project", Schema = "Dic")]
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string Name { get; set; }      
    }
    [Table("Company", Schema = "Dic")]
    public class Comp
    {
        [Key]
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Company { get; set; }
        public bool ToQlik { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
    }
    [Table("DataSourceType", Schema = "Dic")]
    public class DataSourceType
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameEng { get; set; }
    }
    [Table("DataSource", Schema = "Dic")]
    public class DataSource
    {
        [Key]
        public long Id { get; set; }
        public int? ProjectId { get; set; }
        public string Name { get; set; }
        public string NameFull { get; set; }
        public long? DataSourceTypeId { get; set; }
        public virtual DataSourceType DataSourceType { get; set; }
        public virtual Project Project { get; set; }

    }
    [Table("DataType", Schema = "Dic")]
    public class DataType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameEng { get; set; }
        public int? DataSourceTypeId { get; set; }
        public string DataTypeName { get; set; }
       // public virtual DataSourceType DataSourceType { get; set; }
    }
    [Table("DistributionType", Schema = "Dic")]
    public class DistributionType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
    [Table("Distributor", Schema = "Dic")]
    public class Distributor
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
    [Table("DistributorBranch", Schema = "Dic")]
    public class DistributorBranch
    {
        [Key]
        public int Id { get; set; }
        public int DistributorId { get; set; }
        public string NameFull { get; set; }
        public virtual Distributor Distributor { get; set; }
    }
    [Table("FileStatus", Schema = "Loader")]
    public class FileStatus
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
        
    [Table("Relation", Schema = "Loader")]
    public class Relation
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameRus { get; set; }
    }
    [Table("TemplatesMethod", Schema = "Loader")]
    public class TemplatesMethod
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameRus { get; set; }
    }

    [Table("Templates", Schema = "Loader")]
    public class Templates
    {
        [Key]
        public long Id { get; set; }
        public int? ProjectId { get; set; }
        public long? DataSourceId { get; set; }
        public int? TemplatesMethodId { get; set; }
        public bool? IsActual { get; set; }
        public string Name { get; set; }
        [System.ComponentModel.DefaultValue(1)]
        public int? SheetRelationId { get; set; }
        public int? DataTypeId { get; set; }
        public string Sheet { get; set; }
        public virtual Relation SheetRelation { get; set; }
        public virtual DataSource DataSource { get; set; }
        public virtual Project Project { get; set; }
        public virtual TemplatesMethod TemplatesMethod { get; set; }

    }
    [Table("TemplatesFieldName", Schema = "Loader")]
    public class TemplatesFieldName
    {
        [Key]
        public long Id { get; set; }
        public int? ProjectId { get; set; }
        public string FieldName { get; set; }
        public string Description { get; set; }
        public virtual Project Project { get; set; }
    }
    [Table("TemplatesField", Schema = "Loader")]
    public class TemplatesField
    {
        [Key]
        public long Id { get; set; }
        public long TemplateId { get; set; }
      //  public int? ProjectId { get; set; }
        public long TemplatesFieldNameId { get; set; }
        [System.ComponentModel.DefaultValue(1)]
        public int? RelationId { get; set; }
        public string Value { get; set; }
        public long? ColOffSet { get; set; }
        public long? RowOffSet { get; set; }
        public int? DicMappping { get; set; }
        
        public virtual Relation Relation { get; set; }
     //   public virtual Templates Templates { get; set; }
        public virtual TemplatesFieldName TemplatesFieldName { get; set; }
      //  public virtual Project Project { get; set; }

    }
    [Table("FileInfo", Schema = "Loader")]
    public class FileInfo
    {
        [Key]
        public long Id { get; set; }
        public int CompanyId { get; set; }
        public long? DataSourceId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        [System.ComponentModel.DefaultValue(1)]
        public int FileStatusId { get; set; }
        
        public int Year { get; set; }
        public int Month { get; set; }

        public DateTime DateInsert { get; set; }
        public virtual Comp Company { get; set; }
        public virtual DataSource DataSource { get; set; }
        public string FileStatusT { get; set; }
    }

   		

    public class FileInfo_Detail
    {
        public long FileInfoId { get; set; }
        public string Sheet { get; set; }
        public int? Cnt { get; set; }
        public decimal? PurchaseCount { get; set; }
        public decimal? PurchaseSum { get; set; }
        public decimal? SellingCount { get; set; }
        public decimal? SellingSum { get; set; }
        public decimal? StockCount { get; set; }
        public decimal? StockSum { get; set; }
        public int? SheetStatus { get; set; }
        public string SheetStatusT { get; set; }
    }
    public class SelectFilter
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public Comp Company { get; set; }
        public Project Project { get; set; }
        public DataSource DataSource { get; set; }
    }
    public class FileInfoJson
    {
        /// <summary>
        /// Список источников
        /// </summary>
        public List<DataSource> DataSourceList { get; set; }
        public List<Comp> CompanyList { get; set; }
        public List<Project> ProjectList { get; set; }

        /// <summary>
        /// Выбранный фильтр
        /// </summary>
        public SelectFilter Filter { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public FileInfoJson()
        {
            Filter = new SelectFilter();
            var date = DateTime.Now.AddMonths(-1);
            Filter.Year = date.Year;
            Filter.Month = date.Month;
            Filter.DataSource = null;
            Filter.Company = null;
            Filter.Project = null;
        }
    }

    [Table("Region_Alias", Schema = "Dic")]
    public class Region_Alias
    {
        [Key]
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Region_Before { get; set; }
        public int RegionId { get; set; }
    }

    [Table("Region", Schema = "Dic")]
    public class Region
    {
        [Key]
        public int Id { get; set; }
        public string num { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
    }

    [Table("CompanyPeriod", Schema = "Dic")]
    public class CompanyPeriod
    {
        [Key]
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int period { get; set; }
        public bool toProd { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Comp Company { get; set; }
    }
    [Table("Rules_Clients", Schema = "Dic")]
    public class Rules_Clients
    {
        [Key]
        public long Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string INN { get; set; }        
        public string Region_Before { get; set; }
        public string Region_After { get; set; }
        public string Comments { get; set; }
        public virtual Comp Company { get; set; }
    }

    //[Function("TaskList", Schema = "dbo")]
    public class TaskList
    {
        [Key]
        public long Id { get; set; }
        public int CompanyId { get; set; }
        public string Company { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime? DateLastStart { get; set; }
        public DateTime? DateLastComplete { get; set; }
       
    }

    public class ReportSheet
    {
        public long FileInfoId { get; set; }
        public string FilePath { get; set; }
        public string DataSource { get; set; }
        public string DataSourceType { get; set; }        
        public string FileName { get; set; }
        public string Sheet { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }       
        public int? Cnt { get; set; }
        public decimal? PurchaseCount { get; set; }
        public decimal? PurchaseSum { get; set; }
        public decimal? SellingCount { get; set; }
        public decimal? SellingSum { get; set; }
        public decimal? StockCount { get; set; }
        public decimal? StockSum { get; set; }
        public string FileStatusT { get; set; }
        public string SheetStatusT        {get; set;}
        public int? SheetStatus { get; set; }        
        public int? FN_E_Plus { get; set; }
    }
    
    public class ReportSheet_top15
    {
        public string DataSource { get; set; }
        public long FileInfoId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public string Sheet { get; set; }
        public string Drug { get; set; }
        public string Manufacturer { get; set; }
        public string SourceCode { get; set; }
        public string PharmacySourceCode { get; set; }
        public string PharmacyName { get; set; }
        public string LegalName { get; set; }
        public string PostAddress { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string LegalAddress { get; set; }
        public string INN { get; set; }
        public string OtherInfo { get; set; }
        public string Distributor { get; set; }
        public string DistributorBranch { get; set; }
        public string Tender { get; set; }
        public string SalesMarket { get; set; }
        public string Document { get; set; }
        public string Date { get; set; }

        public decimal? PurchaseCount { get; set; }
        public decimal? PurchaseSum { get; set; }
        public decimal? SellingCount { get; set; }
        public decimal? SellingSum { get; set; }
        public decimal? StockCount { get; set; }
        public decimal? StockSum { get; set; }
      
    }

    public class ReportSheet_top15_6FP
    {
        public string DataSource { get; set; }
        public long FileInfoId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public string Sheet { get; set; }
        public string Drug { get; set; }
        public string Manufacturer { get; set; }
        public string SourceCode { get; set; }
        public string PharmacySourceCode { get; set; }
        public string PharmacyName { get; set; }
        public string LegalName { get; set; }
        public string PostAddress { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string LegalAddress { get; set; }
        public string INN { get; set; }
        public string OtherInfo { get; set; }
        public string Distributor { get; set; }
        public string DistributorBranch { get; set; }    
       
        public string Date { get; set; }
        public string OFD { get; set; }
        public string CheckNumber { get; set; }
        public string DocumentCode { get; set; }
        public string CheckDate { get; set; }
        public string CheckTime { get; set; }
        public string FP { get; set; }
        public string FN { get; set; }
        public string FD { get; set; }
        public decimal? PriceWithoutDiscount { get; set; }
        public decimal? PriceWithDiscount { get; set; }
        public decimal? SumWithoutDiscount { get; set; }
        public decimal? SumDiscount { get; set; }
        public decimal? SumWithDiscount { get; set; }
        public decimal? CheckSum { get; set; }
        public decimal? SalesCount { get; set; }
        public decimal? SumWithoutNDS { get; set; }
        public decimal? SumProduct { get; set; }
        public string KKT { get; set; }
        public string RegKKT { get; set; }
    }

    [Table("RawData_Out", Schema = "Fact")]
    public class RawData_Out
    {
        [Key]
        public long Id { get; set; }
        public int? DistributionTypeId { get; set; }
        public Int16? DistributionTypeId_UserId { get; set; }
        public DateTime? DistributionTypeId_Date { get; set; }
    }
        [Table("RawData_Out_View", Schema = "Fact")]
    public class RawData_Out_View
    {
        [Key]
        public long Id { get; set; }

        public long? DataSourceId { get; set; }
        public int? DataSourceTypeId { get; set; }
        public int? CompanyId { get; set; }


        public int Year { get; set; }
        public int Month { get; set; }
        public string DataSourceType { get; set; }
        public string Company { get; set; }
        public string DataSource { get; set; }
        public string PharmacyName { get; set; }
        public string LegalName { get; set; }
        public string INN { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Address { get; set; }

        public int? GSId { get; set; }
        public int? PharmacyId { get; set; }

        public string EntityINN { get; set; }
        public string EntityName { get; set; }
        public string NetworkName { get; set; }
        public string Address_region { get; set; }
        public string Address_city { get; set; }
        public string Address_street { get; set; }
        public string PharmacyBrand { get; set; }
        public string Comments { get; set; }
        public string Spark { get; set; }
        public string Distributor { get; set; }
        public string DistributorBranch { get; set; }
        public string DistributionType { get; set; }
        public int? DistributionTypeId { get; set; }

        public decimal? StockCount { get; set; }
        public decimal? StockSum { get; set; }
        public decimal? PurchaseCount { get; set; }
        public decimal? PurchaseSum { get; set; }
        public decimal? SellingCount { get; set; }
        public decimal? SellingSum { get; set; }


        public Int16? DistributionTypeId_UserId { get; set; }
        public string DistributionTypeId_UserFullName { get; set; }
        public DateTime? DistributionTypeId_Date { get; set; }
    }

    /// <summary>
    /// CheckReloadFile
    /// </summary>
    public class CheckReloadFileInfo
    {
        [Key]
        public long Id { get; set; }
        public int CompanyId { get; set; }
        public string DataSource { get; set; }
        public DateTime DateInsert { get; set; }
        public string FilePath { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Cnt          { get; set; }   //Всего Кол-во записей
        public int CheckCnt     { get; set; }   //Кол-во чеков
        public int IsBad6FP     { get; set; }   //Некорректные или отсутствующие 6ФП
        public int CheckFound   { get; set; }   //Найденные чеки
        public int CheckInWork  { get; set; }   //Чеки в обработке
        public int CheckGood    { get; set; }   //Обработанные чеки(отправленные на классификацию)
        public int CheckIsBad   { get; set; }   //Ошибки при скачивании или парсинге
        public string CompanyName { get; set; }
        public virtual Comp Company { get; set; }
    }

    public class CheckSelectFilter
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public Comp Company { get; set; }
        public Project Project { get; set; }
        public DataSource DataSource { get; set; }
    }

 
    public class OFD_API
    {
        [Key]
        public Int16 Id { get; set; }
        public string OFD { get; set; }
        public string Site_Check { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
    }



}
