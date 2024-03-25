using Newtonsoft.Json;

using System.Dynamic;

namespace XST_DJOND_Console
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.Write("请输入 JSON 文件的绝对路径：");
            string jsonFilePath = Console.ReadLine().Trim();

            if (!File.Exists(jsonFilePath))
            {
                Console.WriteLine("指定的文件不存在！");
                return;
            }

            string json = File.ReadAllText(jsonFilePath);

            // 将JSON字符串解析为dynamic对象列表
            List<dynamic> jsonDataList = JsonConvert.DeserializeObject<List<dynamic>>(json);

            if (jsonDataList.Count == 0)
            {
                Console.WriteLine("JSON 文件中没有数据！");
                return;
            }

            // 显示字段列表供用户选择（假设所有元素的结构相同，这里只取第一个元素的字段）
            dynamic firstJsonData = jsonDataList[0];
            Console.WriteLine("可选字段列表：");
            foreach (var property in firstJsonData)
            {
                Console.WriteLine($"- {property.Name}");
            }

            // 用户选择要下载的字段
            Console.Write("请选择要下载的字段：");
            string field = Console.ReadLine().Trim();

            // 确认选择是否合法

            if (!firstJsonData.HasPropertys(field))
            {
                Console.WriteLine("无效的字段选择！");
                return;
            }

            // 下载指定字段的内容
            foreach (var jsonData in jsonDataList)
            {
                string fieldValue = jsonData[field];
                Console.WriteLine($"已选择的字段值：{fieldValue}");

                // 在这里执行下载操作，使用 fieldValue 作为下载地址
                // 下载操作可以是您的下载逻辑或调用其他方法
            }

        }


    }
    public static class DynamicExtensions
    {
        public static bool HasPropertys(this object obj, string propertyName)
        {
            var objType = obj.GetType();
            return objType.GetProperty(propertyName) != null;
        }
    }
}
