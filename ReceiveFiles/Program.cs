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

    [Serializable]
    public class FileChunk
    {
        public Guid FileGuid { get; set; }
        public byte[] Content { get; set; }
        public int ChunkN { get; set; }
        public int StartPosition { get; set; }
    }

    public class FileInfo
    {
        public string Name { get; set; }
        public string FileType { get; set; }
        public int Size { get; set; }
        public string FileSHA256 { get; set; }
        public Guid FileGuid { get; set; }
        public int CountChunks { get; set; }
    }

    public class Receive
    {
        public List<FileChunk> chunks { get; set; } = new List<FileChunk>();
        

        public void CreateFiles()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "files_info_test",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    var fi = JsonConvert.DeserializeObject<FileInfo>(message);
                    CreatingFile(fi);
                };

                channel.BasicConsume(queue: "files_info_test",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        public void rec()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "files_test",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    var fc = JsonConvert.DeserializeObject<FileChunk>(message);
                    WriteToFile(fc);
                };
                
                channel.BasicConsume(queue: "files_test",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        public void CreatingFile(FileInfo file)
        {
            byte[] arr = new byte[0];

            using (var stream = new FileStream(@"C:\test2\test.jpg", FileMode.Create))
            {
                stream.Write(arr, 0, arr.Length);
            }
        }
        
        public void WriteToFile(FileChunk fileChunk)
        {
            using (FileStream fs = new FileStream(@"C:\test2\test.jpg", FileMode.Append, FileAccess.Write))
            {
                fs.Write(fileChunk.Content, 0, fileChunk.Content.Length);                
            }            
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Receive r = new Receive();
            r.CreateFiles();
            Thread.Sleep(2000);
            r.rec();
        }
    }
}
