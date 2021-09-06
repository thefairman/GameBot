using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTrainer
{
    class Helpers
    {
        public static bool Between(Color color, int R, int G, int B, int halfDiapForAllOrR = 0, int halfDiapG = -1, int halfDiapB = -1)
        {
            if (halfDiapForAllOrR < 0)
                halfDiapForAllOrR = 0;
            if (halfDiapG < 0)
                halfDiapG = halfDiapForAllOrR;
            if (halfDiapB < 0)
                halfDiapB = halfDiapForAllOrR;

            return ((color.R >= R - halfDiapForAllOrR && color.R <= R + halfDiapForAllOrR) &&
                (color.G >= G - halfDiapG && color.G <= G + halfDiapG) &&
                (color.B >= B - halfDiapB && color.B <= B + halfDiapB));
        }

        public static bool NumIsBetween(int num, int dest, int zapas)
        {
            return num >= dest - zapas && num <= dest + zapas;
        }

        public static bool Between(Color color, Color compaired, int halfDiapForAllOrR = 0, int halfDiapG = -1, int halfDiapB = -1)
        {
            return Between(color, compaired.R, compaired.G, compaired.B, halfDiapForAllOrR, halfDiapG, halfDiapB);
        }

        public static bool Less(Color pix, int col)
        {
            return pix.R <= col && pix.G <= col && pix.B <= col;
        }

        public static int GetAvgColor(Color col)
        {
            return (col.R + col.G + col.B) / 3;
        }

        public static bool IsSearchedColor(Color pixel, Color searchedCol, double toleranceSquared)
        {
            int diffR = pixel.R - searchedCol.R;
            int diffG = pixel.G - searchedCol.G;
            int diffB = pixel.B - searchedCol.B;

            int distance = diffR * diffR + diffG * diffG + diffB * diffB;
            return distance <= toleranceSquared;
        }

        static public double GetDifrenceBetweenAngels(double angle1, double angle2)
        {
            if (angle1 > angle2)
                return 360 - (angle1 - angle2);
            else
                return angle2 - angle1;
        }

        public static double MiniusDegrees(double source, double val)
        {
            double tmpAngleMinus = source - (val % 360);
            return tmpAngleMinus < 0 ? tmpAngleMinus + 360 : tmpAngleMinus;
        }

        public static double PlusDegrees(double source, double val)
        {
            return (source + val) % 360;
        }

        public static double MinusOrPlusDegreesBy180(double source, double val)
        {
            if (val <= 180) return (source + val) % 360;
            return MiniusDegrees(source, 360 - val);
        }

        public static double GetAngleBetweenPoints(Point point1, Point point2)
        {
            double x1 = point1.X, y1 = point1.Y;
            double x2 = point2.X, y2 = point2.Y;
            double A = Math.Atan2(y1 - y2, x1 - x2) / Math.PI * 180;
            return (A < 0) ? A + 360 : A;   //Без этого диапазон от 0...180 и -1...-180
        }

        public static bool LessAny(Color pix, int col)
        {
            return pix.R <= col || pix.G <= col || pix.B <= col;
        }

        public static bool More(Color pix, int col)
        {
            return pix.R >= col && pix.G >= col && pix.B >= col;
        }

        public static bool MoreAny(Color pix, int col)
        {
            return pix.R >= col || pix.G >= col || pix.B >= col;
        }

        static Dictionary<char, string> translation = new Dictionary<char, string>()
        {
            { 'q', "кв" },
            { 'w', "ш" },
            { 'e', "е" },
            { 'r', "г" },
            { 't', "Т" },
            { 'y', "у" },
            { 'u', "и" },
            { 'i', "1" },
            { 'o', "о" },
            { 'p', "р" },
            { 'a', "а" },
            { 's', "5" },
            { 'd', "д" },
            { 'f', "ф" },
            { 'g', "г" },
            { 'h', "Н" },
            { 'j', "дж" },
            { 'k', "К" },
            { 'l', "1" },
            { 'z', "2" },
            { 'x', "х" },
            { 'c', "с" },
            { 'v', "в" },
            { 'b', "В" },
            { 'n', "п" },
            { 'm', "М" },

        };

        static string ToTranslit(char c)
        {
            string result;
            if (translation.TryGetValue(c, out result))
                return result;
            else
                return c.ToString();
        }

        static char[] allowedChars = {'й', 'ц', 'у', 'к', 'е', 'н', 'г', 'ш', 'щ', 'з', 'х', 'ъ', 'ё', 'ф', 'ы', 'в', 'а', 'п',
            'р', 'о', 'л', 'д', 'ж', 'э', 'я', 'ч', 'с', 'м', 'и', 'т', 'ь', 'б', 'ю','Й', 'Ц', 'У', 'К', 'Е', 'Н', 'Г', 'Ш', 'Щ',
            'З', 'Х', 'Ъ', 'Ё', 'Ф', 'Ы', 'В', 'А', 'П', 'Р', 'О', 'Л', 'Д', 'Ж', 'Э', 'Я', 'Ч', 'С', 'М', 'И', 'Т', 'Ь', 'Б', 'Ю',
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '_', '.'};

        public static string ToTranslit(string src)
        {
            string nick = string.Join("", src.Select(ToTranslit));
            string nickRes = "";
            foreach (var item in nick)
            {
                if (allowedChars.Contains(item))
                    nickRes += item;
            }
            return nickRes;
        }
    }
}
