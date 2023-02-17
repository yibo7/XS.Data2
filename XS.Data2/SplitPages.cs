using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Dapper;

namespace XS.Data2
{
    public class SplitPages
    {
        public static readonly string sPre = string.Empty;//Comm.TablePrefix

        #region  SqlServer


        /// <summary>
        /// 通用(sqlserver access)，获取分页sql
        /// </summary>
        /// <param name="sTableName">表名称</param>
        /// <param name="PageSize">每页显示多少条数据</param>
        /// <param name="PageIndex">当前页码</param>
        /// <param name="strFields">查询字段</param>
        /// <param name="KeyField">主键字段名称</param>
        /// <param name="OrderBy">排序 如 id desc</param>
        /// <param name="strWhere">条件，如 id=10</param>
        /// <returns></returns>
        public static string GetSplitPagesSql(string sTableName, int PageSize, int PageIndex, string strFields, string KeyField,
                                       string OrderBy, string strWhere)
        {

            return GetSplitPagesSql(sTableName, PageSize, PageIndex, strFields, KeyField,
                                    OrderBy, strWhere, sPre);
        }
        public static string GetSplitPagesSql(string sTableName, int PageSize, int PageIndex, string strFields, string KeyField,
                                      string OrderBy, string strWhere, string TablePrefix)
        {
            return GetSplitPagesSqlPT(sTableName, PageSize, PageIndex, strFields, KeyField,
                                       OrderBy, strWhere, TablePrefix);
        }

        private static string GetSplitPagesSqlPT(string sTableName, int PageSize, int PageIndex, string strFields, string KeyField,
                                      string OrderBy, string strWhere, string TablePrefix)
        {
            sTableName = string.Concat(TablePrefix, sTableName);

            int iPage = ((PageIndex - 1) * PageSize);

            string sWhere1 = "";
            string sWhere2 = "";
            string sOrderBy = string.Format(" order by {0} desc", KeyField);
            string strSql = string.Empty;
            if (!string.IsNullOrEmpty(strWhere))
            {
                sWhere1 = string.Concat(" WHERE ", strWhere);
                sWhere2 = string.Concat(" AND ", strWhere);
            }
            if (!string.IsNullOrEmpty(OrderBy))
            {
                sOrderBy = string.Concat(" ORDER BY ", OrderBy);
            }

            if (string.IsNullOrEmpty(strFields)) strFields = " * ";

            if (PageIndex <= 1)
            {
                strSql = string.Format("SELECT TOP {0} {1} FROM {2} {3} {4}", PageSize, strFields, sTableName, sWhere1, sOrderBy);
            }
            else
            {
                strSql = string.Format("select top {0} {1} from {2} where {3} not in(select top {4} {3} from {2} {5} {6} ) {7} {6} ",
                    PageSize, strFields, sTableName, KeyField, iPage, sWhere1, sOrderBy, sWhere2);
            }
            return strSql;
        }
        public static IDataReader GetListPages_SP(string sTableName, int PageSize, int PageIndex, string strFields, string KeyField,
                                       string OrderBy, string strWhere, out int totalRecords)
        {
            return GetListPages_SP(sTableName, PageSize, PageIndex, strFields, KeyField,
                                   OrderBy, strWhere, out totalRecords, sPre);

        }

        //public static IDataReader GetListPagesSql2005(DbHelperBase DB, string sTableName, int PageSize, int PageIndex, string Fileds, string strWhere, string oderby)
        //{
        //    string commandText = string.Format("select count(*)  from {0}  {1} ", sTableName, string.IsNullOrEmpty(strWhere) ? "" : (" Where " + strWhere));
        //    object objA = DB.ExecuteScalar(CommandType.Text, commandText);
        //    RecordCount = 1;
        //    if (!object.Equals(objA, null))
        //    {
        //        RecordCount = int.Parse(objA.ToString());
        //    }
        //    string str2 = GetListPagesSql2005(sTableName, PageIndex, PageSize, Fileds, strWhere, oderby);
        //    return DB.ExecuteReader(CommandType.Text, str2);
        //}        
        /// <summary>
        /// 适用sql2005及以上版本的分页查询语句
        /// </summary>
        /// <param name="sTableName">要查询的表名称.</param>
        /// <param name="PageSize">每页显示记录数.</param>
        /// <param name="PageIndex">页码.</param>
        /// <param name="Fileds">要查询的字体，用逗号分开，为空查询所有字段.</param>
        /// <param name="strWhere">查询条件.</param>
        /// <param name="oderby">排序字段.</param>
        /// <param name="keyfiledname">主键字段名称，默认为Id.</param>
        /// <returns>System.String.</returns>
        public static string GetListPagesSql2005(string sTableName, int PageSize, int PageIndex, string Fileds, string strWhere, string oderby = "", string keyfiledname = "Id")
        {
            if (PageIndex > 0)
            {
                PageIndex--;
            }
            int num = PageIndex * PageSize;
            int num2 = num + PageSize;
            if (Equals(Fileds, string.Empty))
            {
                Fileds = "*";
            }

            if (Equals(oderby, string.Empty))
            {
                oderby = keyfiledname;
            }


            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("select {0},RankID ", Fileds);
            builder.Append(" FROM  ");
            builder.Append(string.Format(" (select {0},ROW_NUMBER() OVER(ORDER BY {1} ) AS RankID   ", Fileds, oderby));
            builder.Append(string.Format(" FROM {0}", sTableName));
            if (strWhere.Trim() != "")
            {
                builder.Append(" WHERE ");
                builder.Append(strWhere);
            }
            builder.Append(" ) AS NewRowNumber   WHERE RankID>");
            builder.Append(num);
            builder.Append(" AND RankID<=  ");
            builder.Append(num2);

            return builder.ToString();
        }

        private static IDataReader GetListPages_SPPT(DbHelperBase DB, string sTableName, int PageSize, int PageIndex, string strFields, string KeyField,
                                       string OrderBy, string strWhere, out int totalRecords, string TablePrefix)
        {
            DbHelperBase DBInstance = DB;
            //if (DB == null)
            //{
            //    DBInstance = DbHelperCms.Instance;
            //}
            //else
            //{
            //    DBInstance = DB;
            //}
            sTableName = string.Concat(TablePrefix, sTableName);

            if (string.IsNullOrEmpty(strFields)) strFields = "*";

            SqlParameter[] parameters = new SqlParameter[]
                                            {
                                                new SqlParameter("@Tables", SqlDbType.VarChar, 1000), //表名称
                                                new SqlParameter("@PrimaryKey", SqlDbType.VarChar, 10), //表主键
                                                new SqlParameter("@Sort", SqlDbType.VarChar, 200), //排序 如 id desc
                                                new SqlParameter("@CurrentPage", SqlDbType.Int, 4), //当前页码 如 1
                                                new SqlParameter("@PageSize", SqlDbType.Int, 4), //每页显示多少条记录
                                                new SqlParameter("@Fields", SqlDbType.VarChar, 1000), //要查询的字段，为空，默认全部
                                                new SqlParameter("@Filter", SqlDbType.VarChar, 1000),//条件，如果 id>10 不带Where     
                                                //new SqlParameter("@Group", SqlDbType.VarChar, 1000),//分组 Group语句,不带Group By   
                                                //new SqlParameter("@PageCount", SqlDbType.Int), //总页数
                                                //new SqlParameter("@RecordCount", SqlDbType.Int) //总记录 
                                            };
            parameters[0].Value = sTableName;
            parameters[1].Value = KeyField;
            parameters[2].Value = OrderBy.Trim();
            parameters[3].Value = PageIndex;
            parameters[4].Value = PageSize;
            parameters[5].Value = strFields;
            parameters[6].Value = strWhere.Trim();
            //parameters[7].Value = "";
            //parameters[8].Direction = ParameterDirection.Output;
            //parameters[9].Direction = ParameterDirection.Output;

            IDataReader idr = DBInstance.ExecuteReader(CommandType.StoredProcedure,
                                                      string.Format("{0}SplitPages", TablePrefix),
                                                 parameters);
            //当您将 Command 对象用于存储过程时，可以将 Command 对象的 CommandType 属性设置为 StoredProcedure。当 CommandType 为 StoredProcedure 时，可以使用 Command 的 Parameters 属性来访问输入及输出参数和返回值。无论调用哪一个 Execute 方法，都可以访问 Parameters 属性。但是，当调用 ExecuteReader 时，在 DataReader 关闭之前，将无法访问返回值和输出参数
            string sCountSql = string.Format("select count(*)  from {0}  {1} ", sTableName, string.IsNullOrEmpty(strWhere) ? "" : string.Concat(" Where ", strWhere));

            object obCount = DBInstance.ExecuteScalar(CommandType.Text, sCountSql);
            totalRecords = 1;//int.Parse(parameters[9].Value.ToString());
            if (!Equals(obCount, null))
            {
                totalRecords = int.Parse(obCount.ToString());
            }
            return idr;
        }

        public static IDataReader GetListPages_SP(DbHelperBase DB, string sTableName, int PageSize, int PageIndex, string strFields, string KeyField,
                                       string OrderBy, string strWhere, out int totalRecords, string TablePrefix)
        {
            return GetListPages_SPPT(DB, sTableName, PageSize, PageIndex, strFields, KeyField,
                                        OrderBy, strWhere, out totalRecords, TablePrefix);
        }
        public static IDataReader GetListPages_SP(string sTableName, int PageSize, int PageIndex, string strFields, string KeyField,
                                       string OrderBy, string strWhere, out int totalRecords, string TablePrefix)
        {
            return GetListPages_SPPT(null, sTableName, PageSize, PageIndex, strFields, KeyField,
                                        OrderBy, strWhere, out totalRecords, TablePrefix);
        }


        public static IDataReader GetListPages_CusTomSearch(DbHelperBase DB, string sTableName, int PageSize, int PageIndex, string strFields, string KeyField,
                                       string OrderBy, string strWhere, out int totalRecords)
        {


            if (string.IsNullOrEmpty(strFields)) strFields = "*";

            SqlParameter[] parameters = new SqlParameter[]
                                            {
                                                new SqlParameter("@Tables", SqlDbType.VarChar, 1000), //表名称
                                                new SqlParameter("@PrimaryKey", SqlDbType.VarChar, 10), //表主键
                                                new SqlParameter("@Sort", SqlDbType.VarChar, 200), //排序 如 id desc
                                                new SqlParameter("@CurrentPage", SqlDbType.Int, 4), //当前页码 如 1
                                                new SqlParameter("@PageSize", SqlDbType.Int, 4), //每页显示多少条记录
                                                new SqlParameter("@Fields", SqlDbType.VarChar, 1000), //要查询的字段，为空，默认全部
                                                new SqlParameter("@Filter", SqlDbType.VarChar, 1000),//条件，如果 id>10 不带Where     
                                                //new SqlParameter("@Group", SqlDbType.VarChar, 1000),//分组 Group语句,不带Group By   
                                                //new SqlParameter("@PageCount", SqlDbType.Int), //总页数
                                                //new SqlParameter("@RecordCount", SqlDbType.Int) //总记录 
                                            };
            parameters[0].Value = sTableName;
            parameters[1].Value = KeyField;
            parameters[2].Value = OrderBy.Trim();
            parameters[3].Value = PageIndex;
            parameters[4].Value = PageSize;
            parameters[5].Value = strFields;
            parameters[6].Value = strWhere.Trim();
            //parameters[7].Value = "";
            //parameters[8].Direction = ParameterDirection.Output;
            //parameters[9].Direction = ParameterDirection.Output;

            IDataReader idr = DB.ExecuteReader(CommandType.StoredProcedure,
                                                      string.Format("{0}SplitPages", sPre),
                                                 parameters);
            //当您将 Command 对象用于存储过程时，可以将 Command 对象的 CommandType 属性设置为 StoredProcedure。当 CommandType 为 StoredProcedure 时，可以使用 Command 的 Parameters 属性来访问输入及输出参数和返回值。无论调用哪一个 Execute 方法，都可以访问 Parameters 属性。但是，当调用 ExecuteReader 时，在 DataReader 关闭之前，将无法访问返回值和输出参数
            //totalRecords = 1;//int.Parse(parameters[9].Value.ToString());

            //当您将 Command 对象用于存储过程时，可以将 Command 对象的 CommandType 属性设置为 StoredProcedure。当 CommandType 为 StoredProcedure 时，可以使用 Command 的 Parameters 属性来访问输入及输出参数和返回值。无论调用哪一个 Execute 方法，都可以访问 Parameters 属性。但是，当调用 ExecuteReader 时，在 DataReader 关闭之前，将无法访问返回值和输出参数
            string sCountSql = string.Format("select count(*)  from {0}  {1} ", sTableName, string.IsNullOrEmpty(strWhere) ? "" : string.Concat(" Where ", strWhere));

            object obCount = DB.ExecuteScalar(CommandType.Text, sCountSql);
            totalRecords = 1;//int.Parse(parameters[9].Value.ToString());
            if (!Equals(obCount, null))
            {
                totalRecords = int.Parse(obCount.ToString());
            }

            return idr;
        }

        #endregion


        #region MySQL

        /// <summary>
        /// 两个表的联合查询分页sql
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="sTableName1"></param>
        /// <param name="sTableName2"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="strFields"></param>
        /// <param name="sTable1Key"></param>
        /// <param name="sTable2Key"></param>
        /// <param name="OrderBy"></param>
        /// <param name="strWhere"></param>
        /// <param name="TablePrefix"></param>
        /// <returns></returns>
        public static string GetSplitPagesMySql(DbHelperBase DB, string sTableName1, string sTableName2, int PageSize, int PageIndex, string strFields, string sTable1Key, string sTable2Key,
                                      string OrderBy, string strWhere, string TablePrefix)
        {
            sTableName1 = string.Concat(TablePrefix, sTableName1);
            sTableName2 = string.Concat(TablePrefix, sTableName2);
            string strSql = string.Empty;
            string sOrderBy = string.Empty;
            if (PageIndex > 0)
            {
                PageIndex--;
            }
            if (!string.IsNullOrEmpty(OrderBy))
            {
                sOrderBy = string.Concat(" ORDER BY ", OrderBy);
            }
            int numStart = PageIndex * PageSize;
            string WhereTem = string.Format(" where {0}.{1}={2}.{3}", sTableName1, sTable1Key, sTableName2, sTable2Key);
            if (!string.IsNullOrEmpty(strWhere))
            {
                strWhere = string.Concat(WhereTem, " and ", strWhere);
            }
            else
            {
                strWhere = WhereTem;
            }

            strSql = string.Concat("select ", sTableName1, ".", sTable1Key, " from ", sTableName1, ",", sTableName2, " ", strWhere, sOrderBy, " limit ", numStart, ",", PageSize, ";");
            StringBuilder sbIDs = new StringBuilder();
            using (IDataReader dataReader = DB.ExecuteReader(CommandType.Text, strSql))
            {
                while (dataReader.Read())
                {
                    sbIDs.Append(dataReader.GetString(0));
                    sbIDs.Append(",");
                }
            }
            if (sbIDs.Length > 1)
            {
                sbIDs.Remove(sbIDs.Length - 1, 1);
            }
            else
            {
                sbIDs.Append("0");
            }

            if (string.IsNullOrEmpty(strFields))
            {
                strFields = "*";
            }
            else
            {
                strFields = string.Format(strFields, sTableName1, sTableName2);
            }

            strSql = string.Concat("select ", strFields, " from ", sTableName1, ",", sTableName2, " where ", sTableName1, ".", sTable1Key, "=", sTableName2, ".", sTable2Key, " and ", sTableName1, ".", sTable1Key, " in (", sbIDs, ")", sOrderBy);

            return strSql;
        }



        public static string GetSplitPagesMySql(DbHelperBase DB, string sTableName, int PageSize, int PageIndex, string strFields, string KeyField,
                                      string OrderBy, string strWhere, string TablePrefix, bool isint = true)
        {
            sTableName = string.Concat(TablePrefix, sTableName);
            string strSql = string.Empty;
            string sOrderBy = string.Empty;
            if (PageIndex > 0)
            {
                PageIndex--;
            }
            if (!string.IsNullOrEmpty(OrderBy))
            {
                sOrderBy = string.Concat(" ORDER BY ", OrderBy);
            }
            else
            {
                if (!string.IsNullOrEmpty(KeyField))
                    sOrderBy = string.Concat(" ORDER BY ", KeyField, " desc");
            }

            int numStart = PageIndex * PageSize;

            if (!string.IsNullOrEmpty(strWhere))
                strWhere = string.Concat(" where ", strWhere);
            strSql = string.Concat("select ", KeyField, " from ", sTableName, " ", strWhere, sOrderBy, " limit ", numStart, ",", PageSize, ";");

            StringBuilder sbIDs = new StringBuilder();
            if (isint)
            {
                using (IDataReader dataReader = DB.ExecuteReader(CommandType.Text, strSql))
                {
                    while (dataReader.Read())
                    {
                        sbIDs.Append(dataReader.GetString(0));
                        sbIDs.Append(",");
                    }
                }
            }
            else
            {
                using (IDataReader dataReader = DB.ExecuteReader(CommandType.Text, strSql))
                {
                    while (dataReader.Read())
                    {
                        sbIDs.Append("'");
                        sbIDs.Append(dataReader.GetString(0));
                        sbIDs.Append("'");
                        sbIDs.Append(",");
                    }
                }
            }

            if (sbIDs.Length > 1)
            {
                sbIDs.Remove(sbIDs.Length - 1, 1);
            }
            else
            {
                sbIDs.Append("0");
            }

            if (string.IsNullOrEmpty(strFields))
                strFields = "*";
            strSql = string.Concat("select ", strFields, " from ", sTableName, " where ", KeyField, " in (", sbIDs, ")", sOrderBy);


            return strSql;
        }


        #region Dapper 版本

        /// <summary>
        /// 单表获取分页查询语句，适用于MySql与SQLite
        /// </summary>
        /// <param name="DB">The database.</param>
        /// <param name="sTableName">Name of the s table.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="PageIndex">Index of the page.</param>
        /// <param name="strWhere">The string where.</param>
        /// <param name="KeyField">The key field.</param>
        /// <param name="strFields">The string fields.</param>
        /// <param name="TablePrefix">The table prefix.</param>
        /// <param name="OrderBy">The order by.</param>
        /// <param name="isint">if set to <c>true</c> [isint].</param>
        /// <returns>System.String.</returns>
        public static string GetSplitPagesMySql(IDbConnection DB, string sTableName, int PageSize, int PageIndex,
                                       string strWhere, string KeyField = "Id", string strFields = "", string TablePrefix = "", string OrderBy = "", bool isint = true)
        {
            sTableName = string.Concat(TablePrefix, sTableName);
            string strSql = string.Empty;
            string sOrderBy = string.Empty;
            if (PageIndex > 0)
            {
                PageIndex--;
            }
            if (!string.IsNullOrEmpty(OrderBy))
            {
                sOrderBy = string.Concat(" ORDER BY ", OrderBy);
            }
            else
            {
                if (!string.IsNullOrEmpty(KeyField))
                    sOrderBy = string.Concat(" ORDER BY ", KeyField, " desc");
            }

            int numStart = PageIndex * PageSize;

            if (!string.IsNullOrEmpty(strWhere))
                strWhere = string.Concat(" where ", strWhere);
            strSql = string.Concat("select ", KeyField, " from ", sTableName, " ", strWhere, sOrderBy, " limit ", numStart, ",", PageSize, ";");

            StringBuilder sbIDs = new StringBuilder();
            if (isint)
            {
                using (IDataReader dataReader = DB.ExecuteReader(strSql))
                {
                    while (dataReader.Read())
                    {
                        sbIDs.Append(dataReader.GetString(0));
                        sbIDs.Append(",");
                    }
                }


            }
            else
            {
                using (IDataReader dataReader = DB.ExecuteReader(strSql))
                {
                    while (dataReader.Read())
                    {
                        sbIDs.Append("'");
                        sbIDs.Append(dataReader.GetString(0));
                        sbIDs.Append("'");
                        sbIDs.Append(",");
                    }
                }
            }

            if (sbIDs.Length > 1)
            {
                sbIDs.Remove(sbIDs.Length - 1, 1);
            }
            else
            {
                sbIDs.Append("0");
            }

            if (string.IsNullOrEmpty(strFields))
                strFields = "*";
            strSql = string.Concat("select ", strFields, " from ", sTableName, " where ", KeyField, " in (", sbIDs, ")", sOrderBy);


            return strSql;
        }
        /// <summary>
        /// 多表获取分页查询语句，适用于MySql与SQLite
        /// </summary>
        /// <param name="DB">The database.</param>
        /// <param name="sTableName1">The s table name1.</param>
        /// <param name="sTableName2">The s table name2.</param>
        /// <param name="PageSize">Size of the page.</param>
        /// <param name="PageIndex">Index of the page.</param>
        /// <param name="strWhere">The string where.</param>
        /// <param name="strFields">The string fields.</param>
        /// <param name="sTable1Key">The s table1 key.</param>
        /// <param name="sTable2Key">The s table2 key.</param>
        /// <param name="OrderBy">The order by.</param>
        /// <param name="TablePrefix">The table prefix.</param>
        /// <returns>System.String.</returns>
        public static string GetSplitPagesMySql(IDbConnection DB, string sTableName1, string sTableName2, int PageSize, int PageIndex,
                                       string strWhere, string strFields = "", string sTable1Key = "Id", string sTable2Key = "Id", string OrderBy = "", string TablePrefix = "")
        {
            sTableName1 = string.Concat(TablePrefix, sTableName1);
            sTableName2 = string.Concat(TablePrefix, sTableName2);
            string strSql = string.Empty;
            string sOrderBy = string.Empty;
            if (PageIndex > 0)
            {
                PageIndex--;
            }
            if (!string.IsNullOrEmpty(OrderBy))
            {
                sOrderBy = string.Concat(" ORDER BY ", OrderBy);
            }
            int numStart = PageIndex * PageSize;
            string WhereTem = string.Format(" where {0}.{1}={2}.{3}", sTableName1, sTable1Key, sTableName2, sTable2Key);
            if (!string.IsNullOrEmpty(strWhere))
            {
                strWhere = string.Concat(WhereTem, " and ", strWhere);
            }
            else
            {
                strWhere = WhereTem;
            }

            strSql = string.Concat("select ", sTableName1, ".", sTable1Key, " from ", sTableName1, ",", sTableName2, " ", strWhere, sOrderBy, " limit ", numStart, ",", PageSize, ";");
            StringBuilder sbIDs = new StringBuilder();
            using (IDataReader dataReader = DB.ExecuteReader(strSql))
            {
                while (dataReader.Read())
                {
                    sbIDs.Append(dataReader.GetString(0));
                    sbIDs.Append(",");
                }
            }
            if (sbIDs.Length > 1)
            {
                sbIDs.Remove(sbIDs.Length - 1, 1);
            }
            else
            {
                sbIDs.Append("0");
            }

            if (string.IsNullOrEmpty(strFields))
            {
                strFields = "*";
            }
            else
            {
                strFields = string.Format(strFields, sTableName1, sTableName2);
            }

            strSql = string.Concat("select ", strFields, " from ", sTableName1, ",", sTableName2, " where ", sTableName1, ".", sTable1Key, "=", sTableName2, ".", sTable2Key, " and ", sTableName1, ".", sTable1Key, " in (", sbIDs, ")", sOrderBy);

            return strSql;
        }

        #endregion

        #endregion

        #region Sqlite

        //public static string GetSplitPagesSqlite(IDbConnection DB, string sTableName, int PageSize, int PageIndex, string strFields, string KeyField,
        //                              string OrderBy, string strWhere, string TablePrefix, bool isint)
        //{
        //    sTableName = string.Concat(TablePrefix, sTableName);
        //    string strSql = string.Empty;
        //    string sOrderBy = string.Empty;
        //    if (PageIndex > 0)
        //    {
        //        PageIndex--;
        //    }
        //    if (!string.IsNullOrEmpty(OrderBy))
        //    {
        //        sOrderBy = string.Concat(" ORDER BY ", OrderBy);
        //    }
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(KeyField))
        //            sOrderBy = string.Concat(" ORDER BY ", KeyField, " desc");
        //    }

        //    int numStart = PageIndex * PageSize;

        //    if (!string.IsNullOrEmpty(strWhere))
        //        strWhere = string.Concat(" where ", strWhere);
        //    strSql = string.Concat("select ", KeyField, " from ", sTableName, " ", strWhere, sOrderBy, " limit ", numStart, ",", PageSize, ";");
        //    StringBuilder sbIDs = new StringBuilder();
        //    if (isint) //Id是否为整型
        //    {
        //        //using (Imps.Client.Data.SQLiteDataReader dataReader = DB.ExecuteReader(Imps.Client.Data.CommandType.Text, strSql))
        //        //{
        //        //    while (dataReader.Read())
        //        //    {
        //        //        sbIDs.Append(dataReader.GetInt64(0));
        //        //        sbIDs.Append(",");
        //        //    }
        //        //}

        //        using (var connection = DB)
        //        {
        //           var ids =  connection.Query<long>(strSql).ToArray();
        //            foreach (var id in ids)
        //            {
        //                sbIDs.Append(id);
        //                sbIDs.Append(",");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //using (Imps.Client.Data.SQLiteDataReader dataReader = DB.ExecuteReader(Imps.Client.Data.CommandType.Text, strSql))
        //        //{
        //        //    while (dataReader.Read())
        //        //    {
        //        //        sbIDs.Append("'");
        //        //        sbIDs.Append(dataReader.GetString(0));
        //        //        sbIDs.Append("'");
        //        //        sbIDs.Append(",");
        //        //    }
        //        //}

        //        using (var connection = DB)
        //        {
        //            var ids = connection.Query<string>(strSql).ToArray();
        //            foreach (var id in ids)
        //            {
        //                sbIDs.Append("'");
        //                sbIDs.Append(id);
        //                sbIDs.Append("'");
        //                sbIDs.Append(",");
        //            }
        //        }
        //    }

        //    if (sbIDs.Length > 1)
        //    {
        //        sbIDs.Remove(sbIDs.Length - 1, 1);
        //    }
        //    else
        //    {
        //        sbIDs.Append("0");
        //    }

        //    if (string.IsNullOrEmpty(strFields))
        //        strFields = "*";
        //    strSql = string.Concat("select ", strFields, " from ", sTableName, " where ", KeyField, " in (", sbIDs, ")", sOrderBy);


        //    return strSql;
        //}


        ///// <summary>
        ///// 两个表的联合查询分页sql
        ///// </summary>
        ///// <param name="DB"></param>
        ///// <param name="sTableName1"></param>
        ///// <param name="sTableName2"></param>
        ///// <param name="PageSize"></param>
        ///// <param name="PageIndex"></param>
        ///// <param name="strFields"></param>
        ///// <param name="sTable1Key"></param>
        ///// <param name="sTable2Key"></param>
        ///// <param name="OrderBy"></param>
        ///// <param name="strWhere"></param>
        ///// <param name="TablePrefix"></param>
        ///// <returns></returns>
        //public static string GetSplitPagesSqlite(IDbConnection DB, string sTableName1, string sTableName2, int PageSize, int PageIndex, string strFields, string sTable1Key, string sTable2Key,
        //                              string OrderBy, string strWhere, string TablePrefix)
        //{
        //    sTableName1 = string.Concat(TablePrefix, sTableName1);
        //    sTableName2 = string.Concat(TablePrefix, sTableName2);
        //    string strSql = string.Empty;
        //    string sOrderBy = string.Empty;
        //    if (PageIndex > 0)
        //    {
        //        PageIndex--;
        //    }
        //    if (!string.IsNullOrEmpty(OrderBy))
        //    {
        //        sOrderBy = string.Concat(" ORDER BY ", OrderBy);
        //    }
        //    int numStart = PageIndex * PageSize;
        //    string WhereTem = string.Format(" where {0}.{1}={2}.{3}", sTableName1, sTable1Key, sTableName2, sTable2Key);
        //    if (!string.IsNullOrEmpty(strWhere))
        //    {
        //        strWhere = string.Concat(WhereTem, " and ", strWhere);
        //    }
        //    else
        //    {
        //        strWhere = WhereTem;
        //    }

        //    strSql = string.Concat("select ", sTableName1, ".", sTable1Key, " from ", sTableName1, ",", sTableName2, " ", strWhere, sOrderBy, " limit ", numStart, ",", PageSize, ";");
        //    StringBuilder sbIDs = new StringBuilder();
        //    //using (Imps.Client.Data.SQLiteDataReader dataReader = DB.ExecuteReader(Imps.Client.Data.CommandType.Text, strSql))
        //    //{
        //    //    while (dataReader.Read())
        //    //    {
        //    //        sbIDs.Append(dataReader.GetString(0));
        //    //        sbIDs.Append(",");
        //    //    }
        //    //}

        //    using (var connection = DB)
        //    {
        //        var ids = connection.Query<string>(strSql).ToArray();
        //        foreach (var id in ids)
        //        {
        //            sbIDs.Append(id);
        //            sbIDs.Append(",");
        //        }
        //    }

        //    if (sbIDs.Length > 1)
        //    {
        //        sbIDs.Remove(sbIDs.Length - 1, 1);
        //    }
        //    else
        //    {
        //        sbIDs.Append("0");
        //    }

        //    if (string.IsNullOrEmpty(strFields))
        //    {
        //        strFields = "*";
        //    }
        //    else
        //    {
        //        strFields = string.Format(strFields, sTableName1, sTableName2);
        //    }

        //    strSql = string.Concat("select ", strFields, " from ", sTableName1, ",", sTableName2, " where ", sTableName1, ".", sTable1Key, "=", sTableName2, ".", sTable2Key, " and ", sTableName1, ".", sTable1Key, " in (", sbIDs, ")", sOrderBy);

        //    return strSql;
        //}
        #endregion

    }
}
