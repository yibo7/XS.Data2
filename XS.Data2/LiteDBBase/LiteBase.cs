using LiteDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
    public class LiteModelBase
    { 
        [BsonId]
        public ObjectId Id { get; set; }
        public DateTime AddTime { get; set; }
        public long AddTimeLong { get; set; }
    }

    abstract public class LiteBase<T> where T : LiteModelBase
    {
        /// <summary>
        /// 实现返回一个数据库实例,如new LiteDatabase(@"ChatHistory.db")
        /// </summary>
        abstract protected LiteDatabase GetDb { get; } 
        private string TableName = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DbPath">数据库位置，如当前目录下"ChatHistory.db"</param>
        public LiteBase() {


            // 获取类型
            Type type = typeof(T);
            // 获取LiteTableAttribute属性
            var attribute = type.GetCustomAttribute<LiteTableAttribute>();
            if (attribute != null) {
                TableName = attribute.Name;
            }
            else
            {
                TableName = this.GetType().Name;
            }
        }
        private string GetMillisecond()
        {
            DateTime dt_s = DateTime.Now;
            DateTime dt_end = new DateTime(dt_s.Year, dt_s.Month, dt_s.Day, dt_s.Hour, dt_s.Minute, dt_s.Second, dt_s.Millisecond);
            DateTime dt_begin = new DateTime(2013, 1, 1, 0, 0, 0, 0);
            return dt_end.Subtract(dt_begin).TotalMilliseconds.ToString();
        }
        public ObjectId Add(T model)
        {
            using (var db = GetDb)
            {
                model.Id = ObjectId.NewObjectId();
                model.AddTime = DateTime.Now; 
                model.AddTimeLong = long.Parse(GetMillisecond());
                var collection = db.GetCollection<T>(TableName);
                collection.Insert(model);
                return model.Id;
            }
                
        }

        public T GetEntity(int id)
        {
            using (var db = GetDb)
            {
                var collection = db.GetCollection<T>(TableName);
                return collection.FindById(id);
            }
               
        }
        public bool Exists(string id)
        {
            return Exists(new ObjectId(id));
        }
        public bool Exists(ObjectId id)
        {
            using (var db = GetDb)
            {
                var collection = db.GetCollection<T>(TableName);
                // 使用表达式检查是否存在
                return collection.Exists(x => x.Id == id);
            }
        }
        public bool ExistsWhere(string query)
        {
            using (var db = GetDb)
            {
                var collection = db.GetCollection<T>(TableName);
                var search = BsonExpression.Create(query);
                return collection.Exists(search);
            }
        }

        public bool Update(T model)
        {
            using (var db = GetDb)
            {
                var collection = db.GetCollection<T>(TableName);
                return collection.Update(model);
            }
        }
        public T GetEntity(ObjectId id)
        {
            using (var db = GetDb)
            {
                var collection = db.GetCollection<T>(TableName);
                return collection.FindById(id);
            }
        }

        public T GetEntity(string id)
        {
            return GetEntity(new ObjectId(id));
        }
        public void Delete(string id)
        {
             Delete(new ObjectId(id));
        }
        public void Delete(ObjectId id)
        {
            using (var db = GetDb)
            {
                var collection = db.GetCollection<T>(TableName);
                collection.Delete(id);
            }
        }
        public void DeleteAll()
        {
            using (var db = GetDb)
            {
                var collection = db.GetCollection<T>(TableName);
                collection.DeleteMany(_ => true); // 删除所有文档
            }
        }
        /// <summary>
        /// 查询记录
        /// 1.获取所有记录（不筛选，不排序）：GetList()
        /// 2.获取 Message 包含 'fff' 的记录，按 AddTime 降序排序：var filteredAndSortedRecords = GetList(x => x.Message.Contains("fff"),x => x.AddTime,descending: true);
        /// 3.获取所有记录，按 Prompt 升序排序：var sortedRecords = GetList(orderBy: x => x.Prompt);
        /// 4.获取最近24小时内添加的记录，按 Id 降序排序：var recentSortedRecords = GetList(x => x.AddTime >= DateTime.Now.AddHours(-24),x => x.Id,descending: true);
        /// 5.使用 && 组合多个条件（AND 逻辑）包含 "fff" 且在过去 7 天内添加的记录：var result = GetList(x => x.Message.Contains("fff") && x.AddTime >= DateTime.Now.AddDays(-7));
        /// 6.使用 || 组合多个条件（OR 逻辑）返回 Prompt 以 "Hello" 或 "Hi" 开头的记录。：var result = GetList(x => x.Prompt.StartsWith("Hello") || x.Prompt.StartsWith("Hi"));
        /// 7.组合 AND 和 OR 逻辑 返回 Message 包含 "important" 或 "urgent"，且在过去 30 天内添加的记录。：var result = GetList(x => (x.Message.Contains("important") || x.Message.Contains("urgent"))  && x.AddTime >= DateTime.Now.AddDays(-30));
        /// </summary>
        /// <param name="filter">用于筛选记录的条件表达式</param>
        /// <param name="orderBy">用于指定排序字段的表达式</param>
        /// <param name="descending">一个布尔值，指定是否为降序排序（默认为false，即升序）</param>
        /// <returns></returns>
        public List<T> GetList(Expression<Func<T, bool>> filter = null,Expression<Func<T, object>> orderBy = null,bool descending = true)
        {
            using (var db = GetDb)
            {
                var collection = db.GetCollection<T>(TableName);
                ILiteQueryable<T> query = collection.Query();

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                if (orderBy == null)
                {
                    orderBy = x => x.AddTimeLong;
                    
                }
                query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

                return query.ToList();
            }
        }
        /// <summary>
        /// 更加复杂的条件查询
        /// </summary>
        /// <param name="queryBuilder"></param>
        /// <returns></returns>
        public List<T> GetListComplex(Func<ILiteQueryable<T>, ILiteQueryable<T>> queryBuilder)
        {
            using (var db = GetDb)
            {
                var collection = db.GetCollection<T>(TableName);
                var query = queryBuilder(collection.Query());
                return query.ToList();
            }
        }
        public int GetCount(string query)
        {
            using (var db = GetDb)
            {
                var collection = db.GetCollection<T>(TableName);
                var search = BsonExpression.Create(query);
                return collection.Count(search);
            }
        }
        /// <summary>
        /// 分页查询所有记录
        /// </summary>
        /// <param name="pageNumber">当前页码</param>
        /// <param name="pageSize">每页显示数</param>
        /// <returns></returns>
        public List<T> PageAll(int pageNumber, int pageSize)
        {
            return PagedQuery(pageNumber, pageSize);
        }

        /// <summary>
        /// 分页查询指定条件记录
        /// 1.查询Message包含fff的记录：var result = PagedQuery(1, 10, x => x.Message.Contains("fff"));
        /// 2.指定排序查询：PagedQuery(1, 10, orderBy: x => x.Prompt, descending: false);
        /// 3.指定条件与排序：PagedQuery(1,10,x => x.AddTimeLong >= DateTimeOffset.Now.AddDays(-7).ToUnixTimeMilliseconds(),x => x.Id,descending: true);
        /// </summary>
        /// <param name="pageNumber">当前页码</param>
        /// <param name="pageSize">每页显示数</param>
        /// <param name="filter">用于指定筛选条件</param>
        /// <param name="orderBy">用于指定排序字段</param>
        /// <param name="descending">用于指定排序方向（默认为降序）</param>
        /// <returns></returns>

        public List<T> PagedQuery(int pageNumber,int pageSize,Expression<Func<T, bool>> filter = null,Expression<Func<T, object>> orderBy = null,bool descending = true)
        {
            using (var db = GetDb)
            {
                var collection = db.GetCollection<T>(TableName);
                var query = collection.Query();

                // 应用筛选条件
                if (filter != null)
                {
                    query = query.Where(filter);
                }

                // 应用排序
                if (orderBy != null)
                {
                    query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
                }
                else
                {
                    // 默认按 AddTimeLong 降序排序
                    query = query.OrderByDescending(x => x.AddTimeLong);
                }

                // 执行分页
                var results = query
                    .Skip((pageNumber - 1) * pageSize)
                    .Limit(pageSize)
                    .ToList();

                return results;
            }
        }
        /// <summary>
        /// 只保留最新的n条记录
        /// </summary>
        /// <param name="max">保留多少条</param>
        public void KeepNewData(int max)
        {
            using (var db = GetDb)
            {
                var collection = db.GetCollection<T>(TableName);

                // 获取总文档数
                long totalCount = collection.Count();

                // 如果文档总数超过max，则删除超出的部分
                if (totalCount > max)
                {
                    // 查询需要保留的最新的max条记录的最小AddTimeLong
                    var threshold = collection.Query()
                        .OrderByDescending(x => x.AddTimeLong)
                        .Skip(max - 1)
                        .Limit(1)
                        .ToList()
                        .FirstOrDefault();

                    if (threshold != null)
                    {
                        // 删除AddTimeLong小于阈值的所有记录
                        collection.DeleteMany(x => x.AddTimeLong < threshold.AddTimeLong);
                    }
                }
            }
        }

    }
}
