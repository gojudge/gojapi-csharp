﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace gojapi
{
    public class JudgerTCP
    {
        private Socket conn = null;

        /// <summary>
        /// TCP方式连接
        /// </summary>
        /// <param name="host">主机名</param>
        /// <param name="port">端口</param>
        public JudgerTCP(string host, int port) 
        {
            this.conn = this.ConnectSocket(host,port);
            if (this.conn == null) {
                System.Console.WriteLine("Connect Failed!");
                return;
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

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host IP Address is not compatible with the address family
            // (typical in the IPv6 case).
            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket =
                    new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

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

            string content = this.Receive();

            System.Console.WriteLine(content);

            return null;
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <returns></returns>
        private string Receive()
        {
            // recv
            int BUF_LEN = 256;
            Byte[] bytesReceived = new Byte[BUF_LEN];
            int bytes = 0;
            string page = "";
            while (true)
            {
                bytes = this.conn.Receive(bytesReceived);
                if (bytes == 0)
                {
                    break;
                }
                page = page + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
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
            return null;
        }
    }
}