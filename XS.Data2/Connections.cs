using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XS.Data2
{
    static public class Connections
    {
        #region SqlServer

        //public static string ConnectionString => @"Server=127.0.0.1;Database=ABMain;User ID=sa;Password=111111";
        //static public IDbConnection GetOpenConnection()
        //{
        //    var connection = new SqlConnection(ConnectionString);
        //    connection.Open();
        //    return connection;
        //}

        #endregion

        #region MySql

        /// <summary>
        /// 获取MySql连接实例
        /// </summary>
        /// <param name="conn">连接字符串，如：conn => @"Server=localhost;Uid=root;Pwd=Password12"</param>
        /// <returns></returns>
        static public IDbConnection GetConnectionMysql(string conn)
        {
            var connection = new MySqlConnection(conn);
            connection.Open();
            return connection;
        }

        #endregion

        #region SqLite
         
        /// <summary>
        /// 获取MySql连接实例
        /// </summary>
        /// <param name="conn">连接字符串，如：conn => @"Filename=SQLiteDemo.sqlite3;"</param>
        /// <returns></returns>
        static public IDbConnection GetConnectionSqlite(string conn)
        {
            var connection = new SqliteConnection(conn);
            connection.Open();
            return connection;
        }

        #endregion
    }
}
