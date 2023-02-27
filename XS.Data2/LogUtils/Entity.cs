using System;

namespace XS.Data2.Log
{
    public class Entity
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int LogType { get; set; }
        public DateTime AddDate { get; set; }
        public string IP { get; set; }

    }
}