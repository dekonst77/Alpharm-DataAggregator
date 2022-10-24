using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using DataAggregator.Domain.Model.GovernmentPurchasesLoader;
using DataAggregator.Domain.Model.GovernmentPurchasesLoader.purchasefile;
using DataAggregator.Domain.Model.GovernmentPurchasesLoader.search;
using DataAggregator.Domain.Model.GovernmentPurchasesLoader.rts;
using DataAggregator.Domain.Model.OkpdLoader;
using DataAggregator.Domain.Model.GovernmentPurchasesLoader.contract;
using DataAggregator.Domain.Model.GovernmentPurchasesLoader.proxy;


namespace DataAggregator.Domain.DAL
{
    public class GovernmentPurchasesLoaderContext : DbContext
    {
        public GovernmentPurchasesLoaderContext(string APP)
        {
            Database.SetInitializer<GovernmentPurchasesLoaderContext>(null);
            //Database.Connection.ConnectionTimeout = 0;
            Database.CommandTimeout = 0;
            Database.Connection.ConnectionString += "APP=" + APP;//Чтобы триггер увидел, кто меняет
        }
        public DbSet<RTSStage> RTSStage { get; set; }
        public DbSet<RTSSearchTask> RTSSearchTask { get; set; }
        public DbSet<RTSLoadTask> RTSLoadTask { get; set; }
        public DbSet<RTSPurchaseLink> RTSPurchaseLink { get; set; }
        public DbSet<RTSPurchase> RTSPurchase { get; set; }
        public DbSet<RTSObject> RTSObject { get; set; }
        public DbSet<RTSCustomer> RTSCustomer { get; set; }
        public DbSet<Date> Date { get; set; }
        public DbSet<Proxy> Proxy { get; set; }
        public DbSet<ProxyStat> ProxyStat { get; set; }
        public DbSet<FieldHtmlFieldOrhanization> FieldHtmlFieldOrhanization { get; set; }
        public DbSet<OrganizationErrorLog> OrganizationErrorLog { get; set; }
        public DbSet<OrganizationInfo> OrganizationInfo { get; set; }
        public DbSet<OrganizationLink> OrganizationLink { get; set; }
        public DbSet<OrganizationPage> OrganizationPage { get; set; }
        public DbSet<OrganizationSearchPage> OrganizationSearchPage { get; set; }
        public DbSet<OrganizationSearchTask> OrganizationSearchTask { get; set; }
        public DbSet<PostIndexes> PostIndexes { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<SearchFields> SearchFields { get; set; }
        public DbSet<ErrorLog> ErrorLog { get; set; }
        public DbSet<Log> Log { get; set; }
        public DbSet<OKPD> OKPD { get; set; }
        public DbSet<OKPD2> OKPD2 { get; set; }
        public DbSet<PurchaseLink> PurchaseLink { get; set; }
        public DbSet<SearchPage> SearchPage { get; set; }
        public DbSet<SearchTask> SearchTask { get; set; }
        public DbSet<StagePurchase> StagePurchase { get; set; }
        public DbSet<LoadTask> LoadTask { get; set; }
        public DbSet<PageItem> PageItem { get; set; }
        public DbSet<PageTableHeader> PageTableHeader { get; set; }
        public DbSet<Page> Page { get; set; }
        public DbSet<PurchaseFind> PurchaseFind { get; set; }
        public DbSet<Lot> Lot { get; set; }
        public DbSet<Purchase> Purchase { get; set; }
        public DbSet<PurchaseObject> PurchaseObject { get; set; }
        public DbSet<PageTable> PageTable { get; set; }
        public DbSet<WhoIsPurchase> WhoIsPurchase { get; set; }
        public DbSet<LoadedOKPD> LoadedOKPD { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<ContractUrl> ContractUrl { get; set; }
        public DbSet<KOSGUAnalyzeTest> KOSGUAnalyzeTest { get; set; }
        public DbSet<SupplierResultAnalyze> SupplierResultsAnalyze { get; set; }
        public DbSet<SupplierResult> SupplierResult { get; set; }
        public DbSet<SupplierResultsLotStatus> SupplierResultsLotStatus { get; set; }
        public DbSet<SupplierList> SupplierList { get; set; }
        public DbSet<ContractInfo> ContractInfo { get; set; }
        public DbSet<Contract> Contract { get; set; }
        public DbSet<ContractPaymentStage> ContractPaymentStage { get; set; }
        public DbSet<ContractObject> ContractObject { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<PurchaseInfo223> PurchaseInfo223 { get; set; }
        public DbSet<MethodDictionary> MethodDictionary { get; set; }
        public DbSet<StageDictionary> StageDictionary { get; set; }
        public DbSet<DeletePurchase> DeletePurchase { get; set; }
        public DbSet<OrganizationLoad> OrganizationLoad { get; set; }
        public DbSet<Organization> Organization { get; set; }
        public DbSet<ProxyStatLoad> ProxyStatLoad { get; set; }
        public DbSet<FileLoadTask> FileLoadTask { get; set; }
        public DbSet<FilePurchaseData> FilePurchaseData { get; set; }
        public DbSet<FileAnalyzeTask> FileAnalyzeTask { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<SearchINNTask> SearchINNTask { get; set; }
        public DbSet<FilePurchaseDataTemp> FilePurchaseDataTemp { get; set; }
        public DbSet<PurchaseObjectTemp> PurchaseObjectTemp { get; set; }
        public DbSet<OrganizationInfoCodes> OrganizationInfoCodes { get; set; }
        public DbSet<ContractLink> ContractLink { get; set; }
        public DbSet<ContractSearchTask> ContractSearchTask { get; set; }
        public DbSet<SelectionContractLink> SelectionContractLink { get; set; }
        public DbSet<ProxyErrorLog> ProxyErrorLog { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public void ResetSearchRtsTask()
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["GovernmentPurchasesLoaderContext"].ConnectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 60;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "rts.ResetSearchTask";
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ResetSearchTask44()
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["GovernmentPurchasesLoaderContext"].ConnectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 60;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "search.ResetSearchTask44";
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }



        public void AddLoadRtsTask()
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["GovernmentPurchasesLoaderContext"].ConnectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 3600;

                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.CommandText = "rts.AddLoadTask";

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }


        public void AddLoadPurchaseTask()
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["GovernmentPurchasesLoaderContext"].ConnectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 3600;

                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.CommandText = "search.AddLoadPurchaseTask";

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddLoadContractTask()
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["GovernmentPurchasesLoaderContext"].ConnectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 3600;

                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;

                    command.CommandText = "search.AddLoadContractTask";

                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }

        public void ClearPageInfo(long pageId)
        {
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["GovernmentPurchasesLoaderContext"].ConnectionString))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    var numberParam = new SqlParameter("PageId", SqlDbType.BigInt) {Value = pageId};
                    com.CommandType = CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[ClearPageInfo]";
                    com.Connection = con;
                    com.CommandTimeout = 600;
                    com.Parameters.Add(numberParam);
                    con.Open();
                    com.ExecuteNonQuery();
                }
            }

        }


        public void ReloadPurchase(string number)
        {
            var conStr = this.Database.Connection.ConnectionString;

            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["GovernmentPurchasesLoaderContext"].ConnectionString))
            {

                using (SqlCommand com = new SqlCommand())
                {
                    var numberParam = new SqlParameter("PurchaseNumber", number);

                    com.CommandType = CommandType.StoredProcedure;
                    com.CommandText = "[dbo].[ReloadPurchase]";
                    com.Connection = con;
                    com.CommandTimeout = 600;
                    com.Parameters.Add(numberParam);
                    con.Open();
                    com.ExecuteNonQuery();
                }
            }
        }


        public void ResetContractSearchTask()
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["GovernmentPurchasesLoaderContext"].ConnectionString))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandTimeout = 60;
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "[Contract].[ResetSearchTask]";
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    [Table("OrganizationInfoCodes", Schema = "dict")]
    public class OrganizationInfoCodes
    {
        public long Id { get; set; }
        public string Cpz { get; set; }
        public string Fz94Id { get; set; }
        public string Fz223Id { get; set; }
        public string Inn { get; set; }
    }


    [Table("PurchaseObjectTemp", Schema = "temp")]
    public class PurchaseObjectTemp
    {
        public long Id { get; set; }
        public long LotId { get; set; }
        public string Name { get; set; }
        public string OKPD { get; set; }
        public string Unit { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public decimal Sum { get; set; }
        public bool NewOKPD { get; set; }
        public string PurchaseNumber { get; set; }

    }

    [Table("FilePurchaseDataTemp", Schema = "temp")]
    public class FilePurchaseDataTemp
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public string Okpd2 { get; set; }
        public string IsVed { get; set; }
        public string IsVedManual { get; set; }
        public string INN { get; set; }
        public string PurchaseByTradeName { get; set; }
        public string TradeName { get; set; }
        public string NeedPacking { get; set; }
        public string JustificationNeedPacking { get; set; }
        public string FormDosageUnit { get; set; }
        public string Count { get; set; }
        public string Price { get; set; }
        public string Sum { get; set; }
        public string FormProduct { get; set; }
        public string Dosage { get; set; }
        public string Unit { get; set; }
        public string UnitCodeOkei { get; set; }
        public string UnitOkei { get; set; }
        public string CountPrimaryPacking { get; set; }
        public string ConsumerPackingCount { get; set; }
        public string BasicVariantDelivery { get; set; }



        public string ToName()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(INN);
            builder.Append(" ");
            builder.Append(TradeName);
            builder.Append(" ");
            builder.Append(FormProduct);
            builder.Append(" ");
            builder.Append(Dosage);
            builder.Append(" ");
            builder.Append(UnitOkei);
            builder.Append(" ");
            builder.Append(CountPrimaryPacking);
            builder.Append(" ");
            builder.Append(ConsumerPackingCount);


            return Regex.Replace(builder.ToString(), @"\s+", " ").Trim();
        }

    }
}
