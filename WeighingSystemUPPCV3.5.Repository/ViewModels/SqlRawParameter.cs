using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace WeighingSystemUPPCV3_5_Repository.ViewModels
{
    public class SqlRawParameter
    {
        public SqlRawParameter()
        {
            SqlParameters = new List<SqlParameter>();
            SqlQuery = string.Empty;
        }
        public List<SqlParameter> SqlParameters { get; set; }

        public string SqlQuery { get; set; }
    }
}
