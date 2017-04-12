using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ready.src
{
    class Draw
    {

        public static Bitmap drawCounter(Bitmap image, Point[] contur)
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
            
            
            for (int i = 0; i < contur.Length; i++)
            {
                img.SetPixel(contur[i].X, contur[i].Y, Color.LightGreen);
            }
            
            
            img.Unlock();
            return clone;
        }

    }
}
