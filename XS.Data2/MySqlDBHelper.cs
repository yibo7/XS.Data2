using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XS.Data2
{

    public class MySqlDBHelper : DbHelperBase
    {
        private string _sConn;
        public MySqlDBHelper(string sConn)
        {
            _sConn = sConn;
        }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public override string ConnectionString()
        {
            if (base.m_connectionstring == null)
            {
                
                base.m_connectionstring = _sConn;

            }
            return base.m_connectionstring;
        }

        /// <summary>
        /// IDbProvider接口
        /// </summary>
        public override IDbProvider Provider()
        {
            if (m_provider == null)
            {
                lock (lockHelper)
                {
                    base.m_provider = DalFactory.DataBaseTypeProvider;
                }

            }
            return m_provider;
        }

    }
}
