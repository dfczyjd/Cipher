using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cipher
{
    public partial class Form1 : Form
    {
        Bitmap bmp, output;
        Point center;
        int ringCount = 3, ringWidth;
        Bitmap[] slices;
        float[] rotations;
        string[] alphs;

        Brush black = new Pen(Color.Black).Brush,
                    white = new Pen(Color.White).Brush;

        public Form1()
        {
            InitializeComponent();
        }

        private void updateRings()
        {
            slices = slice(bmp, ringCount);
            rotations = new float[ringCount];
            alphs = new string[ringCount];
            for (int i = 0; i < ringCount - 1; ++i)
            {
                alphs[i] = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                inscribe(i);
            }
            refreshAllRings();
        }

        private Bitmap cutCircle(Bitmap bmp, int r)
        {
            Bitmap result = new Bitmap(bmp),
                tmp = new Bitmap(bmp.Width, bmp.Height);
            using (Graphics g = Graphics.FromImage(tmp))
            {
                g.FillRectangle(white, new Rectangle(0, 0, tmp.Width, tmp.Height));
                g.FillEllipse(black, new Rectangle(bmp.Width / 2 - r, bmp.Height / 2 - r, 2 * r, 2 * r));
                tmp.MakeTransparent(Color.Black);
            }
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(tmp, 0, 0);
            }
            result.MakeTransparent(Color.White);
            return result;
        }

        private Bitmap removeCircle(Bitmap bmp, int r)
        {
            Bitmap result = new Bitmap(bmp);
            using (Graphics g = Graphics.FromImage(result))
            {
                Brush b = new Pen(Color.Black).Brush;
                g.FillEllipse(b, new Rectangle(bmp.Width / 2 - r, bmp.Height / 2 - r, 2 * r, 2 * r));
                result.MakeTransparent(Color.Black);
            }
            return result;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bmp = new Bitmap(@"C:\Users\Home\Documents\ДЗ Вышка\1 КР\Текстуры\Дерево.jpg", true);
            center = new Point(this.ClientSize.Width / 2, this.ClientSize.Height / 2);
            bmp = new Bitmap(bmp, new Size(200, 200));
            output = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            updateRings();
        }

        private Bitmap[] slice(Bitmap bmp, int pieces)
        {
            Bitmap[] result = new Bitmap[pieces];
            ringWidth = 30;//bmp.Width / pieces / 2;
            for (int i = 0; i < pieces; ++i)
            {
                result[pieces - i - 1] = cutCircle(bmp, ringWidth * (i + 1) - 1);
                result[pieces - i - 1] = removeCircle(result[pieces - i - 1], ringWidth * i);
            }
            return result;
        }

        private void rotate(Graphics g, float angle, int x, int y)
        {
            g.TranslateTransform(x, y);
            g.RotateTransform(angle);
            g.TranslateTransform(-x, -y);
        }

        private void inscribe(int ringIndex)
        {
            using (Graphics g = Graphics.FromImage(slices[ringIndex]))
            {
                for (int j = 0; j < alphs[ringIndex].Length; ++j)
                {
                    Font f = new Font("Times New Roman", 8);
                    double angle = 2 * Math.PI / alphs[ringIndex].Length * j - Math.PI / 2;
                    float hw = ringWidth * (ringCount - ringIndex - 0.5F);
                    float cos = (float)Math.Cos(angle);
                    float x = hw * (float)Math.Cos(angle) + bmp.Width / 2;
                    float y = hw * (float)Math.Sin(angle) + bmp.Width / 2;
                    Bitmap text = new Bitmap(10, 14);
                    using (Graphics g2 = Graphics.FromImage(text))
                    {
                        rotate(g2, (float)(angle / Math.PI) * 180 + 90, 5, 7);
                        g2.DrawString(alphs[ringIndex].Substring(j, 1), f, black, 0, 0);
                    }
                    g.DrawImage(text, x - f.SizeInPoints / 2, y - f.Height / 2);
                    //g.DrawString(alphs[i].Substring(j, 1), f, black, x - f.SizeInPoints / 2, y - f.Height / 2);
                }
            }
        }

        private void refreshAllRings()
        {
            Bitmap turned;
            using (Graphics res = Graphics.FromImage(output))
            {
                res.FillRectangle(white, new Rectangle(new Point(0, 0), output.Size));
                for (int i = 0; i < ringCount; ++i)
                {
                    turned = new Bitmap(bmp.Width, bmp.Height);
                    using (Graphics g = Graphics.FromImage(turned))
                    {
                        rotate(g, rotations[i], bmp.Width / 2, bmp.Height / 2);
                        g.DrawImage(slices[i], 0, 0);
                    }
                    res.DrawImage(turned, center.X - bmp.Width / 2, center.Y - bmp.Height / 2);
                }
            }
        }

        private void refreshRing(int ringIndex)
        {
            Bitmap turned;
            using (Graphics res = Graphics.FromImage(output))
            {
                turned = new Bitmap(bmp.Width, bmp.Height);
                using (Graphics g = Graphics.FromImage(turned))
                {
                    rotate(g, rotations[ringIndex], bmp.Width / 2, bmp.Height / 2);
                    g.DrawImage(slices[ringIndex], 0, 0);
                }
                res.DrawImage(turned, center.X - bmp.Width / 2, center.Y - bmp.Height / 2);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(output, 0, 0);
        }

        int selectedRing = 0;
        bool mouseDown = false;
        double currentAngle = 0;

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            int dx = e.X - center.X, dy = e.Y - center.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            selectedRing = ringCount - (int)Math.Floor(dist / ringWidth) - 1;
            if (selectedRing >= ringCount - 1 || selectedRing < 0)
                return;
            currentAngle = Math.Atan2(dy, dx);
            mouseDown = true;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseDown)
                return;
            int dx = e.X - center.X, dy = e.Y - center.Y;
            double newAngle = Math.Atan2(dy, dx);
            float angle = (float)((newAngle - currentAngle) / Math.PI * 180);
            rotations[selectedRing] += angle;
            currentAngle = newAngle;
            refreshRing(selectedRing);
            this.Refresh();
        }

        private void plusButton_Click(object sender, EventArgs e)
        {
            if (ringCount < 6)
            {
                ++ringCount;
                updateRings();
                this.Refresh();
            }
        }

        private void minusButton_Click(object sender, EventArgs e)
        {
            if (ringCount > 1)
            {
                --ringCount;
                updateRings();
                this.Refresh();
            }
        }
    }
}
