using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Cipher
{
    public partial class MainForm : Form
    {
        public Bitmap output;
        public Point center;
        public int[] rotations;
        public float[] flrot;
        public float windowRotation;
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

        public void updateRings()
        {
            cipher.clearHighlight();
            cipher.slice();
            int ringCount = cipher.Setup.ringCount;
            rotations = new int[ringCount];
            flrot = new float[ringCount];
            windowRotation = 0;
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

        private void showAbout(object sender, EventArgs e)
        {
            AboutForm dialog = new AboutForm();
            dialog.ShowDialog();
        }

        private void showHistory(object sender, EventArgs e)
        {
            HistoryForm dialog = new HistoryForm();
            dialog.ShowDialog();
        }

        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        private void mainForm_Load(object sender, EventArgs e)
        {
            MenuItem[] info = new MenuItem[2] { new MenuItem("История шифрования", showHistory),
                                                new MenuItem("О программе", showAbout) };
            MenuItem[] main = new MenuItem[2] { new MenuItem("Изменить шифратор", new EventHandler(changeDisks)),
                                                 new MenuItem("Справка", info) };
            this.Menu = new MainMenu(main);
            mainPictureBox.BackColor = Color.Transparent;
            this.Location = new Point(0, 0);
            this.Size = Screen.GetWorkingArea(this).Size;
            

            mainPictureBox.Size = new Size(Constants.IMAGE_WIDTH, Constants.IMAGE_HEIGHT);
            int x = (this.ClientSize.Width - mainPictureBox.Size.Width) / 2,
                y = (this.ClientSize.Height - mainPictureBox.Size.Height) / 2;
            mainPictureBox.Location = new Point(x, y);
            inputTextBox.Location = new Point((x - inputTextBox.Width) / 2, y / 2);
            outputTextBox.Location = new Point((this.ClientSize.Width + x + mainPictureBox.Size.Width - outputTextBox.Width) / 2, y / 2);

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
            cipher.clearHighlight();
            int dx = e.X - center.X, dy = e.Y - center.Y;
            float maxDist = Constants.RING_WIDTH * cipher.Setup.ringCount;
            int distSq = dx * dx + dy * dy;
            double dist = Math.Sqrt(distSq);
            selectedRing = (int)Math.Floor(dist / Constants.RING_WIDTH);
            if (selectedRing == 0)
                return;
            float mouseAngle = (float)(Math.Atan2(dy, dx) + Math.PI / 2);
            if (Math.Abs(mouseAngle / Math.PI * 180 - windowRotation) < cipher.Setup.WindowAngle)
            {
                maxDist += Constants.RING_WIDTH * 0.5f;
                if (distSq >= maxDist * maxDist)
                    return;
                selectedRing = 0;
                currentAngle = -windowRotation / 180 * Math.PI + mouseAngle;
            }
            else
            {
                if (distSq >= maxDist * maxDist)
                    return;
                currentAngle = -flrot[selectedRing] / 180 * Math.PI + mouseAngle;
            }
            mouseDown = true;
        }

        private void mainPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (!mouseDown)
                return;
            int shift;
            if (selectedRing == 0)
            {
                //shift = (int)Math.Round((windowRotation / 360 * cipher.Setup.alphabets[1].Length));
                //shift %= cipher.Setup.alphabets[1].Length;
                //windowRotation = (float)shift / cipher.Setup.alphabets[1].Length * 360;
            }
            else
            {
                shift = (int)Math.Round((flrot[selectedRing] / 360 * cipher.Setup.alphabets[selectedRing].Length));
                shift = (shift + cipher.Setup.alphabets[selectedRing].Length) % cipher.Setup.alphabets[selectedRing].Length;
                rotations[selectedRing] = shift;
                flrot[selectedRing] = (float)shift / cipher.Setup.alphabets[selectedRing].Length * 360;
            }
            cipher.refreshRings();
            mainPictureBox.Refresh();
            mouseDown = false;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            int x = (this.ClientSize.Width - mainPictureBox.Size.Width) / 2,
                y = (this.ClientSize.Height - mainPictureBox.Size.Height) / 2;
            mainPictureBox.Location = new Point(x, y);
            inputTextBox.Location = new Point((x - inputTextBox.Width) / 2, y / 2);
            outputTextBox.Location = new Point((this.ClientSize.Width + x + mainPictureBox.Size.Width - outputTextBox.Width) / 2, y / 2);
        }

        private void inputTextBox_TextChanged(object sender, EventArgs e)
        {
            HideCaret(inputTextBox.Handle);
        }

        private void inputTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar))
            {
                inputTextBox.Text += e.KeyChar;
                outputTextBox.Text += cipher.encrypt(e.KeyChar);
            }
            else if (e.KeyChar == '\r')
            {
                inputTextBox.Text += "\r\n";
                outputTextBox.Text += "\r\n";
            }
            cipher.refreshRings();
            mainPictureBox.Refresh();
            //inputTextBox.SelectionStart = inputTextBox.Text.Length;
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back && inputTextBox.Text.Length != 0)
            {
                inputTextBox.Text = inputTextBox.Text.Remove(inputTextBox.Text.Length - 1);
                outputTextBox.Text = outputTextBox.Text.Remove(inputTextBox.Text.Length);
                cipher.clearHighlight();
                cipher.refreshRings();
                mainPictureBox.Refresh();
            }
        }

        private void mainPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseDown)
                return;
            if (selectedRing == 0)
            {
                //int dx = e.X - center.X, dy = e.Y - center.Y;
                //double newAngle = Math.Atan2(dy, dx) + Math.PI / 2;
                //windowRotation = (float)((newAngle - currentAngle) * 180 / Math.PI);
            }
            else
            {
                int dx = e.X - center.X, dy = e.Y - center.Y;
                double newAngle = Math.Atan2(dy, dx) + Math.PI / 2;
                flrot[selectedRing] = (float)((newAngle - currentAngle) * 180 / Math.PI);
            }
            cipher.refreshRings();
            mainPictureBox.Refresh();
        }
    }
}
