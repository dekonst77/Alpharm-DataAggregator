using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data.SqlTypes;

namespace DataAggregator.Domain.Model.GS
{
    [Table("GS", Schema = "dbo")]
    public class GS
    {
        [Key]
        [Display(Name = "GSId")]
        public int Id { get; set; }
        [Display(Name = "Адрес из лицензии")]
        public string Address { get; set; }
        [Display(Name = "ИНН юр. лица")]
        public string EntityINN { get; set; }
        [Display(Name = "Наименование юридического лица")]
        public string EntityName { get; set; }
        [Display(Name = "PharmacyId")]
        public int? PharmacyId { get; set; }
        [Display(Name = "Тип аптечного учреждения по форме (Аптека, Аптечный пункт, Аптечный киоск, Аптечный магазин)")]
        public string PharmacySellingPlaceType { get; set; }
        [Display(Name = "Бренд аптеки")]
        public string PharmacyBrand { get; set; }
        [Display(Name = "Номер аптеки")]
        public string PharmacyNumber { get; set; }
        [Display(Name = "Регион")]
        public string Address_region { get; set; }
        [Display(Name = "Индекс")]
        public string Address_index { get; set; }
        [Display(Name = "Город")]
        public string Address_city { get; set; }
        [Display(Name = "Улица")]
        public string Address_street { get; set; }
        [Display(Name = "Площадь помещения, кв. м")]
        public string Address_room_area { get; set; }
        [Display(Name = "Комментарий")]
        public string Address_comment { get; set; }
        [Display(Name = "Координаты")]
        public string Address_koor { get; set; }
        [Display(Name = "Номер помещения")]
        public string Address_room { get; set; }
        [Display(Name = "Этаж")]
        public string Address_float { get; set; }
        [Display(Name = "Комментарий")]
        public string Comment { get; set; }
        [Display(Name = "Имя Контакта")]
        public string ContactPersonFullname { get; set; }
        [Display(Name = "Телефон")]
        public string Phone { get; set; }
        [Display(Name = "ВебСайт")]
        public string Website { get; set; }
        [Display(Name = "Формат выкладки (открытая, закрытая, смешанная)")]
        public string FormatLayout { get; set; }
        [Display(Name = "Формат работы (Дискаунтер, Фарммаркет, Luxury, Киоск)")]
        public string WorkFormat { get; set; }
        [Display(Name = "Форма аренды")]
        public string LeaseForm { get; set; }
        [Display(Name = "Срок окончания аренды")]
        public DateTime LeaseExpirationDate { get; set; }
        [Display(Name = "Режим работы")]
        public string OperationMode { get; set; }
        [Display(Name = "Категоризация аптек")]
        public int ABC_Category { get; set; }
        [Display(Name = "Количество касс")]
        public int CashOfficeCount { get; set; }
        [Display(Name = "Оборот, за месяц, млн. руб")]
        public int MonthlyTurnover { get; set; }
        [Display(Name = "Средний чек, руб.")]
        public int AverageReceipt { get; set; }
        [Display(Name = "кол-во SKU, суммарно продающихся в сетевой/несетевой аптеке")]
        public int SKU_Count { get; set; }
        [Display(Name = "Брик")]
        public string BricksId { get; set; }
        [Display(Name = "Тип юр. лица")]
        public string EntityType { get; set; }
        [Display(Name = "ПКУ")]
        public bool PKU { get; set; }
        [Display(Name = "ECom")]
        public bool ECom { get; set; }
        [Display(Name = "Контроль п")]
        public string UserControl_Name { get; set; }
        [Display(Name = "Контроль дата")]
        public DateTime? UserControl_dt { get; set; }
        [Display(Name = "Категория")]
        public Byte PointCategoryId { get; set; }


        public GS()
        {
            Address = "";
            EntityINN = "";
            EntityName = "";
            EntityType = "";
            PharmacyId = null;
            PharmacySellingPlaceType = "";
            PharmacyBrand = "";
            PharmacyNumber = "";
            Address_region = "";
            Address_city = "";
            Address_index = "";
            Address_street = "";
            Address_comment = "";
            Address_float = "";
            Address_room = "";
            Address_room_area = "";
            Address_koor = "";
            //Address_koor_lat = 0;
            //Address_koor_long=0,
            ContactPersonFullname = "";
            Phone = "";
            Website = "";
            FormatLayout = "";
            WorkFormat = "";
            LeaseForm = "";
            LeaseExpirationDate = new DateTime(1900, 1, 1);
            OperationMode = "";
            ABC_Category = 0;
            CashOfficeCount = 0;
            MonthlyTurnover = 0;
            AverageReceipt = 0;
            SKU_Count = 0;
            BricksId = null;
            PKU = false;
            ECom = false;
            UserControl_Name = "";
            UserControl_dt = null;
            Comment = "";
            PointCategoryId = 10;
        }
    }
    [Table("PointCategory", Schema = "dbo")]
    public class PointCategory
    {
        //[Key]
        public Byte Id { get; set; }
        public string Name { get; set; }
    }
    public class GS_View_SP
    {

        [Key]
        [Display(Name = "GSId", GroupName = "Red")]
        public int Id { get; set; }
        [Display(Name = "PharmacyId", GroupName = "Green")]
        public int? PharmacyId { get; set; }
        [Display(Name = "Прозвон")]
        public string Calls { get; set; }
        [Display(Name = "Коммент")]
        public string Comment { get; set; }
        [Display(Name = "ИНН юр. лица")]
        public string EntityINN { get; set; }
        [Display(Name = "Юр. лицо", GroupName = "LightCyan")]
        public string EntityName { get; set; }
        [Display(Name = "Бренд аптеки")]
        public string PharmacyBrand { get; set; }
        [Display(Name = "Адрес из лицензии")]
        public string Address { get; set; }
        [Display(Name = "ФО", GroupName = "LightCyan")]
        public string Bricks_FederalDistrict { get; set; }
        /*  [Display(Name = "СФБ", GroupName = "LightCyan")]
          public string Bricks_FederationSubject { get; set; }*/
        [Display(Name = "СФ", GroupName = "Yellow")]
        public string Address_region { get; set; }
        [Display(Name = "МР/ГО", GroupName = "LightCyan")]
        public string Bricks_City { get; set; }
        [Display(Name = "Индекс", GroupName = "Yellow")]
        public string Address_index { get; set; }
        [Display(Name = "НП", GroupName = "Yellow")]
        public string Address_city { get; set; }
        [Display(Name = "Адрес", GroupName = "Yellow")]
        public string Address_street { get; set; }
        [Display(Name = "Ориентир", GroupName = "Yellow")]
        public string Address_comment { get; set; }
        [Display(Name = "Этаж", GroupName = "Yellow")]
        public string Address_float { get; set; }
        [Display(Name = "№ пом.", GroupName = "Yellow")]
        public string Address_room { get; set; }
        [Display(Name = "S пом., кв. м", GroupName = "Yellow")]
        public string Address_room_area { get; set; }
        [Display(Name = "Тип НП")]
        public string Bricks_CityType { get; set; }
        [Display(Name = "Координаты", GroupName = "Yellow")]
        public string Address_koor { get; set; }
        [Display(Name = "Брик", GroupName = "Green")]
        public string BricksId { get; set; }
        [Display(Name = "Текущий период", GroupName = "PapayaWhip")]
        public DateTime period { get; set; }
        [Display(Name = "-3м", GroupName = "NavajoWhite")]
        public bool? isExists_p3 { get; set; }
        [Display(Name = "-2м", GroupName = "PaleGoldenrod")]
        public bool? isExists_p2 { get; set; }

        [Display(Name = "-1м", GroupName = "PeachPuff")]
        public bool? isExists_p1 { get; set; }

        [Display(Name = "Работает", GroupName = "PapayaWhip")]
        public bool? isExists { get; set; }

        [Display(Name = "-3м Сеть", GroupName = "NavajoWhite")]
        public string NetworkName_p3 { get; set; }
        [Display(Name = "-2м Сеть", GroupName = "PaleGoldenrod")]
        public string NetworkName_p2 { get; set; }

        [Display(Name = "-1м Сеть", GroupName = "PeachPuff")]
        public string NetworkName_p1 { get; set; }

        [Display(Name = "Сеть", GroupName = "PapayaWhip")]
        public string NetworkName { get; set; }
        [Display(Name = "Дата добавления")]
        public DateTime? Date_Create { get; set; }
        [Display(Name = "Min Лицензия")]
        public DateTime? Lic_Period_First { get; set; }
        [Display(Name = "Max Лицензия")]
        public DateTime? Lic_Period { get; set; }
        [Display(Name = "Тип АУ")]
        public string PharmacySellingPlaceType { get; set; }
        [Display(Name = "Номер аптеки")]
        public string PharmacyNumber { get; set; }
        [Display(Name = "Телефон")]
        public string Phone { get; set; }
        [Display(Name = "Web")]
        public string Website { get; set; }
        [Display(Name = "Режим работы")]
        public string OperationMode { get; set; }
        [Display(Name = "Формат выкладки")]
        public string FormatLayout { get; set; }
        [Display(Name = "Формат работы")]
        public string WorkFormat { get; set; }
        [Display(Name = "ФИО зав. аптеки")]
        public string ContactPersonFullname { get; set; }
        [Display(Name = "ПКУ")]
        public bool PKU { get; set; }
        [Display(Name = "ECom")]
        public bool ECom { get; set; }
        [Display(Name = "ECom_License")]
        public string ECom_License { get; set; }
        [Display(Name = "сайты для e-com торговли")]
        public string ECOM_WWW { get; set; }
        [Display(Name = "приложение e-com")]
        public string ECOM_mobileApp { get; set; }
        /* [Display(Name = "Тип юр. лица", GroupName = "LightCyan")]
         public string EntityType { get; set; }*/

        /* [Display(Name = "Форма аренды")]
         public string LeaseForm { get; set; }*/
        /*    [Display(Name = "Срок окончания аренды")]
            public DateTime LeaseExpirationDate { get; set; }*/
        /*[Display(Name = "Категоризация аптек")]
        public int ABC_Category { get; set; }*/
        /* [Display(Name = "Количество касс")]
         public int CashOfficeCount { get; set; }*/
        /*[Display(Name = "Оборот, за месяц, млн. руб")]
        public int MonthlyTurnover { get; set; }*/
        /* [Display(Name = "Средний чек, руб.")]
         public int AverageReceipt { get; set; }*/
        /*[Display(Name = "кол-во SKU, суммарно продающихся в сетевой/несетевой аптеке")]
        public int SKU_Count { get; set; }*/


        [Display(Name = "Контроль п")]
        public string UserControl_Name { get; set; }
        [Display(Name = "Контроль дата")]
        public DateTime? UserControl_dt { get; set; }
        [Display(Name = "Категория")]
        public Byte PointCategoryId { get; set; }
        /* [Display(Name = "Страна", GroupName = "LightCyan")]
         public string Bricks_Country { get; set; }*/


        /* [Display(Name = "+Оборот", GroupName = "PapayaWhip")]
        public decimal Summa { get; set; }
        
        [Display(Name = "-Оборот-1месяц", GroupName = "PeachPuff")]
        public decimal Summa_p1 { get; set; }
        

         [Display(Name = "-Оборот-2месяца", GroupName = "PaleGoldenrod")]
        public decimal Summa_p2 { get; set; }


        [Display(Name = "-Оборот-3месяца", GroupName = "NavajoWhite")]
        public decimal Summa_p3 { get; set; }*/

        /* [Display(Name = "Лицензия номер")]
         public string licenses_number { get; set; }*/


        public void NotNull()
        {
            if (EntityINN == null) EntityINN = "";
            if (EntityName == null) EntityName = "";
            if (PharmacySellingPlaceType == null) PharmacySellingPlaceType = "";
            if (PharmacyBrand == null) PharmacyBrand = "";
            if (PharmacyNumber == null) PharmacyNumber = "";
            if (Address_index == null) Address_index = "";
            if (Address_street == null) Address_street = "";
            if (Address_city == null) Address_city = "";
            if (Address_koor == null) Address_koor = "0.000000, 0.000000";
            if (Address_region == null) Address_region = "";
            if (Address_comment == null) Address_comment = "";
            if (Address_float == null) Address_float = "";
            if (Address_room == null) Address_room = "";
            if (ContactPersonFullname == null) ContactPersonFullname = "";
            if (Phone == null) Phone = "";
            if (Website == null) Website = "";
            if (FormatLayout == null) FormatLayout = "";
            if (WorkFormat == null) WorkFormat = "";
            //if (LeaseForm == null) LeaseForm = "";
            if (Address_room_area == null) Address_room_area = "";
            if (OperationMode == null) OperationMode = "";
            //if (EntityType == null) EntityType = "";
            if (BricksId == "") BricksId = null;
            if (Comment == null) Comment = "";
            if (UserControl_Name == null) UserControl_Name = "";

            if (NetworkName == null) NetworkName = "";
            if (NetworkName_p1 == null) NetworkName_p1 = "";
            if (NetworkName_p2 == null) NetworkName_p2 = "";
            if (NetworkName_p3 == null) NetworkName_p3 = "";
        }
    }

    [Table("GS_Period", Schema = "dbo")]
    public class GS_Period
    {
        [Key]
        public int Id { get; set; }
        public int GSId { get; set; }
        public DateTime period { get; set; }
        public bool? isExists { get; set; }
        public string NetworkName { get; set; }
        public decimal Summa { get; set; }
        public decimal Summa_Region { get; set; }
        public decimal Summa_Start { get; set; }
        public string SourceData { get; set; }
        public GS_Period()
        {
            isExists = null;
            NetworkName = "";
            SourceData = "";
            Summa_Start = 0;
            Summa_Region = 0;
            Summa = 0;
        }
    }
    [Table("GS_Period_Lic_View", Schema = "dbo")]
    public class GS_Period_Lic_View
    {
        [Key]
        public int Id { get; set; }
        public int GSId { get; set; }
        public DateTime period { get; set; }
        public bool? isExists { get; set; }
        public string NetworkName { get; set; }
        public decimal Summa { get; set; }
        public string periods_text { get; set; }
        public string licenses_numbers_text { get; set; }
    }
    [Table("Calls", Schema = "dbo")]
    public class Calls
    {
        [Key]
        public int Id { get; set; }
        public string Creator_User { get; set; }
        public DateTime Creator_Date { get; set; }
        public int GSId { get; set; }
        public string Result_text { get; set; }
        public DateTime? Result_Date { get; set; }
        public string Result_User { get; set; }
        public bool IsOpen { get; set; }
    }
    [Table("Bricks", Schema = "dbo")]
    public class Bricks
    {
        [Key]
        public string Id { get; set; }
        public string L1_label { get; set; }
        public int L1_id { get; set; }
        public string L2_label { get; set; }
        public int L2_id { get; set; }
        public string L3_label { get; set; }
        public int L3_id { get; set; }
        public string L4_label { get; set; }
        public int L4_id { get; set; }
        public string L5_label { get; set; }
        public int L5_id { get; set; }
        public string L6_label { get; set; }
        public string CityType { get; set; }
        public int L6_id { get; set; }
        public string L7_label { get; set; }
        public string L7_label2 { get; set; }
        public int L7_id { get; set; }
        public string post_index { get; set; }
        public string comment { get; set; }
        public string Description { get; set; }

    }

    [Table("Bricks", Schema = "changelog")]
    public class changelogBricks
    {
        [Key]
        public int Id_change { get; set; }
        public DateTime dt_change { get; set; }
        public string desciption_change { get; set; }
        public string Id { get; set; }
        public string L1_label { get; set; }
        public int L1_id { get; set; }
        public string L2_label { get; set; }
        public int L2_id { get; set; }
        public string L3_label { get; set; }
        public int L3_id { get; set; }
        public string L4_label { get; set; }
        public int L4_id { get; set; }
        public string L5_label { get; set; }
        public int L5_id { get; set; }
        public string L6_label { get; set; }
        public string CityType { get; set; }
        public int L6_id { get; set; }
        public string L7_label { get; set; }
        public string L7_label2 { get; set; }
        public int L7_id { get; set; }
        public string post_index { get; set; }
        public string comment { get; set; }

    }



    [Table("Bricks_L3", Schema = "dbo")]
    public class Bricks_L3
    {
        [Key]
        public string Id { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public int L3_id { get; set; }
    }
    [Table("Pharmacy", Schema = "dbo")]
    public class Pharmacy
    {
        [Key]
        public int PharmacyId { get; set; }
        public DateTime date_add { get; set; }
        public int GSId_first { get; set; }

        public decimal? koor_широта { get; set; }
        public decimal? koor_долгота { get; set; }
        public DateTime? koor_DT { get; set; }
        public string Address { get; set; }
        public string BricksId { get; set; }

        public string fias_id_manual { get; set; }
        public string fias_code_manual { get; set; }
        //[Column(TypeName = "decimal(9, 6)")]
        public decimal? geo_lat_manual { get; set; }
        //[Column(TypeName = "decimal(9, 6)")]
        public decimal? geo_lon_manual { get; set; }
        public decimal? Address_koor_lat { get; set; }
        public decimal? Address_koor_long { get; set; }
        public string Address_region { get; set; }
        public string Address_city { get; set; }
        public string Address_index { get; set; }
        public string Address_street { get; set; }
        public string Address_comment { get; set; }
        public string Address_float { get; set; }
        public string Address_room { get; set; }
        public string Address_room_area { get; set; }
        public string fias_id { get; set; }
        public string Post_Index { get; set; }
    }
    [Table("Organization", Schema = "dbo")]
    public class Organization
    {
        [Key]
        public int Id { get; set; }
        public int? ActualId { get; set; }
        public string inn { get; set; }
        public string full_name { get; set; }
        public bool IsUseGS { get; set; }
        public string description { get; set; }
        public string address { get; set; }
        public string ogrn { get; set; }
        public string name { get; set; }
        public string form { get; set; }
        public string EntityType { get; set; }
        public string EntityName { get; set; }

        public string NetWorkName { get; set; }
        public string TypeOf { get; set; }
        public string VidOf { get; set; }
        public string Brand { get; set; }
        public string BricksId { get; set; }
        public string FIO { get; set; }
        public string Boss_Name { get; set; }
        public string Boss_Position { get; set; }
        public string Phone { get; set; }
        public DateTime? Date_registration { get; set; }
        public string Status { get; set; }
        public DateTime? Date_licvidation { get; set; }
        public string Vid_Action { get; set; }
        public string Info { get; set; }
        public bool IsCheck { get; set; }
        public bool IsUseLPU { get; set; }
     
        // public string WWW { get; set; }
    }
    [Table("Organization_without_INN", Schema = "dbo")]
    public class Organization_without_INN
    {
        [Key]
        public int Id { get; set; }
        public int? ActualId { get; set; }
        public string inn { get; set; }
        public string full_name { get; set; }
        public bool IsUseGS { get; set; }
        public string description { get; set; }
        public string address { get; set; }
        public string ogrn { get; set; }
        public string name { get; set; }
        public string form { get; set; }
        public string EntityType { get; set; }
        public string EntityName { get; set; }

        public string NetWorkName { get; set; }
        public string TypeOf { get; set; }
        public string VidOf { get; set; }
        public string Brand { get; set; }
        public string BricksId { get; set; }
        public string FIO { get; set; }
        public string Boss_Name { get; set; }
        public string Boss_Position { get; set; }
        public string Phone { get; set; }
        public DateTime? Date_registration { get; set; }
        public string Status { get; set; }
        public DateTime? Date_licvidation { get; set; }
        public string Vid_Action { get; set; }
        public string Info { get; set; }
        public bool IsCheck { get; set; }

        public bool IsUseLPU { get; set; }
        public string WWW { get; set; }
    }
    [Table("licenses_to_Use", Schema = "dbo")]
    public class licenses_to_Use
    {
        [Key]
        public int Id { get; set; }
        public string inn { get; set; }
        public string full_name_licensee { get; set; }
        public string address { get; set; }
        public string index { get; set; }
        public string region { get; set; }
        public string city { get; set; }
        public string street { get; set; }
        public string BricksId { get; set; }
        public DateTime date { get; set; }
        public DateTime? date_add { get; set; }
        public string number { get; set; }
        public string works { get; set; }
        public bool IsUse { get; set; }
        public int? PharmacyId { get; set; }
        public int? GSId { get; set; }
        public Guid? UserAppendToGS { get; set; }
        public DateTime? DateAppendToGS { get; set; }
    }
    [Table("spr_FormatLayout", Schema = "dbo")]
    public class spr_FormatLayout
    {
        [Key]
        public string Id { get; set; }
        public string Value { get; set; }

    }
    [Table("spr_PharmacySellingPlaceType", Schema = "dbo")]
    public class spr_PharmacySellingPlaceType
    {
        [Key]
        public string Id { get; set; }
        public string Value { get; set; }

    }
    [Table("spr_WorkFormat", Schema = "dbo")]
    public class spr_WorkFormat
    {
        [Key]
        public string Id { get; set; }
        public string Value { get; set; }

    }


    [Table("AlphaBitSums_Period", Schema = "dbo")]
    public class AlphaBitSums_Period
    {
        [Key]
        public long Id { get; set; }
        public int PharmacyId { get; set; }
        public string Supplier { get; set; }
        public DateTime Period { get; set; }
        public string Comment { get; set; }
        public bool IsUse { get; set; }
        public decimal RealSellingSum { get; set; }
        public bool LastSellingSum_IsUse { get; set; }
    }

    [Table("OFDSumms_Period", Schema = "dbo")]
    public class OFDSumms_Period
    {
        [Key]
        public long Id { get; set; }
        public int PharmacyId { get; set; }
        public decimal Kof_A1 { get; set; }
        public int BrickId { get; set; }
        public Int16 SupplierId { get; set; }
        public DateTime Period { get; set; }
        public string Comment { get; set; }
        public bool IsUse { get; set; }
        public decimal SummaBrick { get; set; }
        public int CountTran { get; set; }
        public decimal Kof_Brick { get; set; }
    }
    [Table("DistributorBranch", Schema = "dbo")]
    public class DistributorBranch
    {
        [Key]
        public int Id { get; set; }
        public string EntityINN { get; set; }
        public string EntityName { get; set; }
        public string DistributorBrand { get; set; }
        public string Name_Short { get; set; }
        public string Address_city { get; set; }
        public string Address_city_All { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address_street { get; set; }
        public string Web { get; set; }
        public string Comment { get; set; }
        public void NotNull()
        {
            if (EntityINN == null) EntityINN = "";
            if (EntityName == null) EntityName = "";
            if (DistributorBrand == null) DistributorBrand = "";
            if (Name_Short == null) Name_Short = "";
            if (Address_city == null) Address_city = "";
            if (Address_city_All == null) Address_city_All = "";
            if (Email == null) Email = "";
            if (Phone == null) Phone = "";
            if (Address_street == null) Address_street = "";
            if (Web == null) Web = "";
            if (Comment == null) Comment = "";
        }
    }
    [Table("SummsPeriod_OFD_SP", Schema = "dbo")]
    public class SummsPeriod_OFD_SP
    {
        [Key]
        public long Id { get; set; }
        public DateTime Period { get; set; }
        public Int16 SupplierId { get; set; }
        public int BrickId { get; set; }
        public int PharmacyId { get; set; }
        public string EntityName { get; set; }
        public decimal Kof_A1 { get; set; }
        public decimal Kof_Brick { get; set; }
        public decimal SummaBrick { get; set; }
        public int CountTran { get; set; }
        public int CountDays { get; set; }
        public int CountClassifierId { get; set; }
        public int HH_inMonth { get; set; }
        public int DD_inMonth { get; set; }
        public int HH_inMonth_G { get; set; }
        public int SChek_inMonth { get; set; }
        public int SChek_inMonth_G { get; set; }

        public double NewSUM { get; set; }

        public decimal? SummaBrick_M1 { get; set; }
        public int? CountTran_M1 { get; set; }
        public decimal? SummaBrick_M2 { get; set; }
        public int? CountTran_M2 { get; set; }
        public string Phids { get; set; }
        public bool IsUse { get; set; }
        public string Comment { get; set; }
        public string SourceData { get; set; }
        public string OperationMode { get; set; }
        public string NetworkName { get; set; }
        public string Address_region { get; set; }
        public string Address_city { get; set; }
        public string Address_street { get; set; }
        public decimal MonthlyTurnover { get; set; }
        public decimal MonthlyTurnover_G { get; set; }
        public double Check_AVG { get; set; }
        public double kof_Calc_Gs { get; set; }
        public decimal kof { get; set; }

        public string SourceData_M01 { get; set; }
        public decimal MonthlyTurnover_M01 { get; set; }
        public decimal? AL_RealSellingSum { get; set; }
        public string AL_Comment { get; set; }
        public void NotNull()
        {
            if (Comment == null) Comment = "";
            //if (IsUse == null) IsUse = false
        }
    }
    public class AlphaBitSums_Period_SP
    {
        [Key]
        public long Id { get; set; }
        public int PharmacyId { get; set; }
        public string Supplier { get; set; }
        public string Supplier_Add { get; set; }
        public DateTime Period { get; set; }
        public string Comment { get; set; }
        public string Comment_p1 { get; set; }
        public string Comment_p2 { get; set; }
        public string Address_city { get; set; }
        public string Address_region { get; set; }
        public string Address_street { get; set; }
        public bool? IsUse { get; set; }
        public bool? IsUse_p1 { get; set; }
        public bool? IsUse_p2 { get; set; }
        public int KOF { get; set; }

        public decimal? RealSellingSum_p4 { get; set; }
        public decimal? RealSellingSum_p5 { get; set; }
        public decimal? RealSellingSum_p6 { get; set; }
        public decimal? RealSellingSum_p7 { get; set; }
        public decimal? RealSellingSum_p8 { get; set; }
        public decimal? RealSellingSum_p9 { get; set; }
        public decimal? RealSellingSum_p10 { get; set; }
        public decimal? RealSellingSum_p11 { get; set; }
        public decimal? RealSellingSum_p12 { get; set; }

        [Display(Name = "-3м Сумма", GroupName = "NavajoWhite")]
        public decimal? RealSellingSum_p3 { get; set; }
        [Display(Name = "-2м Сумма", GroupName = "PaleGoldenrod")]
        public decimal? RealSellingSum_p2 { get; set; }

        [Display(Name = "-1м Сумма", GroupName = "PeachPuff")]
        public decimal? RealSellingSum_p1 { get; set; }

        [Display(Name = "Сумма", GroupName = "PapayaWhip")]
        public decimal? RealSellingSum { get; set; }
        public decimal? LastSellingSum { get; set; }
        public decimal? DeltaSellingSum { get; set; }

        [Display(Name = "-2м Работает", GroupName = "PaleGoldenrod")]
        public bool? isExists_p2 { get; set; }

        [Display(Name = "-1м Работает", GroupName = "PeachPuff")]
        public bool? isExists_p1 { get; set; }

        [Display(Name = "Работает", GroupName = "PapayaWhip")]
        public bool? isExists { get; set; }

        [Display(Name = "-2м Сеть", GroupName = "PaleGoldenrod")]
        public string NetworkName_p2 { get; set; }

        [Display(Name = "-1м Сеть", GroupName = "PeachPuff")]
        public string NetworkName_p1 { get; set; }

        [Display(Name = "Сеть", GroupName = "PapayaWhip")]
        public string NetworkName { get; set; }

        public bool LastSellingSum_IsUse { get; set; }

        public decimal? OFD_Sum { get; set; }
        public int? OFD_Tran { get; set; }
        public decimal? OFD_delta { get; set; }

        public void NotNull()
        {
            if (Comment == null) Comment = "";
            if (Comment_p1 == null) Comment_p1 = "";
            if (Comment_p2 == null) Comment_p2 = "";
        }
    }
    public class SummsPeriod_SP
    {
        [Key]
        public int GSId { get; set; }
        public int PharmacyId { get; set; }
        public string EntityINN { get; set; }
        public string EntityName { get; set; }
        public string PharmacyBrand { get; set; }
        public string Address_region { get; set; }
        public string Address_city { get; set; }
        public string Address { get; set; }
        public string Address_comment { get; set; }
        public string OperationMode { get; set; }

        public DateTime Date_Create { get; set; }
        public DateTime Period { get; set; }

        public string NetworkName_p0 { get; set; }
        public string NetworkName_p1 { get; set; }
        public string NetworkName_p2 { get; set; }
        public string NetworkName_p3 { get; set; }
        public string NetworkName_p4 { get; set; }
        public string NetworkName_p5 { get; set; }
        public string NetworkName_p6 { get; set; }
        public string NetworkName_p7 { get; set; }
        public string NetworkName_p8 { get; set; }
        public string NetworkName_p9 { get; set; }
        public string NetworkName_p10 { get; set; }
        public string NetworkName_p11 { get; set; }

        public bool? isExists_p0 { get; set; }
        public bool? isExists_p1 { get; set; }
        public bool? isExists_p2 { get; set; }
        public bool? isExists_p3 { get; set; }
        public bool? isExists_p4 { get; set; }
        public bool? isExists_p5 { get; set; }
        public bool? isExists_p6 { get; set; }
        public bool? isExists_p7 { get; set; }
        public bool? isExists_p8 { get; set; }
        public bool? isExists_p9 { get; set; }
        public bool? isExists_p10 { get; set; }
        public bool? isExists_p11 { get; set; }

        public decimal? Summa_Start_p0 { get; set; }
        public decimal? Summa_Start_p1 { get; set; }
        public decimal? Summa_Start_p2 { get; set; }

        public decimal? Summa_p0 { get; set; }
        public decimal? Summa_p1 { get; set; }
        public decimal? Summa_p2 { get; set; }
        public decimal? Summa_p3 { get; set; }
        public decimal? Summa_p4 { get; set; }
        public decimal? Summa_p5 { get; set; }
        public decimal? Summa_p6 { get; set; }
        public decimal? Summa_p7 { get; set; }
        public decimal? Summa_p8 { get; set; }
        public decimal? Summa_p9 { get; set; }
        public decimal? Summa_p10 { get; set; }
        public decimal? Summa_p11 { get; set; }

        public string SourceData_p0 { get; set; }
        public string SourceData_p1 { get; set; }
        public string SourceData_p2 { get; set; }
        public string SourceData_p3 { get; set; }
        public string SourceData_p4 { get; set; }
        public string SourceData_p5 { get; set; }
        public string SourceData_p6 { get; set; }
        public string SourceData_p7 { get; set; }
        public string SourceData_p8 { get; set; }
        public string SourceData_p9 { get; set; }
        public string SourceData_p10 { get; set; }
        public string SourceData_p11 { get; set; }

        public void NotNull()
        {
            if (NetworkName_p0 == null) NetworkName_p0 = "";
            if (NetworkName_p1 == null) NetworkName_p1 = "";
            if (NetworkName_p2 == null) NetworkName_p2 = "";
        }
    }
    public class SummsPeriod_SP_Simple
    {
        [Key]
        public int GSId { get; set; }
        public int PharmacyId { get; set; }
        public string EntityINN { get; set; }
        public string EntityName { get; set; }
        public string PharmacyBrand { get; set; }
        public string Address_region { get; set; }
        public string Address_city { get; set; }
        public string Address { get; set; }
        public string Address_comment { get; set; }
        public string OperationMode { get; set; }
        public DateTime Period { get; set; }

        public string NetworkName_p0 { get; set; }
        public string NetworkName_p1 { get; set; }
        public string NetworkName_p2 { get; set; }

        public bool? isExists_p0 { get; set; }
        public bool? isExists_p1 { get; set; }
        public bool? isExists_p2 { get; set; }


        public decimal? Summa_Start_p0 { get; set; }
        public decimal? Summa_Start_p1 { get; set; }
        public decimal? Summa_Start_p2 { get; set; }

        public decimal? Summa_p0 { get; set; }
        public decimal? Summa_p1 { get; set; }
        public decimal? Summa_p2 { get; set; }


        public string SourceData_p0 { get; set; }
        public string SourceData_p1 { get; set; }
        public string SourceData_p2 { get; set; }

        public void NotNull()
        {

        }
    }
    [Table("GS_Period_Region", Schema = "dbo")]
    public class GS_Period_Region
    {
        public int Id { get; set; }
        public string Region { get; set; }
        public DateTime Period { get; set; }
        public decimal Kof { get; set; }
        public decimal K_otkl { get; set; }
    }
    public class SummsPeriod_Region_SP
    {
        public int Id { get; set; }
        public string ClassStyle { get; set; }
        public string Region { get; set; }
        public string Fed_ok { get; set; }
        public DateTime Period { get; set; }
        public decimal Kof { get; set; }
        public decimal? Kof_calc { get; set; }
        public decimal? Year_prev { get; set; }
        public decimal? Year_now { get; set; }
        public decimal? Month_0 { get; set; }
        public decimal? Month_12 { get; set; }
        public decimal? Month_1 { get; set; }
        public decimal? Month_2 { get; set; }
        public decimal? Month_3 { get; set; }
        public decimal? Kof_dol_prev_year { get; set; }
        public decimal? Kof_dol_now_year { get; set; }
        public decimal? delta_Kof_dol { get; set; }
        public int? Count_GSId_0 { get; set; }
        public decimal? Kof_rost_month { get; set; }
        public decimal? Kof_rost_month12 { get; set; }
        public decimal? Kof_rost_common_prev_month { get; set; }
        public decimal? Kof_delta { get; set; }
        public decimal? Count_common_prev_month { get; set; }
        public decimal? K_otkl { get; set; }

        public decimal? Kof_rost_common_prev_month_ofd { get; set; }
        public decimal? Kof_delta_ofd { get; set; }
        public decimal? Count_common_prev_month_ofd { get; set; }
        public decimal? TotalSumm { get; set; }

    }
    [Table("GS_Period_Network_Anket", Schema = "dbo")]
    public class GS_Period_Network_Anket
    {
        public int Id { get; set; }
        public string NetworkName { get; set; }
        public string Region { get; set; }
        public DateTime Period { get; set; }
        public decimal Summa { get; set; }
        public int Point { get; set; }
    }
    [Table("GS_Period_Network", Schema = "dbo")]
    public class GS_Period_Network
    {
        public int Id { get; set; }
        public string NetworkName { get; set; }
        public string Region { get; set; }
        public DateTime Period { get; set; }
        public double Kof { get; set; }
    }
    public class SummsPeriod_Anket_SP
    {
        public long Id { get; set; }
        public string ClassStyle { get; set; }
        public string NetworkName { get; set; }
        public string Region { get; set; }
        public DateTime Period { get; set; }
        public double? Kof { get; set; }
        public double? Kof_calc { get; set; }
        public decimal? SummaStartYear { get; set; }
        public decimal? Summa_AlphaBitSumsStartYear { get; set; }

        public decimal? anketSum { get; set; }
        public int? anketPoint { get; set; }
        public decimal? Otkl_proc { get; set; }
        public decimal? Otkl_sum { get; set; }
        public decimal? ProcentInCountry { get; set; }
        public decimal? Summa_0 { get; set; }
        public int? Count_GSId_0 { get; set; }
        public decimal? Summa_1 { get; set; }
        public decimal? Summa_2 { get; set; }
        public decimal? Summa_3 { get; set; }
        public decimal? Summa_4 { get; set; }
        public decimal? Summa_5 { get; set; }
        public decimal? Summa_6 { get; set; }
        public decimal? Summa_7 { get; set; }
        public decimal? Summa_8 { get; set; }
        public decimal? Summa_9 { get; set; }
        public decimal? Summa_10 { get; set; }
        public decimal? Summa_11 { get; set; }
        public decimal? Summa_12 { get; set; }
    }
    public class SummsPeriod_Network_SP
    {
        public long Id { get; set; }
        public string ClassStyle { get; set; }
        public string NetworkName { get; set; }
        public DateTime Period { get; set; }
        public decimal? Kof { get; set; }
        public decimal? SummaStartYear { get; set; }
        public decimal? SummOFD { get; set; }
        public decimal? anketSum { get; set; }
        public int? anketPoint { get; set; }
        public int? periodPoint { get; set; }
        public decimal? SummaStartYearA { get; set; }
        public decimal? Summa { get; set; }
        public decimal? ProcentInCountry { get; set; }

        public decimal? SumByPoint_QP { get; set; }
        public decimal? Dol_3M_P { get; set; }
        public int? RTNG_3M_P { get; set; }
        public decimal? delta_SumByPoint_QP { get; set; }
        public decimal? delta_Dol_3M_P { get; set; }
        public int? delta_RTNG_3M_P { get; set; }


        public decimal? Summa_0 { get; set; }
        public decimal? SumByPointM_0 { get; set; }
        public int? Count_GSId_0 { get; set; }
        public int? Count_Region_0 { get; set; }
        public string NetworkType_0 { get; set; }

        public decimal? Summa_1 { get; set; }
        public decimal? SumByPointM_1 { get; set; }
        public int? Count_GSId_1 { get; set; }
        public int? Count_Region_1 { get; set; }
        public string NetworkType_1 { get; set; }

        public decimal? Summa_2 { get; set; }
        public decimal? SumByPointM_2 { get; set; }
        public int? Count_GSId_2 { get; set; }
        public int? Count_Region_2 { get; set; }
        public string NetworkType_2 { get; set; }

        public decimal? Summa_3 { get; set; }
        public decimal? SumByPointM_3 { get; set; }
        public int? Count_GSId_3 { get; set; }
        public int? Count_Region_3 { get; set; }
        public string NetworkType_3 { get; set; }

        public decimal? Summa_4 { get; set; }
        public decimal? SumByPointM_4 { get; set; }
        public int? Count_GSId_4 { get; set; }
        public int? Count_Region_4 { get; set; }
        public string NetworkType_4 { get; set; }

        public decimal? Summa_5 { get; set; }
        public decimal? SumByPointM_5 { get; set; }
        public int? Count_GSId_5 { get; set; }
        public int? Count_Region_5 { get; set; }
        public string NetworkType_5 { get; set; }

        public decimal? Summa_6 { get; set; }
        public decimal? SumByPointM_6 { get; set; }
        public int? Count_GSId_6 { get; set; }
        public int? Count_Region_6 { get; set; }
        public string NetworkType_6 { get; set; }

        public decimal? Summa_7 { get; set; }
        public decimal? SumByPointM_7 { get; set; }
        public int? Count_GSId_7 { get; set; }
        public int? Count_Region_7 { get; set; }
        public string NetworkType_7 { get; set; }

        public decimal? Summa_8 { get; set; }
        public decimal? SumByPointM_8 { get; set; }
        public int? Count_GSId_8 { get; set; }
        public int? Count_Region_8 { get; set; }
        public string NetworkType_8 { get; set; }

        public decimal? Summa_9 { get; set; }
        public decimal? SumByPointM_9 { get; set; }
        public int? Count_GSId_9 { get; set; }
        public int? Count_Region_9 { get; set; }
        public string NetworkType_9 { get; set; }

        public decimal? Summa_10 { get; set; }
        public decimal? SumByPointM_10 { get; set; }
        public int? Count_GSId_10 { get; set; }
        public int? Count_Region_10 { get; set; }
        public string NetworkType_10 { get; set; }

        public decimal? Summa_11 { get; set; }
        public decimal? SumByPointM_11 { get; set; }
        public int? Count_GSId_11 { get; set; }
        public int? Count_Region_11 { get; set; }
        public string NetworkType_11 { get; set; }
        public decimal? Summa_12 { get; set; }
        public decimal? SumByPointM_12 { get; set; }
        public int? Count_GSId_12 { get; set; }
        public int? Count_Region_12 { get; set; }
        public string NetworkType_12 { get; set; }

        public decimal? SumByPointM_С { get; set; }
    }

    [Table("History_Category", Schema = "adr")]
    public class History_Category
    {
        [Key]
        public byte Id { get; set; }
        public string Value { get; set; }
    }

    [Table("spr_OperationMode", Schema = "dbo")]
    public class spr_OperationMode
    {
        [Key]
        public string OperationMode { get; set; }
        public TimeSpan? Monday { get; set; }
        public TimeSpan? Tuesday { get; set; }
        public TimeSpan? Wednesday { get; set; }
        public TimeSpan? Thursday { get; set; }
        public TimeSpan? Friday { get; set; }
        public TimeSpan? Saturday { get; set; }
        public TimeSpan? Sunday { get; set; }

    }
    [Table("NetworkBrand", Schema = "dbo")]
    public class NetworkBrand
    {
        [Key]
        public int Id { get; set; }
        public string NetworkName { get; set; }
        public string PharmacyBrand { get; set; }
        public string Comment { get; set; }
        public bool Used { get; set; }
    }
    [Table("NetworkBrandView", Schema = "dbo")]
    public class NetworkBrandView
    {
        [Key]
        public int Id { get; set; }
        public string NetworkName { get; set; }
        public string PharmacyBrand { get; set; }
        public string Comment { get; set; }
        public bool Used { get; set; }

        public double? MonthlyTurnover { get; set; }

        public int PharmacyBrand_Count { get; set; }
        public string Associations { get; set; }
        public string Franchise { get; set; }
    }
    [Table("spr_NetworkName_Period", Schema = "dbo")]
    public class spr_NetworkName_Period
    {
        [Key]
        public int Id { get; set; }
        public int spr_NetworkNameId { get; set; }
        public DateTime period { get; set; }

        public double? PreferentialRecipesSalesSum { get; set; }
        public double? AverageReceipt { get; set; }
        public double? SKUTotalCount { get; set; }
        public double? Rx_Share { get; set; }
        public double? OTC_Share { get; set; }
        public double? BAD_Share { get; set; }
        public double? Other_Share { get; set; }
        public double? STM_Share { get; set; }
        public double? Ecom_Share { get; set; }
        public double? TotalSalesSum { get; set; }
    }
    [Table("spr_NetworkName", Schema = "dbo")]
    public class spr_NetworkName
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }
        public string Associations { get; set; }
        public string Franchise { get; set; }
        public string Comment { get; set; }

        public string CompanyDescription { get; set; }
        public string Brand { get; set; }
        public Int16? RegistrationYear { get; set; }
        public string Website { get; set; }
        public string TopManagerPosition { get; set; }
        public string TopManagerName { get; set; }
        public string OwnerName { get; set; }
        public string HeadOfficeLegalAddress { get; set; }
        public string HeadOfficeActualAddress { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Top5Distributors { get; set; }
        public string OtherInformation { get; set; }
        public string STM_Brands { get; set; }
        public string EntityInn { get; set; }


    }

    [Table("spr_NetworkNameView", Schema = "dbo")]
    public class spr_NetworkNameView
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }
        public string Associations { get; set; }
        public string Franchise { get; set; }

        public int? CountPharmacy { get; set; }
        [Key]
        public DateTime Period { get; set; }
        public double? MonthlyTurnover { get; set; }
        public string Comment { get; set; }

        public string CompanyDescription { get; set; }
        public string Brand { get; set; }
        public Int16? RegistrationYear { get; set; }
        public string Website { get; set; }
        public string TopManagerPosition { get; set; }
        public string TopManagerName { get; set; }
        public string OwnerName { get; set; }
        public string HeadOfficeLegalAddress { get; set; }
        public string HeadOfficeActualAddress { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Top5Distributors { get; set; }
        public string OtherInformation { get; set; }
        public string STM_Brands { get; set; }
        public string EntityInn { get; set; }

        public double? PreferentialRecipesSalesSum { get; set; }
        public double? AverageReceipt { get; set; }
        public double? SKUTotalCount { get; set; }
        public double? Rx_Share { get; set; }
        public double? OTC_Share { get; set; }
        public double? BAD_Share { get; set; }
        public double? Other_Share { get; set; }
        public double? STM_Share { get; set; }
        public double? Ecom_Share { get; set; }
        public double? TotalSalesSum { get; set; }

        //public object spr_NetworkName_Periods { get; set; }
        public int treeLevel { get; set; }
    }


    [Table("History_Status", Schema = "adr")]
    public class History_Status
    {
        [Key]
        public byte Id { get; set; }
        public string Value { get; set; }
    }
    [Table("History_coding_inwork", Schema = "adr")]
    public class History_coding_inwork
    {
        [Key]
        public int Id { get; set; }
        public Guid UserWork { get; set; }
        public string DataSourceType { get; set; }
        public string DataSource { get; set; }
        public string PharmacySourceCode { get; set; }
        public string PharmacyName { get; set; }
        public string LegalName { get; set; }
        public byte Category { get; set; }
        public string INN { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public int? GSId { get; set; }
        public int? LPUId { get; set; }
        public int? DistrId { get; set; }
        public int? PharmacyId { get; set; }
        public Guid? UserSetName { get; set; }
        public DateTime? UserSetDate { get; set; }

        public string NetworkName { get; set; }
        public string Address_region { get; set; }
        public string Address_city { get; set; }
        public string Address_street { get; set; }
        public string EntityINN { get; set; }
        public string EntityName { get; set; }
        public string PharmacyBrand { get; set; }
        public string Comments { get; set; }
        public string Spark { get; set; }
        public string Spark2 { get; set; }
        public string BricksId { get; set; }
        public byte Status { get; set; }
        public string TypeClients { get; set; }
        public DateTime Date_Add { get; set; }
        public byte? CheckStat { get; set; }
    }
    [Table("History_coding_inwork_View", Schema = "adr")]
    public class History_coding_inwork_View
    {
        [Key]
        public int Id { get; set; }
        public Guid UserWork { get; set; }
        public string FullName { get; set; }
        public string DataSourceType { get; set; }
        public string DataSource { get; set; }
        public string PharmacySourceCode { get; set; }
        public string PharmacyName { get; set; }
        public string LegalName { get; set; }
        public byte Category { get; set; }
        public string INN { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public int? GSId { get; set; }
        public int? LPUId { get; set; }
        public int? DistrId { get; set; }
        public int? PharmacyId { get; set; }
        public Guid? UserSetName { get; set; }
        public DateTime? UserSetDate { get; set; }

        public string BricksId { get; set; }
        public string NetworkName { get; set; }
        public string Address_region { get; set; }
        public string Address_city { get; set; }
        public string Address_street { get; set; }
        public string EntityINN { get; set; }
        public string EntityName { get; set; }
        public string PharmacyBrand { get; set; }
        public string Comments { get; set; }
        public string Spark { get; set; }
        public string Spark2 { get; set; }
        public byte Status { get; set; }
        public DateTime Date_Add { get; set; }

        public string TypeClients { get; set; }
        public DateTime? TypeClients_When { get; set; }
        public string TypeClients_WhoT { get; set; }
        public int? TypeClients_Who { get; set; }

        public byte? CheckStat { get; set; }
        public bool? GS_IsExists { get; set; }
    }
    [Table("History_coding", Schema = "adr")]
    public class History_coding
    {
        [Key]
        public int Id { get; set; }
        public string DataSourceType { get; set; }
        public string DataSource { get; set; }
        public string PharmacySourceCode { get; set; }
        public string PharmacyName { get; set; }
        public string LegalName { get; set; }
        public byte Category { get; set; }
        public string INN { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public int GSId { get; set; }
        public int? LPUId { get; set; }
        public int? DistrId { get; set; }
        public int? PharmacyId { get; set; }
        public Guid? UserSetName { get; set; }
        public DateTime? UserSetDate { get; set; }
        public string NetworkName { get; set; }
        public string Address_region { get; set; }
        public string Address_city { get; set; }
        public string Address_street { get; set; }
        public string EntityINN { get; set; }
        public string EntityName { get; set; }
        public string PharmacyBrand { get; set; }
        public string Comments { get; set; }
        public string Spark { get; set; }
        public string Spark2 { get; set; }
        public string BricksId { get; set; }
        public byte Status { get; set; }
        public string TypeClients { get; set; }
        public DateTime? TypeClients_When { get; set; }
        public int? TypeClients_Who { get; set; }
        public DateTime Date_Add { get; set; }
        public byte? CheckStat { get; set; }
    }
    [Table("History_SPR_GS_view", Schema = "adr")]
    public class History_SPR_GS_view
    {
        [Key]
        public int? GSId { get; set; }
        public int? LPUId { get; set; }
        public int? DistrId { get; set; }
        public int? PharmacyId { get; set; }
        public bool? isExists { get; set; }
        public string Address { get; set; }
        public string EntityINN { get; set; }
        public string EntityName { get; set; }
        public string Address_region { get; set; }
        public string Address_city { get; set; }
        public string Address_index { get; set; }
        public string Address_street { get; set; }
        public string Address_comment { get; set; }
        public string Address_float { get; set; }
        public string Address_room { get; set; }
        public string NetworkName { get; set; }
        public string PharmacyBrand { get; set; }
        public string PharmacyNumber { get; set; }
        public string BricksId { get; set; }


    }
    [Table("History_SPR_BigGS_view", Schema = "adr")]
    public class History_SPR_BigGS_view
    {
        [Key]
        public long? GSId { get; set; }
        public int? LPUId { get; set; }
        public int? DistrId { get; set; }
        public int? PharmacyId { get; set; }
        public bool? isExists { get; set; }
        public string Address { get; set; }
        public string EntityINN { get; set; }
        public string EntityName { get; set; }
        public string Address_region { get; set; }
        public string Address_city { get; set; }
        public string Address_index { get; set; }
        public string Address_street { get; set; }
        public string Address_comment { get; set; }
        public string Address_float { get; set; }
        public string Address_room { get; set; }
        public string NetworkName { get; set; }
        public string PharmacyBrand { get; set; }
        public string PharmacyNumber { get; set; }
        public string BricksId { get; set; }


    }

    [Table("History_SPR_LPU_view", Schema = "adr")]
    public class History_SPR_LPU_view
    {
        public int? GSId { get; set; }
        [Key]
        public int? LPUId { get; set; }
        public int? DistrId { get; set; }
        public int? PharmacyId { get; set; }
        public bool? isExists { get; set; }
        public string Address { get; set; }
        public string EntityINN { get; set; }
        public string EntityName { get; set; }
        public string Address_region { get; set; }
        public string Address_city { get; set; }
        public string Address_index { get; set; }
        public string Address_street { get; set; }
        public string Address_comment { get; set; }
        public string Address_float { get; set; }
        public string Address_room { get; set; }
        public string NetworkName { get; set; }
        public string PharmacyBrand { get; set; }
        public string PharmacyNumber { get; set; }
        public string BricksId { get; set; }


    }

    [Table("History_SPR_Distr_view", Schema = "adr")]
    public class History_SPR_Distr_view
    {

        public int? GSId { get; set; }
        public int? LPUId { get; set; }
        [Key]
        public int? DistrId { get; set; }
        public int? PharmacyId { get; set; }
        public bool? isExists { get; set; }
        public string Address { get; set; }
        public string EntityINN { get; set; }
        public string EntityName { get; set; }
        public string Address_region { get; set; }
        public string Address_city { get; set; }
        public string Address_index { get; set; }
        public string Address_street { get; set; }
        public string Address_comment { get; set; }
        public string Address_float { get; set; }
        public string Address_room { get; set; }
        public string NetworkName { get; set; }
        public string PharmacyBrand { get; set; }
        public string PharmacyNumber { get; set; }
        public string BricksId { get; set; }


    }

    [Table("History_SPR_Brick_view", Schema = "adr")]
    public class History_SPR_Brick_view
    {

        public int? GSId { get; set; }
        public int? LPUId { get; set; }

        public int? DistrId { get; set; }
        public int? PharmacyId { get; set; }
        public bool? isExists { get; set; }
        public string Address { get; set; }
        public string EntityINN { get; set; }
        public string EntityName { get; set; }
        public string Address_region { get; set; }
        public string Address_city { get; set; }
        public string Address_index { get; set; }
        public string Address_street { get; set; }
        public string Address_comment { get; set; }
        public string Address_float { get; set; }
        public string Address_room { get; set; }
        public string NetworkName { get; set; }
        public string PharmacyBrand { get; set; }
        public string PharmacyNumber { get; set; }
        [Key]
        public string BricksId { get; set; }
    }

    /// <summary>
    /// proc [dbo].[GetPharmacys]
    /// </summary>
    public class GetPharmacys_Result
    {
        public int PharmacyId { get; set; }
        public System.DateTime date_add { get; set; }
        public int GSId_first { get; set; }
        public Nullable<decimal> koor_широта { get; set; }
        public Nullable<decimal> koor_долгота { get; set; }
        public Nullable<System.DateTime> koor_DT { get; set; }
        public string Address { get; set; }
        public string BricksId { get; set; }
        public string fias_id_manual { get; set; }
        public string fias_code_manual { get; set; }
        public Nullable<decimal> geo_lat_manual { get; set; }
        public Nullable<decimal> geo_lon_manual { get; set; }
        public Nullable<decimal> Address_koor_lat { get; set; }
        public Nullable<decimal> Address_koor_long { get; set; }
        public string Address_region { get; set; }
        public string Address_city { get; set; }
        public string Address_index { get; set; }
        public string Address_street { get; set; }
        public string Address_comment { get; set; }
        public string Address_float { get; set; }
        public string Address_room { get; set; }
        public string Address_room_area { get; set; }
        public string fias_id { get; set; }
        public string Post_Index { get; set; }

        /// <summary>
        /// ГАР поля
        /// </summary>
        public Guid? HOUSEGUID { get; set; }
        public string SF { get; set; }
        public string Locality { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string GAR_Address { get; set; }
    }

}

