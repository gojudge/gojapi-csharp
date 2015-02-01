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
            TestJudgerHTTP("oj.duguying.net",1005);
            TestJudgerTCP("oj.duguying.net", 1004);
        }

        static void TestJudgerHTTP(string host, int port) 
        {
            JudgerHTTP jdg = new JudgerHTTP(host,port);
        }

        static void TestJudgerTCP(string host, int port) 
        {
            JudgerTCP jdg = new JudgerTCP(host, port, "123456789");

        }

        
    }
}
