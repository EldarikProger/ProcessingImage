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
        double[,] Eint, Eext; //энергия от формы контура и энергия зависищ. от параметров(grad,яркость)
        double a = 0.1, b = 0.2; //коэф.
        double[,] Econ, Ebal; //сглаживающая энергия и распирающая
        double c = 0.2, d = 0; //коэф.
        double[,] Emag, Egrad; // энергия яркости и градиента в точке.
        double m = 0.5, g = 0.1; //весовые коэф.
        Data data;
        public Point[] contur;
        public Point point;
        public Bitmap image;
        public int n;
        public int curI;

    }
}
