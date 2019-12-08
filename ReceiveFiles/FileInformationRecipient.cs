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
    public class FileInformationRecipient
    {
        public string Path { get; set; }

        public FileInformationRecipient(string path)
        {
            Path = path;            
        }

        public void ReceiivingData()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            //ThreadPool.QueueUserWorkItem(worker =>
            //{
               //while (true)
               // {
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

                            var file = JsonConvert.DeserializeObject<FileInfo>(message);
                            FileCreation(file);
                        };                        

                        channel.BasicConsume(queue: "files_info_test",
                                             autoAck: true,
                                             consumer: consumer);

                        Console.WriteLine(" Press [enter] to exit.");
                        //
                        Console.ReadLine();
                       // Thread.Sleep(100000);
                }
               // }
          //  });
        }
      

        public void FileCreation(FileInfo file)
        {
            byte[] arr = new byte[file.Size];

            using (var stream = new FileStream(Path + file.Name, FileMode.Create))
            {
                stream.Write(arr, 0, arr.Length);
            }
        }
    }
}
