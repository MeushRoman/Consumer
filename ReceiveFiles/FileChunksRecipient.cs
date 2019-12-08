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
    public class FileChunksRecipient
    {
        public string Path { get; set; }
        public FileChunksRecipient(string path)
        {
            Path = path;
        }

        public void ReceiivingData()
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

                    var fileChunk = JsonConvert.DeserializeObject<FileChunk>(message);
                    WriteToFile(fileChunk);
                };

                channel.BasicConsume(queue: "files_test",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }


        public void WriteToFile(FileChunk fileChunk)
        {
            try
            {
                using (FileStream fs = new FileStream(Path + fileChunk.FileName, FileMode.Open))
                {
                    fs.Position = fileChunk.StartPosition;
                    fs.Write(fileChunk.Content, 0, fileChunk.Content.Length);
                }
            }
            catch (Exception)
            {

                
            }
            
        }
    }
}
