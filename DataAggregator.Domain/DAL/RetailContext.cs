using DataAggregator.Domain.Model.Retail;
using DataAggregator.Domain.Model.Retail.CTM;
using DataAggregator.Domain.Model.Retail.Dadata;
using DataAggregator.Domain.Model.Retail.ExternalInteraction;
using DataAggregator.Domain.Model.Retail.FileInfoService;
using DataAggregator.Domain.Model.Retail.history;
using DataAggregator.Domain.Model.Retail.QueryModel;
using DataAggregator.Domain.Model.Retail.Report;
using DataAggregator.Domain.Model.Retail.SalesSKUbySF;
using DataAggregator.Domain.Model.Retail.SendToClassification;
using DataAggregator.Domain.Model.Retail.View;
using DataAggregator.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataAggregator.Domain.DAL
{
    public class RegionGroupModel
    {
        [JsonPropertyName("id")]
        public string Code { get; set; }

        [JsonPropertyName("label")]
        public string Name { get; set; }

        [JsonPropertyName("districtId")]
        public short DistrictId { get; set; }
    }

    public class RetailContext : DbContext
    {
        public DbSet<Data> Data { get; set; }

        public DbSet<FileData> FileData { get; set; }

        public DbSet<FileInfo> FileInfo { get; set; }

        public DbSet<FileInfoLog> FileInfoLog { get; set; }

        public DbSet<Net> Net { get; set; }

        public DbSet<Pharmacy> Pharmacy { get; set; }

        public DbSet<RawData> RawData { get; set; }

        public DbSet<Source> Source { get; set; }

        public DbSet<Template> Template { get; set; }

        public DbSet<TemplateField> TemplateField { get; set; }

        public DbSet<TemplateFieldName> TemplateFieldName { get; set; }

        public DbSet<FileStatus> FileStatus { get; set; }

        public DbSet<Region> Region { get; set; }

        public DbSet<FederalDistrict> FederalDistrict { get; set; }
        public DbSet<SourcePharmacy> SourcePharmacy { get; set; }

        public DbSet<SourcePharmacyFile> SourcePharmacyFile { get; set; }

        public DbSet<SourcePharmacyGroup> SourcePharmacyGroup { get; set; }

        public DbSet<SourcePharmacyGroupFile> SourcePharmacyGroupFile { get; set; }


        public DbSet<PriceCheck> PriceCheck { get; set; }

        public DbSet<PriceRange> PriceRange { get; set; }

        public DbSet<PriceCheckView> PriceCheckView { get; set; }

        public DbSet<CalcInfo> CalcInfo { get; set; }

        public DbSet<CountCheck> CountCheck { get; set; }

        public DbSet<CountCheckView> CountCheckView { get; set; }

        public DbSet<TargetPharmacyBrandBlackListView> TargetPharmacyBrandBlackListView { get; set; }

        public DbSet<TargetPharmacyBrandBlackList> TargetPharmacyBrandBlackList { get; set; }

        public DbSet<PriceRule> PriceRule { get; set; }

        public DbSet<PriceRuleListView> PriceRuleListView { get; set; }

        public DbSet<CountRule> CountRule { get; set; }

        public DbSet<CountRuleView> CountRuleView { get; set; }

        public DbSet<CalcRuleModel> CalcRuleModel { get; set; }

        public DbSet<CountRuleFullVolume> CountRuleFullVolume { get; set; }

        public DbSet<CountRuleFullVolumeView> CountRuleFullVolumeView { get; set; }

        public DbSet<HasDataForAvgPrice> HasDataForAvgPrice { get; set; }

        public DbSet<FileInfoServiceErrorLog> FileInfoServiceErrorLog { get; set; }

        public DbSet<MarkupDefaultView> MarkupDefaultView { get; set; }

        public DbSet<MarkupDefault> MarkupDefault { get; set; }

        public DbSet<FullClassifierWithPrice> FullClassifierWithPrice { get; set; }

        public DbSet<JobInfoLog> JobInfoLog { get; set; }

        public DbSet<Report> Report { get; set; }


        public DbSet<ReportLauncher> ReportLauncher { get; set; }

        public DbSet<CountRuleUsed> CountRuleUsed { get; set; }

        public DbSet<CTMView> CTMView { get; set; }

        public DbSet<SourceBrandBlackList> SourceBrandBlackList { get; set; }

        #region dadata

        public DbSet<Result> Result { get; set; }

        public DbSet<SourceString> SourceString { get; set; }

        #endregion dadata

        #region ExternalInteraction

        public DbSet<ExternalLog> ExternalLog { get; set; }

        public DbSet<OrenlekGroup> OrenlekGroup { get; set; }

        public DbSet<RegionCode> RegionCode { get; set; }

        public DbSet<PharmacyDataView> PharmacyDataView { get; set; }

        public DbSet<PharmacySellingStructure> PharmacySellingStructure { get; set; }
        public DbSet<RegionPM12View> RegionPM12View { get; set; }

        public DbSet<RegionPM01View> RegionPM01View { get; set; }

        #endregion ExternalInteraction

        public List<FileInfoView> GetErrorInfo(int month, int year)
        {
            var query = string.Format(@"SELECT f.Id, f.FileName, f.Path, f.LastWriteTime, s.Id as [FileStatusId], s.Name as [FileStatus],s.Id as FileStatusId, ErrorLog.Description
                                        FROM dbo.FileInfo as f 
                                        INNER JOIN dbo.FileStatus as s ON f.FileStatusId = s.Id
                                        INNER JOIN 
                                        (
                                            SELECT lastLog.FileInfoId, l.Text as [Description]
                                            FROM 
                                        	(
                                        		SELECT  f.Id as FileInfoId, MAX(l.Id) as LogId
                                        		FROM dbo.FileInfo as f
                                        		left outer join dbo.FileInfoLog as l ON f.Id = l.FileInfoId
                                        		WHERE f.FileStatusId = 3
                                        		GROUP BY f.Id
                                        	) as lastLog
                                        	LEFT OUTER JOIN FileInfoLog as l ON lastLog.LogId = l.Id
                                        ) as ErrorLog ON f.Id = ErrorLog.FileInfoId
                                        WHERE 
                                        DATEDIFF(MONTH,DATEFROMPARTS(f.YearStart, f.MonthStart, 1), DATEFROMPARTS({0}, {1}, 1)) >= 0 AND 
                                        DATEDIFF(MONTH,DATEFROMPARTS(f.YearEnd, f.MonthEnd, 1), DATEFROMPARTS({0}, {1}, 1)) <= 0 ", year, month);


            return this.Database.SqlQuery<FileInfoView>(query).ToList();
        }

        public List<FileInfoView> GetFileInfo(int month, int year, long sourceId)
        {
            //            var query = string.Format(@"select fi.Id, s.Name SourceName, fi.Path, fi.FileStatusId, fil.Text Description from FileInfo fi
            //                                        inner join Source s on fi.SourceId = s.Id
            //                                        left join (select max(Id) Id, FileInfoId from FileInfoLog group by FileInfoId) afl on fi.Id = afl.FileInfoId
            //                                        left join FileInfoLog fil on afl.Id = fil.Id
            //                                        where DATEFROMPARTS(fi.YearStart, fi.MonthStart, 1) <= DATEFROMPARTS({0}, {1}, 1)
            //                                        and DATEFROMPARTS(fi.YearEnd, fi.MonthEnd, 1) >= DATEFROMPARTS({0}, {1}, 1)", year, month);


            var query = string.Format(@"SELECT f.Id, f.FileName, f.Path, f.LastWriteTime, s.Id as [FileStatusId], s.Name as [FileStatus],s.Id as FileStatusId, ErrorLog.Description
                                        FROM    FileInfo as f INNER JOIN
                                                FileStatus as s ON f.FileStatusId =s.Id LEFT JOIN
                                                (   SELECT l.FileInfoId, l.Text as [Description]
                                                    FROM    (   SELECT  l.FileInfoId, MAX(l.Id) as LogId
                                                                FROM    FileInfo as f INNER JOIN
                                                                        FileInfoLog as l ON f.Id = l.FileInfoId
                                                                WHERE   f.FileStatusId = 3 and l.IsError = 1
                                                                GROUP BY l.FileInfoId) as lastLog INNER JOIN
                                                            FileInfoLog as l  ON lastLog.LogId = l.Id) as ErrorLog ON f.Id = ErrorLog.FileInfoId
                                        WHERE	DATEDIFF(MONTH,DATEFROMPARTS(f.YearStart, f.MonthStart, 1) , DATEFROMPARTS({0}, {1}, 1)) >= 0 AND 
		                                                                    DATEDIFF(MONTH,DATEFROMPARTS(f.YearEnd, f.MonthEnd, 1) ,DATEFROMPARTS({0}, {1}, 1)) <= 0 AND 
                                                                            f.sourceId = {2}", year, month, sourceId);


            return this.Database.SqlQuery<FileInfoView>(query).ToList();
        }

        public async Task<List<RegionPM12View>> SearchRegionAsync(params string[] values)
        {
            return await RegionPM12View.Where(s => values.Any(v => (s.RegionPM12 + " " + s.FullName).Contains(v))).ToListAsync();
        }

        public async Task<List<RegionPM01View>> SearchRegionPM01Async(params string[] values)
        {
            return await RegionPM01View.Where(s => values.Any(v => (s.RegionPM01 + " " + s.FullName).Contains(v))).ToListAsync();
        }

        public List<CalcRuleModel> SearchDrugPriceRuleModel(long ClassifierId, int year, int month)
        {

            int period = year * 100 + month;


            var query = @"  select 
                                CAST(t.ClassifierId as bigint) as ClassifierId,                               
                                ev.BrandId,
                                ev.Brand,
                                ev.TradeName + ' ' + ev.DrugDescription as DrugDescription, 
                                ev.TradeName, 
                                ev.OwnerTradeMark as OwnerTradeMark, 
                                ev.Packer as Packer,
                                null as SellingSumNDS,
                                null as PurchaseSumNDS,
                                null as SellingCount,
                                null as PurchaseCount,
                                null as SellingSumNDSPart,
                                null as PurchaseSumNDSPart,
                                null as SellingCountPart,
                                null as PurchaseCountPart,
                                cast(0 as bit) as IsInCountRules                               
                            from
                            (
                               select rd.ClassifierId
                               from calc.Region0DataView as rd
                               where rd.YearMonth = " + period + @" group by rd.DrugId, rd.OwnerTradeMarkId, rd.PackerId
                            ) as t
                          
                            inner join Classifier.ExternalViewAllPeriod as ev 
                                on ev.ClassifierId = t.ClassifierId
                            where t.ClassifierId = " + ClassifierId;

            return this.CalcRuleModel.SqlQuery(query).ToList();
        }

        public List<CalcRuleModel> SearchDrugPriceRuleModel(string value, int year, int month)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "drugDescription", SqlDbType = SqlDbType.NVarChar, Value = value} ,
                new SqlParameter() { ParameterName = "year", SqlDbType = SqlDbType.Int, Value = year} ,
                new SqlParameter() { ParameterName = "month", SqlDbType = SqlDbType.Int, Value = month} ,
            }.Cast<object>().ToArray();

            return CalcRuleModel.SqlQuery("[dbo].[SearchDrugPriceRule] @drugDescription, @year, @month", parameters).ToList();

        }

        public List<CalcRuleModel> SearchDrugCountRuleModel(int classifierId, int brandId, int year, int month, string regionCode)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "classifierId", SqlDbType = SqlDbType.Int, Value = classifierId} ,
                new SqlParameter() { ParameterName = "brandId", SqlDbType = SqlDbType.Int, Value = brandId} ,
                new SqlParameter() { ParameterName = "year", SqlDbType = SqlDbType.Int, Value = year} ,
                new SqlParameter() { ParameterName = "month", SqlDbType = SqlDbType.Int, Value = month} ,
                new SqlParameter() { ParameterName = "regionCode", SqlDbType = SqlDbType.NVarChar, Value = regionCode ?? $"0"} ,
            }.Cast<object>().ToArray();

            return CalcRuleModel.SqlQuery("[dbo].[SearchDrugCountRule] @classifierId, @brandId, @year, @month, @regionCode", parameters).ToList();


        }

        public List<CalcRuleModel> SearchDrugCountRuleModel2(int brandId, int year, int month, string regionCode)
        {

            int period = (year - 2000) * 100 + month;

            var query = @"
                    select 
                    dop.ClassifierId,              
                    dop.BrandId,
                    evAll.Brand,
                    evAll.TradeName + ' ' + evAll.DrugDescription as DrugDescription, 
                    evAll.TradeName, 
                    evAll.OwnerTradeMark as OwnerTradeMark, 
                    evAll.Packer as Packer,
                    dop.SellingSumNDS,
                    dop.PurchaseSumNDS,
                    dop.SellingCount,
                    dop.PurchaseCount,
                    iif(dop.SellingSumNDS > 0, 100 * dop.SellingSumNDS/brand.SellingSumNDS, 0) as SellingSumNDSPart,
                    iif(dop.PurchaseSumNDS > 0, 100 * dop.PurchaseSumNDS/brand.PurchaseSumNDS, 0) as PurchaseSumNDSPart,
                    iif(dop.SellingCount > 0, 100 * dop.SellingCount/brand.SellingCount, 0) as SellingCountPart,
                    iif(dop.PurchaseCount > 0, 100 * dop.PurchaseCount/brand.PurchaseCount, 0) as PurchaseCountPart,
                    cast(case when cr.ClassifierId is null then 0 else 1 end as bit) as IsInCountRules             
                    from
                    (
                     select 
                       cast(rd.ClassifierId as int) as ClassifierId, 	               
                       ev.BrandId,
                       sum(SellingSumNDS)  as SellingSumNDS,
                       sum(PurchaseSumNDS) as PurchaseSumNDS,
                       sum(SellingCount)   as SellingCount,
                       sum(PurchaseCount)  as PurchaseCount
                     from calc.Region0DataView as rd	                
                     inner join Classifier.ExternalViewAllPeriod as ev on ev.ClassifierId = rd.ClassifierId
                     where ev.BrandId = " + brandId + @" and rd.YearMonth = " + period + @" 
                     group by rd.ClassifierId, ev.BrandId
                    ) as dop
                    inner join
                    (
                     select 
                       ev.BrandId,
                       sum(SellingSumNDS)  as SellingSumNDS,
                       sum(PurchaseSumNDS) as PurchaseSumNDS,
                       sum(SellingCount)   as SellingCount,
                       sum(PurchaseCount)  as PurchaseCount
                     from calc.Region0DataView as rd	                
                     inner join Classifier.ExternalViewAllPeriod as ev on ev.ClassifierId = rd.ClassifierId
                     where ev.BrandId = " + brandId + @" and rd.YearMonth = " + period + @" 
                     group by ev.BrandId
                    ) as brand on dop.BrandId = brand.BrandId               
                    inner join Classifier.ExternalViewAllPeriod as evAll on evAll.ClassifierId = dop.ClassifierId
        left outer join
        (
        	select
        		cr.ClassifierId						
        	from calc.CountRule as cr
        	where cr.Year = 2000 + " + year + @" and cr.Month = " + month + @" and cr.RegionRus = 1
        	group by cr.ClassifierId
        ) as cr
        on cr.ClassifierId = dop.ClassifierId
                ";

            var data = Database.SqlQuery<CalcRuleModel>(query).ToList();
            return data;
        }



        public HasDataForAvgPrice CheckDataForAvgPrice(int year, int month, string regionCode, bool msk, bool spb, bool rf, int classifierId)
        {
            var query = @"
                declare @year int = " + year + @";
                declare @month int = " + month + @";
                declare @regionCode nvarchar(10) = '" + regionCode + @"';
                declare @msk bit = " + (msk ? 1 : 0) + @";
                declare @spb bit = " + (spb ? 1 : 0) + @";
                declare @rf bit = " + (rf ? 1 : 0) + @";
                declare @ClassifierId bigint = " + classifierId + @";
              

                SELECT CAST([HasSellingData] as bit) as HasSellingData, CAST([HasPurchaseData] as bit) as HasPurchaseData
                FROM (            
                SELECT CAST(MAX(HasSellingData) as bit) as [HasSellingData], CAST(MAX(HasPurchaseData) as bit) as HasPurchaseData
				FROM
				(   SELECT cast(iif(sum(SellingSumNDS) is null, 0, 1) as int) as HasSellingData, cast(iif(sum(PurchaseSumNDS) is null, 0, 1) as int) as HasPurchaseData
					FROM RetailData.calc.Region12DataView as rd INNER JOIN 
					(
						SELECT	ci.Year, ci.Month
						FROM	calc.CalcInfo as ci
						WHERE	(@year-ci.Year)*12 + (@month-ci.Month) <= 6 and
								(@year-ci.Year)*12 + (@month-ci.Month) > 0) as ml on (ml.Year-2000)*100 + ml.Month = rd.YearMonth
						WHERE	rd.ClassifierId = @ClassifierId AND 
								(	rd.RegionCode = @regionCode OR 
									@msk = 1 AND rd.RegionCode like '77.%' OR 
									@spb = 1 and rd.RegionCode like '78.%' OR 
									@rf = 1)
				) as a
				UNION 
				SELECT 1 as HasSellingData, 1 as HasPurchaseData
				FROM RetailCalculation.[link].[Price_Etalon] 
				WHERE YEAR(Period) = @year and MONTH(Period) = @month and ClassifierId = @ClassifierId) as b
            ";

            HasDataForAvgPrice hasDataForAvgPrice = this.HasDataForAvgPrice.SqlQuery(query).ToList().FirstOrDefault();

            return hasDataForAvgPrice;
        }

        public async Task<List<AggregatedRawDataByDrugClear>> SearchRawDataByDrugClear(int startYear, int startMonth, int endYear, int endMonth, IEnumerable<int> drugClearIds, string drugName)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("p_start_year", startYear),
                new SqlParameter("p_start_month", startMonth),
                new SqlParameter("p_end_year", endYear),
                new SqlParameter("p_end_month", endMonth),
                new SqlParameter("p_drug_clear_ids", ProcedureHelper.GetXmlRows(drugClearIds)),
                new SqlParameter("p_drug_name", drugName)
            }.Cast<object>().ToArray();

            return await Database.SqlQuery<AggregatedRawDataByDrugClear>("[web].[SearchRawDataByDrugClear] @p_start_year, @p_start_month, @p_end_year, @p_end_month, @p_drug_clear_ids, @p_drug_name", parameters).ToListAsync();
        }

        public DataTable GetTargetPharmacyBrandBlackListTable()
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["RetailContext"].ConnectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 6000;
                    command.Connection = connection;
                    command.CommandText = "select TargetPharmacyId, BrandId, Brand from dbo.TargetPharmacyBrandBlackListView";

                    connection.Open();
                    var ds = new DataSet();
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds);

                    return ds.Tables[0];
                }
            }
        }

        public void ImportPharmacies_from_Excel(string filename)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;
                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@filename", SqlDbType.NVarChar).Value = filename;

                command.CommandText = "ImportPharmacies_from_Excel";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public RetailContext()
        {
            Database.SetInitializer<RetailContext>(null);
            Database.CommandTimeout = 0;
            Database.Log = (query) => Debug.Write(query);
        }

        public RetailContext(string APP)
        {
            Database.SetInitializer<RetailContext>(null);
            Database.Connection.ConnectionString += "APP=" + APP; //Чтобы триггер увидел, кто меняет
            Database.CommandTimeout = 0;
            Database.Log = (query) => Debug.Write(query);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<FileData>().Property(d => d.PurchasePrice).HasPrecision(18, 9);
            modelBuilder.Entity<FileData>().Property(d => d.PurchasePriceNDS).HasPrecision(18, 9);
            modelBuilder.Entity<FileData>().Property(d => d.PurchaseSum).HasPrecision(18, 9);
            modelBuilder.Entity<FileData>().Property(d => d.PurchaseSumNDS).HasPrecision(18, 9);
            modelBuilder.Entity<FileData>().Property(d => d.SellingPrice).HasPrecision(18, 9);
            modelBuilder.Entity<FileData>().Property(d => d.SellingPriceNDS).HasPrecision(18, 9);
            modelBuilder.Entity<FileData>().Property(d => d.SellingSum).HasPrecision(18, 9);
            modelBuilder.Entity<FileData>().Property(d => d.SellingSumNDS).HasPrecision(18, 9);
            modelBuilder.Entity<FileData>().Property(d => d.PurchaseCount).HasPrecision(18, 9);
            modelBuilder.Entity<FileData>().Property(d => d.SellingCount).HasPrecision(18, 9);
            modelBuilder.Entity<FileData>().Property(d => d.Barcode).HasMaxLength(20);

            modelBuilder.Entity<PriceCheck>().Property(e => e.AvgSellingPrice).HasPrecision(18, 9);
            modelBuilder.Entity<PriceCheck>().Property(e => e.AvgPurchasePrice).HasPrecision(18, 9);
            modelBuilder.Entity<PriceCheck>().Property(e => e.AvgSellingCount).HasPrecision(18, 9);
            modelBuilder.Entity<PriceCheck>().Property(e => e.AvgPurchaseCount).HasPrecision(18, 9);
            modelBuilder.Entity<PriceCheck>().Property(e => e.SellingSum).HasPrecision(37, 18);
            modelBuilder.Entity<PriceCheck>().Property(e => e.PurchaseSum).HasPrecision(37, 18);
            modelBuilder.Entity<PriceCheck>().Property(e => e.PriceDeviation).HasPrecision(18, 9);

            modelBuilder.Entity<PriceRange>().Property(e => e.PurchasePriceMin).HasPrecision(18, 9);
            modelBuilder.Entity<PriceRange>().Property(e => e.PurchasePriceMax).HasPrecision(18, 9);
            modelBuilder.Entity<PriceRange>().Property(e => e.SellingPriceMin).HasPrecision(18, 9);
            modelBuilder.Entity<PriceRange>().Property(e => e.SellingPriceMax).HasPrecision(18, 9);

            modelBuilder.Entity<PriceCheckView>().Property(e => e.AvgSellingPrice).HasPrecision(18, 9);
            modelBuilder.Entity<PriceCheckView>().Property(e => e.AvgPurchasePrice).HasPrecision(18, 9);
            modelBuilder.Entity<PriceCheckView>().Property(e => e.AvgSellingCount).HasPrecision(18, 9);
            modelBuilder.Entity<PriceCheckView>().Property(e => e.AvgPurchaseCount).HasPrecision(18, 9);
            modelBuilder.Entity<PriceCheckView>().Property(e => e.SellingSum).HasPrecision(37, 18);
            modelBuilder.Entity<PriceCheckView>().Property(e => e.PurchaseSum).HasPrecision(37, 18);
            modelBuilder.Entity<PriceCheckView>().Property(e => e.PriceDeviation).HasPrecision(18, 9);
            modelBuilder.Entity<PriceCheckView>().Property(e => e.PurchasePriceMin).HasPrecision(18, 9);
            modelBuilder.Entity<PriceCheckView>().Property(e => e.PurchasePriceMax).HasPrecision(18, 9);
            modelBuilder.Entity<PriceCheckView>().Property(e => e.SellingPriceMin).HasPrecision(18, 9);
            modelBuilder.Entity<PriceCheckView>().Property(e => e.SellingPriceMax).HasPrecision(18, 9);

            modelBuilder.Entity<CountCheck>().Property(e => e.Coefficient).HasPrecision(18, 9);
            modelBuilder.Entity<CountCheck>().Property(e => e.SellingCount).HasPrecision(36, 18);
            modelBuilder.Entity<CountCheck>().Property(e => e.AvgSellingCountHalfYear).HasPrecision(36, 18);

            modelBuilder.Entity<CountCheckView>().Property(e => e.Coefficient).HasPrecision(18, 9);
            modelBuilder.Entity<CountCheckView>().Property(e => e.AvgSellingCount).HasPrecision(36, 18);
            modelBuilder.Entity<CountCheckView>().Property(e => e.AvgSellingCountHalfYear).HasPrecision(36, 18);

            modelBuilder.Entity<PriceRule>().Property(e => e.SellingPriceMin).HasPrecision(18, 6);
            modelBuilder.Entity<PriceRule>().Property(e => e.SellingPriceMax).HasPrecision(18, 6);
            modelBuilder.Entity<PriceRule>().Property(e => e.PurchasePriceMin).HasPrecision(18, 6);
            modelBuilder.Entity<PriceRule>().Property(e => e.PurchasePriceMax).HasPrecision(18, 6);

            modelBuilder.Entity<PriceRuleListView>().Property(e => e.SellingPriceMin).HasPrecision(18, 6);
            modelBuilder.Entity<PriceRuleListView>().Property(e => e.SellingPriceMax).HasPrecision(18, 6);
            modelBuilder.Entity<PriceRuleListView>().Property(e => e.PurchasePriceMin).HasPrecision(18, 6);
            modelBuilder.Entity<PriceRuleListView>().Property(e => e.PurchasePriceMax).HasPrecision(18, 6);

            modelBuilder.Entity<MarkupDefault>().Property(e => e.Markup).HasPrecision(18, 6);
            modelBuilder.Entity<MarkupDefaultView>().Property(e => e.Markup).HasPrecision(18, 6);
        }


        /// <summary>
        /// Помечаем файл на перезагрузку
        /// </summary>
        /// <param name="id">Идентификатор FileInfoId</param>
        public void SetReloadFileInfo(long id)
        {
            var conStr = this.Database.Connection.ConnectionString;

            using (var connection = new SqlConnection(conStr))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 600;

                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;


                    command.Parameters.Add("@FileInfoId", SqlDbType.BigInt).Value = id;

                    command.CommandText = "[FileInfoService].[SetReload]";

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Помечаем файл на удаление
        /// </summary>
        /// <param name="id">Идентификатор FileInfoId</param>
        public void SetDeleteFileInfo(long id)
        {
            var conStr = this.Database.Connection.ConnectionString;

            using (var connection = new SqlConnection(conStr))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 600;

                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;


                    command.Parameters.Add("@FileInfoId", SqlDbType.BigInt).Value = id;

                    command.CommandText = "[FileInfoService].[SetDelete]";

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }


        public void Reset()
        {
            var conStr = this.Database.Connection.ConnectionString;

            using (var connection = new SqlConnection(conStr))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 600;

                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.CommandText = "[FileInfoService].[ResetFileLoad]";

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Процедура, которая ставит файл на перезагрузку
        /// </summary>
        /// <param name="id">Идентификатор FileInfoId</param>
        public void ReloadFileInfo(long id)
        {
            var conStr = this.Database.Connection.ConnectionString;

            using (var connection = new SqlConnection(conStr))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 600;

                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;


                    command.Parameters.Add("@Id", SqlDbType.BigInt).Value = id;

                    command.CommandText = "[FileInfoService].[ReloadFileInfo]";

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Удаляем файлИнфо - помечаем как отложенный
        /// </summary>
        /// <param name="id">Идентификатор FileInfoId</param>
        public void DeleteFileInfo(long id)
        {
            var conStr = this.Database.Connection.ConnectionString;

            using (var connection = new SqlConnection(conStr))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 600;

                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;


                    command.Parameters.Add("@Id", SqlDbType.BigInt).Value = id;

                    command.CommandText = "[FileInfoService].[DeleteFileInfo]";

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Запуск службы на загрузку данныз на обработку
        /// </summary>
        /// <param name="check">Если true, то служба запускается, в противном случае возращается только текущий статус job</param>
        /// <returns></returns>
        public string SendToClassificationRun(bool run)
        {

            SqlParameter outparam = new SqlParameter()
            {
                ParameterName = "out",
                SqlDbType = SqlDbType.NVarChar,
                Size = 500,
                Direction = System.Data.ParameterDirection.Output
            };

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("run", run? 1:0),
                outparam
            }.Cast<object>().ToArray();



            Database.ExecuteSqlCommand("exec [ControlALG].dbo.Start_Job 'SendToClassificationJob',@run,@out output", parameters);

            string value = (String)outparam.Value;
            value = value.Substring(0, value.IndexOf("The job"));

            return value;
        }


        /// <summary>
        /// Запуск службы на загрузку данныз на обработку
        /// </summary>
        /// <param name="check">Если true, то служба запускается, в противном случае возращается только текущий статус job</param>
        /// <returns></returns>
        public void SendToClassificationStop()
        {
            Database.ExecuteSqlCommand("exec [ControlALG].[dbo].[Stop_Job] 'SendToClassificationJob'");
        }

        #region Job
        public string JobStatus(string jobName)
        {
            SqlParameter outparam = new SqlParameter()
            {
                ParameterName = "out",
                SqlDbType = SqlDbType.NVarChar,
                Size = 500,
                Direction = System.Data.ParameterDirection.Output
            };

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("jobName", jobName),
                outparam
            }.Cast<object>().ToArray();


            Database.ExecuteSqlCommand("exec [ControlALG].dbo.Start_Job @jobName,0,@out output", parameters);

            string value = (String)outparam.Value;
            value = value.Substring(0, value.IndexOf("The job"));

            return value;
        }


        /// <summary>
        /// Запуск службы на загрузку данных на обработку
        /// </summary>
        /// <param name="check">Если true, то служба запускается, в противном случае возращается только текущий статус job</param>
        /// <returns></returns>
        public void JobRun(string jobName)
        {

            SqlParameter outparam = new SqlParameter()
            {
                ParameterName = "out",
                SqlDbType = SqlDbType.NVarChar,
                Size = 500,
                Direction = System.Data.ParameterDirection.Output
            };

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("jobName", jobName),
                outparam
            }.Cast<object>().ToArray();


            Database.ExecuteSqlCommand("exec [ControlALG].dbo.Start_Job @jobName,1,@out output", parameters);


        }


        /// <summary>
        /// Запуск службы на загрузку данныз на обработку
        /// </summary>
        /// <param name="check">Если true, то служба запускается, в противном случае возращается только текущий статус job</param>
        /// <returns></returns>
        public void JobStop(string jobName)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("jobName", jobName)
            }.Cast<object>().ToArray();

            Database.ExecuteSqlCommand("exec [ControlALG].[dbo].[Stop_Job] @jobName", parameters);
        }
        #endregion Job

        public List<long> CheckSimilarRule(long? Id, int Year, int Month, int? YearEnd, int? MonthEnd, int? ClassifierId, int? DistributionClassifierId, string RegionCode)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "id", SqlDbType = SqlDbType.Int, Value = Id ?? 0} ,
                new SqlParameter() { ParameterName = "year", SqlDbType = SqlDbType.Int, Value = Year} ,
                new SqlParameter() { ParameterName = "month", SqlDbType = SqlDbType.Int, Value = Month} ,
                new SqlParameter() { ParameterName = "yearEnd", SqlDbType = SqlDbType.Int, Value = (object)YearEnd ?? DBNull.Value},
                new SqlParameter() { ParameterName = "monthEnd", SqlDbType = SqlDbType.Int, Value = (object)MonthEnd ?? DBNull.Value} ,
                new SqlParameter() { ParameterName = "classifierId", SqlDbType = SqlDbType.Int, Value = ClassifierId} ,
                new SqlParameter() { ParameterName = "distributionClassifierId", SqlDbType = SqlDbType.Int, Value = DistributionClassifierId} ,
                new SqlParameter() { ParameterName = "regionCode", SqlDbType = SqlDbType.NVarChar, Value = RegionCode} ,
            }.Cast<object>().ToArray();

            return Database.SqlQuery<long>("[history].[CheckSimilarRule] @id, @year, @month,@yearEnd, @monthEnd, @classifierId, @distributionClassifierId, @regionCode", parameters).ToList();
        }

        public List<long> CheckRelatedRule(long? Id, int Year, int Month, int? YearEnd, int? MonthEnd, int? ClassifierId, int? DistributionClassifierId, string RegionCode)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "id", SqlDbType = SqlDbType.Int, Value = Id ?? 0} ,
                new SqlParameter() { ParameterName = "year", SqlDbType = SqlDbType.Int, Value = Year} ,
                new SqlParameter() { ParameterName = "month", SqlDbType = SqlDbType.Int, Value = Month} ,
                new SqlParameter() { ParameterName = "yearEnd", SqlDbType = SqlDbType.Int, Value = (object)YearEnd ?? DBNull.Value} ,
                new SqlParameter() { ParameterName = "monthEnd", SqlDbType = SqlDbType.Int, Value = (object)MonthEnd ?? DBNull.Value} ,
                new SqlParameter() { ParameterName = "classifierId", SqlDbType = SqlDbType.Int, Value = ClassifierId} ,
                new SqlParameter() { ParameterName = "distributionClassifierId", SqlDbType = SqlDbType.Int, Value = DistributionClassifierId} ,
                new SqlParameter() { ParameterName = "regionCode", SqlDbType = SqlDbType.NVarChar, Value = RegionCode} ,
            }.Cast<object>().ToArray();

            return Database.SqlQuery<long>("[history].[CheckRelatedRule] @id, @year, @month,@yearEnd, @monthEnd, @classifierId, @distributionClassifierId, @regionCode", parameters).ToList();
        }

        /// <summary>
        /// Импорт черного списка брендов - [RetailData].[dbo].[SourceBrandBlackList]
        /// </summary>
        /// <param name="month">месяц</param>
        /// <param name="year">год</param>
        /// <param name="filename">файл</param>
        public void ImportSourceBrandBlackList_from_Excel(int month, int year, string filename)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@month", SqlDbType.Int).Value = month;
                command.Parameters.Add("@year", SqlDbType.Int).Value = year;
                command.Parameters.Add("@filename", SqlDbType.NVarChar).Value = filename;

                command.CommandText = "ImportSourceBrandBlackList_from_Excel";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
        }

        #region CTM
        public IEnumerable<CTMView> LoadCTMView(int year, int month)
        {
            var ret = Database.SqlQuery<CTMView>("dbo.LoadCTM_SP @year, @month",
                new SqlParameter { ParameterName = "@year", SqlDbType = SqlDbType.Int, Value = year },
                new SqlParameter { ParameterName = "@month", SqlDbType = SqlDbType.Int, Value = month });
            return ret;
        }

        public IEnumerable<CTMView> LoadCTMRecord(DateTime period, CTMView record, string fieldname)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(record, options);
            Debug.WriteLine(json);
            return Database.SqlQuery<CTMView>("dbo.LoadCTMRecord_SP @period, @CTMRecord, @fieldname",
                new SqlParameter { ParameterName = "period", SqlDbType = SqlDbType.Date, Value = period },
                new SqlParameter { ParameterName = "CTMRecord", SqlDbType = SqlDbType.NVarChar, Value = json },
                new SqlParameter { ParameterName = "fieldname", SqlDbType = SqlDbType.NVarChar, Value = fieldname }
                );
        }

        public async Task<bool> Network_FromExcelAsync(string filename)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@guid", SqlDbType.NVarChar).Value = filename;

                command.CommandText = "dbo.[spr_NetworkName_FromExcel]";

                Database.Connection.Open();

                await command.ExecuteNonQueryAsync();
            }
            return true;
        }

        public async Task<List<Network>> GetAllNetworksAsync(int year, int month)
        {
            var ret = await Database.SqlQuery<Network>("Classifier.sp_LoadNetworkNameByPeriod @year, @month",
                new SqlParameter { ParameterName = "@year", SqlDbType = SqlDbType.Int, Value = year },
                new SqlParameter { ParameterName = "@month", SqlDbType = SqlDbType.Int, Value = month }).ToListAsync();
            return ret;
        }
        #endregion

        #region Продажи SKU по СФ
        public IEnumerable<ViewSalesSKUByFederationSubject_SP_Result> ViewSalesSKUByFederationSubject_SP(int year, short month, short? districtId, short? region_code, bool isExcel = false)
        {
            return Database.SqlQuery<ViewSalesSKUByFederationSubject_SP_Result>("[SalesSKU].[LoadSalesSKUByFederationSubject_SP] @year, @month, @districtId, @region_code, @isExcel",
                new SqlParameter { ParameterName = "@year", SqlDbType = SqlDbType.Int, Value = year },
                new SqlParameter { ParameterName = "@month", SqlDbType = SqlDbType.TinyInt, Value = month },
                new SqlParameter { ParameterName = "@districtId", SqlDbType = SqlDbType.TinyInt, Value = (object)districtId ?? DBNull.Value },
                new SqlParameter { ParameterName = "@region_code", SqlDbType = SqlDbType.TinyInt, Value = (object)region_code ?? DBNull.Value },
                new SqlParameter { ParameterName = "@isExcel", SqlDbType = SqlDbType.Bit, Value = isExcel }
                );
        }
        public IEnumerable<ViewSalesSKUByFederationSubject_SP_Result> ViewSalesSKUByFedSubGroupModel_SP(int year, short month, List<RegionGroupModel> regions, bool isExcel = false, string searchText = null)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            string regions_json = JsonSerializer.Serialize<List<RegionGroupModel>>(regions, options);
            return Database.SqlQuery<ViewSalesSKUByFederationSubject_SP_Result>("[SalesSKU].[LoadSalesSKUByFedSubGroupModel_SP] @year, @month, @regions, @isExcel, @searchText",
                new SqlParameter { ParameterName = "@year", SqlDbType = SqlDbType.Int, Value = year },
                new SqlParameter { ParameterName = "@month", SqlDbType = SqlDbType.TinyInt, Value = month },
                new SqlParameter { ParameterName = "@regions", SqlDbType = SqlDbType.VarChar, Value = regions_json },
                new SqlParameter { ParameterName = "@isExcel", SqlDbType = SqlDbType.Bit, Value = isExcel },
                new SqlParameter { ParameterName = "@searchText", SqlDbType = SqlDbType.VarChar, Value = (object)searchText ?? DBNull.Value }
                );
        }

        public void RecalcDistrData_SP(int year, short month)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "@year", SqlDbType = SqlDbType.Int, Value = year} ,
                new SqlParameter() { ParameterName = "@month", SqlDbType = SqlDbType.TinyInt, Value = month}
            }.Cast<object>().ToArray();

            Database.ExecuteSqlCommand("exec [SalesSKU].[RecalcDistrData_SP] @year, @month", parameters);
        }

        public void RecalcInitialData_SP(int year, short month)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "@year", SqlDbType = SqlDbType.Int, Value = year} ,
                new SqlParameter() { ParameterName = "@month", SqlDbType = SqlDbType.TinyInt, Value = month}
            }.Cast<object>().ToArray();

            Database.ExecuteSqlCommand("exec [SalesSKU].[RecalcInitialData_SP] @year, @month", parameters);
        }

        public void RecalcOFDData_SP(int year, short month)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "@year", SqlDbType = SqlDbType.Int, Value = year} ,
                new SqlParameter() { ParameterName = "@month", SqlDbType = SqlDbType.TinyInt, Value = month}
            }.Cast<object>().ToArray();

            Database.ExecuteSqlCommand("exec [SalesSKU].[RecalcOFDData_SP] @year, @month", parameters);
        }

        public void RecalcCalculatedData_SP(int year, short month)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "@year", SqlDbType = SqlDbType.Int, Value = year} ,
                new SqlParameter() { ParameterName = "@month", SqlDbType = SqlDbType.TinyInt, Value = month}
            }.Cast<object>().ToArray();

            Database.ExecuteSqlCommand("exec [SalesSKU].[RecalcCalculatedData_SP] @year, @month", parameters);
        }

        public void SalesCalculationAlgorithmByRegion(Nullable<System.DateTime> currPeriod, string regionName, Nullable<bool> isdebug)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "@currPeriod", SqlDbType = SqlDbType.Date, Value = currPeriod},
                new SqlParameter() { ParameterName = "@RegionName", SqlDbType = SqlDbType.VarChar, Size = 150, Value = regionName},
                new SqlParameter() { ParameterName = "@ClassifierId", SqlDbType = SqlDbType.BigInt, Value = DBNull.Value, IsNullable = true},                
                new SqlParameter() { ParameterName = "@isdebug", SqlDbType = SqlDbType.Bit, Value = isdebug}
            }.Cast<object>().ToArray();

            Database.ExecuteSqlCommand("exec [SalesSKU].[SalesCalculationAlgorithmByRegion] @currPeriod, @RegionName, @ClassifierId, @isdebug", parameters);
        }

        public IEnumerable<ViewSalesSKUByFederationSubject_SP_Result> Load_SalesSKUByFederationSubject_Record(ViewSalesSKUByFederationSubject_SP_Result record, string fieldname)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(record, options);
            Debug.WriteLine(json);
            return Database.SqlQuery<ViewSalesSKUByFederationSubject_SP_Result>("SalesSKU.Load_SalesSKUByFederationSubject_Record_SP @Record, @fieldname",
                new SqlParameter { ParameterName = "Record", SqlDbType = SqlDbType.NVarChar, Value = json },
                new SqlParameter { ParameterName = "fieldname", SqlDbType = SqlDbType.NVarChar, Value = fieldname }
                );
        }
        public DataTable Get_SalesSKUbySF_ListTable(int year, short month, short? districtId, short? region_code)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["RetailContext"].ConnectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 0;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@year", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@month", SqlDbType.TinyInt).Value = month;
                    command.Parameters.Add("@districtId", SqlDbType.TinyInt).Value = (object)districtId ?? DBNull.Value;
                    command.Parameters.Add("@region_code", SqlDbType.TinyInt).Value = (object)region_code ?? DBNull.Value;
                    command.Parameters.Add("@isExcel", SqlDbType.Int).Value = true;

                    command.CommandText = "[SalesSKU].[LoadSalesSKUByFederationSubject_SP]";

                    connection.Open();
                    command.ExecuteNonQuery();

                    var ds = new DataSet();
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds);

                    return ds.Tables[0];
                }
            }
        }
        public DataTable Get_SalesSKUbyFedSubGroupModel_ListTable(int year, short month, List<RegionGroupModel> regions, string searchText = null)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            string regions_json = JsonSerializer.Serialize<List<RegionGroupModel>>(regions, options);

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["RetailContext"].ConnectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 0;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@year", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@month", SqlDbType.TinyInt).Value = month;
                    command.Parameters.Add("@regions", SqlDbType.VarChar).Value = regions_json;
                    command.Parameters.Add("@searchText", SqlDbType.VarChar).Value = searchText;
                    command.Parameters.Add("@isExcel", SqlDbType.Int).Value = true;

                    command.CommandText = "[SalesSKU].[LoadSalesSKUByFedSubGroupModel_SP]";

                    connection.Open();
                    command.ExecuteNonQuery();

                    var ds = new DataSet();
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds);

                    return ds.Tables[0];
                }
            }
        }

        /// <summary>
        /// Импорт коэффициентов коррекции
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="currentperiod"></param>
        /// <returns></returns>
        public bool SalesSKUbySF_from_Excel(string filename, string currentperiod)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@filename", SqlDbType.NVarChar).Value = filename;
                command.Parameters.Add("@currentperiod", SqlDbType.NVarChar).Value = currentperiod;

                command.CommandText = "SalesSKU.SalesSKUbySF_from_Excel";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }

        /// <summary>
        /// Импорт цены по субъектам федерации
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="currentperiod"></param>
        /// <returns></returns>
        public bool Price_SalesSKUbySF_from_Excel(string filename, string currentperiod)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@filename", SqlDbType.NVarChar).Value = filename;
                command.Parameters.Add("@currentperiod", SqlDbType.Date).Value = currentperiod;

                command.CommandText = "SalesSKU.Price_SalesSKUbySF_from_Excel";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }

        /// <summary>
        /// Получить рейтинги по РФ и бренду
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns>DataTable</returns>
        public DataTable GetRatingByRFandBrand_Table(int year, short month)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["RetailContext"].ConnectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 0;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@year", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@month", SqlDbType.TinyInt).Value = month;

                    command.CommandText = "[SalesSKU].[GetRatingByRFandBrand]";

                    connection.Open();
                    command.ExecuteNonQuery();

                    var ds = new DataSet();
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds);

                    var tbl = ds.Tables[0];
                    tbl.Columns[1].Caption  = "Бренд";
                    tbl.Columns[2].Caption  = "Место в рейтинге, уп.";
                    tbl.Columns[3].Caption  = "Место в рейтинге, уп. -1";
                    tbl.Columns[4].Caption  = "Изменение рейтинга, уп.";
                    tbl.Columns[5].Caption  = "уп. тек.";
                    tbl.Columns[6].Caption  = "уп. -1";
                    tbl.Columns[7].Caption  = "Прирост, уп.";
                    tbl.Columns[8].Caption  = "Доля тек. от СФ";
                    tbl.Columns[9].Caption  = "Доля -1 от СФ";
                    tbl.Columns[10].Caption = "Место в рейтинге, руб.";
                    tbl.Columns[11].Caption = "Место в рейтинге, руб. -1";
                    tbl.Columns[12].Caption = "Изменение рейтинга, руб.";
                    tbl.Columns[13].Caption = "∑ руб.";
                    tbl.Columns[14].Caption = "∑ руб. -1";
                    tbl.Columns[15].Caption = "Прирост, руб.";
                    tbl.Columns[16].Caption = "Доля тек. от СФ";
                    tbl.Columns[17].Caption = "Доля -1 от СФ";

                    return ds.Tables[0];
                }
            }
        }

        public async Task<DataTable> GetRatingByRFandBrand_TableAsync(int year, short month)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["RetailContext"].ConnectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 0;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@year", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@month", SqlDbType.TinyInt).Value = month;

                    command.CommandText = "[SalesSKU].[GetRatingByRFandBrand]";

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    var ds = new DataSet();
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds);

                    var tbl = ds.Tables[0];
                    tbl.Columns[1].Caption = "Бренд";
                    tbl.Columns[2].Caption = "Место в рейтинге, уп.";
                    tbl.Columns[3].Caption = "Место в рейтинге, уп. -1";
                    tbl.Columns[4].Caption = "Изменение рейтинга, уп.";
                    tbl.Columns[5].Caption = "уп. тек.";
                    tbl.Columns[6].Caption = "уп. -1";
                    tbl.Columns[7].Caption = "Прирост, уп.";
                    tbl.Columns[8].Caption = "Доля тек. от СФ";
                    tbl.Columns[9].Caption = "Доля -1 от СФ";
                    tbl.Columns[10].Caption = "Место в рейтинге, руб.";
                    tbl.Columns[11].Caption = "Место в рейтинге, руб. -1";
                    tbl.Columns[12].Caption = "Изменение рейтинга, руб.";
                    tbl.Columns[13].Caption = "∑ руб.";
                    tbl.Columns[14].Caption = "∑ руб. -1";
                    tbl.Columns[15].Caption = "Прирост, руб.";
                    tbl.Columns[16].Caption = "Доля тек. от СФ";
                    tbl.Columns[17].Caption = "Доля -1 от СФ";

                    return ds.Tables[0];
                }
            }
        }

        /// <summary>
        /// Получить рейтинги по РФ и правообладателю
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns>DataTable</returns>
        public DataTable GetRatingByRFandOwnerTradeMark_Table(int year, short month)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["RetailContext"].ConnectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 0;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@year", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@month", SqlDbType.TinyInt).Value = month;

                    command.CommandText = "[SalesSKU].[GetRatingByRFandOwnerTradeMark]";

                    connection.Open();
                    command.ExecuteNonQuery();

                    var ds = new DataSet();
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds);

                    var tbl = ds.Tables[0];
                    tbl.Columns[1].Caption = "Правообладатель";
                    tbl.Columns[2].Caption = "Место в рейтинге, уп.";
                    tbl.Columns[3].Caption = "Место в рейтинге, уп. -1";
                    tbl.Columns[4].Caption = "Изменение рейтинга, уп.";
                    tbl.Columns[5].Caption = "уп. тек.";
                    tbl.Columns[6].Caption = "уп. -1";
                    tbl.Columns[7].Caption = "Прирост, уп.";
                    tbl.Columns[8].Caption = "Доля тек. от СФ";
                    tbl.Columns[9].Caption = "Доля -1 от СФ";
                    tbl.Columns[10].Caption = "Место в рейтинге, руб.";
                    tbl.Columns[11].Caption = "Место в рейтинге, руб. -1";
                    tbl.Columns[12].Caption = "Изменение рейтинга, руб.";
                    tbl.Columns[13].Caption = "∑ руб.";
                    tbl.Columns[14].Caption = "∑ руб. -1";
                    tbl.Columns[15].Caption = "Прирост, руб.";
                    tbl.Columns[16].Caption = "Доля тек. от СФ";
                    tbl.Columns[17].Caption = "Доля -1 от СФ";

                    return ds.Tables[0];
                }
            }
        }

        /// <summary>
        /// Получить рейтинги по СФ (субъектам федерации) и бренду
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns>DataTable</returns>
        public DataTable GetRatingBySubjectFederationAndBrand(int year, short month)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["RetailContext"].ConnectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 0;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@year", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@month", SqlDbType.TinyInt).Value = month;

                    command.CommandText = "[SalesSKU].[GetRatingBySubjectFederationAndBrand]";

                    connection.Open();
                    command.ExecuteNonQuery();

                    var ds = new DataSet();
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds);

                    var tbl = ds.Tables[0];
                    tbl.Columns[0].Caption = "СФ";
                    tbl.Columns[2].Caption = "Бренд";
                    tbl.Columns[3].Caption = "Место в рейтинге, уп.";
                    tbl.Columns[4].Caption = "Место в рейтинге, уп. -1";
                    tbl.Columns[5].Caption = "Изменение рейтинга, уп.";
                    tbl.Columns[6].Caption = "уп. тек.";
                    tbl.Columns[7].Caption = "уп. -1";
                    tbl.Columns[8].Caption = "Прирост, уп.";
                    tbl.Columns[9].Caption = "Доля тек. от СФ";
                    tbl.Columns[10].Caption = "Доля -1 от СФ";
                    tbl.Columns[11].Caption = "Место в рейтинге, руб.";
                    tbl.Columns[12].Caption = "Место в рейтинге, руб. -1";
                    tbl.Columns[13].Caption = "Изменение рейтинга, руб.";
                    tbl.Columns[14].Caption = "∑ руб.";
                    tbl.Columns[15].Caption = "∑ руб. -1";
                    tbl.Columns[16].Caption = "Прирост, руб.";
                    tbl.Columns[17].Caption = "Доля тек. от СФ";
                    tbl.Columns[18].Caption = "Доля -1 от СФ";

                    return ds.Tables[0];
                }
            }
        }

        /// <summary>
        /// Получить рейтинги по СФ (субъектам федерации) и правообладателю
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public DataTable GetRatingBySubjectFederationAndOwnerTradeMark(int year, short month)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["RetailContext"].ConnectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 0;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@year", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@month", SqlDbType.TinyInt).Value = month;

                    command.CommandText = "[SalesSKU].[GetRatingBySubjectFederationAndOwnerTradeMark]";

                    connection.Open();
                    command.ExecuteNonQuery();

                    var ds = new DataSet();
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds);

                    var tbl = ds.Tables[0];
                    tbl.Columns[0].Caption = "СФ";
                    tbl.Columns[2].Caption = "Правообладатель";
                    tbl.Columns[3].Caption = "Место в рейтинге, уп.";
                    tbl.Columns[4].Caption = "Место в рейтинге, уп. -1";
                    tbl.Columns[5].Caption = "Изменение рейтинга, уп.";
                    tbl.Columns[6].Caption = "уп. тек.";
                    tbl.Columns[7].Caption = "уп. -1";
                    tbl.Columns[8].Caption = "Прирост, уп.";
                    tbl.Columns[9].Caption = "Доля тек. от СФ";
                    tbl.Columns[10].Caption = "Доля -1 от СФ";
                    tbl.Columns[11].Caption = "Место в рейтинге, руб.";
                    tbl.Columns[12].Caption = "Место в рейтинге, руб. -1";
                    tbl.Columns[13].Caption = "Изменение рейтинга, руб.";
                    tbl.Columns[14].Caption = "∑ руб.";
                    tbl.Columns[15].Caption = "∑ руб. -1";
                    tbl.Columns[16].Caption = "Прирост, руб.";
                    tbl.Columns[17].Caption = "Доля тек. от СФ";
                    tbl.Columns[18].Caption = "Доля -1 от СФ";

                    return ds.Tables[0];
                }
            }
        }

        public DataTable Get_PricesByFederalSubjects_ListTable(int year, short month)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["RetailContext"].ConnectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 0;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@year", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@month", SqlDbType.TinyInt).Value = month;

                    command.CommandText = "[SalesSKU].[GetPricesByFederalSubjects]";

                    connection.Open();
                    command.ExecuteNonQuery();

                    var ds = new DataSet();
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(ds);

                    return ds.Tables[0];
                }
            }
        }
        #endregion
    }
}