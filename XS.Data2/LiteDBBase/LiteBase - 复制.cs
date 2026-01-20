//using LiteDB;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace XS.Data2.LiteDBBase
//{
//    /**  
        
//       LiteDB BsonExpression 查询表达式示例参考：
       

//       【比较操作符】
//       1. 等于：
//          查询 Name 字段等于 'John'
//          "Name = 'John'"

//       2. 不等于：
//          查询 Age 字段不等于 30
//          "Age != 30"

//       3. 大于：
//          查询 Score 大于 80
//          "Score > 80"

//       4. 小于：
//          查询 Age 小于 25
//          "Age < 25"

//       5. 大于等于：
//          查询 Height 大于等于 180
//          "Height >= 180"

//       6. 小于等于：
//          查询 Salary 小于等于 10000
//          "Salary <= 10000"

//       【逻辑操作符】
//       7. 并且（AND）：
//          查询 Age 大于 30 且小于 40
//          "Age > 30 && Age < 40"

//       8. 或者（OR）：
//          查询 Name 等于 'John' 或 Age 小于 25
//          "Name = 'John' || Age < 25"

//       9. 非（NOT）：
//          查询 IsActive 字段为 false 或不存在
//          "!IsActive"

//       【字符串函数】
//       10. contains()：
//           查询 Name 包含 'John'
//           "Name.contains('John')"

//       11. startswith()：
//           查询 Email 以 'admin' 开头
//           "Email.startswith('admin')"

//       12. endswith()：
//           查询 FileName 以 '.txt' 结尾
//           "FileName.endswith('.txt')"

//       13. length：
//           查询 Name 长度大于 5
//           "Name.length > 5"

//       14. lower()：
//           查询 Name 转小写后等于 'john'（忽略大小写）
//           "Name.lower() = 'john'"

//       15. upper()：
//           查询 Code 转大写后等于 'ABC123'
//           "Code.upper() = 'ABC123'"

//       【集合操作】
//       16. in：
//           查询 Category 属于 ['Book', 'Music', 'Movie']
//           "$.in(Category, ['Book', 'Music', 'Movie'])"

//       17. Any（数组 contains）：
//           查询 Tags 数组中有包含 'urgent' 的项
//           "Tags.Any($.contains('urgent'))"

//       【组合查询】
//       18. 多条件组合查询：
//           查询 Age 在 18 到 25 之间，或 Name 以 'J' 开头
//           "(Age >= 18 && Age <= 25) || Name.startswith('J')"

//       【嵌套数组字段】
//       19. 嵌套数组字段查询：
//           查询 Orders 数组中存在 Amount > 100 的元素
//           "Orders.Any($.Amount > 100)"

//       */
     

//    abstract public class LiteBase<T> where T : LiteModelBase
//    {
//        /// <summary>
//        /// 实现返回一个数据库实例,如new LiteDatabase(@"ChatHistory.db")
//        /// </summary>
//        abstract protected LiteDatabase GetDb { get; } 
//        private string TableName = string.Empty;
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="DbPath">数据库位置，如当前目录下"ChatHistory.db"</param>
//        public LiteBase() {


//            // 获取类型
//            Type type = typeof(T);
//            // 获取LiteTableAttribute属性
//            var attribute = type.GetCustomAttribute<LiteTableAttribute>();
//            if (attribute != null) {
//                TableName = attribute.Name;
//            }
//            else
//            {
//                TableName = this.GetType().Name;
//            }
//        }
//        private string GetMillisecond()
//        {
//            DateTime dt_s = DateTime.Now;
//            DateTime dt_end = new DateTime(dt_s.Year, dt_s.Month, dt_s.Day, dt_s.Hour, dt_s.Minute, dt_s.Second, dt_s.Millisecond);
//            DateTime dt_begin = new DateTime(2013, 1, 1, 0, 0, 0, 0);
//            return dt_end.Subtract(dt_begin).TotalMilliseconds.ToString();
//        }
//        public ObjectId Add(T model)
//        {
//            using (var db = GetDb)
//            {
//                //model.Id = ObjectId.NewObjectId();
//                model.AddTime = DateTime.Now; 
//                model.AddTimeLong = long.Parse(GetMillisecond());
//                var collection = db.GetCollection<T>(TableName);
//                collection.Insert(model);
//                return model.Id;
//            }
                
//        }

//        public T GetEntity(int id)
//        {
//            using (var db = GetDb)
//            {
//                var collection = db.GetCollection<T>(TableName);
//                return collection.FindById(id);
//            }
               
//        }
//        public bool Exists(string id)
//        {
//            return Exists(new ObjectId(id));
//        }
//        public bool Exists(ObjectId id)
//        {
//            using (var db = GetDb)
//            {
//                var collection = db.GetCollection<T>(TableName);
//                // 使用表达式检查是否存在
//                return collection.Exists(x => x.Id == id);
//            }
//        }
//        /// <summary>
//        /// 是否存在某记录
//        /// </summary>
//        /// <param name="query">
//        /// var query = "Name = 'John'";
//        /// query = "Age > 30" 
//        /// quety = "Age != 30"
//        /// query = "Age > 30 && Age < 40"
//        /// query = "Name.contains('John')";
//        /// query = "Name.startswith('J')";
//        /// query = "Name.endswith('n')";
//        /// </param>
//        /// <returns></returns>
//        public bool ExistsWhere(string query)
//        {
//            using (var db = GetDb)
//            {
//                var collection = db.GetCollection<T>(TableName);
//                var search = BsonExpression.Create(query);
//                return collection.Exists(search);
//            }
//        }

//        /// <summary>
//        /// 删除符合条件的记录
//        /// </summary>
//        /// var query = "Name = 'John'";
//        /// query = "Age > 30" 
//        /// quety = "Age != 30"
//        /// query = "Age > 30 && Age < 40"
//        /// query = "Name.contains('John')";
//        /// query = "Name.startswith('J')";
//        /// query = "Name.endswith('n')";
//        /// <returns>删除的记录数</returns>
//        public int DeleteByWhere(string sWhere)
//        {
//            using (var db = GetDb)
//            {
//                var collection = db.GetCollection<T>(TableName);
//                int deletedCount = collection.DeleteMany(sWhere);
//                return deletedCount;
//            }
//        }

//        public bool Update(T model)
//        {
//            using (var db = GetDb)
//            {
//                var collection = db.GetCollection<T>(TableName);
//                return collection.Update(model);
//            }
//        }
//        public T GetEntity(ObjectId id)
//        {
//            using (var db = GetDb)
//            {
//                var collection = db.GetCollection<T>(TableName);
//                return collection.FindById(id);
//            }
//        }

//        public T GetEntity(string id)
//        {
//            return GetEntity(new ObjectId(id));
//        }
//        public void Delete(string id)
//        {
//             Delete(new ObjectId(id));
//        }
//        public void Delete(ObjectId id)
//        {
//            using (var db = GetDb)
//            {
//                var collection = db.GetCollection<T>(TableName);
//                collection.Delete(id);
//            }
//        }
//        public void DeleteAll()
//        {
//            using (var db = GetDb)
//            {
//                var collection = db.GetCollection<T>(TableName);
//                collection.DeleteMany(_ => true); // 删除所有文档
//            }
//        }
//        /// <summary>
//        /// 简单查询
//        /// </summary>
//        /// <param name="query">支持SQL语句</param>
//        ///var query1 = @"Name = ""John""";
//        ///var query2 = @"Age > 30 AND Age < 50";
//        ///var query3 = @"Name LIKE ""%股票%""";
//        ///var query4 = @"Age < 18 OR Age > 65";
//        ///var query5 = @"NOT Name = ""Tom""";
//        /// <returns></returns>
//        public List<T> Find(string query)
//        {
//            using (var db = GetDb)
//            {
//                var collection = db.GetCollection<T>(TableName);
//                // 使用 ToList() 将查询结果转换为 List<T>
//                return collection.Find(query).ToList();
//            }
//        }

//        /// <summary>
//        /// 简单查询（支持限制条数和排序）
//        /// 调用示例：
//        ///var query1 = @"Name = ""John""";
//        ///var query2 = @"Age > 30 AND Age < 50";
//        ///var query3 = @"Name LIKE ""%股票%""";
//        ///var query4 = @"Age < 18 OR Age > 65";
//        ///var query5 = @"NOT Name = ""Tom""";
//        /// </summary>
//        /// <param name="query">支持SQL语句</param>
//        /// <param name="limit">最多返回的记录条数，null 表示不限制</param>
//        /// <param name="orderBy">排序字段名，例如 "Age"</param>
//        /// <param name="descending">是否降序</param>
//        /// <returns>符合条件的记录列表</returns>
//        public List<T> Find(string? query = null, int? limit = null, string? orderBy = null, bool descending = false)
//        {
//            using (var db = GetDb)
//            {
//                var collection = db.GetCollection<T>(TableName);

//                ILiteQueryable<T> queryable;

//                if (string.IsNullOrWhiteSpace(query))
//                {
//                    // 查询全部
//                    queryable = collection.Query();
//                }
//                else
//                {
                    
//                    var expr = BsonExpression.Create(query);
//                    queryable = collection.Query().Where(expr);
//                }

//                if (!string.IsNullOrEmpty(orderBy))
//                {
//                    queryable = descending
//                        ? queryable.OrderByDescending(orderBy)
//                        : queryable.OrderBy(orderBy);
//                }

//                if (limit.HasValue)
//                {
//                    var limitedResult = queryable.Limit(limit.Value);
//                    return limitedResult.ToList();
//                }
//                else
//                {
//                    return queryable.ToList();
//                }
//            }
//        }





//        public List<T> FindNews(int Top=100)
//        {
//            using (var db = GetDb)
//            {
//                var collection = db.GetCollection<T>(TableName);
//                // 使用 LINQ 进行排序和限制
//                return collection.FindAll()
//                                 .OrderByDescending(x => x.Id) // 按 Id 降序排序，最新的在前面
//                                 .Take(Top) // 取前 1000 条
//                                 .ToList();
//            }
//        }
//        public List<T> FindAll()
//        {
//            using (var db = GetDb)
//            {
//                var collection = db.GetCollection<T>(TableName);
//                // 使用 FindAll() 获取所有记录，并使用 ToList() 将结果转换为 List<T>
//                return collection.FindAll().ToList();
//            }
//        }
//        /// <summary>
//        /// 查询记录
//        /// 1.获取所有记录（不筛选，不排序）：GetList()
//        /// 2.获取 Message 包含 'fff' 的记录，按 AddTime 降序排序：var filteredAndSortedRecords = GetList(x => x.Message.Contains("fff"),x => x.AddTime,descending: true);
//        /// 3.获取所有记录，按 Prompt 升序排序：var sortedRecords = GetList(orderBy: x => x.Prompt);
//        /// 4.获取最近24小时内添加的记录，按 Id 降序排序：var recentSortedRecords = GetList(x => x.AddTime >= DateTime.Now.AddHours(-24),x => x.Id,descending: true);
//        /// 5.使用 && 组合多个条件（AND 逻辑）包含 "fff" 且在过去 7 天内添加的记录：var result = GetList(x => x.Message.Contains("fff") && x.AddTime >= DateTime.Now.AddDays(-7));
//        /// 6.使用 || 组合多个条件（OR 逻辑）返回 Prompt 以 "Hello" 或 "Hi" 开头的记录。：var result = GetList(x => x.Prompt.StartsWith("Hello") || x.Prompt.StartsWith("Hi"));
//        /// 7.组合 AND 和 OR 逻辑 返回 Message 包含 "important" 或 "urgent"，且在过去 30 天内添加的记录。：var result = GetList(x => (x.Message.Contains("important") || x.Message.Contains("urgent"))  && x.AddTime >= DateTime.Now.AddDays(-30));
//        /// </summary>
//        /// <param name="filter">用于筛选记录的条件表达式</param>
//        /// <param name="orderBy">用于指定排序字段的表达式</param>
//        /// <param name="descending">一个布尔值，指定是否为降序排序（默认为false，即升序）</param>
//        /// <returns></returns>
//        public List<T> GetList(Expression<Func<T, bool>> filter = null,Expression<Func<T, object>> orderBy = null,bool descending = true)
//        {
//            using (var db = GetDb)
//            {
//                var collection = db.GetCollection<T>(TableName);
//                ILiteQueryable<T> query = collection.Query();

//                if (filter != null)
//                {
//                    query = query.Where(filter);
//                }

//                if (orderBy == null)
//                {
//                    orderBy = x => x.AddTimeLong;
                    
//                }
//                query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

//                return query.ToList();
//            }
//        }
//        /// <summary>
//        /// 更加复杂的条件查询
//        /// </summary>
//        /// <param name="queryBuilder"></param>
//        /// <returns></returns>
//        public List<T> GetListComplex(Func<ILiteQueryable<T>, ILiteQueryable<T>> queryBuilder)
//        {
//            using (var db = GetDb)
//            {
//                var collection = db.GetCollection<T>(TableName);
//                var query = queryBuilder(collection.Query());
//                return query.ToList();
//            }
//        }
//        public int GetCount(string query)
//        {
//            using (var db = GetDb)
//            {
//                var collection = db.GetCollection<T>(TableName);
//                var search = BsonExpression.Create(query);
//                return collection.Count(search);
//            }
//        }
//        /// <summary>
//        /// 分页查询所有记录
//        /// </summary>
//        /// <param name="pageNumber">当前页码</param>
//        /// <param name="pageSize">每页显示数</param>
//        /// <returns></returns>
//        public List<T> PageAll(int pageNumber, int pageSize)
//        {
//            return PagedQuery(pageNumber, pageSize);
//        }

//        /// <summary>
//        /// 分页查询指定条件记录
//        /// 1.查询Message包含fff的记录：var result = PagedQuery(1, 10, x => x.Message.Contains("fff"));
//        /// 2.指定排序查询：PagedQuery(1, 10, orderBy: x => x.Prompt, descending: false);
//        /// 3.指定条件与排序：PagedQuery(1,10,x => x.AddTimeLong >= DateTimeOffset.Now.AddDays(-7).ToUnixTimeMilliseconds(),x => x.Id,descending: true);
//        /// </summary>
//        /// <param name="pageNumber">当前页码</param>
//        /// <param name="pageSize">每页显示数</param>
//        /// <param name="filter">用于指定筛选条件</param>
//        /// <param name="orderBy">用于指定排序字段</param>
//        /// <param name="descending">用于指定排序方向（默认为降序）</param>
//        /// <returns></returns>

//        public List<T> PagedQuery(int pageNumber,int pageSize,Expression<Func<T, bool>> filter = null,Expression<Func<T, object>> orderBy = null,bool descending = true)
//        {
//            using (var db = GetDb)
//            {
//                var collection = db.GetCollection<T>(TableName);
//                var query = collection.Query();

//                // 应用筛选条件
//                if (filter != null)
//                {
//                    query = query.Where(filter);
//                }

//                // 应用排序
//                if (orderBy != null)
//                {
//                    query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
//                }
//                else
//                {
//                    // 默认按 AddTimeLong 降序排序
//                    query = query.OrderByDescending(x => x.AddTimeLong);
//                }

//                // 执行分页
//                var results = query
//                    .Skip((pageNumber - 1) * pageSize)
//                    .Limit(pageSize)
//                    .ToList();

//                return results;
//            }
//        }
//        /// <summary>
//        /// 只保留最新的n条记录
//        /// </summary>
//        /// <param name="max">保留多少条</param>
//        public void KeepNewData(int max)
//        {
//            using (var db = GetDb)
//            {
//                var collection = db.GetCollection<T>(TableName);

//                // 获取总文档数
//                long totalCount = collection.Count();

//                // 如果文档总数超过max，则删除超出的部分
//                if (totalCount > max)
//                {
//                    // 查询需要保留的最新的max条记录的最小AddTimeLong
//                    var threshold = collection.Query()
//                        .OrderByDescending(x => x.AddTimeLong)
//                        .Skip(max - 1)
//                        .Limit(1)
//                        .ToList()
//                        .FirstOrDefault();

//                    if (threshold != null)
//                    {
//                        // 删除AddTimeLong小于阈值的所有记录
//                        collection.DeleteMany(x => x.AddTimeLong < threshold.AddTimeLong);
//                    }
//                }
//            }
//        }

//    }
//}
