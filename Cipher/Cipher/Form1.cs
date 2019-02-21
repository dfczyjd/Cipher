using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Resources;

namespace Cipher
{
    public partial class MainForm : Form
    {
        public Bitmap output;
        public Point center;
        public int[] rotations;
        public float[] flrot;
        public CipherType cipher;

        public static Brush black = new Pen(Color.Black).Brush,
                    white = new Pen(Color.White).Brush;

        public static Pen paperPen = new Pen(Color.Black);
        public static Pen metalPen = new Pen(Color.FromArgb(192, Color.Black), 3);
        public static Pen woodPen = new Pen(Color.FromArgb(0x80, 0x40, 0x20), 3);
        public static Pen bronzePen = new Pen(Color.FromArgb(0x80, 0x40, 0x20), 3);//new Pen(Color.FromArgb(0xE2, 0xA5, 0x6F), 3);

        public MainForm()
        {
            InitializeComponent();
        }

        /*public void selectTexture(string texture)
        {
            switch (texture)
            {
                case "Бумага":
                    cipher.pen = paperPen;
                    cipher.brush = paperPen.Brush;
                    cipher.bmp = paperBmp;
                    cipher.material = MaterialName.Paper;
                    break;

                case "Дерево":
                    cipher.pen = woodPen;
                    cipher.brush = woodPen.Brush;
                    cipher.bmp = woodBmp;
                    cipher.material = MaterialName.Wood;
                    break;

                case "Металл":
                    cipher.pen = metalPen;
                    cipher.brush = metalPen.Brush;
                    cipher.bmp = metalBmp;
                    cipher.material = MaterialName.Metal;
                    break;

                case "Бронза":
                    cipher.pen = bronzePen;
                    cipher.brush = bronzePen.Brush;
                    cipher.bmp = bronzeBmp;
                    cipher.material = MaterialName.Bronze;
                    break;
            }
        }*/

        public void updateRings()
        {
            cipher.slice();
            int ringCount = cipher.setup.ringCount;
            rotations = new int[ringCount];
            flrot = new float[ringCount];
            for (int i = 1; i < ringCount; ++i)
                cipher.inscribe(i);
            cipher.refreshRings();
            mainPictureBox.Refresh();
        }

        private void changeDisks(object sender, EventArgs e)
        {
            Form2 dialog = new Form2();
            dialog.owner = this;
            dialog.cipher = cipher;
            dialog.ShowDialog();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            MenuItem[] items = new MenuItem[1] { new MenuItem("Изменить шифратор", new EventHandler(changeDisks)) };
            this.Menu = new MainMenu(items);
            mainPictureBox.BackColor = Color.FromArgb(255, Color.Black);

            center = new Point(Constants.HALF_IMAGE_WIDTH, Constants.HALF_IMAGE_WIDTH);
            output = new Bitmap(mainPictureBox.Width, mainPictureBox.Height);
            CipherSetup setup = CipherSetup.builtInSetups[0];
            cipher = new CipherType(this, setup);
            updateRings();
        }

        public static void rotate(Graphics g, float angle, int x, int y)
        {
            g.TranslateTransform(x, y);
            g.RotateTransform(angle);
            g.TranslateTransform(-x, -y);
        }

        private void mainPictureBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(output, 0, 0);
        }

        int selectedRing = -1;
        bool mouseDown = false;
        double currentAngle = 0;

        private void mainPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            int dx = e.X - center.X, dy = e.Y - center.Y;
            int maxDist = Constants.RING_WIDTH * cipher.setup.ringCount,
                distSq = dx * dx + dy * dy;
            if (distSq >= maxDist * maxDist)
                return;
            double dist = Math.Sqrt(distSq);
            selectedRing = (int)Math.Floor(dist / Constants.RING_WIDTH);
            if (selectedRing == 0)
                return;
            currentAngle = -flrot[selectedRing] / 180 * Math.PI + Math.Atan2(dy, dx) + Math.PI / 2;
            mouseDown = true;
        }

        private void mainPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (!mouseDown)
                return;
            int shift = (int)Math.Round((flrot[selectedRing] / 360 * cipher.setup.alphabets[selectedRing].Length));
            shift %= cipher.setup.alphabets[selectedRing].Length;
            flrot[selectedRing] = (float)shift / cipher.setup.alphabets[selectedRing].Length * 360;
            cipher.refreshRings();
            mainPictureBox.Refresh();
            mouseDown = false;
        }

        private void mainPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseDown)
                return;
            int dx = e.X - center.X, dy = e.Y - center.Y;
            double newAngle = Math.Atan2(dy, dx) + Math.PI / 2;
            flrot[selectedRing] = (float)((newAngle - currentAngle) * 180 / Math.PI);
            cipher.refreshRings();
            mainPictureBox.Refresh();
        }
    }
}
