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
            using (HttpClient _httpClient = new())
            {
                HttpResponseMessage response = await _httpClient.GetAsync(uri, token);

                if (ImageStarted != null) ImageStarted(fileName);
                
                response.EnsureSuccessStatusCode();
                byte[] bytes = await response.Content.ReadAsByteArrayAsync(token);
                _ = File.WriteAllBytesAsync(fileName, bytes, token);

                if (ImageCompleted != null) ImageCompleted(fileName);
            }
        }
    }
}