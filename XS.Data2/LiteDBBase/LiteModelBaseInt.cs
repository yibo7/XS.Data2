using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XS.Data2.LiteDBBase
{
    public class LiteModelBaseInt: LiteModelBase
    {
        [BsonId]
        new public long Id { get; set; } 
    }
}
