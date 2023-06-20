using DataAggregator.Domain.Model.DrugClassifier.Bad;
using DataAggregator.Domain.Model.DrugClassifier.Changes;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.ClassifierCheckReport;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.FederalBenefit;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.Function;
using DataAggregator.Domain.Model.DrugClassifier.Classifier.View;
using DataAggregator.Domain.Model.DrugClassifier.DataAnalyzer;
using DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier;
using DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier.AddingDOPMonitoringDatabase;
using DataAggregator.Domain.Model.DrugClassifier.GoodsSystematization.View;
using DataAggregator.Domain.Model.DrugClassifier.InputData;
using DataAggregator.Domain.Model.DrugClassifier.Log;
using DataAggregator.Domain.Model.DrugClassifier.Log.FederalBenefit;
using DataAggregator.Domain.Model.DrugClassifier.Log.View;
using DataAggregator.Domain.Model.DrugClassifier.SearchTerms;
using DataAggregator.Domain.Model.DrugClassifier.Stat;
using DataAggregator.Domain.Model.DrugClassifier.Systematization;
using DataAggregator.Domain.Model.DrugClassifier.Systematization.View;
using DataAggregator.Domain.Model.Retail.SendToClassification;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace DataAggregator.Domain.DAL
{
    public class DrugClassifierContext : DbContext
    {

        public DbSet<MaskView> MaskView { get; set; }

        public DbSet<Mask> Mask { get; set; }

        #region Changes

        public DbSet<ClassifierTransfer> ClassifierTransfer { get; set; }

        public DbSet<ClassifierReplacement> ClassifierReplacement { get; set; }
        #endregion

        #region Classifier

        public DbSet<Generic> Generic { get; set; }
        public DbSet<ClassificationGeneric> ClassificationGeneric { get; set; }

        public DbSet<SQAView> SQAView { get; set; }

        public DbSet<SQA> SQA { get; set; }

        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<VEDChecked> VEDChecked { get; set; }

        //public DbSet<BrandClassification> BrandClassification { get; set; }

        public DbSet<VEDClassification> VEDClassification { get; set; }

        public DbSet<VedView> VedView { get; set; }

        public DbSet<VEDClassificationByTN> VEDClassificationByTN { get; set; }

        public DbSet<NoVedView> NoVedView { get; set; }

        public DbSet<VEDPeriod> VEDPeriod { get; set; }

        public DbSet<ClassifierInfo> ClassifierInfo { get; set; }

        public DbSet<ClassifierView> ClassifierView { get; set; }

        public DbSet<DataTransferClassifierView> DataTransferClassifierView { get; set; }

        public DbSet<TransferedDataView> TransferedDataView { get; set; }

        public DbSet<BadData> Bads { get; set; }

        public DbSet<ATCWho> ATCWho { get; set; }

        public DbSet<ATCEphmra> ATCEphmra { get; set; }
        public DbSet<MKB> MKB { get; set; }
        public DbSet<ATCWhoLinkMKBView> ATCWhoLinkMKBView { get; set; }

        public DbSet<ATCBaa> ATCBaa { get; set; }

        public DbSet<FTG> FTG { get; set; }

        public DbSet<NFC> NFC { get; set; }

        public DbSet<NFCLineView> NFCLineView { get; set; }
        public DbSet<RouteAdministration> RouteAdministration { get; set; }
        public DbSet<DDD_Norma> DDD_Norma { get; set; }
        public DbSet<DDD_Units> DDD_Units { get; set; }
        public DbSet<DDD_Units_Standart> DDD_Units_Standart { get; set; }
        public DbSet<DDDView> DDDView { get; set; }
        public DbSet<StandardUnitsView> StandardUnitsView { get; set; }
        public DbSet<EI> EI { get; set; }
        public DbSet<Dosage> Dosages { get; set; }

        public DbSet<FormProduct> FormProducts { get; set; }

        public DbSet<INN> INNs { get; set; }

        public DbSet<INNGroup> INNGroups { get; set; }

        public DbSet<Packing> Packings { get; set; }

        public DbSet<TradeMark> TradeMarks { get; set; }

        public DbSet<TradeName> TradeNames { get; set; }

        public DbSet<DosageGroup> DosageGroups { get; set; }

        public DbSet<INNDosage> INNDosage { get; set; }

        public DbSet<Drug> Drugs { get; set; }

        public DbSet<ProductionStage> ProductionStage { get; set; }

        public DbSet<RegistrationCertificate> RegistrationCertificates { get; set; }

        public DbSet<ClassifierPacking> ClassifierPacking { get; set; }

        public DbSet<InputType> InputTypes { get; set; }

        public DbSet<InputDataSource> InputDataSources { get; set; }

        public DbSet<InputDrugDescription> InputDrugDescriptions { get; set; }

        public DbSet<ClearDrugDescription> ClearDrugDescriptions { get; set; }

        public DbSet<HtmlSource> HtmlSources { get; set; }

        public DbSet<HtmlSourceOld> HtmlSourceOld { get; set; }

        public DbSet<LinkedTable> LinkedTables { get; set; }

        public DbSet<ActualClassifierView> ActualClassifierView { get; set; }

        public DbSet<ProductionInfo> ProductionInfo { get; set; }
        public DbSet<ProductionInfoView> ProductionInfoView { get; set; }

        public DbSet<ClassifierInfoHistory> ClassifierInfoHistory { get; set; }


        public DbSet<RegistrationCertificateView> RegistrationCertificateView { get; set; }

        public DbSet<CirculationPeriod> CirculationPeriod { get; set; }

        public DbSet<DrugType> DrugType { get; set; }

        public DbSet<Manufacturer> Manufacturer { get; set; }
        public DbSet<ManufacturerClear> ManufacturerClear { get; set; }

        public DbSet<INNGroup_INN> INNGroup_INN { get; set; }

        public DbSet<SystematizationView> SystematizationView { get; set; }
        public DbSet<ExternalView_FULL> ExternalView_FULL { get; set; }

        public DbSet<RealPacking> RealPacking { get; set; }

        public DbSet<ActionType> ActionType { get; set; }

        public DbSet<ActionLog> ActionLog { get; set; }

        public DbSet<GoodsActionLog> GoodsActionLog { get; set; }

        public DbSet<Replacement> Replacement { get; set; }

        public DbSet<Brand> Brand { get; set; }

        public DbSet<Country> Country { get; set; }

        public DbSet<Corporation> Corporation { get; set; }

        public DbSet<DrugClassification> DrugClassification { get; set; }
        public DbSet<ExternalViewAllPeriod> ExternalViewAllPeriod { get; set; }

        public DbSet<Localization> Localization { get; set; }

        public virtual IList<GetLocalizationByManufacturerTable_Result> GetLocalizationByManufacturerTable(Nullable<long> iD)
        {
            SqlParameter param = new SqlParameter("@Id", iD);
            return Database.SqlQuery<GetLocalizationByManufacturerTable_Result>("select Id, Value from Classifier.GetLocalizationByManufacturerTable (@ID)", param).ToList();
        }

        public IEnumerable<ClassifierEditorFilterView> GetClassifierEditorView_Result(string filter)
        {
            return Database.SqlQuery<ClassifierEditorFilterView>("[Classifier].[GetClassifierEditorView] @Filter",
                new SqlParameter { ParameterName = "@Filter", SqlDbType = SqlDbType.NVarChar, Value = filter }
                );
        }

        #region Blister Block

        public DbSet<BlisterBlockView> BlisterBlockView { get; set; }
        public DbSet<ClassifierPacking_BlisterBlock_View> ClassifierPacking_BlisterBlock_View { get; set; }
        public DbSet<BlisterBlock> BlisterBlock { get; set; }

        #endregion

        /// <summary>
        /// Отчет проверки классификатора
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ClassifierCheckReportExceptionListResult> ClassifierCheckReport_SP(string reportName)
        {
            return Database.SqlQuery<ClassifierCheckReportExceptionListResult>("[report].[ClassifierCheckReport] {0}", reportName);
        }

        #endregion

        #region FederalBenefit

        public DbSet<FederalBenefitChecked> FederalBenefitChecked { get; set; }
        public DbSet<FederalBenefitClassification> FederalBenefitClassification { get; set; }
        public DbSet<FederalBenefitPeriod> FederalBenefitPeriod { get; set; }

        public DbSet<FederalBenefitChange> FederalBenefitChange { get; set; }
        public DbSet<FederalBenefitCheckedChange> FederalBenefitCheckedChange { get; set; }
        public DbSet<FederalBenefitPeriodChange> FederalBenefitPeriodChange { get; set; }

        public DbSet<FederalBenefitView> FederalBenefitView { get; set; }
        public DbSet<NoFederalBenefitView> NoFederalBenefitView { get; set; }

        #endregion

        #region GoodsClassifier

        public DbSet<Goods> Goods { get; set; }

        //public DbSet<Brand> GoodsBrand { get; set; }

        public DbSet<GoodsBrandClassification> GoodsBrandClassification { get; set; }

        public DbSet<GoodsProductionInfoParameter> GoodsProductionInfoParameter { get; set; }

        public DbSet<Parameter> Parameter { get; set; }

        public DbSet<ParameterGroup> ParameterGroup { get; set; }

        public DbSet<GoodsProductionInfo> GoodsProductionInfo { get; set; }

        public DbSet<GoodsTradeName> GoodsTradeName { get; set; }

        public DbSet<GoodsSystematizationView> GoodsSystematizationView { get; set; }

        /// <summary>
        /// проц-ра [GoodsClassifier].[GetDOPBlockingForMonitoringDatabase]
        /// <remark>(СКЮ + блокировки)</remark>
        /// </summary>
        /// <returns>таблица</returns>
        public IEnumerable<GetDOPBlockingForMonitoringDatabase_Result> GetDOPBlockingForMonitoringDatabase_Result()
        {
            return Database.SqlQuery<GetDOPBlockingForMonitoringDatabase_Result>("[GoodsClassifier].[GetDOPBlockingForMonitoringDatabase]");
        }

        /// <summary>
        /// проц-ра [GoodsClassifier].[GetBlocking_Result]
        /// <remark>(только блокировки)</remark>
        /// </summary>
        /// <returns>таблица блокировок</returns>
        public IEnumerable<GetBlocking_Result> GetBlocking_Result()
        {
            return Database.SqlQuery<GetBlocking_Result>("[GoodsClassifier].[GetBlocking]");
        }

        /// <summary>
        /// Поставить заглушку целой категории
        /// </summary>
        /// <param name="GoodsCategoryId"></param>
        public void SetPlugOnByCategory_SP(long GoodsCategoryId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "@GoodsCategoryId", SqlDbType = SqlDbType.BigInt, Value = GoodsCategoryId}
            }.Cast<object>().ToArray();

            Database.ExecuteSqlCommand("exec [GoodsClassifier].[SetPlugOnByCategory] @GoodsCategoryId", parameters);
        }

        /// <summary>
        /// Поставить заглушку целой категории + доп. свойство
        /// </summary>
        /// <param name="GoodsCategoryId"></param>
        /// <param name="ParameterID"></param>
        public void SetPlugOnByCategoryAndProperty_SP(long GoodsCategoryId, long ParameterID)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "@GoodsCategoryId", SqlDbType = SqlDbType.BigInt, Value = GoodsCategoryId},
                new SqlParameter() { ParameterName = "@ParameterID", SqlDbType = SqlDbType.BigInt, Value = ParameterID}
            }.Cast<object>().ToArray();

            Database.ExecuteSqlCommand("exec [GoodsClassifier].[SetPlugOnByCategoryAndProperty] @GoodsCategoryId, @ParameterID", parameters);
        }

        /// <summary>
        /// Снять заглушку c категории
        /// </summary>
        /// <param name="GoodsCategoryId"></param>
        /// <param name="PouringStartDate"></param>
        public void SetPlugOffByCategory_SP(long GoodsCategoryId, DateTime PouringStartDate)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "@GoodsCategoryId", SqlDbType = SqlDbType.BigInt, Value = GoodsCategoryId},
                new SqlParameter() { ParameterName = "@PouringStartDate", SqlDbType = SqlDbType.Date, Value = PouringStartDate}
            }.Cast<object>().ToArray();

            Database.ExecuteSqlCommand("exec [GoodsClassifier].[SetPlugOffByCategory] @GoodsCategoryId, @PouringStartDate", parameters);
        }


        public void SetPlugOnByClassifierList_SP(long[] ClassifierIdList)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "@ClassifierIdList", SqlDbType = SqlDbType.VarChar, Value = String.Join(",", ClassifierIdList)}
            }.Cast<object>().ToArray();

            Database.ExecuteSqlCommand("exec [GoodsClassifier].[SetPlugOnByClassifierList] @ClassifierIdList", parameters);
        }

        /// <summary>
        /// Снять заглушку c категории + доп. свойство
        /// </summary>
        /// <param name="GoodsCategoryId"></param>
        /// <param name="ParameterID"></param>
        /// <param name="PouringStartDate"></param>
        public void SetPlugOffByCategoryAndProperty_SP(long GoodsCategoryId, long ParameterID, DateTime PouringStartDate)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "@GoodsCategoryId", SqlDbType = SqlDbType.BigInt, Value = GoodsCategoryId},
                new SqlParameter() { ParameterName = "@ParameterID", SqlDbType = SqlDbType.BigInt, Value = ParameterID},
                new SqlParameter() { ParameterName = "@PouringStartDate", SqlDbType = SqlDbType.Date, Value = PouringStartDate}
            }.Cast<object>().ToArray();

            Database.ExecuteSqlCommand("exec [GoodsClassifier].[SetPlugOffByCategoryAndProperty] @GoodsCategoryId, @ParameterID, @PouringStartDate", parameters);
        }

        public void SetPlugOffByClassifierList_SP(long[] ClassifierIdList, DateTime PouringStartDate)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "@ClassifierIdList", SqlDbType = SqlDbType.VarChar, Value = String.Join(",", ClassifierIdList)},
                new SqlParameter() { ParameterName = "@PouringStartDate", SqlDbType = SqlDbType.Date, Value = PouringStartDate}
            }.Cast<object>().ToArray();

            Database.ExecuteSqlCommand("exec [GoodsClassifier].[SetPlugOffByClassifierList] @ClassifierIdList, @PouringStartDate", parameters);
        }
        #endregion

        #region Systematization

        public DbSet<DrugClassifier> DrugClassifier { get; set; }

        public DbSet<DrugClassifierInWork> DrugClassifierInWork { get; set; }

        public DbSet<DrugClassifierRobot> DrugClassifierRobot { get; set; }

        public DbSet<DrugClear> DrugClear { get; set; }

        public DbSet<DrugRaw> DrugRaw { get; set; }

        public DbSet<DrugInWorkView> DrugInWorkView { get; set; }

        public DbSet<User> User { get; set; }

        public DbSet<Status> Status { get; set; }

        //public DbSet<StatusHistory> StatusHistory { get; set; }

        public DbSet<Robot> Robot { get; set; }

        public DbSet<Source> Source { get; set; }
        public DbSet<PrioritetWords> PrioritetWords { get; set; }
        public DbSet<PrioritetDrugClassifier> PrioritetDrugClassifier { get; set; }

        public DbSet<UserSource> UserSource { get; set; }

        public DbSet<Period> Period { get; set; }

        public DbSet<ClassifierEditorFilterView> ClassifierEditorFilterView { get; set; }
        public DbSet<ClassifierEditorFilterClassifierPackingView> ClassifierEditorFilterClassifierPackingView { get; set; }

        public DbSet<DrugClearPeriod> DrugClearPeriod { get; set; }

        public DbSet<SourceStat> SourceStat { get; set; }

        public DbSet<PrioritetWordsWithQueueView> PrioritetWordsWithQueueView { get; set; }

        #endregion Systematization

        #region GoodsSystematization

        public DbSet<GoodsClassifierInWork> GoodsClassifierInWork { get; set; }
        public DbSet<GoodsInWorkView> GoodsInWorkView { get; set; }

        public DbSet<GoodsSection> GoodsSection { get; set; }
        public DbSet<GoodsCategory> GoodsCategory { get; set; }
        public DbSet<GoodsCategoryKeyword> GoodsCategoryKeyword { get; set; }

        public DbSet<GoodsClear> GoodsClear { get; set; }

        public DbSet<CategoryStatView> CategoryStatView { get; set; }
        public DbSet<UserStatView> UserStatView { get; set; }

        #endregion

        #region SearchTerms

        public DbSet<LogSynonym> LogSynonym { get; set; }

        public DbSet<SynDosageGroup> SynDosageGroup { get; set; }

        public DbSet<SynFormProduct> SynFormProduct { get; set; }

        public DbSet<SynINNGroup> SynINNGroup { get; set; }

        public DbSet<SynTradeName> SynTradeName { get; set; }


        #endregion

        #region Stat

        public DbSet<DrugClearWorkStat> DrugClearWorkStats { get; set; }
        public DbSet<DataTypeStat> DataTypeStats { get; set; }
        public DbSet<UserWorkStat> UserWorkStats { get; set; }
        public DbSet<RobotStat> RobotStat { get; set; }
        public DbSet<GoodsCategoryStat> GoodsCategoryStat { get; set; }
        public DbSet<GoodsUserStat> GoodsUserStat { get; set; }
        public DbSet<DateStat> DateStat { get; set; }
        public DbSet<PrioritetStat> PrioritetStat { get; set; }

        public DbSet<CategoryStatDrugView> CategoryStatDrugView { get; set; }


        #endregion

        #region Log

        public DbSet<ProductionInfoDescription> ProductionInfoDescription { get; set; }
        public DbSet<GoodsProductionInfoDescription> GoodsProductionInfoDescription { get; set; }

        public DbSet<VEDChange> VedChange { get; set; }
        public DbSet<VEDPeriodChange> VedPeriodChange { get; set; }
        public DbSet<VEDCheckedChange> VEDCheckedChange { get; set; }

        /// <summary>
        /// Получить текстовое описание изменений
        /// </summary>
        /// <param name="productionInfoId"></param>
        /// <returns></returns>
        public string GetProductionInfoDescription_Result(long productionInfoId)
        {
            return Database.SqlQuery<string>("[Log].[GetProductionInfoDescription] @ProductionInfoId",
                new SqlParameter { ParameterName = "@ProductionInfoId", SqlDbType = SqlDbType.BigInt, Value = productionInfoId }
                ).FirstOrDefault<string>();
        }

        /// <summary>
        /// Получить текстовое описание изменений
        /// </summary>
        /// <param name="productionInfoId"></param>
        /// <returns></returns>
        public string GetGoodsProductionInfoDescription_Result(long goodsproductionInfoId)
        {
            return Database.SqlQuery<string>("[Log].[GetGoodsProductionInfoDescription] @GoodsProductionInfoId",
                new SqlParameter { ParameterName = "@GoodsProductionInfoId", SqlDbType = SqlDbType.BigInt, Value = goodsproductionInfoId }
                ).FirstOrDefault<string>();
        }

        #endregion

        #region Certificate
        public DbSet<DataAggregator.Domain.Model.DrugClassifier.Certificate.Certificate> Cert_Certificate { get; set; }
        public DbSet<DataAggregator.Domain.Model.DrugClassifier.Certificate.FV> Cert_FV { get; set; }
        public DbSet<DataAggregator.Domain.Model.DrugClassifier.Certificate.ManufactureWay> Cert_ManufactureWay { get; set; }
        public DbSet<DataAggregator.Domain.Model.DrugClassifier.Certificate.ManufactureWayView> Cert_ManufactureWayView { get; set; }
        public DbSet<DataAggregator.Domain.Model.DrugClassifier.Certificate.SubstRaw> Cert_SubstRaw { get; set; }
        public DbSet<DataAggregator.Domain.Model.DrugClassifier.Certificate.NumberINN> NumberINN { get; set; }
        public DbSet<DataAggregator.Domain.Model.DrugClassifier.Certificate.Chemicals> Chemicals { get; set; }
        public DbSet<DataAggregator.Domain.Model.DrugClassifier.Certificate.SubstanceView> SubstanceView { get; set; }
        public DbSet<DataAggregator.Domain.Model.DrugClassifier.Certificate.ChemicalSPR> ChemicalSPR { get; set; }
        public DbSet<DataAggregator.Domain.Model.DrugClassifier.Certificate.ChemicalsView> ChemicalsView { get; set; }

        public DbSet<DataAggregator.Domain.Model.DrugClassifier.Certificate.ESKLP> ESKLP { get; set; }
        public DbSet<DataAggregator.Domain.Model.DrugClassifier.Certificate.ESKLPView> ESKLPView { get; set; }

        #endregion

        #region rasp
        public DbSet<DataAggregator.Domain.Model.DrugClassifier.Rasp.Data> Rasp_Data { get; set; }
        public DbSet<DataAggregator.Domain.Model.DrugClassifier.Rasp.DataView> Rasp_DataView { get; set; }
        public DbSet<DataAggregator.Domain.Model.DrugClassifier.Rasp.Raspredelenie> Rasp_Raspredelenie { get; set; }
        public DbSet<DataAggregator.Domain.Model.DrugClassifier.Rasp.Tables> Rasp_Tables { get; set; }

        #endregion

        #region report
        public DbSet<DataAggregator.Domain.Model.DrugClassifier.Classifier.ClassifierReport> ClassifierReport { get; set; }
        public DbSet<RegCertificateNumberExceptions> RegCertificateNumberExceptions { get; set; }
        #endregion

        public DbSet<RegistrationCertificateClassification> RegistrationCertificateClassification { get; set; }

        public DbSet<ProductionInfoEtalonPriceView> ProductionInfoEtalonPriceView { get; set; }

        #region Job

        public DbSet<JobInfoLog> JobInfoLog { get; set; }
        public DbSet<ClassifierHistoryView> ClassifierHistoryView { get; set; }


        #endregion

        [DbFunction("Classifier.GetDrugDescription", "GetDrugDescription")]
        public static string GetDrugDescription(string fp, string dg, int? ConsumerPackingCount)
        {
            throw new NotSupportedException("Direct calls are not supported.");
        }

        public DrugClassifierContext(string APP)
        {
            Database.SetInitializer<DrugClassifierContext>(null);
            Database.Connection.ConnectionString += "APP=" + APP; //Чтобы триггер увидел, кто меняет
            Database.Log = (query) => Debug.Write(query);
        }
        public DrugClassifierContext(DrugClassifierContext Clon)
        {
            Database.SetInitializer<DrugClassifierContext>(null);
            Database.Connection.ConnectionString = Clon.Database.Connection.ConnectionString;
            Database.Log = (query) => Debug.Write(query);
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.HasDefaultSchema("Classifier");

            modelBuilder.Entity<DDDView>().Property(d => d.DDDs).HasPrecision(18, 6);
            modelBuilder.Entity<DrugClassification>().Property(d => d.DDDs).HasPrecision(18, 6);
            modelBuilder.Entity<BlisterBlock>().Property(d => d.ClassifierId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
        }

        public void SetDrugs(Guid userId)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 600;

                command.Connection = (SqlConnection)this.Database.Connection;
                command.CommandType = CommandType.StoredProcedure;


                command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

                command.CommandText = "Systematization.SetDrugs";

                command.Connection.Open();

                command.ExecuteNonQuery();
            }
        }


        public void ClassifierUpdateSQA()
        {

            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 600;
                command.Connection = (SqlConnection)this.Database.Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "[Classifier].[UpdateSQA]";
                command.Connection.Open();
                command.ExecuteNonQuery();
            }

        }

        public void PublishFull(long ClassifierId)//DrugClassifierContext
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 600;

                command.Connection = (SqlConnection)this.Database.Connection;
                command.CommandType = CommandType.StoredProcedure;


                command.Parameters.Add("@ClassifierId", SqlDbType.BigInt).Value = ClassifierId;

                command.CommandText = "[dbo].[PublishFull]";

                command.Connection.Open();

                command.ExecuteNonQuery();
            }

        }
        public void CreateNew(string Name, int TableId, DateTime Date_Begin, DateTime Date_End, bool withRegion, int RaspredelenieId)
        {

            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)this.Database.Connection;
                command.CommandType = CommandType.StoredProcedure;


                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = Name;
                command.Parameters.Add("@TableId", SqlDbType.TinyInt).Value = TableId;
                command.Parameters.Add("@Date_Begin", SqlDbType.Date).Value = Date_Begin;
                command.Parameters.Add("@Date_End", SqlDbType.Date).Value = Date_End;
                command.Parameters.Add("@withRegion", SqlDbType.Bit).Value = withRegion;
                command.Parameters.Add("@RaspredelenieId", SqlDbType.Int).Value = RaspredelenieId;

                command.CommandText = "[rasp].[CreateNew]";

                command.Connection.Open();

                command.ExecuteNonQuery();
            }

        }
        public IEnumerable<ClassifierInfo_Report> ClassifierInfo_GetReport(bool IsBrick, bool isOther)
        {
            Database.CommandTimeout = 0;
            var ret = Database.SqlQuery<ClassifierInfo_Report>("dbo.ClassifierInfo_GetReport @IsBrick,@isOther",
                new SqlParameter { ParameterName = "@IsBrick", SqlDbType = SqlDbType.Bit, Value = IsBrick },
                new SqlParameter { ParameterName = "@isOther", SqlDbType = SqlDbType.Bit, Value = isOther });
            return ret;
        }

        public void GetDrugs(string filter, Guid userId, int Count)
        {
            using (var command = new SqlCommand())
            {
                //чтобы при больших запросах в итоге он отваливался, а не вешал всех на весь день
                command.CommandTimeout = 20 * 60;
                //cnt.Database.Connection.ConnectionString += "APP=user:" + User.Identity.Name;//Чтобы триггер увидел, кто меняет
                command.Connection = (SqlConnection)this.Database.Connection;

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@sql", SqlDbType.NVarChar).Value = filter;

                command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;
                command.Parameters.Add("@Count", SqlDbType.Int).Value = Count;

                command.CommandText = "Systematization.GetDrugsV2";

                command.Connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void GetDrugsV2(string filter, Guid userId, int Count)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 20 * 60;
                command.Connection = (SqlConnection)this.Database.Connection;

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@sql", SqlDbType.NVarChar).Value = filter;

                command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;
                command.Parameters.Add("@Count", SqlDbType.Int).Value = Count;

                command.CommandText = "Systematization.GetDrugsV2";

                command.Connection.Open();

                command.ExecuteNonQuery();
            }
        }


        public void SetGoods(Guid userId)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 600;

                command.Connection = (SqlConnection)this.Database.Connection;
                command.CommandType = CommandType.StoredProcedure;


                command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

                command.CommandText = "GoodsSystematization.SetGoods";

                command.Connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void GetGoods(string filter, Guid userId)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 600;

                command.Connection = (SqlConnection)this.Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@sql", SqlDbType.NVarChar).Value = filter;

                command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

                command.CommandText = "GoodsSystematization.GetGoods";
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void FederalBenefitCopyPeriod(long periodIdFrom, long periodIdTo, Guid userId)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 600;

                command.Connection = (SqlConnection)this.Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@periodIdFrom", SqlDbType.BigInt).Value = periodIdFrom;
                command.Parameters.Add("@periodIdTo", SqlDbType.BigInt).Value = periodIdTo;

                command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

                command.CommandText = "Classifier.FederalBenefitCopyPeriod";

                command.Connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void CopyPeriod(long periodIdFrom, long periodIdTo, Guid userId)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 600;

                command.Connection = (SqlConnection)this.Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@periodIdFrom", SqlDbType.BigInt).Value = periodIdFrom;
                command.Parameters.Add("@periodIdTo", SqlDbType.BigInt).Value = periodIdTo;

                command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

                command.CommandText = "VED.CopyPeriod";

                command.Connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void GetDrugsForAnalyze(long robotId, int currentVersion, int count)
        {

            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 600;

                command.Connection = (SqlConnection)this.Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@robotId", SqlDbType.BigInt).Value = robotId;

                command.Parameters.Add("@currentVersion", SqlDbType.Int).Value = currentVersion;

                command.Parameters.Add("@count", SqlDbType.Int).Value = count;

                command.CommandText = "Systematization.GetDrugsForAnalyze";

                command.Connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public List<INNGroup> InnGroupCheckUnique(long INNGroupId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("INNGroupId", INNGroupId),
            }.Cast<object>().ToArray();

            var ids = Database.SqlQuery<long>("[Classifier].[InnGroupCheckUnique] @INNGroupId", parameters).ToList();

            return this.INNGroups.Where(d => ids.Contains(d.Id)).ToList();

        }

        public void StartHydra(string value, int period_id)
        {
            //using (var connection = new SqlConnection(this.Database.Connection.ConnectionString))
            // {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 600;

                command.Connection = (SqlConnection)this.Database.Connection;
                command.CommandType = CommandType.StoredProcedure;


                command.Parameters.Add("@value", SqlDbType.NVarChar).Value = value;
                command.Parameters.Add("@period_id", SqlDbType.Int).Value = period_id;

                command.CommandText = "[dbo].[Robot_Гидра_Add]";

                command.Connection.Open();

                command.ExecuteNonQuery();
            }
            //}
        }


        public T getSyn<TL, T>(TL list, string value, long OriginalId)
            where TL : IEnumerable<AbstractSynonym>
            where T : AbstractSynonym, new()
        {
            var result = list.FirstOrDefault(l => l.OriginalId == OriginalId && l.Value.Equals(value));

            if (result != null)
                return (T)result;



            result = new T()
            {
                Value = value,
                OriginalId = OriginalId,
                Count = 0
            };

            this.Set<T>().Add((T)result);

            return (T)result;
        }

        public void UpdateMask()
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 600;
                command.Connection = (SqlConnection)this.Database.Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "Classifier.UpdateMask";
                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();
                command.ExecuteNonQuery();

            }

        }

        /// <summary>
        /// Загрузка сертификатов ГРЛС
        /// </summary>
        /// <param name="searchText">фильтр</param>
        /// <returns></returns>
        public IEnumerable<Model.GRLS.GetCertificates_SP_Result> GetCertificates_SP(string searchText = null)
        {
            return Database.SqlQuery<Model.GRLS.GetCertificates_SP_Result>("[grls].[GetCertificates_SP] @searchText",
                new SqlParameter { ParameterName = "@searchText", SqlDbType = SqlDbType.VarChar, Value = (object)searchText ?? DBNull.Value }
                );
        }

    }
}