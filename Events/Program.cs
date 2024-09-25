

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
            Task consoleKeyTask = Task.Run(() => { imageDownloader.MonitorKeypress(cancelTokenSource, tasks); });

            imageDownloader.ImageStarted += imageDownloader.LoadingStarted;
            imageDownloader.ImageCompleted += imageDownloader.LoadingCompleted;
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

                imageDownloader.ImageStarted -= imageDownloader.LoadingStarted;
                imageDownloader.ImageCompleted -= imageDownloader.LoadingCompleted;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load the image: {0}", ex.Message);
            }
        }
    }
}
