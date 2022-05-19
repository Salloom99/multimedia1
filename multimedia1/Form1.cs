using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace multimedia1
{
    public partial class Form1 : Form
    {
        private Bitmap buffer;
        private bool draw;
        private Graphics graphics;

        private Color color;

        FloodFill fillAlgo ;

        public Form1()
        {
            InitializeComponent();
            buffer = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = pictureBox1.CreateGraphics();
            Init();
        }
        void Init() {
            fillAlgo = new FloodFill(fillAlgo);
            fillAlgo.Bitmap = new CustomBitmap((Bitmap)pictureBox1.Image, PixelFormat.Format32bppRgb);

        }

        void StartFill(Point pt) {
            fillAlgo.StartFill(pt);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(buffer, 0, 0);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (draw)
            {
                using (var context = Graphics.FromImage(buffer))
                {
                    SolidBrush brush = new SolidBrush(color);
                    graphics.FillEllipse(brush, e.X, e.Y, 2, 2);
                    double x = (double)((double)e.X / (double)pictureBox1.Width) * (double)pictureBox1.Image.Width;
                    double y  = ((double)((double)e.Y / (double)pictureBox1.Height )* (double)pictureBox1.Image.Height);
                    StartFill(new Point((int)x, (int)y));
                  //  Trace.WriteLine("e.X " + (double)((double)e.X / (double)pictureBox1.Width) + "e.Y" + ((double)e.Y / (double)pictureBox1.Height));
                  //  Trace.WriteLine("pic.w " + pictureBox1.Width+ "h" + pictureBox1.Height);
                 //   Trace.WriteLine("s " + x + " s " + y);
                    pictureBox1.Image = fillAlgo.Bitmap.Bitmap;
                }
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            draw = true;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            draw = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            color = colorDialog1.Color;
            button1.BackColor = colorDialog1.Color;
            fillAlgo.FillColor = color;
        }

        private void UploadImageButtonClick(object sender, EventArgs e)
        {
            OpenFileDialog opendFile = new OpenFileDialog();
            if (opendFile.ShowDialog() == DialogResult.OK)
            {

                pictureBox1.Image = new Bitmap(opendFile.FileName);
                fillAlgo.Bitmap = new CustomBitmap((Bitmap) pictureBox1.Image,PixelFormat.Format32bppRgb);
            }

        }



        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}