using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XS.Data2;
using XS.Data2.Dapper;

namespace XS.Data2Test.DapperExample
{

    /// <summary>
    /// 本示例演示了如何通过继承通用基类来简单操作数据
    /// 基类默认提供了常用的数据操作方法，不过你也可以在你的子类中实现自己的类
    /// 如果你要完全自定义数据操作类，请参照BPartBll类的使用方法
    /// </summary>
    public class BPartBllSimple: DbBaseSqlite<BPart>
    {
        //你还可以在这里实现自己的数据表操作方法
    }

    #region 通用基类代码

    public class DbBaseSqlite<T> : DbBase<T> where T : class, new()
    {
        protected override IDbConnection GetConn => Connections.GetConnectionSqlite(@"Filename=SQLiteDemo.db;");

        private static DbBaseSqlite<T> _instance;
        private static readonly object syslock = new object();
        /// <summary>
        /// 生成此类的单例实例
        /// </summary>
        /// <returns></returns>
        public static DbBaseSqlite<T> GetInstance()
        {
            if (_instance == null)
            {
                lock (syslock)
                {
                    if (_instance == null)
                    {
                        _instance = new DbBaseSqlite<T>();
                    }
                }
            }
            return _instance;
        }
    }

    #endregion
}
