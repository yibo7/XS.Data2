using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XS.Data2.LiteDBBase
{
    public class LiteModelBase
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public DateTime AddTime { get; set; }
        public long AddTimeLong { get; set; }
        
        public string? MdWu { get; set; }
    }
}
