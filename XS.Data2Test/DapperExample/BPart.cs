using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XS.Data2Test.DapperExample
{
    [Table("B_Part")]//实际表名称，如果不添加将默认类名称
    public class BPart
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Computed { get; set; }

    }
}
