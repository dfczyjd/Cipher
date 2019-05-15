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
            inputTextBox.Clear();
            outputTextBox.Clear();
            cipher.clearHighlight();
            cipher.slice();
            int ringCount = cipher.Setup.ringCount;
            rotations = new int[ringCount];
            flrot = new float[ringCount];
            windowRotation = 0;
            for (int i = 1; i < ringCount; ++i)
                cipher.printSymbols(i);
            inputAllowed = outputAllowed = cipher.Setup.autoEncrypt;
            cipher.refreshRings();
            mainPictureBox.Refresh();
            moveControls();
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

        private void moveControls()
        {
            int x = (this.ClientSize.Width - mainPictureBox.Size.Width) / 2,
                y = (this.ClientSize.Height - mainPictureBox.Size.Height) / 2;
            mainPictureBox.Location = new Point(x, y);
            if (cipher == null)
                return;
            int textBoxX = this.ClientSize.Width / 2 - Constants.RING_WIDTH * cipher.Setup.ringCount,
                textBoxY = this.ClientSize.Height / 2 - Constants.RING_WIDTH * cipher.Setup.ringCount;
            inputTextBox.Size = new Size(textBoxX / 2, Constants.RING_WIDTH * cipher.Setup.ringCount + this.ClientSize.Height / 2);
            outputTextBox.Size = new Size(textBoxX / 2, Constants.RING_WIDTH * cipher.Setup.ringCount + this.ClientSize.Height / 2);
            inputTextBox.Location = new Point((textBoxX - inputTextBox.Width) / 2, textBoxY / 2);
            outputTextBox.Location = new Point((this.ClientSize.Width + textBoxX +
                Constants.RING_WIDTH * cipher.Setup.ringCount * 2 - outputTextBox.Width) / 2, textBoxY / 2);
        }

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
            
            center = new Point(Constants.HALF_IMAGE_WIDTH, Constants.HALF_IMAGE_WIDTH);
            output = new Bitmap(mainPictureBox.Width, mainPictureBox.Height);
            CipherSetup setup = CipherSetup.builtInSetups[0];
            cipher = new CipherType(this, setup);
            moveControls();
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

        private void MainForm_Resize(object sender, EventArgs e)
        {
            moveControls();
            /*int x = (this.ClientSize.Width - mainPictureBox.Size.Width) / 2,
                y = (this.ClientSize.Height - mainPictureBox.Size.Height) / 2;
            mainPictureBox.Location = new Point(x, y);
            inputTextBox.Location = new Point((x - inputTextBox.Width) / 2, y / 2);
            outputTextBox.Location = new Point((this.ClientSize.Width + x + mainPictureBox.Size.Width - outputTextBox.Width) / 2, y / 2);*/
        }

        private bool inputAllowed = true,
                    outputAllowed = true;

        private void outputTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!outputAllowed)
                return;
            if (!char.IsControl(e.KeyChar))
            {
                outputTextBox.Text += e.KeyChar;
                inputTextBox.Text += cipher.decrypt(e.KeyChar);
                inputAllowed = false;
            }
            else if (e.KeyChar == '\r')
            {
                inputTextBox.Text += "\r\n";
                outputTextBox.Text += "\r\n";
                inputAllowed = false;
            }
            cipher.refreshRings();
            mainPictureBox.Refresh();
        }

        private void outputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!outputAllowed)
                return;
            if (e.KeyCode == Keys.Back && outputTextBox.Text.Length != 0)
            {
                try
                {
                    int diskIndex = (cipher.Setup.ringCount - 2) - cipher.encryptCount;
                    cipher.Setup.alphabets[diskIndex].First(c => (char.ToUpper(c) == char.ToUpper(inputTextBox.Text.Last())));
                    cipher.encryptCount = (cipher.encryptCount + cipher.Setup.ringCount - 3) % (cipher.Setup.ringCount - 2);
                }
                catch { }
                inputTextBox.Text = inputTextBox.Text.Remove(inputTextBox.Text.Length - 1);
                outputTextBox.Text = outputTextBox.Text.Remove(inputTextBox.Text.Length);
                if (inputTextBox.Text.Length == 0)
                    inputAllowed = true;
                cipher.clearHighlight();
                cipher.refreshRings();
                mainPictureBox.Refresh();
            }
        }

        private void inputTextBox_TextChanged(object sender, EventArgs e)
        {
            HideCaret(inputTextBox.Handle);
        }

        private void inputTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!inputAllowed)
                return;
            if (!char.IsControl(e.KeyChar))
            {
                inputTextBox.Text += e.KeyChar;
                outputTextBox.Text += cipher.encrypt(e.KeyChar);
                outputAllowed = false;
            }
            else if (e.KeyChar == '\r')
            {
                inputTextBox.Text += "\r\n";
                outputTextBox.Text += "\r\n";
                outputAllowed = false;
            }
            cipher.refreshRings();
            mainPictureBox.Refresh();
            //inputTextBox.SelectionStart = inputTextBox.Text.Length;
        }

        private void inputTextBox_Enter(object sender, EventArgs e)
        {
            HideCaret(inputTextBox.Handle);
        }

        private void outputTextBox_TextChanged(object sender, EventArgs e)
        {
            HideCaret(outputTextBox.Handle);
        }

        private void outputTextBox_Enter(object sender, EventArgs e)
        {
            HideCaret(outputTextBox.Handle);
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!inputAllowed)
                return;
            if (e.KeyCode == Keys.Back && inputTextBox.Text.Length != 0)
            {
                try
                {
                    cipher.Setup.alphabets.Last().First(c => (char.ToUpper(c) == char.ToUpper(inputTextBox.Text.Last())));
                    cipher.encryptCount = (cipher.encryptCount + cipher.Setup.ringCount - 3) % (cipher.Setup.ringCount - 2);
                }
                catch (Exception exc)
                {
                    int x = 0;
                }
                inputTextBox.Text = inputTextBox.Text.Remove(inputTextBox.Text.Length - 1);
                outputTextBox.Text = outputTextBox.Text.Remove(inputTextBox.Text.Length);
                if (outputTextBox.Text.Length == 0)
                    outputAllowed = true;
                cipher.clearHighlight();
                cipher.refreshRings();
                mainPictureBox.Refresh();
            }
        }

        int selectedRing = -1;
        bool mouseDown = false;
        float currentAngle = 0;

        public static int mod(int left, int right)
        {
            return (left + right) % right;
        }

        public static float toDegrees(float radians)
        {
            return radians / (float)Math.PI * 180f;
        }

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
            float mouseAngle = toDegrees((float)Math.Atan2(dy, dx)) + 180;
            if (distSq >= maxDist * maxDist)
                return;
            currentAngle = mouseAngle - flrot[selectedRing];
            mouseDown = true;
        }

        private void mainPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (!mouseDown)
                return;
            int shift = (int)Math.Round((flrot[selectedRing] / 360 * cipher.Setup.alphabets[selectedRing].Length));
            shift = mod(shift, cipher.Setup.alphabets[selectedRing].Length);
            rotations[selectedRing] = shift;
            flrot[selectedRing] = (float)shift / cipher.Setup.alphabets[selectedRing].Length * 360;
            cipher.refreshRings();
            mainPictureBox.Refresh();
            mouseDown = false;
        }

        private void mainPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseDown)
                return;
            int dx = e.X - center.X, dy = e.Y - center.Y;
            float newAngle = toDegrees((float)Math.Atan2(dy, dx)) + 180;
            flrot[selectedRing] = newAngle - currentAngle;
            cipher.refreshRings();
            mainPictureBox.Refresh();
        }
    }
}
