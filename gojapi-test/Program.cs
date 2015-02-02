using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gojapi;

namespace gojapi_test
{
    class Program
    {

        static void Main(string[] args)
        {
            TestJudgerHTTP("127.0.0.1",1005);
            TestJudgerTCP("127.0.0.1", 1004);
        }

        static void TestJudgerHTTP(string host, int port) 
        {
            JudgerHTTP jdg = new JudgerHTTP(host,port);
        }

        static void TestJudgerTCP(string host, int port) 
        {
            JudgerTCP jdg = new JudgerTCP(host, port, "123456789");
            jdg.Ping();
            jdg.AddTask(112, "randomstring", "C", "main(void){\n  printf(&quot;hello world.\\n&quot;);\n  return 0;\n}");
            jdg.GetStatus(112, "randomstring");
        }

        
    }
}
