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
        public Bitmap output;
        public Point center;
        public int ringCount = 6, ringWidth;
        public int[] rotations;
        public float[] flrot;
        public string[] alphs;
        public CipherType cipher;

        private Bitmap paperBmp, woodBmp, metalBmp;

        public static Brush black = new Pen(Color.Black).Brush,
                    white = new Pen(Color.White).Brush;

        public Form1()
        {
            InitializeComponent();
        }

        public void selectTexture(string texture)
        {
            switch (texture)
            {
                case "Бумага":
                    cipher = new Paper(this);
                    cipher.bmp = paperBmp;
                    break;

                case "Дерево":
                    cipher = new Wood(this);
                    cipher.bmp = woodBmp;
                    break;

                case "Металл":
                    cipher = new Metal(this);
                    cipher.bmp = metalBmp;
                    break;
            }
        }

        public void updateRings()
        {
            cipher.slice(cipher.bmp, ringCount);
            rotations = new int[ringCount];
            flrot = new float[ringCount];
            for (int i = 1; i < ringCount; ++i)
                cipher.inscribe(i);
            cipher.refreshAllRings();
            pictureBox1.Refresh();
        }

        private void changeDisks(object sender, EventArgs e)
        {
            Form2 dialog = new Form2();
            dialog.owner = this;
            dialog.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MenuItem[] items = new MenuItem[1] { new MenuItem("Изменить шифратор", new EventHandler(changeDisks)) };
            this.Menu = new MainMenu(items);
            cipher = new Paper(this);
            paperBmp = new Bitmap(@"C:\Users\Home\Documents\ДЗ Вышка\1 КР\Текстуры\Бумага.jpg", true);
            woodBmp = new Bitmap(@"C:\Users\Home\Documents\ДЗ Вышка\1 КР\Текстуры\Дерево.jpg", true);
            //wood.bmp2 = new Bitmap(@"C:\Users\Home\Documents\ДЗ Вышка\1 КР\Текстуры\Дерево 2.jpg", true);
            metalBmp = new Bitmap(@"C:\Users\Home\Documents\ДЗ Вышка\1 КР\Текстуры\Металл 2.jpg", true);
            center = new Point(pictureBox1.Width / 2, pictureBox1.Height / 2);
            paperBmp = new Bitmap(paperBmp, new Size(400, 400));
            woodBmp = new Bitmap(woodBmp, new Size(400, 400));
            //wood.bmp2 = new Bitmap(wood.bmp2, new Size(400, 400));
            metalBmp = new Bitmap(metalBmp, new Size(400, 400));
            output = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            selectTexture("Металл");
            alphs = new string[ringCount];
            for (int i = 1; i < ringCount; ++i)
                alphs[i] = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            updateRings();
        }

        public static void rotate(Graphics g, float angle, int x, int y)
        {
            g.TranslateTransform(x, y);
            g.RotateTransform(angle);
            g.TranslateTransform(-x, -y);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(output, 0, 0);
        }

        int selectedRing = -1;
        bool mouseDown = false;
        double currentAngle = 0;

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            int dx = e.X - center.X, dy = e.Y - center.Y;
            int maxDist = ringWidth * ringCount,
                distSq = dx * dx + dy * dy;
            if (distSq >= maxDist * maxDist)
                return;
            double dist = Math.Sqrt(distSq);
            selectedRing = (int)Math.Floor(dist / ringWidth);
            if (selectedRing == 0)
                return;
            currentAngle = -flrot[selectedRing] / 180 * Math.PI + Math.Atan2(dy, dx) + Math.PI / 2;
            mouseDown = true;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!mouseDown)
                return;
            int shift = (int)Math.Round((flrot[selectedRing] / 360 * alphs[selectedRing].Length));
            shift %= alphs[selectedRing].Length;
            flrot[selectedRing] = (float)shift / alphs[selectedRing].Length * 360;
            cipher.refreshRing(selectedRing);
            pictureBox1.Refresh();
            mouseDown = false;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseDown)
                return;
            int dx = e.X - center.X, dy = e.Y - center.Y;
            double newAngle = Math.Atan2(dy, dx) + Math.PI / 2;
            flrot[selectedRing] = (float)((newAngle - currentAngle) * 180 / Math.PI);
            cipher.refreshRing(selectedRing);
            pictureBox1.Refresh();
        }
    }
}
