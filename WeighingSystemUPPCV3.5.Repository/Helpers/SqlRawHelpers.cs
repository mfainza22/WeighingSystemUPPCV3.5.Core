using Microsoft.Data.SqlClient;

namespace WeighingSystemUPPCV3_5_Repository.Helpers
{
    public static class SqlRawHelpers
    {
        public static SqlParameter CreateSqlParameter(string fieldName, object value)
        {
            return new SqlParameter(fieldName.Parametarize(), value);
        }

        public static string CreateSqlWhereClause(string fieldname, string condition = "=")
        {
            return $"{fieldname} {condition} {fieldname.Parametarize()}";
        }
    }
}
