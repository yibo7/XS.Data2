using System; 

namespace XS.Data2.Log
{
    /// <summary>
    /// 通用的日志记录类，使用前需要根据Entity建表
    /// </summary>
    public class Bll
    {
        private Dal DalIns;
        private string sLogCategory = "xslogs"; 
        public Bll(MySqlDBHelper _db, string _tableName)
        {
            DalIns = new Dal(_db, _tableName);
        }
        #region  成员方法

        /// <summary>
        /// 得到最大ID
        /// </summary>
        public int GetMaxId()
        {
            return DalIns.Logs_GetMaxId();
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            return DalIns.Logs_Exists(id);
        }

        

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public void Update(Entity model)
        {
           // base.InvalidateCache();
            DalIns.Logs_Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public void Delete(int id)
        {
           // base.InvalidateCache();

            DalIns.Logs_Delete(id);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Entity GetEntity(int id)
        {
            return DalIns.Logs_GetEntity(id);
            //string rawKey = string.Concat("GetEntity-", id);
            //Entity etEntity = CacheWebRaw.Instance.GetCacheItem<Entity>(rawKey, sLogCategory);
            //if (Equals(etEntity, null))
            //{
            //    etEntity = DalIns.Logs_GetEntity(id);
            //    if (!Equals(etEntity, null))
            //        CacheWebRaw.Instance.AddCacheItem(rawKey, etEntity, CacheTime,spanModel, sLogCategory);
            //}
            //return etEntity;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public int GetCount(string strWhere)
        {
            return DalIns.Logs_GetCount(strWhere);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public int GetCountCache(string strWhere)
        {
            string rawKey = string.Concat("GetCount-", strWhere);
            //string sCount = CacheWebRaw.Instance.GetCacheItem<string>(rawKey, sLogCategory);
            //if (string.IsNullOrEmpty(sCount))
            //{
            //    sCount = GetCount(strWhere).ToString();
            //    if (!string.IsNullOrEmpty(sCount))
            //        CacheWebRaw.Instance.AddCacheItem(rawKey, sCount,CacheTime,spanModel, sLogCategory);
            //}
            string sCount = GetCount(strWhere).ToString();
            if (!string.IsNullOrEmpty(sCount))
            {
                return int.Parse(sCount);
            }
            return 0;
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public int GetCount()
        {
            return GetCountCache("");
        }
        
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Entity> GetListArray(int Top, string strWhere, string filedOrder)
        {
            return DalIns.Logs_GetListArray(Top, strWhere, filedOrder);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Entity> GetListArrayCache(int Top, string strWhere, string filedOrder)
        {
            return GetListArray(Top, strWhere, filedOrder);
            //string rawKey = string.Concat("GetListArray-", strWhere, Top, filedOrder);
            //List<Entity> lstData = CacheWebRaw.Instance.GetCacheItem<List<Entity>>(rawKey, sLogCategory);
            //if (Equals(lstData, null))
            //{
            //    //从基类调用，激活事件
            //    lstData = GetListArray(Top, strWhere, filedOrder);
            //    if (!Equals(lstData, null))
            //        CacheWebRaw.Instance.AddCacheItem(rawKey, lstData,CacheTime,spanModel, sLogCategory);
            //}
            //return lstData;
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Entity> GetListArray(int Top, string filedOrder)
        {
            return GetListArrayCache(Top, "", filedOrder);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Entity> GetListArray(string strWhere)
        {
            return GetListArrayCache(0, strWhere, "");
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Entity> GetListPages(int PageIndex, int PageSize, string strWhere, string Fileds, string oderby, out int RecordCount)
        {
            return DalIns.Logs_GetListPages(PageIndex, PageSize, strWhere, Fileds, oderby, out RecordCount);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Entity> GetListPagesCache(int PageIndex, int PageSize, string strWhere, string Fileds, string oderby, out int RecordCount)
        {
            //string rawKey = string.Concat("GlPages-", PageIndex, PageSize, strWhere, Fileds, oderby);
            //string rawKeyCount = string.Concat("C-", rawKey);
            //List<Entity> lstData = CacheWebRaw.Instance.GetCacheItem<List<Entity>>(rawKey, sLogCategory);
            //int iRecordCount = -1;
            //if (Equals(lstData, null))
            //{
            //    //从基类调用，激活事件
            //    lstData = GetListPages(PageIndex, PageSize, strWhere, Fileds, oderby, out RecordCount);
            //    if (!Equals(lstData, null))
            //    {
            //        CacheWebRaw.Instance.AddCacheItem(rawKey, lstData, CacheTime,spanModel, sLogCategory);
            //        CacheWebRaw.Instance.AddCacheItem(rawKeyCount, RecordCount.ToString(),CacheTime,spanModel, sLogCategory);
            //    }
            //}
            int iRecordCount = -1;
            List<Entity> lstData = GetListPages(PageIndex, PageSize, strWhere, Fileds, oderby, out RecordCount);
            if (iRecordCount == -1)
            {
                RecordCount = GetCountCache(strWhere);
                //string sCount = CacheWebRaw.Instance.GetCacheItem<string>(rawKeyCount, sLogCategory);
                //if (!string.IsNullOrEmpty(sCount))
                //{
                //    RecordCount = int.Parse(sCount);
                //}
                //else
                //{
                //    RecordCount = GetCountCache(strWhere);
                //}
            }
            else
            {
                RecordCount = iRecordCount;
            }
            return lstData;
        }
        /// <summary>
        /// 获得数据列表-分页
        /// </summary>
        public List<Entity> GetListPages(int PageIndex, int PageSize, out int RecordCount)
        {
            return GetListPagesCache(PageIndex, PageSize, "", "", "", out RecordCount);
        }
        /// <summary>
        /// 获得数据列表-分页
        /// </summary>
        public List<Entity> GetListPages(int PageIndex, int PageSize, string strWhere, string oderby, out int RecordCount)
        {
            return GetListPagesCache(PageIndex, PageSize, strWhere, "", oderby, out RecordCount);
        }
        /// <summary>
        /// 获得数据列表-分页
        /// </summary>
        public List<Entity> GetListPages(int PageIndex, int PageSize, string strWhere, string oderby)
        {
            int iCount = 0;
            return GetListPagesCache(PageIndex, PageSize, strWhere, "", oderby, out iCount);
        }
        /// <summary>
        /// 搜索-分页
        /// </summary>
        public List<Entity> SearchLike(int PageIndex, int PageSize, string oderby, out int RecordCount, string sKeyWord, string ColumnName)
        {
            string strWhere = "";
            if (!string.IsNullOrEmpty(sKeyWord)) strWhere = string.Format("{0} like '%{1}%'", ColumnName, sKeyWord);
            if (string.IsNullOrEmpty(strWhere))
            {
                RecordCount = 0;
                return null;
            }
            return GetListPagesCache(PageIndex, PageSize, strWhere, "", oderby, out RecordCount);
        }
         

        #endregion  成员方法

        #region  自定义方法
        public void DeleteByType(int typeid)
        {
            DalIns.Logs_DeleteByType(typeid);
        }
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Entity model)
        {
            //base.InvalidateCache();
            return DalIns.Logs_Add(model);
        }
        private void Add(object model)
        {
            Entity et = model as Entity;
            if(!Equals(et,null))
                Add(et);
        }
        public void AddLog(string sTitle, string Msg,int iLogType,string sIP)
        {
            Entity model = new Entity();
            model.Title = sTitle;
            model.Description = Msg;
            model.LogType = iLogType;
            model.IP = sIP;
            model.AddDate = DateTime.Now;

            ThreadPool.SetMaxThreads(3, 3);
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.Add), model);
             
        }
        #endregion  自定义方法
    }
}