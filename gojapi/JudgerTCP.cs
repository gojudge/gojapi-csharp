using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gojapi
{
    public class JudgerTCP
    {
        /// <summary>
        /// TCP方式连接
        /// </summary>
        /// <param name="host">主机名</param>
        /// <param name="port">端口</param>
        public JudgerTCP(string host, int port) 
        {
        }

        /// <summary>
        /// 通用发送请求
        /// </summary>
        /// <param name="req">请求内容</param>
        /// <returns>服务器返回内容</returns>
        public Object Request(Object req)
        {
            return null;
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
            return null;
        }
    }
}
