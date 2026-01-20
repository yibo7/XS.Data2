using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XS.Data2.LiteDBBase
{
    public class LiteTableAttribute : Attribute
    {
        /// <summary>
        /// 可以在实体类里通过属性设置表的名称
        /// </summary>
        /// <param name="tableName">数据库中表的名称</param>
        public LiteTableAttribute(string tableName)
        {
            Name = tableName;
        }

        /// <summary>
        /// The name of the table in the database
        /// </summary>
        public string Name { get; set; }
    }
}
