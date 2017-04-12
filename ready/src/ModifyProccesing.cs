using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ready.src
{
    public class ModifyProccesing
    {
        int N = 20;
        double[,] Ei; // энергия в точке i
        double c = 40, b = 10, d = 10; //коэф.
        double[,] Econ, Ebomb, Ebright; //сглаживающая энергия и распирающая

        Data data;
        public Point[] contur;
        public Point point;
        public Bitmap image;
        public int n;
        public int curI;


        public void initContur()
        {
            contur = new Point[8];
            contur[0] = new Point(point.X - 1, point.Y - 1);
            contur[1] = new Point(point.X, point.Y - 1);
            contur[2] = new Point(point.X + 1, point.Y - 1);
            contur[3] = new Point(point.X + 1, point.Y);
            contur[4] = new Point(point.X + 1, point.Y + 1);
            contur[5] = new Point(point.X, point.Y + 1);
            contur[6] = new Point(point.X - 1, point.Y + 1);
            contur[7] = new Point(point.X - 1, point.Y);
        }

        public void startProcessing()
        {
            if (contur.Length % 2 == 0)
                n = contur.Length - 1;
            else
                n = contur.Length;
            for (int i = 0; i < contur.Length; i++)
            {
                curI = i;
                findEi(contur[i]);
                minEi(ref contur[i]);
            }
        }


        void minEi(ref Point vi)
        {
            double min = Ei[0, 0];
            int jv = (n - 1) / 2;
            int kv = (n - 1) / 2;
            int J = 0, K = 0;
            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    if (Ei[j, k] < min)
                    {
                        min = Ei[j, k];
                        J = j;
                        K = k;
                    }
                }
            }
            Point p = this.p(J, K, vi, jv, kv);
            vi = p;
        }

        Point p(int j, int k, Point vi, int jv, int kv)
        {
            Point p = new Point();
            p.X = vi.X + j - jv;
            p.Y = vi.Y + k - kv;
            return p;
        }

        void findEi(Point vi)
        {
            findEbomb(vi);
            findEbright(vi);
            findEcon(vi);
            //высчитываем Еi
            Ei = new double[n, n];
            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    Ei[j, k] = b * Ebright[j, k] + c * Econ[j, k] + d * Ebomb[j, k];
                }
            }
        }

        Point prevVi()
        {
            if (curI == 0)
                return contur[contur.Length - 1];
            return contur[curI - 1];
        }

        Point nextVi()
        {
            if (curI == contur.Length - 1)
                return contur[0];
            return contur[curI + 1];
        }

        Point Gv(Point vi)
        {
            Point a = new Point();
            a.X = nextVi().X - prevVi().X;
            a.Y = nextVi().Y - prevVi().Y;
            return a;
        }

        double norma(Point v1, Point v2)
        {
            int x = v2.X - v1.X;
            int y = v2.Y - v1.Y;
            return Math.Sqrt(x * x + y * y);
        }

        double norma(double x1, double x2, double y1, double y2)
        {
            double x = x2 - x1;
            double y = y2 - y1;
            return Math.Sqrt(x * x + y * y);
        }

        double lV()
        {
            double n = 0;
            for (int i = 0; i < contur.Length - 1; i++)
            {
                n += Math.Pow(norma(contur[i + 1], contur[i]), 2);
            }
            n += Math.Pow(norma(contur[0], contur[contur.Length - 1]), 2);
            n *= contur.Length;
            return n;
        }

        void findEcon(Point vi)
        {
            double gamma = 1 / (2 * Math.Cos(2 * Math.PI / n));
            Econ = new double[n, n];
            int jv = (n - 1) / 2;
            int kv = (n - 1) / 2;
            Point v = this.Gv(vi);
            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    Point p = this.p(j, k, vi, jv, kv);
                    Econ[j, k] = 1 / lV() * Math.Pow(norma(p.X, v.X * gamma, p.Y, v.Y * gamma), 2);
                }
            }
        }

        double I(Color pixel)
        {
            return (0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
        }

        void findEbright(Point vi)
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



            Ebright = new double[n, n];
            int jv = (n - 1) / 2;
            int kv = (n - 1) / 2;
            double sum=0;
            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    Point p = this.p(j, k, vi, jv, kv);
                    sum += I(img.GetPixel(p.X, p.Y));
                }
            }
            sum = sum /n /n;
            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    Ebright[j, k] = sum;
                }
            }

            img.Unlock();
        }


        void findEbomb(Point vi)
        {
            Ebomb = new double[n, n];
            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    Ebomb[j, k] = Math.Sqrt( Math.Pow(j-vi.X,2) + Math.Pow(k-vi.Y,2) );
                }
            }
        }

    }
}
