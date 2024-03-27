using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Dynamic;
using System.Net;

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

            if (!jsonDataList[0].ContainsKey(field))
            {
                Console.WriteLine("无效的字段选择！");
                return;
            }
            Console.WriteLine($"- 图片");
            Console.WriteLine($"- 视频");
            Console.Write("请选择下载类型：");
            string DownloadType = Console.ReadLine().Trim();


            Console.WriteLine("可选字段列表：");
            foreach (var property in firstJsonData)
            {
                Console.WriteLine($"- {property.Name}");
            }
            Console.Write("请输入去重字段：");
            string idFieldName = Console.ReadLine().Trim();


            Console.Write("请输入保存名称字段：");
            string fileName = Console.ReadLine().Trim();


            // 存储已下载的ID
            HashSet<string> downloadedIds = new HashSet<string>();

            int totalProgress = jsonDataList.Count;
            int i = 0;
            // 下载指定字段的内容

            foreach (var jsonData in jsonDataList)
            {
                string fieldValue = jsonData[field];
              
                string idValue = jsonData[idFieldName].ToString();
                // 如果ID已经存在于已下载集合中，则跳过
                if (downloadedIds.Contains(idValue))
                {
                    Console.WriteLine($"ID为 {idValue} 的数据已经下载过，跳过...");
                    continue;
                }
                //Console.WriteLine($"已选择的字段值：{fieldValue}");
                if (DownloadType == "图片")
                {
                    SaveImg(fieldValue);
                }
                else
                {
                    SaveVideo(fieldValue, jsonData[fileName]);
                }
                // 将ID添加到已下载集合中
                downloadedIds.Add(idValue);
                i++;
                DrawProgressBar(i, totalProgress);
                Thread.Sleep(1000);
              
            }
            Console.WriteLine("\n下载完成!");
            Console.ReadLine();
        }
        public static void SaveVideo(string videoUrl,string name="", string path = "Video")
        {
            try
            {
                // 创建Web请求
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(videoUrl);
                webRequest.Method = "GET";
                webRequest.Accept = "video/mp4,video/x-m4v,video/*;q=0.9";
                webRequest.Headers.Add("Accept-Language", "zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3");
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.2; rv:12.0) Gecko/20100101 Firefox/12.0";

                // 获取Web响应
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

                // 打开文件流以保存视频
                string fileName = DateTime.Now.ToFileTime().ToString() + ".mp4";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filePath = Path.Combine(path, fileName);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    // 将视频数据写入文件流
                    using (Stream webStream = webResponse.GetResponseStream())
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        while ((bytesRead = webStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fileStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }

                // Console.WriteLine($"视频已保存到：{filePath}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"异常：{e}");
            }
        }
        public static void SaveImg(string data, string path = "Img")
        {
            System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(data);
            webRequest.Method = "GET";
            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            webRequest.Headers.Add("Accept-Language", "zh-cn,zh;q=0.8,en-us;q=0.5,en;q=0.3");
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.2; rv:12.0) Gecko/20100101 Firefox/12.0";
            System.Net.HttpWebResponse webResponse = (System.Net.HttpWebResponse)webRequest.GetResponse();

            System.IO.Stream s = webResponse.GetResponseStream();
            System.Drawing.Image img = System.Drawing.Bitmap.FromStream(s);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileName = DateTime.Now.ToFileTime().ToString() + ".jpg";
            string filePath = Path.Combine(path, fileName);
            Bitmap bmp = new Bitmap(img);

            bmp.Save(filePath);
            img.Dispose();
            s.Close();

        }
        public static void DrawProgressBar(int progress, int total)
        {
            Console.Write("\r"); // 光标移动到行首
            int barLength = 30;
            int filledLength = (int)(((double)progress / total) * barLength);


            string progressBar = "[" + new string('#', filledLength) + new string('-', barLength - filledLength) + "]";
            Console.Write(progressBar + $" {progress * 100.0 / total}%");
        }


    }

}
