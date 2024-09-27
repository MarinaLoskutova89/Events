

using System.Threading;
using System.Threading.Tasks;

namespace Events
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            ImageDownloader imageDownloader = new();
            Dictionary<string, Task> tasks = new();

            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;
            
            Console.CursorVisible = false;
            Task consoleKeyTask = Task.Run(() => { MonitorKeypress(cancelTokenSource, tasks); });

            imageDownloader.ImageStarted += LoadingStarted;
            imageDownloader.ImageCompleted += LoadingCompleted;
            try
            {
                for (int i = 1; i <= 10; i++)
                {
                    int counter = i;
                    string fileName = $"{counter}_image.jpg";

                    Task sample = Task.Run(async () => await imageDownloader.DownloadAsync(
                        "https://s1.bloknot-voronezh.ru/thumb/850x0xcut/upload/iblock/509/0d1587dc21_7605080_8213488.jpg",
                        fileName,
                        token));
                    tasks.Add(fileName, sample);
                    await Task.Delay(500);
                }

                imageDownloader.ImageStarted -= LoadingStarted;
                imageDownloader.ImageCompleted -= LoadingCompleted;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load the image: {0}", ex.Message);
            }
        }

        public static void LoadingStarted(string fileName)
        {
            Console.WriteLine($"File: {fileName} download has started");
        }

        public static void LoadingCompleted(string fileName)
        {
            Console.WriteLine($"File: {fileName} download has completed");
        }

        public static void MonitorKeypress(CancellationTokenSource cts, Dictionary<string, Task> tasks)
        {
            ConsoleKeyInfo consoleKeyInfo;

            do
            {
                consoleKeyInfo = Console.ReadKey(true);

                if (consoleKeyInfo.Key == ConsoleKey.A) cts.Cancel();
                else
                {
                    Console.WriteLine();
                    foreach (KeyValuePair<string, Task> tsak in tasks)
                    {
                        bool taskComplete = tsak.Value.IsCompleted;
                        Console.WriteLine($"File: {tsak.Key}; loading:{taskComplete}");
                        Task.Delay(300);
                    }
                }
                if (cts.Token.IsCancellationRequested) return;

            } while(true);
        }
    }
}
