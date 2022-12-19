using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Data;
using DataAggregator.Domain.Model.GS;
using System;
using System.Collections.Generic;
using System.Linq;
using DataAggregator.Domain.Model.LPU;
using System.Diagnostics;

namespace DataAggregator.Domain.DAL
{
    public class GSContext : DbContext
    {

        #region LPU

        public DbSet<LPUView> LPUView { get; set; }

        public DbSet<LPU> LPU { get; set; }

        public DbSet<LPUPointView> LPUPointView { get; set;}

        public DbSet<LPUPoint> LPUPoint { get; set; }

        public DbSet<LPULicensesView> LPULicensesView { get; set; }

        public DbSet<Department> Department { get; set; }

        public DbSet<LPUType> LPUType { get; set; }
        public DbSet<LPUKind> LPUKind { get; set; }

        #endregion LPU

        public DbSet<LPU_Departments> LPU_Departments { get; set; }

        public DbSet<licenses> licenses { get; set; }
        public DbSet<licenses_ViewAll> licenses_ViewAll { get; set; }
        public DbSet<licenses_adress> licenses_adress { get; set; }
        public DbSet<GS> GS { get; set; }
       // public DbSet<GS_View> GS_View { get; set; }
        public DbSet<Bricks> Bricks { get; set; }
        public DbSet<changelogBricks> changelogBricks { get; set; }
        public DbSet<Bricks_L3> Bricks_L3 { get; set; }
        //public DbSet<Reestr_Lic> Reestr_Lic { get; set; }
        public DbSet<GS_Period> GS_Period { get; set; }
        public DbSet<DistributorBranch> DistributorBranch { get; set; }
        public DbSet<GS_Period_Network> GS_Period_Network { get; set; }
        public DbSet<AlphaBitSums_Period> AlphaBitSums_Period { get; set; }
        public DbSet<OFDSumms_Period> OFDSumms_Period { get; set; }
        public DbSet<GS_Period_Region> GS_Period_Region { get; set; }        
        public DbSet<History_Status> History_Status { get; set; }
        public DbSet<GS_Period_Lic_View> GS_Period_Lic_View { get; set; }
        public DbSet<Calls> Calls { get; set; }
        public DbSet<spr_FormatLayout> spr_FormatLayout { get; set; }
        public DbSet<spr_PharmacySellingPlaceType> spr_PharmacySellingPlaceType { get; set; }
        public DbSet<PointCategory> PointCategory { get; set; }
        public DbSet<spr_WorkFormat> spr_WorkFormat { get; set; }
        public DbSet<Pharmacy> Pharmacy { get; set; }
        public DbSet<Organization> Organization { get; set; }
        public DbSet<Organization_without_INN> Organization_without_INN { get; set; }
        public DbSet<licenses_to_Use> licenses_to_Use { get; set; }
        public DbSet<History_coding_inwork> History_coding_inwork { get; set; }
        public DbSet<History_SPR_GS_view> History_SPR_GS_view { get; set; }
        public DbSet<History_SPR_BigGS_view> History_SPR_BigGS_view { get; set; }

        public void LpuPoint_FromExcel(string filepath, string user)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@filename", SqlDbType.NVarChar).Value = filepath;
                command.Parameters.Add("@userId", SqlDbType.NVarChar).Value = user;

                command.CommandText = "loadexcel.LpuPoint";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
           
        }

        public DbSet<History_SPR_LPU_view> History_SPR_LPU_view { get; set; }
        public DbSet<History_SPR_Distr_view> History_SPR_Distr_view { get; set; }
        public DbSet<History_SPR_Brick_view> History_SPR_Brick_view { get; set; }
        public DbSet<History_coding_inwork_View> History_coding_inwork_View { get; set; }
        public DbSet<History_coding> History_coding { get; set; }
        public DbSet<History_Category> History_Category { get; set; }
        public DbSet<spr_OperationMode> spr_OperationMode { get; set; }
        public DbSet<NetworkBrand> NetworkBrand { get; set; }

        public DbSet<spr_NetworkName> spr_NetworkName { get; set; }
        public DbSet<spr_NetworkName_Period> spr_NetworkName_Period { get; set; }
        public DbSet<GS_Period_Network_Anket> GS_Period_Network_Anket { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Pharmacy>().Property(d => d.geo_lat_manual).HasPrecision(9, 6);
            modelBuilder.Entity<Pharmacy>().Property(d => d.geo_lon_manual).HasPrecision(9, 6);
        }
            public GSContext(string APP)
        {
            Database.SetInitializer<GSContext>(null);
            Database.Connection.ConnectionString += "APP=" + APP;//Чтобы триггер увидел, кто меняет
            Database.CommandTimeout = 0;
            Database.Log = (query) => Debug.Write(query);
        }

        public IEnumerable<GS_View_SP> GS_View_SP(string filter,DateTime period)
        {
            var ret = Database.SqlQuery<GS_View_SP>("dbo.GS_View_SP @filter,@period", new SqlParameter("@filter", filter), new SqlParameter { ParameterName= "@period",SqlDbType= SqlDbType.Date, Value= period });
            return ret;
        }
        public bool BrickDelete(string Id)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@id", SqlDbType.NVarChar).Value = Id;

                command.CommandText = "dbo.BrickDelete";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }
        public int GS_add_from_History_coding(int History_codingId)
        {
            int GSId = 0;
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@History_codingId", SqlDbType.Int).Value = History_codingId;
                SqlParameter outparam = new SqlParameter()
                {
                    ParameterName = "@GSId",
                    SqlDbType = SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(outparam);

                command.CommandText = "dbo.GS_add_from_History_coding";

                Database.Connection.Open();

                command.ExecuteNonQuery();
                GSId = (int)outparam.Value;
            }
            return GSId;
        }

        //public bool licenses_to_Use_To_LPU(string userId)
        //{
        //    using (var command = new SqlCommand())
        //    {
        //        command.CommandTimeout = 0;

        //        command.Connection = (SqlConnection)Database.Connection;
        //        command.CommandType = CommandType.StoredProcedure;

        //        command.Parameters.Add("@userId", SqlDbType.NVarChar).Value = userId; 

        //        command.CommandText = "dbo.licenses_to_Use_To_LPU";

        //        Database.Connection.Open();

        //        command.ExecuteNonQuery();
        //    }
        //    return true;
        //}

        public bool licenses_to_Use_To_GS(string userId)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@userId", SqlDbType.NVarChar).Value = userId;

                command.CommandText = "dbo.licenses_to_Use_To_GS";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }


        public bool CreateLPUId(string userId)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;                               

                command.CommandText = "dbo.LPU_Sync";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }
        public bool AlphaBitSums_update (int Year,int Month)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@Year", SqlDbType.Int).Value = Year;
                command.Parameters.Add("@Month", SqlDbType.Int).Value = Month;

                command.CommandText = "dbo.AlphaBitSums_update";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }
        public bool SummsPeriod_OFD_Update(DateTime Period)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@period", SqlDbType.Date).Value = Period;

                command.CommandText = "dbo.SummsPeriod_OFD_Update";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }
        public bool SummsPeriod_OFD_Apply(DateTime Period)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@period", SqlDbType.Date).Value = Period;

                command.CommandText = "dbo.SummsPeriod_OFD_Apply";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }
        public bool Organization_Set()
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.CommandText = "dbo.Organization_Set";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }
        
        public bool Organization_Without_INN_Set()
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.CommandText = "dbo.Organization_Without_INN_Set";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }

        public bool spr_NetworkName_Update(string NetworkName_old, string NetworkName_new)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@NetworkName_old", SqlDbType.NVarChar).Value = NetworkName_old;
                command.Parameters.Add("@NetworkName_new", SqlDbType.NVarChar).Value = NetworkName_new;

                command.CommandText = "dbo.spr_NetworkName_Update";

                if (Database.Connection.State != ConnectionState.Open)
                    Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }

        public bool NetworkBrand_UpdateBrand(string NetworkName, string PharmacyBrand_old, string PharmacyBrand_new)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@NetworkName", SqlDbType.NVarChar).Value = NetworkName;
                command.Parameters.Add("@PharmacyBrand_old", SqlDbType.NVarChar).Value = PharmacyBrand_old;
                command.Parameters.Add("@PharmacyBrand_new", SqlDbType.NVarChar).Value = PharmacyBrand_new;

                command.CommandText = "dbo.NetworkBrand_UpdateBrand";

                if (Database.Connection.State != ConnectionState.Open)
                    Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }

        public bool GS_delete(int GSId)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@GSId", SqlDbType.NVarChar).Value = GSId;
                command.CommandText = "dbo.GS_delete";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }
        public bool GS_restore_From_changelog(string IDS)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@IDS", SqlDbType.NVarChar).Value = IDS;
                command.CommandText = "dbo.GS_restore_From_changelog";
                Database.Connection.Open();
                command.ExecuteNonQuery();
            }
            return true;
        }
        public bool GS_BrickIdSet()
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;              
                command.CommandText = "dbo.GS_BrickIdSet";
                if (Database.Connection.State != ConnectionState.Open)
                    Database.Connection.Open();
                command.ExecuteNonQuery();
            }
            return true;
        }
        public bool GS_PharmacyIdSet()
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "dbo.GS_PharmacyIdSet";
                if (Database.Connection.State != ConnectionState.Open)
                    Database.Connection.Open();
                command.ExecuteNonQuery();
            }
            return true;
        }
        /// <summary>
        /// Добавление нового ЛПУ 
        /// </summary>
        /// <param name="LPUModel"></param>
        /// <returns></returns>
        public void LPU_Add(LPUView LPUModel )
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;            
                command.Parameters.AddWithValue("@inn", LPUModel.EntityINN);
                command.Parameters.AddWithValue("@ogrn", LPUModel.EntityOGRN);
                command.Parameters.AddWithValue("@form", LPUModel.form);
                command.Parameters.AddWithValue("@full_name", LPUModel.full_name);
                command.Parameters.AddWithValue("@name", LPUModel.name);
                command.Parameters.AddWithValue("@EntityName", LPUModel.EntityName);
                command.Parameters.AddWithValue("@address", LPUModel.Address);
                command.Parameters.AddWithValue("@TypeId", LPUModel.TypeId);
                command.Parameters.AddWithValue("@KindId", LPUModel.KindId);
                command.CommandText = "dbo.LPU_Add";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
           
        }

        public void LPU_Merge(int LPUId, List<int> LPUIds, string UserId)
        {
            DataTable dt_ids = new DataTable();
            dt_ids.Columns.Add(new DataColumn("Id", typeof(int)));
            foreach (var id in LPUIds)
            {
                DataRow dr = dt_ids.NewRow();
                dr["Id"] = id;
                dt_ids.Rows.Add(dr);
            }

            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@newid", SqlDbType.NVarChar).Value = LPUId;
                command.Parameters.AddWithValue("@LPUIds", dt_ids);
                command.Parameters.AddWithValue("@UserId", UserId);
                command.CommandText = "[lpu].[MergeLPU]";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }

        }

        public void LPUPoint_Merge(int LPUId, List<int> LPUIds, string UserId)
        {
            DataTable dt_ids = new DataTable();
            dt_ids.Columns.Add(new DataColumn("Id", typeof(int)));
            foreach (var id in LPUIds)
            {
                DataRow dr = dt_ids.NewRow();
                dr["Id"] = id;
                dt_ids.Rows.Add(dr);
            }

            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@newPointid", SqlDbType.NVarChar).Value = LPUId;
                command.Parameters.AddWithValue("@PointIds", dt_ids);
                command.Parameters.AddWithValue("@UserId", UserId);
                command.CommandText = "[lpu].[MergePoint]";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            
        }
        public bool GS_Merge(int GSId, List<int> GSIds)
        {

            DataTable dt_ids = new DataTable();
            dt_ids.Columns.Add(new DataColumn("Id", typeof(long)));
            foreach (var id in GSIds)
            {
                DataRow dr = dt_ids.NewRow();
                dr["Id"] = id;
                dt_ids.Rows.Add(dr);
            }

            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@GSId", SqlDbType.NVarChar).Value = GSId;
                command.Parameters.AddWithValue("@GSIds", dt_ids);
                command.CommandText = "dbo.GS_Merge";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }
        public bool History_from_Excel(string filename)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@guid", SqlDbType.NVarChar).Value = filename;

                command.CommandText = "adr.History_from_Excel";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }

        public bool SummsOFD_FromExcel(string filename)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@guid", SqlDbType.NVarChar).Value = filename;

                command.CommandText = "dbo.SummsOFD_FromExcel";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }

        public bool Network_FromExcel(string filename)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@guid", SqlDbType.NVarChar).Value = filename;

                command.CommandText = "dbo.[spr_NetworkName_FromExcel]";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }
        public bool SummsPeriod_from_Excel(string filename, string currentperiod)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@guid", SqlDbType.NVarChar).Value = filename;
                command.Parameters.Add("@currentperiod", SqlDbType.NVarChar).Value = currentperiod;

                command.CommandText = "dbo.SummsPeriod_from_Excel";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }
        public bool DistributorBranch_from_Excel(string filename)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@guid", SqlDbType.NVarChar).Value = filename;

                command.CommandText = "dbo.DistributorBranch_from_Excel";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }

        public bool LPU_from_Excel(string filename)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@filename", SqlDbType.NVarChar).Value = filename;

                command.CommandText = "dbo.LPU_from_Excel";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }

        public bool GS_from_Excel(string filename)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@filename", SqlDbType.NVarChar).Value = filename;

                command.CommandText = "dbo.GS_from_Excel";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }
        public bool Organization_FromExcel(string filename)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@filename", SqlDbType.NVarChar).Value = filename;

                command.CommandText = "dbo.Organization_FromExcel";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }
        public bool Organization_without_INN_FromExcel(string filename)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@filename", SqlDbType.NVarChar).Value = filename;

                command.CommandText = "dbo.Organization_without_INN_FromExcel";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }

        public bool SummsAnket_FromTemplate(string filename)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@filename", SqlDbType.NVarChar).Value = filename;

                command.CommandText = "dbo.SummsAnket_FromTemplate";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }
        public bool licenses_to_Use_BrickIdSet(string userId)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@userId", SqlDbType.NVarChar).Value = userId;

                command.CommandText = "dbo.licenses_to_Use_BrickIdSet";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }
        public bool GS_Period_AddAll()
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.CommandText = "dbo.GS_Period_AddAll";

                if (Database.Connection.State != ConnectionState.Open)
                    Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }
        public bool SyncWithSPR_inwork()
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.CommandText = "adr.SyncWithSPR_inwork";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }
        public List<History_coding_inwork_View> adr_GetData(Guid user, int top, string Source_client, string text, string GSIDs, string PharmacyIDs,string Ids, byte Category, int Status, string Comments,
            string INN, string Address, string DataSource, string NetworkName, string Spec, string DataSourceType, bool IsOnline)
        {
            List<SqlParameter> spc = new List<SqlParameter>();
            spc.Add(new SqlParameter() { ParameterName = "@user", SqlDbType = SqlDbType.UniqueIdentifier, Value = user , Direction =ParameterDirection.Input});
            spc.Add(new SqlParameter() { ParameterName = "@top", SqlDbType = SqlDbType.Int, Value = top, Direction = ParameterDirection.Input });
            spc.Add(new SqlParameter() { ParameterName = "@Source_client", SqlDbType = SqlDbType.NVarChar, Value = Source_client, Direction = ParameterDirection.Input });
            spc.Add(new SqlParameter() { ParameterName = "@text", SqlDbType = SqlDbType.NVarChar, Value = text, Direction = ParameterDirection.Input });
            spc.Add(new SqlParameter() { ParameterName = "@GSIDs", SqlDbType = SqlDbType.NVarChar, Value = GSIDs, Direction = ParameterDirection.Input });
            spc.Add(new SqlParameter() { ParameterName = "@PharmacyIDs", SqlDbType = SqlDbType.NVarChar, Value = PharmacyIDs, Direction = ParameterDirection.Input });
            spc.Add(new SqlParameter() { ParameterName = "@Ids", SqlDbType = SqlDbType.NVarChar, Value = Ids, Direction = ParameterDirection.Input });
            spc.Add(new SqlParameter() { ParameterName = "@Category", SqlDbType = SqlDbType.TinyInt, Value = Category, Direction = ParameterDirection.Input });
            spc.Add(new SqlParameter() { ParameterName = "@Status", SqlDbType = SqlDbType.Int, Value = Status, Direction = ParameterDirection.Input });
            spc.Add(new SqlParameter() { ParameterName = "@Comments", SqlDbType = SqlDbType.NVarChar, Value = Comments, Direction = ParameterDirection.Input });
            spc.Add(new SqlParameter() { ParameterName = "@INN", SqlDbType = SqlDbType.NVarChar, Value = INN, Direction = ParameterDirection.Input });
            spc.Add(new SqlParameter() { ParameterName = "@Address", SqlDbType = SqlDbType.NVarChar, Value = Address, Direction = ParameterDirection.Input });
            spc.Add(new SqlParameter() { ParameterName = "@DataSource", SqlDbType = SqlDbType.NVarChar, Value = DataSource, Direction = ParameterDirection.Input });
            spc.Add(new SqlParameter() { ParameterName = "@NetworkName", SqlDbType = SqlDbType.NVarChar, Value = NetworkName, Direction = ParameterDirection.Input });
            spc.Add(new SqlParameter() { ParameterName = "@Spec", SqlDbType = SqlDbType.NVarChar, Value = Spec, Direction = ParameterDirection.Input });
            spc.Add(new SqlParameter() { ParameterName = "@DataSourceType", SqlDbType = SqlDbType.NVarChar, Value = DataSourceType, Direction = ParameterDirection.Input });
            spc.Add(new SqlParameter() { ParameterName = "@IsOnline", SqlDbType = SqlDbType.Bit, Value = IsOnline, Direction = ParameterDirection.Input });

            string sparams=string.Join(",", spc.Select(s => s.ParameterName));
            
                if (IsOnline)
            {
                var ret= Database.SqlQuery<History_coding_inwork_View>("exec [adr].[GetData] "+ sparams, spc.Cast<object>().ToArray());
                return ret.ToList();
            }
            else
            {
                Database.ExecuteSqlCommand("exec [adr].[GetData] "+ sparams, spc.Cast<object>().ToArray());
                return null;
            }
        }
        public bool adr_SetData(Guid user)
        {
            using (var command = new SqlCommand())
            {
                command.CommandTimeout = 0;

                command.Connection = (SqlConnection)Database.Connection;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@user", SqlDbType.UniqueIdentifier).Value = user;
                command.CommandText = "adr.SetData";

                Database.Connection.Open();

                command.ExecuteNonQuery();
            }
            return true;
        }
    }
}
