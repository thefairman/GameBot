using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    public class WFDigits
    {
        Dictionary<int, List<bool[,]>> digits;
        public WFDigits()
        {
            digits = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, List<bool[,]>>>(File.ReadAllText("digitsmaparray.json"), new CustomDateTimeConverter());
        }

        void SaveImage(Bitmap img, int digit, List<int> not100percent)
        {
            if (img == null)
                return;
            string imgDirPath = Environment.CurrentDirectory + "\\img_not100percents\\";
            if (!Directory.Exists(imgDirPath))
                Directory.CreateDirectory(imgDirPath);
            string digitsStr = digit.ToString();
            string resStr = "";
            int iter = 0;
            foreach (var item in digitsStr)
            {
                if (not100percent.Contains(iter++))
                    resStr += $"({item})";
                else
                    resStr += item;
            }
            string plLogin = Global.userLogic.GetNoSetUserInfo() == null ? "unknown" : Global.userLogic.GetUserInfo().Login.Replace("@", "(at)");
            string fileName = $"{imgDirPath}{plLogin}_{resStr}_{(Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds}.bmp";
            img.Save(fileName, System.Drawing.Imaging.ImageFormat.Bmp);
            img.Dispose();
        }

        class compareData
        {
            public int key = 0;
            public int skip = 0;
            public double match = 0;
        }
        int GetMaxMatchDigit(int startX, int startY, out int skippedX, out bool isMaxMatch)
        {
            compareData maxMatch = new compareData();
            int x = 0;
            for (; x < 3; x++)
            {
                foreach (var item in digits)
                {
                    foreach (var compareDigit in item.Value)
                    {
                        int mathesCur = 0;
                        for (int yc = 0; yc < compareDigit.GetLength(1); yc++)
                        {
                            for (int xc = 0; xc < compareDigit.GetLength(0); xc++)
                            {
                                Color cl = ActionsWithGameLogic.ScreenOfGame[x + startX + xc, startY + yc];
                                if (compareDigit[xc, yc] != ((cl.R + cl.G + cl.B) / 3) > 80)
                                    mathesCur++;
                            }
                        }
                        if (mathesCur == compareDigit.Length)
                        {
                            skippedX = x + compareDigit.GetLength(0) - 1;
                            isMaxMatch = true;
                            return item.Key;
                        }
                        else
                        {
                            double match = Convert.ToDouble(mathesCur) / compareDigit.Length;
                            if (maxMatch.match < match)
                            {
                                maxMatch.key = item.Key;
                                maxMatch.match = match;
                                maxMatch.skip = x + compareDigit.GetLength(0) - 1;
                            }
                        }
                    }
                }
            }
            skippedX = maxMatch.skip;
            isMaxMatch = false;
            return maxMatch.key;
        }

        int GetInt(int startX, int startY, int lenghtX)
        {
            List<int> not100percent = new List<int>();
            int digits = 0;
            for (int x = 0; x <= lenghtX; x++)
            {
                // проверяем вертикальную полоску на наличие пикселей
                bool haveSomePix = false;
                for (int y1 = 0; y1 < 7; y1++)
                {
                    Color cl = ActionsWithGameLogic.ScreenOfGame[x + startX, y1 + startY];
                    if (((cl.R + cl.G + cl.B) / 3) > 80)
                    {
                        haveSomePix = true;
                        break;
                    }
                }
                if (!haveSomePix)
                    continue;
                int skippedX;
                bool isMaxMatch;
                int curDigit = GetMaxMatchDigit(x + startX, startY, out skippedX, out isMaxMatch);
                x += skippedX;

                if (digits == 0)
                    digits = curDigit;
                else
                {
                    digits *= 10;
                    digits += curDigit;
                }

                if (!isMaxMatch)
                {
                    if (digits != 0)
                        not100percent.Add((int)Math.Log10(digits));
                }
            }

            if (not100percent.Count > 0)
            {
                Bitmap img = new Bitmap(lenghtX + 5, 14);
                for (int ys = 0; ys < 7; ys++)
                {
                    for (int xs = 0; xs < lenghtX + 5; xs++)
                    {
                        Color curc = ActionsWithGameLogic.ScreenOfGame[startX + xs, startY + ys];
                        img.SetPixel(xs, ys, curc);
                        bool black = ((curc.R + curc.G + curc.B) / 3) > 80;
                        img.SetPixel(xs, ys + 7, black ? Color.Black : Color.White);
                    }
                }
                SaveImage(img, digits, not100percent);
            }
            return digits;
        }

        public int GetWB()
        {
            return GetInt(546, 580, 32);
        }

        public int GetCrowns()
        {
            return GetInt(463, 580, 32);
        }

        int FindDefis(int startX, int startY)
        {
            int more100Once = 0;
            for (int x = 0; x < 30; x++)
            {
                int more100 = 0;
                for (int y = 0; y < 7; y++)
                {
                    if (ActionsWithGameLogic.ScreenOfGame[startX + x, startY + y].R > 100)
                        more100++;
                    if (more100 > 1)
                    {
                        more100Once = 0;
                        break;
                    }
                }
                if (more100 == 1)
                    more100Once++;
                if (more100Once > 1)
                    return x;
            }
            return 30;
        }

        WFCurrency CheckCurrency()
        {
            for (int x = 39; x < 47; x++)
            {
                Color pix = ActionsWithGameLogic.ScreenOfGame[x, 407];
                if (Helpers.Between(pix, 236, 0, 0, 5))
                    return WFCurrency.Credits;
                if (Helpers.Between(pix, 242, 198, 76, 5))
                    return WFCurrency.Crowns;
                if (Helpers.Between(pix, 164, 233, 164, 5))
                    return WFCurrency.WB;
            }
            return WFCurrency.Credits;
        }

        public int GetCostOfWarBox(out WFCurrency currency)
        {
            currency = CheckCurrency();
            int lenghtX = FindDefis(51, 404) - 6;
            return GetInt(51, 404, lenghtX);
        }

        int FindPercentChar(int startX, int startY)
        {
            int haveInRow = 0;
            for (int x = 24; x >= 0; x--)
            {
                bool haveSome = false;
                for (int y = 0; y < 7; y++)
                {
                    if (Helpers.MoreAny(ActionsWithGameLogic.ScreenOfGame[startX + x, startY + y], 60))
                    {
                        haveSome = true;
                        break;
                    }
                }
                if (haveSome)
                    haveInRow++;
                else
                    haveInRow = 0;
                if (haveInRow >= 5)
                    return x;
            }
            return 0;
        }

        public int GetVipValue()
        {
            int lenghtX = FindPercentChar(298, 580) - 6;
            return GetInt(298, 580, lenghtX);
        }
    }
}
