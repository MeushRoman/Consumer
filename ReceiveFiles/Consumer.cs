using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReceiveFiles
{
    public class Consumer
    {
        public string HostName;
        public string UerName;
        public string Password;
        public static string Path { get; set; } = @"C:\test2\";

        public FileChunksRecipient fileChunks { get; set; } 
        public FileInformationRecipient fileInformation { get; set; }

        public Consumer()
        {
            fileInformation = new FileInformationRecipient(Path);
            fileChunks = new FileChunksRecipient(Path);

            fileInformation.ReceiivingData();
            fileChunks.ReceiivingData();
        }
    }
}
