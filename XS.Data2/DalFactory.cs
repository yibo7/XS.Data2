using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace XS.Data2
{
    public class DalFactory
    {

        //private static object _SyncRoot = new object();
        //private static DataLayer _DalProvider;
        //public static DataLayer DalProvider
        //{
        //    get
        //    {

        //        if (_DalProvider == null)
        //        {
        //            lock (_SyncRoot)
        //            {
        //                if (_DalProvider == null)
        //                {
        //                    _DalProvider = new DataLayer();
        //                }
        //            }
        //        }

        //        return _DalProvider;


        //    }


        //}

        public static IDbProvider DataBaseTypeProvider
        {
            get
            {
                return new MySqlProvider();
            }


        }


    }
}
