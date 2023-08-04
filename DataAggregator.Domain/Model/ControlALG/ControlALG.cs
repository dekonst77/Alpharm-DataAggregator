using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace DataAggregator.Domain.Model.ControlALG
{
    public class ControlALG
    {
        public enum JobStartAction : int { info = 0, start = 1 };
        static public string Start_Job(DbContext _context,string Name, JobStartAction action, Guid? UserId = null)
        {
            //@job_name nvarchar(255)='',@action int=1,@status
            //[ControlALG].dbo.Start_Job
            SqlParameter outparam = new SqlParameter()
            {
                ParameterName = "status",
                SqlDbType = SqlDbType.NVarChar,
                Size = 4000,
                Direction = System.Data.ParameterDirection.Output
            };

            var parameters = new List<SqlParameter>
            {
                new SqlParameter(){
                ParameterName = "job_name",
                SqlDbType = SqlDbType.NVarChar,
                Size=255,
                Value=Name
            },
                new SqlParameter{
                ParameterName = "action",
                SqlDbType = SqlDbType.Int,
                Size=4,
                Value=(int)action
            },
                new SqlParameter{
                ParameterName = "userId",
                SqlDbType = SqlDbType.UniqueIdentifier,
                Value = (object)UserId ?? DBNull.Value
            },
                outparam
            }.Cast<object>().ToArray();

            _context.Database.ExecuteSqlCommand("[ControlALG].dbo.[Start_Job] @job_name, @action, @status output, @userId", parameters);

            return (string)outparam.Value;
        }
    }
}
