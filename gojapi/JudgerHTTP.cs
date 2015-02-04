using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace gojapi
{
    public class JudgerHTTP
    {
        private string url = "";

        public JudgerHTTP(string host, int port) 
        {
            if (port <= 0)
            {
                port = 80;
            }

            this.url = "http://" + host + ":" + port.ToString();

            this.Post(this.url, "{\"test\":\"hello world.\"}");
        }

        private string Post(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
            request.CookieContainer = null;
            Stream myRequestStream = request.GetRequestStream();
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

            Object respObj = fastJSON.JSON.Parse(resp);

            return respObj;
        }
    }
}
