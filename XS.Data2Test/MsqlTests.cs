

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using XS.Data2;

namespace XS.Data2Test
{
    /// <summary>
    /// 如下代码演示了直接操作Mysql数据库
    /// 除非有特殊情况，否则建议使用Dapper模式操作数据
    /// 如下是最原始的数据操作方法，使用起来比较麻烦
    /// </summary>
    [TestClass]
    public class MsqlTests
    {
        [TestMethod]
        public void Finds()
        {
            
            string strSql = "SELECT * FROM eb_newscontent ";
             
            DataSet dataSet = DataLayer.Instance.DB.ExecuteDataset(CommandType.Text, strSql);
            Dictionary<int,string> sDictionary = new Dictionary<int, string>();
            //遍历一个表多行多列
            foreach (DataRow mDr in dataSet.Tables[0].Rows)
            {
                
                sDictionary.Add(int.Parse(mDr["ID"].ToString()), mDr["ContentInfo"].ToString());
            }
           
            foreach (var model in sDictionary)
            {
               
                
                string strSqlUpdate = "UPDATE eb_newscontent SET ContentInfo=?ContentInfo WHERE ID=?ID";
                
                MySqlParameter[] parameters = {
                    new MySqlParameter("?ID", MySqlDbType.Int32,4),
                    new MySqlParameter("?ContentInfo", MySqlDbType.Text)
                   };
                parameters[0].Value = model.Key;
                parameters[1].Value = model.Value.Replace("<pre class=\"prettyprint lang - cs\">", "<pre class=\"brush: c#;toolbar:false\">");

                DataLayer.Instance.DB.ExecuteNonQuery(CommandType.Text, strSqlUpdate, parameters);
            }

        }
        
    }
    /// <summary>
    /// 只要调用这个方法构建数据操作帮助类实例就可以像上面那样使用了
    /// </summary>
    public class DataLayer
    {

        public MySqlDBHelper DB;
        private static object _SyncRoot = new object();
        private static DataLayer _DalProvider;

        public static DataLayer Instance
        {
            get
            {
                if (_DalProvider == null)
                {
                    lock (_SyncRoot)
                    {
                        if (_DalProvider == null)
                        {
                            _DalProvider = new DataLayer();
                            _DalProvider.DB = new MySqlDBHelper(@"Data Source=192.168.3.195;UserId=beimai5; Port=3306;Password=beimai16888; database=bbsebnet; charset=utf8;Allow User Variables=True");
                        }
                    }
                }
                return _DalProvider;
            }
        }
    }
}