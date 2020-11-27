using System;
using System.IO;
using System.Net.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace CollectionLogBot.Helpers
{
    public static class ImageHelpers
    {
        private static readonly HttpClient ImageSource = new HttpClient { BaseAddress = new Uri("https://chisel.weirdgloop.org/static/img/osrs-sprite/") };

        public static Image GetImage(string imageName, string baseDir)
        {
            var imgPath = Path.Combine(baseDir, "ItemCache", imageName);
            var fileInCache = File.Exists(imgPath);

            if (fileInCache) return Image.Load(imgPath);

            using var bytes = ImageSource.GetStreamAsync(imageName).Result;
            var itemImage = Image.Load(bytes, new PngDecoder());
            itemImage.Save(imgPath);
            return itemImage;
        }
    }
}
