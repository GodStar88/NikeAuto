using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace NikeAuto.Engine
{
    class ImageDownloader
    {
        public string imageUrl;

        public ImageDownloader(string downloadImgUrl)
        {
            imageUrl = downloadImgUrl;
        }

        public void SaveImage(string filePath, ImageFormat format)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(imageUrl);
            using (var ms = new MemoryStream())
            {
                Bitmap bitmap = new Bitmap(stream);
                bitmap.Save(ms, format);
                var img = Image.FromStream(ms);
                img.Save(filePath);
            }
            stream.Flush();
            stream.Close();
            client.Dispose();
        }
    }
}
