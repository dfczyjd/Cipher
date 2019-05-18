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
    //Класс формы основного окна
    public partial class MainForm : Form
    {
        public Bitmap output;                   // Поле для вывода модели классом шифратора
        public int[] cellRotations;             // Повороты дисков в ячейках
        public float[] degreeRotations;         // Повороты дисков в градусах
        public CipherType cipher;               // Отображаемая модель шифратора

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обновить модель
        /// </summary>
        public void updateCipher()
        {
            inputTextBox.Clear();
            outputTextBox.Clear();
            cipher.clearHighlight();
            cipher.createRings();
            int ringCount = cipher.Setup.ringCount;
            cellRotations = new int[ringCount];
            degreeRotations = new float[ringCount];
            for (int i = 1; i < ringCount; ++i)
                cipher.printSymbols(i);
            inputAllowed = outputAllowed = cipher.Setup.autoEncrypt;
            cipher.refreshRings();
            mainPictureBox.Refresh();
            MainForm_Resize(null, null);
        }

        /// <summary>
        /// Открыть окно настроек
        /// </summary>
        /// <param name="sender">Не используется</param>
        /// <param name="e">Не используется</param>
        private void changeDisks(object sender, EventArgs e)
        {
            EditForm dialog = new EditForm
            {
                owner = this,
                cipher = cipher
            };
            dialog.ShowDialog();
        }

        /// <summary>
        /// Открыть окно "О программе"
        /// </summary>
        /// <param name="sender">Не используется</param>
        /// <param name="e">Не используется</param>
        private void showAbout(object sender, EventArgs e)
        {
            AboutForm dialog = new AboutForm();
            dialog.ShowDialog();
        }

        /// <summary>
        /// Открыть окно исторической справки
        /// </summary>
        /// <param name="sender">Не используется</param>
        /// <param name="e">Не используется</param>
        private void showHistory(object sender, EventArgs e)
        {
            HistoryForm dialog = new HistoryForm();
            dialog.ShowDialog();
        }

        /// <summary>
        /// Скрыть курсор в окне
        /// </summary>
        /// <param name="hWnd">Дескриптор окна</param>
        /// <returns></returns>
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
            mainPictureBox.Size = new Size(CipherType.IMAGE_WIDTH, CipherType.IMAGE_HEIGHT);
            
            output = new Bitmap(mainPictureBox.Width, mainPictureBox.Height);
            CipherSetup setup = CipherSetup.loadedSetups[0];
            cipher = new CipherType(this, setup);
            MainForm_Resize(null, null);
            updateCipher();
        }

        private void mainPictureBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(output, 0, 0);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            int x = (this.ClientSize.Width - mainPictureBox.Size.Width) / 2,
                y = (this.ClientSize.Height - mainPictureBox.Size.Height) / 2;
            mainPictureBox.Location = new Point(x, y);
            if (cipher == null)
                return;
            int textBoxX = this.ClientSize.Width / 2 - CipherType.RING_WIDTH * cipher.Setup.ringCount,
                textBoxY = this.ClientSize.Height / 2 - CipherType.RING_WIDTH * cipher.Setup.ringCount;
            // Установить поля ввода открытого и зашифрованного текстов
            // посередине областей справа и слева от изображения модели
            inputTextBox.Size = new Size(textBoxX / 2, CipherType.RING_WIDTH * cipher.Setup.ringCount +
                                                        this.ClientSize.Height / 2);
            outputTextBox.Size = new Size(textBoxX / 2, CipherType.RING_WIDTH * cipher.Setup.ringCount +
                                                        this.ClientSize.Height / 2);
            inputTextBox.Location = new Point((textBoxX - inputTextBox.Width) / 2, textBoxY / 2);
            outputTextBox.Location = new Point((this.ClientSize.Width + textBoxX +
                CipherType.RING_WIDTH * cipher.Setup.ringCount * 2 - outputTextBox.Width) / 2, textBoxY / 2);
        }

        // Поля, хранящие, возможен ли ввод в поля открытого и зашифрованного текстов
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
            if (e.KeyCode == Keys.Back && outputTextBox.TextLength != 0)
            {
                int diskIndex = (cipher.Setup.ringCount - 2) - cipher.encryptCount;
                if (cipher.Setup.findSymbol(diskIndex, inputTextBox.Text.Last()) != -1)
                    cipher.encryptCount = mod(cipher.encryptCount - 1, cipher.Setup.ringCount - 2);
                inputTextBox.Text = inputTextBox.Text.Remove(inputTextBox.TextLength - 1);
                outputTextBox.Text = outputTextBox.Text.Remove(inputTextBox.TextLength);
                if (inputTextBox.TextLength == 0)
                    inputAllowed = true;
                cipher.clearHighlight();
                cipher.refreshRings();
                mainPictureBox.Refresh();
            }
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
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.SelectionStart = textBox.TextLength;
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!inputAllowed)
                return;
            if (e.KeyCode == Keys.Back && inputTextBox.TextLength != 0)
            {
                if (cipher.Setup.findSymbol(cipher.Setup.ringCount - 1, inputTextBox.Text.Last()) != -1)
                    cipher.encryptCount = mod(cipher.encryptCount - 1, cipher.Setup.ringCount - 2);
                inputTextBox.Text = inputTextBox.Text.Remove(inputTextBox.TextLength - 1);
                outputTextBox.Text = outputTextBox.Text.Remove(inputTextBox.TextLength);
                if (outputTextBox.TextLength == 0)
                    outputAllowed = true;
                cipher.clearHighlight();
                cipher.refreshRings();
                mainPictureBox.Refresh();
            }
        }

        int selectedRing = -1;      // Вращаемый диск, -1, если такового нет
        bool mouseDown = false;     // Зажата ли кнопка мыши для вращения диска
        float currentAngle = 0;     // Текущий угол поворота диска

        /// <summary>
        /// Операция % для отрицательных чисел
        /// </summary>
        /// <param name="left">Левый операнд</param>
        /// <param name="right">Правый операнд</param>
        /// <returns>Результат операции</returns>
        public static int mod(int left, int right)
        {
            return (left + right) % right;
        }

        /// <summary>
        /// Перевод угла из радиан в градусы
        /// </summary>
        /// <param name="radians">Величина угла в радианах</param>
        /// <returns>Величина угла в градусах</returns>
        public static float toDegrees(float radians)
        {
            return radians / (float)Math.PI * 180f;
        }

        private void mainPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            cipher.clearHighlight();
            int dx = e.X - CipherType.HALF_IMAGE_WIDTH, dy = e.Y - CipherType.HALF_IMAGE_HEIGHT;
            float maxDist = CipherType.RING_WIDTH * cipher.Setup.ringCount;
            int distSq = dx * dx + dy * dy;
            double dist = Math.Sqrt(distSq);
            selectedRing = (int)Math.Floor(dist / CipherType.RING_WIDTH);
            if (selectedRing == 0)
                return;
            float mouseAngle = toDegrees((float)Math.Atan2(dy, dx)) + 180;
            if (distSq >= maxDist * maxDist)
                return;
            currentAngle = mouseAngle - degreeRotations[selectedRing];
            mouseDown = true;
        }

        private void mainPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (!mouseDown)
                return;
            int shift = (int)Math.Round((degreeRotations[selectedRing] / 360 *
                                cipher.Setup.alphabets[selectedRing].Length));
            shift = mod(shift, cipher.Setup.alphabets[selectedRing].Length);
            cellRotations[selectedRing] = shift;
            degreeRotations[selectedRing] = (float)shift / cipher.Setup.alphabets[selectedRing].Length * 360;
            cipher.refreshRings();
            mainPictureBox.Refresh();
            mouseDown = false;
        }

        private void mainPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseDown)
                return;
            int dx = e.X - CipherType.HALF_IMAGE_WIDTH, dy = e.Y - CipherType.HALF_IMAGE_HEIGHT;
            float newAngle = toDegrees((float)Math.Atan2(dy, dx)) + 180;
            degreeRotations[selectedRing] = newAngle - currentAngle;
            cipher.refreshRings();
            mainPictureBox.Refresh();
        }
    }
}
