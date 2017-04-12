using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ready.src
{
    class Processing
    {

        int N = 20;
        double[,] Ei; // энергия в точке i
        double[,] Eint, Eext; //энергия от формы контура и энергия зависищ. от параметров(grad,яркость)
        double a = 1, b = 1; //коэф.
        double[,] Econ, Ebal; //сглаживающая энергия и распирающая
        double c = 30, d = 5; //коэф.
        double[,] Emag, Egrad; // энергия яркости и градиента в точке.
        double m = 10, g = 10; //весовые коэф.
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
                n = contur.Length + 1;
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
            int J=0, K=0;
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

        void findEi(Point vi)
        {
            findEint(vi);
            findEext(vi);
            //высчитываем Еi
            Ei = new double[n, n];
            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    Ei[j, k] = a * Eint[j, k] + b * Eext[j, k];
                }
            }
        }

        void findEint(Point vi)
        {
            findEcon(vi);
            findEbal(vi);
            //высчитываем Eint
            Eint = new double[n, n];
            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    Eint[j, k] = c * Econ[j, k] + d * Ebal[j, k];
                }
            }
        }

        Point p(int j, int k, Point vi, int jv, int kv)
        {
            Point p = new Point();
            p.X = vi.X + j - jv;
            p.Y = vi.Y + k - kv;
            return p;
        }

        Point Gv(Point vi)
        {
            Point a = new Point();
            a.X = nextVi().X - prevVi().X;
            a.Y = nextVi().Y - prevVi().Y;
            return a;
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

        double Ebjk(Point vi, Point q)
        {
            double x1 = (double)(vi.X - prevVi().X) / norma(vi, prevVi());
            double y1 = (double)(vi.Y - prevVi().Y) / norma(vi, prevVi());
            double x2 = (double)(nextVi().X - vi.X) / norma(nextVi(), vi);
            double y2 = (double)(nextVi().Y - vi.Y) / norma(nextVi(), vi);
            double x = x1 + x2;
            double y = y1 + y2; // координаты вектора нормали
            double n = Math.Sqrt(x * x + y * y);
            double tq = Math.Sqrt(q.X * q.X + q.Y * q.Y);
            //угол между векторами и окочательная формула
            double ab = x * q.X + y * q.Y;
            double cosf = ab / (n * tq);
            double f = Math.Acos(cosf);
            return n * tq * Math.Sin(f);
        }

        void findEbal(Point vi)
        {
            Ebal = new double[n, n];
            int jv = (n - 1) / 2;
            int kv = (n - 1) / 2;
            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    Point p = this.p(j, k, vi, jv, kv);
                    Point q = new Point(vi.X - p.X, vi.Y - p.Y);
                    Ebal[j, k] = Ebjk(vi, q);
                }
            }
        }

        void findEext(Point vi)
        {
            findEmag(vi);
            findEgrad(vi);
            //высчитываем Eint
            Eext = new double[n, n];
            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    Eext[j, k] = m * Emag[j, k] + g * Egrad[j, k];
                }
            }
        }

        double I(Color pixel)
        {
            return (0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
        }

        void findEmag(Point vi)
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



            Emag = new double[n, n];
            int jv = (n - 1) / 2;
            int kv = (n - 1) / 2;
            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    Point p = this.p(j, k, vi, jv, kv);
                    Emag[j, k] = I(img.GetPixel(p.X, p.Y));
                }
            }

            img.Unlock();
        }

        double modulGrad(Point p, FastBitmap img)
        {
            if (p.X == 0 || p.Y == 0)
                return 0;
            double df1 = I(img.GetPixel(p.X, p.Y)) - I(img.GetPixel(p.X - 1, p.Y));
            double df2 = I(img.GetPixel(p.X, p.Y)) - I(img.GetPixel(p.X, p.Y - 1));
            return -Math.Sqrt(df1 * df1 + df2 * df2);
        }

        void findEgrad(Point vi)
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


            Egrad = new double[n, n];
            int jv = (n - 1) / 2;
            int kv = (n - 1) / 2;
            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    Point p = this.p(j, k, vi, jv, kv);
                    Egrad[j, k] = modulGrad(p, img);
                }
            }

            img.Unlock();
        }
    }
}
