using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace gojapi
{
    public class JudgerTCP
    {
        private Socket conn = null;
        private char mark = '#';
        private bool login = false;
        private string judger_os = "";

        /// <summary>
        /// TCP方式连接
        /// </summary>
        /// <param name="host">主机名</param>
        /// <param name="port">端口</param>
        public JudgerTCP(string host, int port, string password) 
        {
            this.conn = this.ConnectSocket(host,port);
            if (this.conn == null) {
                System.Console.WriteLine("Connect Failed!");
                return;
            }
            else
            {
                string header = this.Receive();
                int idx = header.IndexOf(this.mark);

                if(idx - 1 > 0){
                    idx = idx - 1;
                }else{
                    idx = 0;
                }
                
                this.mark = header[idx];
            }

            // login
            if (this.Login(password))
            {
                this.login = true;
            }
            else
            {
                this.login = false;
            }

        }

        /// <summary>
        /// 创建socket连接
        /// </summary>
        /// <param name="server">服务器地址</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        private Socket ConnectSocket(string server, int port)
        {
            Socket s = null;
            IPHostEntry hostEntry = null;

            // Get host related information.
            hostEntry = Dns.GetHostEntry(server);

            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    s = tempSocket;
                    break;
                }
                else
                {
                    continue;
                }
            }
            return s;
        }

        /// <summary>
        /// 通用发送请求
        /// </summary>
        /// <param name="req">请求内容</param>
        /// <returns>服务器返回内容</returns>
        public Object Request(Object req)
        {
            if (this.conn == null) {
                System.Console.WriteLine("Failure, Socket connection not exist.");
                return null;
            }

            // send
            string reqStr = this.MsgPack(req);
            Byte[] bytesSent = Encoding.ASCII.GetBytes(reqStr);
            this.conn.Send(bytesSent, bytesSent.Length, 0);
            // recv
            string content = this.Receive();
            int idx = content.IndexOf(this.mark);
            string subContent = content.Substring(0, idx);

            System.Console.WriteLine(subContent);
            return this.jsonDecode(subContent);
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <returns>串</returns>
        private string Receive()
        {
            // recv
            int BUF_LEN = 10;
            Byte[] bytesReceived = new Byte[BUF_LEN];
            int bytes = 0;
            string page = "";
            string frame = "";
            while (true)
            {
                bytes = this.conn.Receive(bytesReceived);
                page = page + frame;

                if (bytes > 0)
                {
                    // check mark
                    frame = Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                    int idx = frame.IndexOf(this.mark);
                    if(idx >= 0){
                        page = page + frame;
                        break;
                    }
                }
                
            }

            return page;
        }

        /// <summary>
        /// 消息打包
        /// </summary>
        /// <param name="msg">消息内容</param>
        /// <returns>待发送信息流</returns>
        public string MsgPack(Object msg)
        {
            return this.jsonEncode(msg)+"\x3";
        }

        /// <summary>
        /// json编码
        /// </summary>
        /// <param name="obj">Dictionary对象</param>
        /// <returns>json串</returns>
        private string jsonEncode(Object obj) 
        {
            string json = fastJSON.JSON.ToJSON(obj);
            return json;
        }

        /// <summary>
        /// json解码
        /// </summary>
        /// <param name="json">json串</param>
        /// <returns>对象</returns>
        private Object jsonDecode(string json) 
        {
            return fastJSON.JSON.Parse(json);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username">用户/session id</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        private bool Login(string password)
        {
            Dictionary<String, String> obj = new Dictionary<String, String>();
            obj.Add("action", "login");
            obj.Add("password", password);

            Object resp = this.Request(obj);
            if(resp == null) 
            {
                return false;
            }

            Dictionary<string, object> respDict = (Dictionary<string, object>)resp;
            bool result =  (bool)respDict["result"];
            this.judger_os = (string)respDict["os"];

            return result;
        }

        /// <summary>
        /// 测试网络连接是否连通
        /// </summary>
        /// <returns></returns>
        public bool Ping()
        {
            Dictionary<String, String> obj = new Dictionary<string, string>();
            obj.Add("action", "ping");

            Object resp = this.Request(obj);

            if (resp == null)
            {
                return false;
            }

            Dictionary<string, object> respDict = (Dictionary<string, object>)resp;
            bool result = (bool)respDict["result"];
            string pong = (string)respDict["msg"];

            if ("pong" != pong)
            {
                return false;
            }

            return result;
        }

        /// <summary>
        /// 添加判题任务
        /// </summary>
        /// <param name="id">题目id/全局唯一</param>
        /// <param name="sid">会话id</param>
        /// <param name="language">语言C/C++</param>
        /// <param name="code">源码（htmlencode之前）</param>
        /// <returns>judger返回内容，null失败</returns>
        public Dictionary<string, object> AddTask(int id, string sid, string language, string code)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();
            obj.Add("action", "task_add");
            obj.Add("sid", sid);
            obj.Add("id", id);
            obj.Add("time", DateTime.Now);
            obj.Add("language", language);

            // htmlencode
            code = HttpUtility.HtmlEncode(code);

            obj.Add("code", code);

            if (!this.login)
            {
                return null;
            }

            object resp = this.Request(obj);

            return (Dictionary<string, object>)resp;
        }

        /// <summary>
        /// 获取当前任务的判题状态
        /// </summary>
        /// <param name="id">任务id</param>
        /// <param name="sid">连接标识sid</param>
        /// <returns>返回内容</returns>
        public Dictionary<string, object> GetStatus(int id, string sid)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();
            obj.Add("action", "task_info");
            obj.Add("sid", sid);
            obj.Add("id", id);

            if (!this.login)
            {
                return null;
            }

            object resp = this.Request(obj);
            Dictionary<string, object> respDict = (Dictionary<string, object>)resp;
            return respDict;
        }

    }

}
