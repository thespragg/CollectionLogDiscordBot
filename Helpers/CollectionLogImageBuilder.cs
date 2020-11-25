using System;
using System.Collections.Generic;
using System.IO;
using CollectionLogBot.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace CollectionLogBot.Helpers
{
    public static class CollectionLogImageBuilder
    {
        const int rowheight = 14;
        const int width = 488;
        const int header = 30;
        const int tabs = 22;
        const int sidebarWidth = 208;
        const int bgheight = 331;
        static string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "Resources");

        static Dictionary<string, Image> frameComponents = new Dictionary<string, Image>
            {
                {"top", Image.Load(Path.Combine(baseDir,"frame-top.png")) },
                {"bottom", Image.Load(Path.Combine(baseDir,"frame-bottom.png")) },
                {"left", Image.Load(Path.Combine(baseDir,"frame-left.png")) },
                {"right", Image.Load(Path.Combine(baseDir,"frame-right.png")) },
                {"top-left", Image.Load(Path.Combine(baseDir,"frame-top-left.png")) },
                {"top-right", Image.Load(Path.Combine(baseDir,"frame-top-right.png")) },
                {"bottom-left", Image.Load(Path.Combine(baseDir,"frame-bottom-left.png")) },
                {"bottom-right", Image.Load(Path.Combine(baseDir,"frame-bottom-right.png")) },
                {"left-join", Image.Load(Path.Combine(baseDir,"frame-left-join.png")) },
                {"right-join", Image.Load(Path.Combine(baseDir,"frame-right-join.png")) },
            };

        //Type is tab the user has requested, collection must contain merged list of bank items
        public static string CreateImage(string type, Collection collection)
        {
            var sidebar = CollectionHandler.CollectionNamesByType(collection.Type);
            var sidebarHeight = tabs + header + (sidebar.Count * rowheight);
            var itemsHeight = (collection.Items.Count / 6) * 34;
            var height = Math.Max(sidebarHeight, itemsHeight);
            height = Math.Max(bgheight, height);

            var bg = Path.Combine(baseDir, "tileable-bank.jpg");

            using var img = new Image<Rgba32>(width, height);
            var y = 0;

            while (y <= height)
            {
                var yCoord = y;
                img.Mutate(proc => proc.DrawImage(Image.Load(bg), new Point(0, yCoord), 1));
                y += bgheight;
            }

            
            img.Mutate(proc => proc.DrawImage(frameComponents["top-left"], 1));
            img.Mutate(proc => proc.DrawImage(frameComponents["left-join"], new Point(0, 32), 1));

            //Draw top
            var x = 32;
            while (x <= width - 32)
            {
                var xCoord = x;
                img.Mutate(proc => proc.DrawImage(frameComponents["top"], new Point(xCoord, 0), 1));
                x += 32;
            }

            //Draw left
            y = 44;
            while (y <= height - 32)
            {
                var yCoord = y;
                img.Mutate(proc => proc.DrawImage(frameComponents["left"], new Point(0, yCoord), 1));
                y += 32;
            }

            img.Mutate(proc => proc.DrawImage(frameComponents["top-right"], new Point(width - 32, 0), 1));

            //Draw right
            y = 44;
            while (y <= height - 32)
            {
                var yCoord = y;
                img.Mutate(proc => proc.DrawImage(frameComponents["right"], new Point(width-7, yCoord), 1));
                y += 32;
            }

            img.Mutate(proc => proc.DrawImage(frameComponents["top-right"], new Point(width - 32, 0), 1));

            //Draw bottom
            x = 32;
            while (x <= width - 32)
            {
                var xCoord = x;
                img.Mutate(proc => proc.DrawImage(frameComponents["bottom"], new Point(xCoord, height-7), 1));
                x += 32;
            }

            img.Mutate(proc => proc.DrawImage(frameComponents["bottom-left"], new Point(0, height - 32), 1));
            img.Mutate(proc => proc.DrawImage(frameComponents["bottom-right"], new Point(width - 32, height - 32), 1));

            img.Mutate(proc => proc.DrawImage(Image.Load(Path.Combine(baseDir, "close.png")), new Point(width - 30, 11), 1));
           
            img.Mutate(proc => proc.DrawImage(frameComponents["right-join"], new Point(width-11, 32), 1));
            
            img.Save(Path.Combine(baseDir, "colImage.jpg"));
            return Path.Combine(baseDir, "colImage.jpg");
        }
    }
}
