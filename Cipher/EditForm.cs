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
    /// <summary>
    /// Класс формы окна настроек
    /// </summary>
    public partial class EditForm : Form
    {
        // Максимальное количество дисков (требуется в силу особенностей реализации)
        const int MAX_RING_COUNT = 5;

        public MainForm owner;          // Форма-владелец
        public CipherType cipher;       // Набор параметров новой модели шифратора

        private TextBox[] alphabets;    // Массив текстовых полей для ввода алфавитов дисков
        
        public EditForm()
        {
            InitializeComponent();

            alphabets = new TextBox[MAX_RING_COUNT];
            alphabets[0] = textBox1;
            alphabets[1] = textBox2;
            alphabets[2] = textBox3;
            alphabets[3] = textBox4;
            alphabets[4] = textBox5;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (cipherComboBox.SelectedIndex == CipherSetup.loadedSetups.Length)
            {
                int newRingCount = (int)diskCountNumeric.Value;
                string[] tmp = new string[newRingCount + 1];
                for (int i = 1; i < newRingCount + 1; ++i)
                    tmp[i] = alphabets[i - 1].Text;
                cipher.Setup = CipherSetup.createSetup(Material.getMaterialByName(textureComboBox.Text),
                                                               newRingCount + 1,
                                                               tmp,
                                                               autoEncryptCheckBox.Checked);
            }
            else
                cipher.Setup = CipherSetup.loadedSetups[cipherComboBox.SelectedIndex];
            owner.updateCipher();
            this.Close();
        }

        /// <summary>
        /// Установить значения полей формы согласно набору параметров
        /// </summary>
        /// <param name="setup">Набор параметров</param>
        private void loadParams(CipherSetup setup)
        {
            diskCountNumeric.Value = setup.ringCount - 1;
            for (int i = 1; i < setup.ringCount; ++i)
                alphabets[i - 1].Text = setup.alphabets[i];
            for (int i = setup.ringCount - 1; i < alphabets.Length; ++i)
                alphabets[i].Visible = false;
            textureComboBox.SelectedItem = setup.material.getName();
            autoEncryptCheckBox.Checked = setup.autoEncrypt;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            diskCountNumeric.Maximum = MAX_RING_COUNT;

            cipherComboBox.Items.Clear();
            foreach (var setup in CipherSetup.loadedSetups)
                cipherComboBox.Items.Add(setup.name);
            cipherComboBox.Items.Add(CipherSetup.CUSTOM_SETUP_NAME);
            loadParams(cipher.Setup);
            cipherComboBox.SelectedItem = cipher.Setup.name;
        }

        private void cipherComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cipherComboBox.SelectedIndex == CipherSetup.loadedSetups.Length)
            {
                // Если выбран пользовательский шифратор, разрешить редактировать поля
                textureComboBox.Enabled = true;
                diskCountNumeric.Enabled = true;
                autoEncryptCheckBox.Enabled = true;
                foreach (var elem in alphabets)
                    elem.Enabled = true;
            }
            else
            {
                // Если выбран загруженный шифратор, установить значения полей
                // согласно его параметрам и заблокировать изменение
                textureComboBox.Enabled = false;
                diskCountNumeric.Enabled = false;
                autoEncryptCheckBox.Enabled = false;
                foreach (var elem in alphabets)
                    elem.Enabled = false;
                loadParams(CipherSetup.loadedSetups[cipherComboBox.SelectedIndex]);
            }
        }

        private void diskCountNumeric_ValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < alphabets.Length; ++i)
                alphabets[i].Visible = (i < diskCountNumeric.Value);
        }

        private void autoEncryptCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!autoEncryptCheckBox.Checked)
                return;
            for (int i = 1; i < diskCountNumeric.Value; ++i)
            {
                if (alphabets[i].TextLength != alphabets[0].TextLength)
                {
                    MessageBox.Show("Автоматическое шифрование невозможно для алфавитов разной длины.");
                    autoEncryptCheckBox.Checked = false;
                    return;
                }
            }
        }
    }
}
