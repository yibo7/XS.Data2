using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XS.Data2;

namespace XS.Data2Test.DapperExample
{
    /// <summary>
    /// 这个类演示了如何自定义一个dapper的数据处理类
    /// 不过建议你直接参照BPartBllSimple的使用方法，代码更加简洁
    /// </summary>
    public class BPartBll
    {

        public static readonly BPartBll Instane = new BPartBll();
        /// <summary>
        /// 这个方法在实际项目的使用中应该放在一个共用的工具类中使用
        /// </summary>
        /// <returns></returns>
        static public IDbConnection GetConn()
        {
            return Connections.GetConnectionSqlite(@"Filename=SQLiteDemo.db;");
            //return Connections.GetConnectionMysql(@"Data Source=127.0.0.1;Port=3306;UserId=root;Password=MySql2015;database=parts_test;charset=utf8;");
        }

        public void Add(BPart model)
        {
            using (var connection = GetConn())
            {
                connection.Insert(model);//只有Id为自动增长类型时返回一个整数Id
            }
        }


        public bool Update(BPart model)
        {
            using (var connection = GetConn())
            {
                return connection.Update(model);
            }
        }


        public BPart GetEntity(int Id)
        {
            using (var connection = GetConn())
            {
                return connection.Get<BPart>(Id);
            }
        }


        public BPart GetEntityByPartNo(string PartNo)
        {
            using (var connection = GetConn())
            {
                return connection.QuerySingle<BPart>(string.Format("select * from B_Part where PartNo='{0}'", PartNo));
            }
        }


        public List<BPart> GetEntitysByPartNo(string PartNo)
        {
            using (var connection = GetConn())
            {
                return connection.Query<BPart>(string.Format("select * from B_Part where PartNo='{0}'", PartNo)).ToList();
            }
        }


        public void DeleteAll(int Id)
        {
            using (var connection = GetConn())
            {
                connection.Delete(new BPart() { Id = Id });
            }
        }


        public void DeleteAll()
        {
            using (var connection = GetConn())
            {
                connection.DeleteAll<BPart>();
            }
        }


        public List<BPart> GetAll()
        {
            using (var connection = GetConn())
            {

                return connection.GetAll<BPart>().ToList();
            }
        }


        public List<BPart> GetAllSql()
        {
            using (var connection = GetConn())
            {
                return connection.Query<BPart>("select * from B_Part").ToList();
            }
        }
        /// <summary>
        /// 获取统计
        /// </summary>

        public int GetCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("select count(*)  from {0}Part ", "B_");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            int iCount = 0;
            using (var connection = GetConn())
            {
                iCount = connection.QuerySingleOrDefault<int>(strSql.ToString());
            }
            return iCount;
        }


        public List<BPart> GetListPages(int PageIndex, int PageSize, string strWhere, string Fileds, string oderby, out int RecordCount)
        {

            StringBuilder sbSql = new StringBuilder();
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                sbSql.AppendFormat(strWhere);
            }
            RecordCount = GetCount(sbSql.ToString());
            string strSql = SplitPages.GetSplitPagesSql("B_Part", PageSize, PageIndex, Fileds, "PartInno", "", strWhere, "");
            using (var connection = GetConn())
            {
                return connection.Query<BPart>(strSql).ToList();
            }
        }

        public void Transactions()
        {
            using (var connection = GetConn())
            {
                var id = connection.Insert(new BPart { Name = "one car" });
                var tran = connection.BeginTransaction();
                var car = connection.Get<BPart>(id, tran);
                var orgName = car.Name;
                car.Name = "Another car";
                connection.Update(car, tran);
                tran.Rollback();
                car = connection.Get<BPart>(id);
            }
        }
    }
}
