using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using CollectionLogBot.Models;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace CollectionLogBot.Helpers
{
    public static class CollectionLogImageBuilder
    {
        private const int RowHeight = 20;
        private const int Width = 488;
        private const int Header = 50;
        private const int Tabs = 22;
        private const int SidebarWidth = 186;
        private const int BgHeight = 331;
        private const int TabWidth = 93;
        private const int Margin = 5;
        private const int FrameWidth = 7;
       
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
                {"divider", Image.Load(Path.Combine(baseDir, "frame-divider.png"))}
            };

        //Type is tab the user has requested, collection must contain merged list of bank items
        public static string CreateImage(string type, Collection collection)
        {
            var fontsCollection = new FontCollection();
            var family = fontsCollection.Install(Path.Combine(baseDir, "runescape_uf.ttf"));
            var font = family.CreateFont(22, FontStyle.Regular);

            var sidebar = CollectionHandler.CollectionNamesByTypeWithCompletedBool(collection.Type);
            var sidebarHeight = Tabs + Header + (sidebar.Count * RowHeight) + Margin;
            var itemsHeight = (collection.Items.Count / 6) * (24 + (Margin*3)) + 200;
            var height = Math.Max(sidebarHeight, itemsHeight) + 32 + 12 + 20 ;
            height = Math.Max(BgHeight, height);
            var textColor = Color.FromRgb(255, 153, 30);

            var bg = Path.Combine(baseDir, "tileable-bank.jpg");

            using var img = new Image<Rgba32>(Width, height);
            img.Mutate(proc =>
            {
                var y = 0;

                while (y <= height)
                {
                    var yCoord = y;
                    proc.DrawImage(Image.Load(bg), new Point(0, yCoord), 1);
                    y += BgHeight;
                }

                proc.DrawImage(frameComponents["top-left"], 1);
                proc.DrawImage(frameComponents["left-join"], new Point(0, 32), 1);

                //Draw top
                var x = 32;
                while (x <= Width - 32)
                {
                    var xCoord = x;
                    proc.DrawImage(frameComponents["top"], new Point(xCoord, 0), 1);
                    x += 32;
                }

                //Draw left
                y = 44;
                while (y <= height - 32)
                {
                    var yCoord = y;
                    proc.DrawImage(frameComponents["left"], new Point(0, yCoord), 1);
                    y += 32;
                }

                proc.DrawImage(frameComponents["top-right"], new Point(Width - 32, 0), 1);

                //Draw right
                y = 44;
                while (y <= height - 32)
                {
                    var yCoord = y;
                    proc.DrawImage(frameComponents["right"], new Point(Width - FrameWidth, yCoord), 1);
                    y += 32;
                }

                proc.DrawImage(frameComponents["top-right"], new Point(Width - 32, 0), 1);

                //Draw bottom
                x = 32;
                while (x <= Width - 32)
                {
                    var xCoord = x;
                    proc.DrawImage(frameComponents["bottom"], new Point(xCoord, height - FrameWidth), 1);
                    x += 32;
                }

                proc.DrawImage(frameComponents["bottom-left"], new Point(0, height - 32), 1);
                proc.DrawImage(frameComponents["bottom-right"], new Point(Width - 32, height - 32), 1);
                proc.DrawImage(Image.Load(Path.Combine(baseDir, "close.png")), new Point(Width - 30, 11), 1);

                //Draw divider
                x = 11;
                while (x <= Width - 11)
                {
                    var xCoord = x;
                    proc.DrawImage(frameComponents["divider"], new Point(xCoord, 35), 1);
                    x += 32;
                }

                proc.DrawImage(frameComponents["right-join"], new Point(Width - 11, 32), 1);

                var size = TextMeasurer.Measure("Collection Log", new RendererOptions(font));

                //Draw top text
                proc.DrawText("Collection Log", font, Color.FromRgb(255, 153, 30), new Point((Width / 2) - (int)(size.Width / 2), 6));

                font = family.CreateFont(14, FontStyle.Regular);

                //Draw tabs
                var tabs = CollectionHandler.GetTypes();
                var tabImage = Image.Load(Path.Combine(baseDir, "tab.png"));
                var tabActiveImage = Image.Load(Path.Combine(baseDir, "tab-active.png"));

                x = Margin + FrameWidth;
                foreach (var tab in tabs)
                {
                    var xCoord = x;
                    var imgToUse = collection.Type == tab ? tabActiveImage : tabImage;

                    proc.DrawImage(imgToUse, new Point(xCoord, 32 + 12), 1);
                    var textSize = TextMeasurer.Measure(tab, new RendererOptions(font));
                    var capitalizedTabName = tab[0].ToString().ToUpper() + tab.Substring(1);
                    proc.DrawText(capitalizedTabName, font, textColor, new Point(xCoord + (TabWidth / 2) - (int)(textSize.Width / 2) - 10, 32 + 14));
                    x += TabWidth;
                }

                //Draw sidebar
                var i = 0; //Used to change background colour for every second row
                y = 32 + 12 + 20; //Header + tabs
                x = Margin + FrameWidth;

                foreach (var sidebarItem in sidebar)
                {
                    var xCoord = x;
                    var yCoord = y;

                    //Sets the background color for the row
                    var bgColour = i % 2 == 0 ? Color.FromRgb(87, 78, 67) : Color.FromRgb(71, 61, 50);
                    if (sidebarItem.Key == collection.Name) bgColour = Rgba32.ParseHex("70695f");

                    var rect = new Rectangle(xCoord, yCoord, SidebarWidth, RowHeight);
                    proc.Fill(bgColour, rect);

                    var itemColor = sidebarItem.Value ? Color.FromRgb(16, 184, 15) : textColor;

                    proc.DrawText(sidebarItem.Key, font, itemColor, new Point(x + 5, y + 2));

                    y += 20;
                    i += 1;
                }

                font = family.CreateFont(24, FontStyle.Regular);

                //Draw info bar
                x = SidebarWidth + FrameWidth + (Margin * 2);
                y = 32 + 12 + 20 + 3;//Header + tabs
                proc.DrawText(collection.Name, font, textColor, new Point(x, y));

                font = family.CreateFont(16, FontStyle.Regular);

                y += (int)TextMeasurer.Measure(collection.Name, new RendererOptions(font)).Height + (Margin * 2);
                proc.DrawText($"Obtained:", font, textColor, new Point(x, y));
                var obtained = collection.Items.Count(x => x.Quantity > 0);
                var obtainedColor = obtained == collection.Items.Count ? Color.FromRgb(16, 184, 15) : textColor;
                var offset = TextMeasurer.Measure("Obtained:", new RendererOptions(font)).Width;
                proc.DrawText($"{obtained}/{collection.Items.Count}", font, obtainedColor, new Point(x + (int)offset + 5, y));

                var div = new Rectangle(SidebarWidth + FrameWidth + Margin, y + 18, 488 - SidebarWidth - (Margin * 3), 2);
                proc.Fill(Rgba32.ParseHex("52493b"), div);

                //Draw items
                y += 28;
                var itemCount = 0;
                var itemContainer = (488 - SidebarWidth - (Margin * 3)) / 6;
                var startX = x;

                foreach (var item in collection.Items)
                {
                    var itemImage = ImageHelpers.GetImage($"{item.Id}.png",baseDir);
                    var opacity = item.Quantity > 0 ? 1 : 0.5f;
                    try
                    {
                        proc.DrawImage(itemImage, new Point(x, y), opacity);
                    }
                    catch (Exception ex)
                    {

                    }

                    itemCount += 1;
                    x += itemContainer;
                    if (itemCount % 6 != 0) continue;

                    y += itemImage.Height + (Margin*3);
                    x = startX;
                }
            });


            img.Save(Path.Combine(baseDir, "colImage.png"));
            return Path.Combine(baseDir, "colImage.png");
        }

        
    }
}
