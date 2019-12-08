using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReceiveFiles
{  
    class Program
    {
        static void Main(string[] args)
        {
            Consumer consumer = new Consumer();

            Console.ReadLine();            
        }
    }
}
