using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Events
{
    public class ImageDownloader
    {
        public delegate void MethodContainer(string fileName);

        public event MethodContainer ImageStarted;
        public event MethodContainer ImageCompleted;
        public async Task DownloadAsync(string uri, string fileName, CancellationToken token)
        {
            HttpClient _httpClient = new HttpClient();
            if (token.IsCancellationRequested) return;
            if (ImageStarted != null) ImageStarted(fileName);

            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            byte[] bytes = await response.Content.ReadAsByteArrayAsync();
            _ = File.WriteAllBytesAsync(fileName, bytes);

            if (ImageCompleted != null) ImageCompleted(fileName);
        }

        public void LoadingStarted(string fileName)
        {
            Console.WriteLine($"File: {fileName} download has started");
        }

        public void LoadingCompleted(string fileName)
        {
            Console.WriteLine($"File: {fileName} download has completed");
        }

        public void MonitorKeypress(CancellationTokenSource cts, Dictionary<string, Task> tasks)
        {
            ConsoleKeyInfo consoleKeyInfo;

            do
            {
                consoleKeyInfo = Console.ReadKey(true);

                if (consoleKeyInfo.Key == ConsoleKey.A) cts.Cancel();
                else 
                {
                    Console.WriteLine();
                    foreach (var tsak in tasks)
                    {
                        bool taskComplete = tsak.Value.IsCompleted;
                        Console.WriteLine($"File: {tsak.Key}; loading:{taskComplete}");
                        Task.Delay(300);
                    }
                }
                if (cts.IsCancellationRequested) return;

            } while (true);
        }

    }
}