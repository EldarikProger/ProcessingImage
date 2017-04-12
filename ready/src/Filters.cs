using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ready.src
{
    public class Filters
    {

        public static Bitmap MedianFilter3x3(Bitmap image)
        {
            Bitmap clone = new Bitmap(image.Width, image.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics gr = Graphics.FromImage(clone))
            {
                gr.DrawImage(image, new Rectangle(0, 0, clone.Width, clone.Height));
            }
            FastBitmap img = new FastBitmap(clone);
            img.Lock();
            if (img == null)
                throw new Exception();
            int xx, yy;
            int[] filter = new int[9];
            for (int y = 1; y < img.Height-1; y++)
            {
                for (int x = 1; x < img.Width-1; x++)
                {
                    xx = x - 1;
                    yy = y - 1;

                    for (int j = 0; j < 3; j++)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Color pixel = img.GetPixel(xx + i, yy + j);
                            filter[3 * i + j] = pixel.G;
                        }
                    }
                    int a = median3(filter);
                    img.SetPixel(x, y, Color.FromArgb(a, a, a));
                }
            }


            img.Unlock();
            return clone;
        }

        public static Bitmap MedianFilter5x5(Bitmap image)
        {
            Bitmap clone = new Bitmap(image.Width, image.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics gr = Graphics.FromImage(clone))
            {
                gr.DrawImage(image, new Rectangle(0, 0, clone.Width, clone.Height));
            }
            FastBitmap img = new FastBitmap(clone);
            img.Lock();
            if (img == null)
                throw new Exception();
            int xx, yy;
            int[] filter = new int[25];
            for (int y = 1; y < img.Height - 2; y++)
            {
                for (int x = 1; x < img.Width - 2; x++)
                {
                    xx = x - 2;
                    yy = y - 2;

                    for (int j = 0; j < 5; j++)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Color pixel = img.GetPixel(xx + i, yy + j);
                            filter[5 * i + j] = pixel.G;
                        }
                    }
                    int a = median5(filter);
                    img.SetPixel(x, y, Color.FromArgb(a, a, a));
                }
            }


            img.Unlock();
            return clone;
        }

        public static int median5(int[] filter)
        {
            Array.Sort(filter);
            return filter[4];
        }

        public static int median3(int[] filter)
        {
            Array.Sort(filter);
            return filter[4];
        }

    }
}
