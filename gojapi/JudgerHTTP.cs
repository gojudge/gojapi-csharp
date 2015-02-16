using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.IO;

namespace gojapi
{
    public class JudgerHTTP
    {
        private string url = "";
        private string password = "";
        private bool login = false;

        public JudgerHTTP(string host, int port, string password) 
        {
            if (port <= 0)
            {
                port = 80;
            }

            this.url = "http://" + host + ":" + port.ToString();

            bool rst = this.Login(password);
            if (!rst)
            {
                System.Console.WriteLine("Login Failed!");
            }
            else
            {
                this.login = true;
            }
        }

        private string Post(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
            request.CookieContainer = null;

            Stream myRequestStream = null;

            try
            {
                myRequestStream = request.GetRequestStream();
            }
            catch (Exception)
            {
                System.Console.WriteLine("Connection Failure.");
                return null;
            }
            
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();
 
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            response.Cookies = null;
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
 
            return retString;
        }

        /// <summary>
        /// 信息打包
        /// </summary>
        /// <param name="obj">Dictionary对象</param>
        /// <returns>json串</returns>
        public string MsgPack(Object obj)
        {
            string json = fastJSON.JSON.ToJSON(obj);
            return json;
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="obj">请求Dictionary</param>
        /// <returns>返回Dictionary</returns>
        public Object Request(Object obj)
        {
            if (obj == null)
            {
                return null;
            }

            string req = this.MsgPack(obj);
            string resp = this.Post(this.url, req);

            Object respObj = null;
            try
            {
                respObj = fastJSON.JSON.Parse(resp);
            }catch(Exception e){
                return null;
            }
            
            return respObj;
        }

        /// <summary>
        /// 客户端登陆验证
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool Login(string password)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();
            obj.Add("action", "login");
            obj.Add("password", password);

            this.password = password;

            object resp = this.Request(obj);
            Dictionary<string, object> respDict = (Dictionary<string, object>)resp;
            
            return (bool)respDict["result"];
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="sid">session id</param>
        /// <param name="language">语言</param>
        /// <param name="code">未encode的源码</param>
        /// <returns>返回对象</returns>
        public Dictionary<string, object> AddTask(int id, String sid, String language, String code)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();
            obj.Add("action", "task_add");
            obj.Add("password", this.password);
            obj.Add("id", id);
            obj.Add("sid", sid);
            obj.Add("language", language);

            // htmlencode
            code = HttpUtility.HtmlEncode(code);

            obj.Add("code", code);

            object respObj = this.Request(obj);

            return (Dictionary<string, object>)respObj;
        }

        /// <summary>
        /// 获取任务状态
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="sid">session id</param>
        /// <returns>返回对象</returns>
        public Dictionary<string, object> GetStatus(int id, String sid)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();
            obj.Add("action", "task_info");
            obj.Add("password", this.password);
            obj.Add("id", id);
            obj.Add("sid", sid);

            object respObj = this.Request(obj);

            return (Dictionary<string, object>)respObj;
        }
    }
}
