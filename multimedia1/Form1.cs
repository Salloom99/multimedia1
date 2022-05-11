using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

        public Form1()
        {
            InitializeComponent();
            buffer = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = pictureBox1.CreateGraphics();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(buffer,0,0);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (draw)
            {
                using (var context = Graphics.FromImage(buffer))
                {
                    SolidBrush brush = new SolidBrush(color);
                    graphics.FillEllipse(brush, e.X, e.Y,10,10);
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
        }
    }
}
