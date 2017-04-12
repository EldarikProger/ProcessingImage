using ready.src;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ready
{
    public partial class Form1 : Form
    {

        Bitmap  image,newImage;

        public Form1()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "All Pictures|*.jpg;*.png;*.bmp;*.jpeg;*.tiff|Bitmap|*.bmp|Jpeg|*.jpg;*.jpeg|Png|*.png";

            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                    return;
                string filename = openFileDialog1.FileName;
                image = new Bitmap(filename);
                Bitmap clone = new Bitmap(image.Width, image.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics gr = Graphics.FromImage(clone))
                {
                    gr.DrawImage(image, new Rectangle(0, 0, clone.Width, clone.Height));
                }
                image = clone;
                newImage = image;
                pictureBox1.Image = image;
                pictureBox1.Invalidate();
            }
            catch
            {
                DialogResult rezult = MessageBox.Show("Невозможно открыть выбранный файл",
                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (newImage == null)
            {
                MessageBox.Show("Файл не открыт!",
                 "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Jpeg|*.jpg|Bitmap|*.bmp|Png|*.png|Gif|*.gif";
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = saveFileDialog1.FileName;
            newImage.Save(filename);
            MessageBox.Show("Файл сохранен");
        }

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        Processing processing = new Processing();

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            float x = ((float)e.X) / pictureBox1.Width * image.Width;
            float y = ((float)e.Y) / pictureBox1.Height * image.Height;
            processing.point = new Point((int)x,(int)y);
            processing.initContur();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                newImage = Draw.drawCounter(image, processing.contur);
            }
            catch
            {
                MessageBox.Show("Файл не открыт!",
                 "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            pictureBox1.Image = newImage;
            pictureBox1.Invalidate();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            processing.image = image;
            processing.startProcessing();
            try
            {
                newImage = Draw.drawCounter(image, processing.contur);
            }
            catch
            {
                MessageBox.Show("Файл не открыт!",
                 "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            pictureBox1.Image = newImage;
            pictureBox1.Invalidate();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            try
            {
                newImage = Filters.MedianFilter3x3(image);
            }
            catch(Exception ee)
            {
                MessageBox.Show(ee.ToString(),
                 "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            pictureBox1.Image = newImage;
            pictureBox1.Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                newImage = Filters.MedianFilter3x3(image);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString(),
                 "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            pictureBox1.Image = newImage;
            pictureBox1.Invalidate();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            image = newImage;
        }
    }
}
