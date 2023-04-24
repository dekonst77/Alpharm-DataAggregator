using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Xml;
using DataAggregator.Domain.Model.GovernmentPurchases;
using DataAggregator.Domain.Model.GovernmentPurchases.Keywords;
using DataAggregator.Domain.Model.GovernmentPurchases.View;
using DataAggregator.Domain.Utils;
using DataAggregator.Domain.Model.GovernmentPurchases.QueryModel;
using DataAggregator.Domain.Model.GovernmentPurchases.Search;
using DataAggregator.Domain.Model.GovernmentPurchases.StoredProcedures;
using DataAggregator.Domain.Model.GovernmentPurchases.View.Search;

namespace DataAggregator.Domain.DAL
{
    public class GovernmentPurchasesContext : DbContext
    {
        public DbSet<LawType> LawType { get; set; }

        public DbSet<PurchaseNatureMixed> PurchaseNatureMixed { get; set; }

        public DbSet<Lot> Lot { get; set; }

        public DbSet<Method> Method { get; set; }

        public DbSet<PurchaseObject> PurchaseObject { get; set; }

        public DbSet<PurchaseObjectReady> PurchaseObjectReady { get; set; }

        public DbSet<PurchaseObjectReadyBulkInsert> PurchaseObjectReadyBulkInsert { get; set; }

        public DbSet<PurchaseObjectCalculated> PurchaseObjectCalculated { get; set; }

        public DbSet<Organization> Organization { get; set; }
        public DbSet<OrganizationRaw> OrganizationRaw { get; set; }
        public DbSet<OrganizationType> OrganizationType { get; set; }

        public DbSet<Purchase> Purchase { get; set; }
        public DbSet<PlanG> PlanG { get; set; }
        public DbSet<PlanG_View> PlanG_View { get; set; }

        public DbSet<Stage> Stage { get; set; }

        public DbSet<PurchaseClass> PurchaseClass { get; set; }

        public DbSet<Funding> Funding { get; set; }

        public DbSet<LotFunding> LotFunding { get; set; }

        public DbSet<Category> Category { get; set; }

        public DbSet<Nature> Nature { get; set; }
        public DbSet<Nature_L2> Nature_L2 { get; set; }

        public DbSet<ContractKK> ContractKK { get; set; }

        public DbSet<Region> Region { get; set; }

        public DbSet<DeliveryTimeInfo> DeliveryTimeInfo { get; set; }

        public DbSet<DeliveryTimePeriod> DeliveryTimePeriod { get; set; }

        public DbSet<Payment> Payment { get; set; }
        public DbSet<PaymentType> PaymentType { get; set; }

        public DbSet<PaymentYear> PaymentYear { get; set; }

        public DbSet<PurchaseAveragePrice> PurchaseAveragePrice { get; set; }

        public DbSet<Contract_check_view> Contract_check_view { get; set; }
        public DbSet<Contract_check_ContractPaymentStage_view> Contract_check_ContractPaymentStage_view { get; set; }
        public DbSet<ContractAveragePrice> ContractAveragePrice { get; set; }

        public DbSet<Contract> Contract { get; set; }

        public DbSet<ContractObject> ContractObject { get; set; }

        public DbSet<ContractObjectReady> ContractObjectReady { get; set; }
        public DbSet<ContractObjectReady_History> ContractObjectReady_History { get; set; }
        public DbSet<contract_stage_Objects_View> contract_stage_Objects_View { get; set; }

        public DbSet<ContractStatus> ContractStatus { get; set; }

        public DbSet<ContractPaymentStage> ContractPaymentStage { get; set; }
        public DbSet<Contract_check_ContractPaymentStage> Contract_check_ContractPaymentStage { get; set; }

        public DbSet<ContractObjectCalculated> ContractObjectCalculated { get; set; }

        public DbSet<SupplierRaw> SupplierRaw { get; set; }

        public DbSet<SupplierRawBinding> SupplierRawBinding { get; set; }

        public DbSet<Supplier> Supplier { get; set; }

        public DbSet<DataBaseTransfer> DataBaseTransfer { get; set; }

        public DbSet<AutoNature_Text> AutoNature_Text  { get; set; }
        public DbSet<KBK> KBK { get; set; }
        public DbSet<KBK_Funding> KBK_Funding { get; set; }
        public DbSet<KBK_Main_Rasp> KBK_Main_Rasp { get; set; }
        public DbSet<KBK_ZS> KBK_ZS { get; set; }
        public DbSet<KBK_Razdel> KBK_Razdel { get; set; }
        public DbSet<KBK_Razdel2> KBK_Razdel2 { get; set; }
        public DbSet<KBK_KodVidRashod> KBK_KodVidRashod { get; set; }

        public DbSet<SelectionPurchaseLinkView> SelectionPurchaseLinkView { get; set; }

        #region Search

        public DbSet<ContractLink> ContractLink { get; set; }

        public DbSet<ListType> ListType { get; set; }

        public DbSet<PurchaseClassAutoList> PurchaseClassAutoList { get; set; }

        public DbSet<PurchaseLink> PurchaseLink { get; set; }

        public DbSet<SelectionPurchaseLink> SelectionPurchaseLink { get; set; }

        public DbSet<SelectionContractLink> SelectionContractLink { get; set; }

        #endregion Search

        #region Keywords

        public DbSet<Client> Client { get; set; }

        public DbSet<Keyword> Keyword { get; set; }

        public DbSet<ClientKeyword> ClientKeyword { get; set; }

        #endregion Keywords

        #region views

        #region Search

        public DbSet<ContractLinkView> ContractLinkView { get; set; }

        public DbSet<PurchaseClassAutoListView> PurchaseClassAutoListView { get; set; }

        public DbSet<PurchaseLinkView> PurchaseLinkView { get; set; }

        #endregion Search

        #region Keywords

        public DbSet<ClientKeywordView> ClientKeywordView { get; set; }

        #endregion Keywords

        public DbSet<User> User { get; set; }

        public DbSet<DistributionWork> DistributionWork { get; set; }

        public DbSet<ContractDistributionWork> ContractDistributionWork { get; set; }

        public DbSet<PurchaseInWork> PurchaseInWork { get; set; }


        public DbSet<TriggerLog> TriggerLog { get; set; }
        public DbSet<RegionName> RegionName { get; set; }
        public DbSet<Source> Source { get; set; }

        public DbSet<UnitType> UnitType { get; set; }
        public DbSet<AutoCorrectAmountInfo> AutoCorrectAmountInfo { get; set; }
        public DbSet<AutoCorrectDosageRecount> AutoCorrectDosageRecount { get; set; }

        public DbSet<CalculatedDataView> CalculatedDataView { get; set; }
        public DbSet<SupplierResult> SupplierResult { get; set; }
        public DbSet<SupplierList> SupplierList { get; set; }

        public DbSet<CreateExternalShipmentLog> CreateExternalShipmentLog { get; set; }
        public DbSet<DataBaseChecked> DataBaseChecked { get; set; }
        public DbSet<LotStatus> LotStatus { get; set; }

        public DbSet<ShipmentInfo> ShipmentInfo { get; set; }

        public DbSet<AutomaticProcessesLogView> AutomaticProcessesLogView { get; set; }

        public DbSet<DrugIdWithMinMaxPriceView> DrugIdWithMinMaxPriceView { get; set; }
        public DbSet<WrongPricesView> WrongPricesView { get; set; }

        public DbSet<NotExportedToExternalPurchasesView> NotExportedToExternalPurchasesView { get; set; }
        public DbSet<NotExportedToExternalContractsView> NotExportedToExternalContractsView { get; set; }

        public DbSet<MassFixesDataView> MassFixesDataView { get; set; }

        #region Report
        public DbSet<ExecutionTerminatedContractView> ExecutionTerminatedContractView { get; set; }
        #endregion Report

        #endregion views


        #region Covid19
        public DbSet<Covid19_Region> Covid19_Region { get; set; }
        public DbSet<Covid19_Product> Covid19_Product { get; set; }
        public DbSet<Covid19_Vaccinate> Covid19_Vaccinate { get; set; }
        public DbSet<Covid19_Vaccinate_View> Covid19_Vaccinate_View { get; set; }
        public DbSet<Covid19_Product_Price> Covid19_Product_Price { get; set; }
        public DbSet<Covid19_Column_View> Covid19_Column_View { get; set; }
        public DbSet<Covid19_Vaccinate_Product> Covid19_Vaccinate_Product { get; set; }
        public DbSet<Covid19_Product_Price_History> Covid19_Product_Price_History { get; set; }
        public DbSet<Covid19_Product_History> Covid19_Product_History { get; set; }





        #endregion Covid19

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Lot>().Property(d => d.Sum).HasPrecision(38, 10);
            modelBuilder.Entity<PurchaseObject>().Property(d => d.Amount).HasPrecision(38, 10);
            modelBuilder.Entity<PurchaseObject>().Property(d => d.Price).HasPrecision(38, 10);
            modelBuilder.Entity<PurchaseObject>().Property(d => d.Sum).HasPrecision(38, 10);

            modelBuilder.Entity<PurchaseObjectReady>().Property(d => d.AmountCorrected).HasPrecision(38, 10);

            modelBuilder.Entity<PurchaseObjectCalculated>().Property(d => d.Amount).HasPrecision(38, 10);
            modelBuilder.Entity<PurchaseObjectCalculated>().Property(d => d.Price).HasPrecision(38, 10);
            modelBuilder.Entity<PurchaseObjectCalculated>().Property(d => d.Sum).HasPrecision(38, 10);

            modelBuilder.Entity<PurchaseObjectCalculated>()
                .HasKey(poc => poc.PurchaseObjectReadyId);

            modelBuilder.Entity<PurchaseObjectReady>()
                .HasOptional(por => por.PurchaseObjectCalculated)
                .WithRequired(poc => poc.PurchaseObjectReady);

            modelBuilder.Entity<Contract>().Property(c => c.Sum).HasPrecision(38, 10);
            modelBuilder.Entity<Contract>().Property(c => c.ActuallyPaid).HasPrecision(38, 10);

            modelBuilder.Entity<ContractObject>().Property(c => c.Amount).HasPrecision(38, 10);
            modelBuilder.Entity<ContractObject>().Property(c => c.Price).HasPrecision(38, 10);
            modelBuilder.Entity<ContractObject>().Property(c => c.Sum).HasPrecision(38, 10);

            modelBuilder.Entity<ContractObjectReady>().Property(c => c.Amount).HasPrecision(38, 10);
            modelBuilder.Entity<ContractObjectReady>().Property(c => c.Price).HasPrecision(38, 10);
            modelBuilder.Entity<ContractObjectReady>().Property(c => c.Sum).HasPrecision(38, 10);
            modelBuilder.Entity<ContractObjectReady>().Property(c => c.AmountCorrected).HasPrecision(38, 10);
            modelBuilder.Entity<ContractObjectReady>().Property(c => c.PriceCorrected).HasPrecision(38, 10);
            modelBuilder.Entity<ContractObjectReady>().Property(c => c.SumCorrected).HasPrecision(38, 10);

            modelBuilder.Entity<ContractPaymentStage>().Property(c => c.Sum).HasPrecision(38, 10);

            modelBuilder.Entity<ContractObjectCalculated>().Property(c => c.Amount).HasPrecision(38, 10);
            modelBuilder.Entity<ContractObjectCalculated>().Property(c => c.RecoveredPrice).HasPrecision(18, 2);
            modelBuilder.Entity<ContractObjectCalculated>().Property(c => c.Price).HasPrecision(18, 2);
            modelBuilder.Entity<ContractObjectCalculated>().Property(c => c.Sum).HasPrecision(18, 2);
            modelBuilder.Entity<ContractObjectCalculated>().Property(c => c.Coefficient).HasPrecision(38, 20);


            modelBuilder.Entity<PurchaseNatureMixed>().Property(o => o.Percentage).HasPrecision(6, 3);
        }


        public void GetPurchasesSupplierResult(int count, Guid userId)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 600;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@Count", SqlDbType.Int).Value = count;

                command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

                command.CommandText = "dbo.GetPurchasesSupplierResult";

                Connection_Open();

                command.ExecuteNonQuery();
            }
        }
        public void Connection_Open()
        {
            if (Database.Connection.State != ConnectionState.Open)
            {
                Database.Connection.Open();
            }
        }
        public void GetPurchases(int count, Guid userId)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 600;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@Count", SqlDbType.Int).Value = count;

                command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

                command.CommandText = "dbo.GetPurchases";

                Connection_Open();

                command.ExecuteNonQuery();
            }
        }

        public void GetPurchasesWithContracts(int count, Guid userId, Byte KK)
        {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 600;

                    command.Connection = (SqlConnection)Database.Connection; 
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@Count", SqlDbType.Int).Value = count;

                    command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;
                if (KK > 0)
                {
                    command.Parameters.Add("@KK", SqlDbType.TinyInt).Value = KK;
                    command.CommandText = "dbo.GetPurchasesWithContractsKK";
                }
                else
                    command.CommandText = "dbo.GetPurchasesWithContracts";

                Connection_Open();

                command.ExecuteNonQuery();
                }
            
        }

        public void GetPurchasesByFilter(string sql, Guid userId)
        {

                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 600;

                    command.Connection = (SqlConnection)Database.Connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@sql", SqlDbType.NVarChar).Value = sql;

                    command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

                    command.CommandText = "dbo.GetPurchasesByFilter";

                Connection_Open();

                command.ExecuteNonQuery();
                }
            
        }
        public void SetPurchases(Guid userId)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 600;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

                command.CommandText = "dbo.SetPurchases";

                Connection_Open();

                command.ExecuteNonQuery();
            }

        }
        public void Organization_Refresh()
        {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 600;

                    command.Connection = (SqlConnection)Database.Connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.CommandText = "dbo.Organization_Refresh";

                Connection_Open();

                command.ExecuteNonQuery();
                }
            
        }

        public void DrugClassifierIdFilling()
        {

                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 3600;

                    command.Connection = (SqlConnection)Database.Connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.CommandText = "dbo.DrugClassifierIdFilling";

                Connection_Open();

                command.ExecuteNonQuery();
                }
            
        }

        public void SendToClassif()
        {

                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 3600;

                    command.Connection = (SqlConnection)Database.Connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.CommandText = "dbo.SendToClassif";

                Connection_Open();

                command.ExecuteNonQuery();
                }

        }


        public void TransferSupplierResult(long id)
        {
            var conStr = new GovernmentPurchasesContext().Database.Connection.ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {

                using (SqlCommand com = new SqlCommand())
                {
                    var bufferSupplierResultId = new SqlParameter("BufferSupplierResultId", id);

                    com.CommandType = CommandType.StoredProcedure;
                    com.CommandText = "[buffer].TransferSupplierResult";
                    com.Connection = con;
                    com.CommandTimeout = 600;
                    com.Parameters.Add(bufferSupplierResultId);
                    con.Open();
                    com.ExecuteNonQuery();
                }
            }

        }

        public void TransferContract()
        {
            var conStr = new GovernmentPurchasesContext().Database.Connection.ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {

                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = CommandType.StoredProcedure;
                    com.CommandText = "[buffer].TransferContract";
                    com.Connection = con;
                    com.CommandTimeout = 6000;
                    con.Open();
                    com.ExecuteNonQuery();
                }
            }

        }

        public void TransferRtsPurchase(long id)
        {
            var conStr = new GovernmentPurchasesContext().Database.Connection.ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {

                using (SqlCommand com = new SqlCommand())
                {
                    var idPurchase = new SqlParameter("Id", id);

                    com.CommandType = CommandType.StoredProcedure;
                    com.CommandText = "[rts].TransferPurchase";
                    com.Connection = con;
                    com.CommandTimeout = 600;
                    com.Parameters.Add(idPurchase);
                    con.Open();
                    com.ExecuteNonQuery();
                }
            }
        }


        public void TransferPurchase(long id)
        {
            var conStr = new GovernmentPurchasesContext().Database.Connection.ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {

                using (SqlCommand com = new SqlCommand())
                {
                    var idPurchaseBuffer = new SqlParameter("IdPurchaseBuffer", id);

                    com.CommandType = CommandType.StoredProcedure;
                    com.CommandText = "[buffer].TransferPurchase";
                    com.Connection = con;
                    com.CommandTimeout = 600;
                    com.Parameters.Add(idPurchaseBuffer);
                    con.Open();
                    com.ExecuteNonQuery();
                }
            }
        }

        public void RefreshSupplierRawForBinding(string APP)
        {
            var conStr = new GovernmentPurchasesContext(APP).Database.Connection.ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = CommandType.StoredProcedure;
                    com.CommandText = "dbo.RefreshSupplierRawForBinding";
                    com.Connection = con;
                    com.CommandTimeout = 600;
                    con.Open();
                    com.ExecuteNonQuery();
                }
            }
        }

        public int GetOrganizationsCount(string filter)
        {

                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 0;
                    command.Connection = (SqlConnection)Database.Connection;
                    command.CommandText = "SELECT COUNT(*) FROM OrganizationView AS o " + filter;


                Connection_Open();

                return (int)command.ExecuteScalar();
                }
            
        }

        public List<OrganizationView> GetOrganizationsByFilter(string filter)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine(@"SELECT o.Id,o.ActualId,o.FZ,o.GosZakId,o.OrganizationTypeId,o.Url,o.FullName,o.ShortName,o.OGRN,o.INN,o.KPP,o.LocationAddress,o.Is_LO,o.Is_CP,o.Is_Customer,o.Is_Recipient,o.comment,o.PostAddress,o.OrganizationTypeText,
o.RegionId, o.FixedNatureId, o.FederalDistrict, o.FederationSubject, o.LastChangedUser, o.LastChangedUserId, o.LastChangedDate, o.RegionOfLocalizationId, o.FederalDistrictOfLocalization, o.FederationSubjectOfLocalization
FROM OrganizationView AS o ");
            queryBuilder.AppendLine(filter);
            queryBuilder.AppendLine("ORDER BY o.Id");
            var query = queryBuilder.ToString();
            this.Database.CommandTimeout = 0;
            var ret = this.Database.SqlQuery<OrganizationView>(query).ToList();
            return ret;
        }

        public List<CalculatedDataView> GetCalculatedDataByFilter(string whereQuery)
        {
            var queryBuilder = new StringBuilder();
            var baseQuery = "select top 100000 * from CalculatedDataView";
            queryBuilder.AppendLine(baseQuery);
            queryBuilder.AppendLine(whereQuery);
            queryBuilder.AppendLine("order by PurchaseNumber");
            var query = queryBuilder.ToString();
            this.Database.CommandTimeout = 0;
            return this.Database.SqlQuery<CalculatedDataView>(query).ToList();
        }

        public List<AveragePriceView> GetAveragePriceList(string whereQuery)
        {
            var queryBuilder = new StringBuilder();
            var baseQuery = @"select top 10000
                            ap.Id,
                            ap.RegionId,
                            r.FederalDistrict,
                            r.FederationSubject,
                            r.District,
                            r.City,
                            ap.Year,
                            ap.Month,
                            ap.ClassifierId,
                             CL.TradeName,
                             CL.DrugDescription,
                            CL.OwnerTradeMark,
                            CL.Packer,
                            ap.Price
                            from calc.AveragePrice ap
                            inner join DrugClassifier.Classifier.ExternalView_FULL CL WITH (nolock) ON ap.ClassifierID=CL.ClassifierID
                            inner join Region r
                            on ap.RegionId = r.Id";
            queryBuilder.AppendLine(baseQuery);
            queryBuilder.AppendLine(whereQuery);
            queryBuilder.AppendLine("order by ap.Id asc");
            var query = queryBuilder.ToString();
            return this.Database.SqlQuery<AveragePriceView>(query).ToList();
        }

        public List<PurchasesStatisticsModel> GetPurchasesStatistics(DateTime startDate, DateTime endDate)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine(@"              SELECT pc.Name as ClassName, s.Name as StatusName, count(1) as Count
                                                    FROM [GovernmentPurchases].[dbo].[Purchase] as p with (nolock)
                                                    inner join GovernmentPurchases.dbo.StatusHistory as sh with (nolock) on sh.PurchaseId = p.Id and sh.IsActual = 1
                                                    inner join GovernmentPurchases.dbo.Status as s with (nolock) on s.Id = sh.StatusId
                                                    inner join GovernmentPurchases.dbo.PurchaseClass as pc with (nolock) on pc.Id = p.PurchaseClassId ");
            queryBuilder.AppendLine(String.Format(" where p.DateBegin >= '{0}' ", startDate.ToString("yyyyMMdd")));
            queryBuilder.AppendLine(String.Format(" and p.DateBegin < '{0}'", endDate.ToString("yyyyMMdd")));
            queryBuilder.AppendLine("               group by pc.Name, s.Name");
            var query = queryBuilder.ToString();
            this.Database.CommandTimeout = 0;
            return this.Database.SqlQuery<PurchasesStatisticsModel>(query).ToList();
        }

        public List<PurchasesStatisticsModel> GetContractsStatistics(DateTime startDate, DateTime endDate)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine(@"              SELECT pc.Name as ClassName, contr.KK, 
iif(rc.ReadyContractId is not null, 'готово', 'не готово') as StatusName, count(1) as Count
                                                    FROM [GovernmentPurchases].[dbo].[Contract] as contr
                                                    left outer join (
                                                             select contr.Id as ReadyContractId
                                                             FROM [GovernmentPurchases].[dbo].[Contract] as contr
                                                             inner join [GovernmentPurchases].[dbo].ContractObjectReady as cor on cor.ContractId = contr.Id
                                                             group by contr.Id
                                                    ) as rc on rc.ReadyContractId = contr.Id
                                                    inner join [GovernmentPurchases].[dbo].[Lot] as l on l.Id = contr.LotId
                                                    inner join [GovernmentPurchases].[dbo].[Purchase] as p on p.Id = l.PurchaseId
                                                    inner join GovernmentPurchases.dbo.PurchaseClass as pc on pc.Id = p.PurchaseClassId ");
            queryBuilder.AppendLine(String.Format(" where p.DateBegin >= '{0}' ", startDate.ToString("yyyyMMdd")));
            queryBuilder.AppendLine(String.Format(" and p.DateBegin < '{0}'", endDate.ToString("yyyyMMdd")));
            queryBuilder.AppendLine("               group by pc.Name, contr.KK, iif(rc.ReadyContractId is not null, 'готово', 'не готово')");
            var query = queryBuilder.ToString();
            this.Database.CommandTimeout = 0;
            return this.Database.SqlQuery<PurchasesStatisticsModel>(query).ToList();
        }

        public GovernmentPurchasesContext()
        {
            Database.SetInitializer<GovernmentPurchasesContext>(null);
            //Database.Connection.ConnectionTimeout = 0;
            Database.CommandTimeout = 0;
        }

        public GovernmentPurchasesContext(string APP)
        {
            Database.SetInitializer<GovernmentPurchasesContext>(null);
            //Database.Connection.ConnectionTimeout = 0;
            Database.CommandTimeout = 0;
            Database.Connection.ConnectionString += "APP=" + APP;//Чтобы триггер увидел, кто меняет
        }
        public void Set_CONTEXT_INFO(string UserName)
        {
            //Database.SetInitializer<GovernmentPurchasesContext>(null);
            this.Database.ExecuteSqlCommand(@"DECLARE @BinVar varbinary(128);  
SET @BinVar = CAST(N'" + UserName + @"' AS varbinary(128) );  
SET CONTEXT_INFO @BinVar;");
        }

        public void DeletePurchase(long id, Guid userId)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 600;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@PurchaseId", SqlDbType.BigInt).Value = id;
                command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

                command.CommandText = "dbo.DeletePurchase";

                Connection_Open();

                command.ExecuteNonQuery();
            }
        }

        public void TransferRtsOrganization()
        {
            var conStr = new GovernmentPurchasesContext().Database.Connection.ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.CommandType = CommandType.StoredProcedure;
                    com.CommandText = "[rts].TransferOrganization";
                    com.Connection = con;
                    com.CommandTimeout = 600;
                    con.Open();
                    com.ExecuteNonQuery();
                }
            }
        }

        public void TransferOrganization(long id)
        {
            var conStr = new GovernmentPurchasesContext().Database.Connection.ConnectionString;

            using (SqlConnection con = new SqlConnection(conStr))
            {

                using (SqlCommand com = new SqlCommand())
                {
                    var idPurchaseBuffer = new SqlParameter("Id", id);

                    com.CommandType = CommandType.StoredProcedure;
                    com.CommandText = "[buffer].TransferOrganization";
                    com.Connection = con;
                    com.CommandTimeout = 600;
                    com.Parameters.Add(idPurchaseBuffer);
                    con.Open();
                    com.ExecuteNonQuery();
                }
            }
        }

        public void TransferPurchaseObjectReadyFromBulkInsert(Guid groupId, Guid userid, long lotId)
        {
            SqlParameter group = new SqlParameter("@groupId", groupId);
            SqlParameter user = new SqlParameter("@userid", userid);
            SqlParameter lot = new SqlParameter("@lotId", lotId);
            this.Database.ExecuteSqlCommand("dbo.TransferPurchaseObjectReadyFromBulkInsert @groupId, @userid, @lotid", group, user, lot);


        }

        public void ObjectsToObjectsReady(DateTime dateStart, DateTime dateEnd, Guid userId)
        {
 
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 600;

                    command.Connection = (SqlConnection)Database.Connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@dateStart", SqlDbType.DateTime).Value = dateStart;
                    command.Parameters.Add("@dateEnd ", SqlDbType.DateTime).Value = dateEnd;
                    command.Parameters.Add("@userId ", SqlDbType.UniqueIdentifier).Value = userId;

                    command.CommandText = "dbo.ObjectsToObjectsReady";

                Connection_Open();

                command.ExecuteNonQuery();
                }
            
        }

        public void ContractObjectsToContractObjectsReady(DateTime dateStart, DateTime dateEnd, Guid userId)
        {

                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 600;

                    command.Connection = (SqlConnection)Database.Connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("@dateStart", SqlDbType.DateTime).Value = dateStart;
                    command.Parameters.Add("@dateEnd ", SqlDbType.DateTime).Value = dateEnd;
                    command.Parameters.Add("@userId ", SqlDbType.UniqueIdentifier).Value = userId;

                    command.CommandText = "dbo.ContractObjectsToContractObjectsReady";

                Connection_Open();

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Метод объединяет Поставщиков
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="resultSupplierId"></param>
        public void MergeSuppliers(List<long> ids, long resultSupplierId)
        {
            var fromSuppliersIdXml = ProcedureHelper.GetXmlRows(ids);

            var fromSuppliersIdXmlParam = new SqlParameter("FromSuppliersIdXML", SqlDbType.Xml)
            {
                Value = new SqlXml(new XmlTextReader(fromSuppliersIdXml, XmlNodeType.Document, null))
            };

            var supplierIdParam = new SqlParameter("SupplierId", resultSupplierId);

            var parameters = new List<SqlParameter>
            {
                supplierIdParam,
                fromSuppliersIdXmlParam
            }.Cast<object>().ToArray();

            Database.ExecuteSqlCommand("dbo.MergeSuppliers @SupplierId, @FromSuppliersIdXML", parameters);
        }
        /// <summary>
        /// Установить класс закупки
        /// </summary>
        /// <param name="ids">Набор идентификаторов для изменения</param>
        /// <param name="purchaseClassId">Идентификатор класса, который будет устанавливаться</param>
        /// <param name="user">Пользователь внесший изменения</param>
        /// <returns>Дата изменения</returns>
        public DateTime SetPurchaseClass(List<long> ids, Byte purchaseClassId, Guid user)
        {

            var idsParamValue = ProcedureHelper.GetXmlRows(ids);

            var idsParam = new SqlParameter("idXml", SqlDbType.Xml)
            {
                Value = new SqlXml(new XmlTextReader(idsParamValue, XmlNodeType.Document, null))
            };

            var userParam = new SqlParameter("UserId", SqlDbType.UniqueIdentifier);
            userParam.Value = user;

            SqlParameter outparam = new SqlParameter()
            {
                ParameterName = "DateUpdate",
                SqlDbType = SqlDbType.DateTime,
                Direction = System.Data.ParameterDirection.Output
            };

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("purchaseClassId", purchaseClassId),
                idsParam,
                userParam,
                outparam
            }.Cast<object>().ToArray();

            Database.ExecuteSqlCommand("search.SetPurchaseClass @purchaseClassId, @IdXml, @UserId, @DateUpdate output", parameters);

            return (DateTime)outparam.Value;

        }

        public void AutoCorrectAmount(DataTable lotIds, Guid userId)
        {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 3600 * 3;

                    command.Connection = (SqlConnection)Database.Connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@LotIdList", lotIds);
                    command.Parameters.Add("@userId", SqlDbType.UniqueIdentifier).Value = userId;

                    command.CommandText = "calc.AutoCorrectAmount";

                Connection_Open();

                command.ExecuteNonQuery();
                }
        }

        public void CopyToCalculatedPurchaseObjectPrepareThenExecute(DataTable lotIds)
        {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 3600 * 3;

                    command.Connection = (SqlConnection)Database.Connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@LotIdList", lotIds);

                    command.CommandText = "calc.CopyToCalculatedPurchaseObjectPrepareThenExecute";


                Connection_Open();
                command.ExecuteNonQuery();
                }
        }

        public void CopyToCalculatedContractObjectPrepareThenExecute(DataTable lotIds)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 3600 * 3;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@LotIdList", lotIds);

                command.CommandText = "calc.CopyToCalculatedContractObjectPrepareThenExecute";

                Connection_Open();

                command.ExecuteNonQuery();
            }
        }

        public List<NotExportedToExternalLots> GetNotExportedToExternalLots(DateTime dateStart, DateTime dateEnd)
        {
            //var dateStartParameter = new SqlParameter("@DateStart", dateStart);
            //var dateEndParameter = new SqlParameter("@DateEnd", dateEnd);

            //var parameters = new object[] {dateStartParameter, dateEndParameter};

            //return this.Database.SqlQuery<NotExportedToExternalLots>("dbo.NotExportedToExternalLots @DateStart, @DateEnd", parameters).ToList();
            var result = this.Database.SqlQuery<NotExportedToExternalLots>("select * from dbo.NotExportedToExternalLots ('" + dateStart.ToString("yyyyMMdd") + "', '" + dateEnd.ToString("yyyyMMdd") + "')");
            return result.ToList();
        }

        public string Lock(bool IsSet,Guid UserId, string typeLock,string query,string ID)
        {
            string sql = "";
            sql = string.Format(@"
delete from [dbo].[Locks] where [typeLock]='{1}' and [dt_set]<DATEADD(HH,-12,getdate())
delete from [dbo].[Locks] where [UserId]='{0}' and [typeLock]='{1}'
", UserId, typeLock);
            
            this.Database.ExecuteSqlCommand(sql);//Сброс блокировки

            if (IsSet == true)
            {
                sql = string.Format(@"create table #IDS(ID int)
insert into #IDS(ID)
{2} AND {3} NOT IN (select Id from [dbo].[Locks] where [typeLock]='{1}')

insert into [dbo].[Locks]([UserId],[dt_set],[typeLock],[Id]) 
select '{0}',Getdate(),'{1}',ID from #IDS group by ID", UserId, typeLock, query, ID);

            }
            this.Database.ExecuteSqlCommand(sql);//Установка блокировки

            sql = string.Format(" IN (select Id from [dbo].[Locks] where [typeLock]='{1}' and [UserId]='{0}')", UserId, typeLock);

            return sql;
        }
    }
}
