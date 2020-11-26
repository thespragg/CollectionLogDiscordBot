using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace CollectionLogBot.Helpers
{
    public static class BankImageBuilder
    {
        static string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "Resources");

        private const int Margin = 5;
        private const int FrameWidth = 7;
        private const int Width = 488;
        private const int BgHeight = 331;

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

        public static async Task<string> CreateBankImage()
        {
            var fontsCollection = new FontCollection();
            var family = fontsCollection.Install(Path.Combine(baseDir, "runescape_uf.ttf"));
            var font = family.CreateFont(20, FontStyle.Regular);
            var items = BankHelper.GetFullBank();
            var height = Math.Max((items.Count / 6) * (24 + (Margin * 3)) + 200 + 20,BgHeight) + 32 + 12;
            var textColor = Color.FromRgb(255, 153, 30);
            var bg = Path.Combine(baseDir, "tileable-bank.jpg");

            var totalPrice = 0L;

            foreach (var item in items) totalPrice += await BankHelper.GetItemPrice(item);
            var priceString = "Bank Value: " + BankHelper.ValueToKmb(totalPrice);

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

                var size = TextMeasurer.Measure(priceString, new RendererOptions(font));

                //Draw top text
                proc.DrawText(priceString, font, Color.FromRgb(255, 153, 30), new Point((Width / 2) - (int)(size.Width / 2), 10));

                var widthForItems = Width - (FrameWidth * 2) - (Margin * 4);
                var singleItemWidth = widthForItems / 8;
                x = FrameWidth + Margin * 3;
                y = 32 + 12 + Margin * 2;
                var itemCount = 0;
                var startX = x;

                foreach (var item in items)
                {
                    var itemImage = ImageHelpers.GetImage($"{item.Id}.png", baseDir);
                    proc.DrawImage(itemImage, new Point(x, y), 1);

                    itemCount += 1;
                    x += singleItemWidth;
                    if (itemCount % 8 != 0) continue;

                    y += itemImage.Height + (Margin * 3);
                    x = startX;
                }
            });

            img.Save(Path.Combine(baseDir, "bank.png"));
            return Path.Combine(baseDir, "bank.png");
        }
    }
}
