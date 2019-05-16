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
    public partial class EditForm : Form
    {
        public MainForm owner;
        public CipherType cipher;

        private TextBox[] alphabets;

        public EditForm()
        {
            InitializeComponent();

            alphabets = new TextBox[Constants.MAX_RING_COUNT];
            alphabets[0] = textBox1;
            alphabets[1] = textBox2;
            alphabets[2] = textBox3;
            alphabets[3] = textBox4;
            alphabets[4] = textBox5;
            alphabets[5] = textBox6;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (cipherComboBox.SelectedIndex == CipherSetup.builtInSetups.Length)
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
                cipher.Setup = CipherSetup.builtInSetups[cipherComboBox.SelectedIndex];
            owner.updateRings();
            this.Close();
        }

        private void loadParams(CipherSetup setup)
        {
            diskCountNumeric.Value = setup.ringCount - 1;
            for (int i = 1; i < setup.ringCount; ++i)
                alphabets[i - 1].Text = setup.alphabets[i];
            textureComboBox.SelectedItem = setup.material.getName();
            autoEncryptCheckBox.Checked = setup.autoEncrypt;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            diskCountNumeric.Maximum = Constants.MAX_RING_COUNT;

            cipherComboBox.Items.Clear();
            foreach (var setup in CipherSetup.builtInSetups)
                cipherComboBox.Items.Add(setup.name);
            cipherComboBox.Items.Add(Constants.CUSTOM_SETUP_NAME);
            loadParams(cipher.Setup);
            cipherComboBox.SelectedItem = cipher.Setup.name;
        }

        private void cipherComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cipherComboBox.SelectedIndex == CipherSetup.builtInSetups.Length)
            {
                textureComboBox.Enabled = true;
                diskCountNumeric.Enabled = true;
                autoEncryptCheckBox.Enabled = true;
                foreach (var elem in alphabets)
                    elem.Enabled = true;
            }
            else
            {
                textureComboBox.Enabled = false;
                diskCountNumeric.Enabled = false;
                autoEncryptCheckBox.Enabled = false;
                foreach (var elem in alphabets)
                    elem.Enabled = false;
                loadParams(CipherSetup.builtInSetups[cipherComboBox.SelectedIndex]);
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
