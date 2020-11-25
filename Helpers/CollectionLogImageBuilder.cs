using System;
using System.IO;
using CollectionLogBot.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace CollectionLogBot.Helpers
{
    public static class CollectionLogImageBuilder
    {
        //Type is tab the user has requested, collection must contain merged list of bank items
        public static string CreateImage(string type, Collection collection)
        {
            const int rowheight = 14;
            const int width = 488;
            const int header = 30;
            const int tabs = 22;
            const int sidebarWidth = 208;
            const int bgheight = 331;

            var sidebar = CollectionHandler.CollectionNamesByType(collection.Type);
            var sidebarHeight = tabs + header + (sidebar.Count * rowheight);
            var itemsHeight = (collection.Items.Count / 6) * 34;
            var height = Math.Max(sidebarHeight, itemsHeight);
            height = Math.Max(bgheight, height);

            var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "Resources");
            var bg =  Path.Combine(baseDir,"tileable-bank.jpg");

            using var img = new Image<Rgba32>(width, height);
            var y = 0;

            while (y <= height)
            {
                var yCoord = y;
                img.Mutate(proc => proc.DrawImage(Image.Load(bg),new Point(0,yCoord),1));
                y += bgheight;
            }
            
            img.Save(Path.Combine(baseDir,"colImage.jpg"));

            return Path.Combine(baseDir, "colImage.jpg");
        }
    }
}
