using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_4
{
    public partial class Form1 : Form
    {
        Graphics g;
        PointF[] points;

        Pen pen_black = new Pen(Color.Black);
        Pen pen_blue = new Pen(Color.Blue);
        List<int> outer = new List<int>();

        public Form1()
        {
            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(pictureBox1.BackColor);
            pictureBox1.Invalidate();

            int w = pictureBox1.Width;
            int h = pictureBox1.Height;

            int k = trackBar1.Value * 2;

            points = new PointF[k];

            var rand = new Random();

            for (int i = 0; i < k; ++i)
            {
                points[i].X = rand.Next(w + 1);
                points[i].Y = rand.Next(h + 1);
            }

            if (k != 0)
            {
                g.DrawRectangle(pen_blue, new Rectangle(70, 80, 100, 100));
            }

            for (int i = 0; i < k - 1; i += 2)
            {
                g.DrawLine(pen_black, points[i], points[i + 1]);
            }

            label1.Text = $"{k / 2} отрезков";
        }

        int get_code(PointF point)
        {
            int code = 0b0000;
            code |= point.X < 70 ? 0b0001 : 0b0000;
            code |= point.X > 170 ? 0b0010 : 0b0000;
            code |= point.Y < 80 ? 0b1000 : 0b0000;
            code |= point.Y > 180 ? 0b0100 : 0b0000;
            return code;
        }

        int clip_segment(int i)
        {
            int code_a = get_code(points[i]);
            int code_b = get_code(points[i + 1]);
            int code;

            float x, y;

            PointF temp;

            while ((code_a | code_b) != 0b0000)
            {
                if ((code_a & code_b) != 0b0000)
                {
                    return 2;
                }

                if (code_a != 0b0000)
                {
                    code = code_a;
                    temp = points[i];
                }
                else
                {
                    code = code_b;
                    temp = points[i + 1];
                }

                if ((code & 0b0001) != 0b0000)
                {
                    temp.Y += (points[i].Y - points[i + 1].Y) * (70 - temp.X) / (points[i].X - points[i + 1].X);
                    temp.X = 70;
                }
                else if ((code & 0b0010) != 0b0000)
                {
                    temp.Y += (points[i].Y - points[i + 1].Y) * (170 - temp.X) / (points[i].X - points[i + 1].X);
                    temp.X = 170;
                }
                else if ((code & 0b0100) != 0b0000)
                {
                    temp.X += (points[i].X - points[i + 1].X) * (180 - temp.Y) / (points[i].Y - points[i + 1].Y);
                    temp.Y = 180;
                }
                else if ((code & 0b1000) != 0b0000)
                {
                    temp.X += (points[i].X - points[i + 1].X) * (80 - temp.Y) / (points[i].Y - points[i + 1].Y);
                    temp.Y = 80;
                }

                if(code == code_a)
                {
                    points[i] = temp;
                    code_a = get_code(points[i]);
                }
                else
                {
                    points[i + 1] = temp;
                    code_b = get_code(points[i + 1]);
                }
            }
            return 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (points == null)
                return;
            else if (points.Length == 0)
                return;
            g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(pictureBox1.BackColor);
            pictureBox1.Invalidate();

            int w = pictureBox1.Width;
            int h = pictureBox1.Height;

            for (int i = 0; i < points.Length - 1; i += 2)
            {
                switch (clip_segment(i))
                {
                    case 1:
                        continue;
                    case 2:
                        outer.Add(i);
                        break;
                }
            }

            g.DrawRectangle(pen_blue, new Rectangle(70, 80, 100, 100));

            for (int i = 0; i < points.Length - 1; i += 2)
            {
                if (outer.Contains(i))
                {
                    continue;
                }
                g.DrawLine(pen_black, points[i], points[i + 1]);
            }

            outer.Clear();
        }
    }
}
