using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Encoder = System.Drawing.Imaging.Encoder;

namespace multimedia1
{
    public partial class Form1 : Form
    {
        private Bitmap buffer;
        private bool draw;
        private Graphics graphics;
        
        private Color color;

        FloodFill fillAlgo;

        public Form1()
        {
            InitializeComponent();
            buffer = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = pictureBox1.CreateGraphics();
            Init();
        }
        void Init()
        {
            label1.Text = "25";
            fillAlgo = new FloodFill(fillAlgo);
            fillAlgo.Bitmap = new CustomBitmap((Bitmap)pictureBox1.Image, PixelFormat.Format32bppRgb);

        }

        void StartFill(Point pt)
        {
            try { fillAlgo.StartFill(pt); } catch (Exception ex) { }
            
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(buffer, 0, 0);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (draw)
            {
                try
                {
                    Coloring(sender, e);
                }
                catch (Exception c) { 
                }
                }
        }
        void Coloring(object sender, MouseEventArgs e) {
            using (var context = Graphics.FromImage(buffer))
            {
                SolidBrush brush = new SolidBrush(color);
                graphics.FillEllipse(brush, e.X, e.Y, 7, 7);
                double x = (double)((double)e.X / (double)pictureBox1.Width) * (double)pictureBox1.Image.Width;
                double y = ((double)((double)e.Y / (double)pictureBox1.Height) * (double)pictureBox1.Image.Height);
                StartFill(new Point((int)x, (int)y));
                //  Trace.WriteLine("e.X " + (double)((double)e.X / (double)pictureBox1.Width) + "e.Y" + ((double)e.Y / (double)pictureBox1.Height));
                //  Trace.WriteLine("pic.w " + pictureBox1.Width+ "h" + pictureBox1.Height);
                //   Trace.WriteLine("s " + x + " s " + y);
                pictureBox1.Image = fillAlgo.Bitmap.Bitmap;
            }
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            draw = true;
            Coloring(sender, e);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            draw = false;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog opendFile = new OpenFileDialog();
            if (opendFile.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(opendFile.FileName);
                getBoxNewSize();
                fillAlgo.Bitmap = new CustomBitmap((Bitmap)pictureBox1.Image, PixelFormat.Format32bppRgb);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e) {
            colorDialog1.ShowDialog();
            color = colorDialog1.Color;
            toolStripButton2.BackColor = colorDialog1.Color;
            fillAlgo.FillColor = color;
        }
        private void getBoxNewSize()
        {

            double widthRatio, hightRatio;
            widthRatio = (double)pictureBox1.Width / (double)pictureBox1.Image.Width;
            hightRatio = (double)pictureBox1.Height / (double)pictureBox1.Image.Height;

            if (widthRatio < hightRatio)
            {
                pictureBox1.Height = (int)(pictureBox1.Image.Height * widthRatio);
            }
            else
            {
                pictureBox1.Width = (int)(pictureBox1.Image.Width * (hightRatio));
            }

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            color = colorDialog1.Color;
            toolStripButton2.BackColor = colorDialog1.Color;
            fillAlgo.FillColor = color;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog.Title = "Save an Image File";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
            }
        }

        private void toolStripProgressBar1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
            fillAlgo.Tolerance = new byte[] { (byte)trackBar1.Value, (byte)trackBar1.Value, (byte)trackBar1.Value};
        }
    }


    public class ToolStripTraceBarItem : ToolStripControlHost
    {
        public ToolStripTraceBarItem() : base(new TrackBar())
        {
            TrackBar tb = (TrackBar)this.Control;
        }
    }
}
