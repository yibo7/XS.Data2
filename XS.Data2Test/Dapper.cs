using System; 
using System.Data; 
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions; 
using XS.Data2;
using XS.Data2Test.DapperExample;

namespace XS.Data2Test
{
    
    [TestClass]
    public class Dapper
    {

        [TestMethod]
        public void Add()
        {
            BPart model = new BPart();
            model.Name = "测试" + DateTime.Now;
            model.Computed = "fsdfsdfsfsf"+ DateTime.Now;

            BPartBll.Instane.Add(model);

            Console.Write("添加成功!");
        }

        [TestMethod]
        public void GetModel()
        {
            var lst = BPartBll.Instane.GetAll();
            Console.Write("数据量："+ lst.Count);
        }

    }

      
}