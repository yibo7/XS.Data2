using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace XS.Data2
{

    public class MySqlProvider : IDbProvider
    {
        public DbProviderFactory Instance()
        {

            // return SqlClientFactory.Instance;

            return MySqlClientFactory.Instance;
        }

        public void DeriveParameters(IDbCommand cmd)
        {
            //if ((cmd as SqlCommand) != null)
            //{
            //    SqlCommandBuilder.DeriveParameters(cmd as SqlCommand);
            //}

            if ((cmd as MySqlCommand) != null)
            {
                MySqlCommandBuilder.DeriveParameters(cmd as MySqlCommand);
            }
        }

        public DbParameter MakeParam(string ParamName, DbType DbType, Int32 Size)
        {
            MySqlParameter param;

            if (Size > 0)
                param = new MySqlParameter(ParamName, (MySqlDbType)DbType, Size);
            else
                param = new MySqlParameter(ParamName, (MySqlDbType)DbType);

            return param;
        }

        public bool IsFullTextSearchEnabled()
        {
            return true;
        }

        public bool IsCompactDatabase()
        {
            return true;
        }

        public bool IsBackupDatabase()
        {
            return true;
        }

        public string GetLastIdSql()
        {
            return "SELECT SCOPE_IDENTITY()";
        }
        public bool IsDbOptimize()
        {

            return false;
        }
        public bool IsShrinkData()
        {


            return true;
        }

        public bool IsStoreProc()
        {

            return true;
        }
    }
}
